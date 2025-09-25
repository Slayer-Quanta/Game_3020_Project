using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public bool isShooting, readyToFire;
    bool allowReset = true;
    public float shootingDelay = 2f;

    public int bulletsPerBurst = 3;
    public int currentBurst;

    public float spreadIntensity;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;

    public GameObject muzzleflash;
    private Animator animator;
    public enum ShootingMode
    {
        Single,
        Burst,
        Automatic
    }
    public ShootingMode shootingMode;

    private void Awake()
    {
        readyToFire = true;
        currentBurst = bulletsPerBurst;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shootingMode == ShootingMode.Automatic)
        {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (shootingMode == ShootingMode.Single ||
            shootingMode == ShootingMode.Burst)
        { 
          isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

    if (readyToFire && isShooting)
        {
            currentBurst = bulletsPerBurst;
            FireWeapon();
        }
        
    }

    private void FireWeapon()
    {

      muzzleflash.GetComponent<ParticleSystem>().Play();

        animator.SetTrigger("Recoil");

        AudioManager.Instance.SMGsound.Play();

        readyToFire = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        bullet.transform.forward= shootingDirection;

        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
        
        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        if (shootingMode == ShootingMode.Burst && currentBurst > 1 )
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

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetpoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetpoint = hit.point;
        }
        else
        {
            targetpoint = ray.GetPoint(100);
        }

        Vector3 direction = targetpoint - bulletSpawn.position;
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy (bullet);
    }
}
