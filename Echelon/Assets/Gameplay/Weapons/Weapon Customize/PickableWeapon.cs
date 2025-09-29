using UnityEngine;

public class PickableWeapon : MonoBehaviour
{
    [Header("Weapon Reference")]
    public GameObject weaponPrefab; 

    [Header("Visual Effects")]
    public float rotationSpeed = 50f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        //this rotates and bobs the pickup for showing what is able to be picked up
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        Vector3 newPosition = startPosition;
        newPosition.y += Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = newPosition;
    }

    public void PickupWeapon()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            WeaponManager weaponManager = player.GetComponent<WeaponManager>();
       
            if (weaponManager != null && weaponPrefab != null)
            {
                weaponManager.EquipWeaponDirect(weaponPrefab);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Failure No prefab has been found");
            }
        }
    }
}