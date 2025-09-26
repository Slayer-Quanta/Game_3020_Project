using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Mathematics.math;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public bool isShooting, readyToFire;
    bool allowReset = true;
    public float shootingDelay = 2f;
    public int bulletsPerBurst = 3;
    public int currentBurst;
    public float spreadIntensity;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;

    [Header("Effects")]
    public GameObject muzzleflash;
    private Animator animator;

    private Unity.Mathematics.Random mathRandom;
    private InputSystem_Actions inputActions;

    public enum ShootingMode
    {
        Single,
        Burst,
        Automatic
    }
    public ShootingMode shootingMode;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        readyToFire = true;
        currentBurst = bulletsPerBurst;
        animator = GetComponent<Animator>();
        mathRandom = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100000));
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Update()
    {
        HandleShooting();

        if (readyToFire && isShooting)
        {
            currentBurst = bulletsPerBurst;
            FireWeapon();
        }
    }

    void HandleShooting()
    {
        if (shootingMode == ShootingMode.Automatic)
        {
            isShooting = inputActions.Player.Attack.IsPressed();
        }
        else if (shootingMode == ShootingMode.Single || shootingMode == ShootingMode.Burst)
        {
            isShooting = inputActions.Player.Attack.WasPressedThisFrame();
        }
    }

    private void FireWeapon()
    {
        muzzleflash.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("Recoil");
        //AudioManager.Instance.SMGsound.Play();

        readyToFire = false;
        float3 shootingDirection = normalize(CalculateDirectionAndSpread());

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        if (shootingMode == ShootingMode.Burst && currentBurst > 1)
        {
            currentBurst--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private void ResetShot()
    {
        readyToFire = true;
        allowReset = true;
    }

    public float3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new float3(0.5f, 0.5f, 0));
        RaycastHit hit;

        float3 targetpoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetpoint = hit.point;
        }
        else
        {
            targetpoint = ray.GetPoint(100);
        }

        float3 direction = targetpoint - (float3)bulletSpawn.position;
        float x = mathRandom.NextFloat(-spreadIntensity, spreadIntensity);
        float y = mathRandom.NextFloat(-spreadIntensity, spreadIntensity);

        return direction + new float3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}