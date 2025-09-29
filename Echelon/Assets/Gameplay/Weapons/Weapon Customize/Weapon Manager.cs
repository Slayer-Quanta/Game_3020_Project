using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    public Transform weaponSpawnPoint;
    public GameObject[] startingWeaponPrefabs;

    [Header("UI References")]
    public GameObject pickupPromptUI;
    public TextMeshProUGUI pickupText;

    [Header("Pickup Settings")]
    public float pickupRange = 3f;

    private InputSystem_Actions inputActions;
    private GameObject currentWeaponObject;
    private PickableWeapon nearbyPickup;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Interact.performed += OnPickupPressed;
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Interact.performed -= OnPickupPressed;
    }

    void Start()
    {
        if (pickupPromptUI != null)
            pickupPromptUI.SetActive(false);

        if (startingWeaponPrefabs != null && startingWeaponPrefabs.Length > 0 && startingWeaponPrefabs[0] != null)
        {
            EquipWeaponDirect(startingWeaponPrefabs[0]);
        }
    }

    void Update()
    {
        FindNearbyPickup();
        UpdatePickupUI();
    }

    void FindNearbyPickup()
    {
        PickableWeapon[] pickups = FindObjectsOfType<PickableWeapon>();
        PickableWeapon closest = null;
        float closestDistance = pickupRange;

        foreach (PickableWeapon pickup in pickups)
        {
            float distance = Vector3.Distance(transform.position, pickup.transform.position);
            if (distance < closestDistance)
            {
                closest = pickup;
                closestDistance = distance;
            }
        }
        nearbyPickup = closest;
    }

    void UpdatePickupUI()
    {
        if (pickupPromptUI != null)
        {
            bool shouldShow = nearbyPickup != null;
            pickupPromptUI.SetActive(shouldShow);

            if (shouldShow && pickupText != null && nearbyPickup.weaponPrefab != null)
            {
                //this is for showing you which prompt it is when you switch from controller to keyboard 
                string inputDevice = GetCurrentInputDevice();
                string buttonPrompt = inputDevice == "Gamepad" ? "East Button" : "E";
                string weaponName = nearbyPickup.weaponPrefab.name;
                pickupText.text = $"Press {buttonPrompt} to pickup {weaponName}";
            }
        }
    }

    string GetCurrentInputDevice()
    {
        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
            return "Gamepad";
        else
            return "Keyboard";
    }

    void OnPickupPressed(InputAction.CallbackContext context)
    {
        if (nearbyPickup != null)
        {
            nearbyPickup.PickupWeapon();
        }
    }

    public void EquipWeaponDirect(GameObject weaponPrefab)
    {
        if (weaponPrefab == null) return;
        if (weaponSpawnPoint == null) return;

        if (currentWeaponObject != null)
        {
            Destroy(currentWeaponObject);
        }

        currentWeaponObject = Instantiate(weaponPrefab);

        currentWeaponObject.transform.SetParent(weaponSpawnPoint);
        currentWeaponObject.transform.localPosition = new Vector3(0, 0, 2.0f); 

        // This was me trying to give this specific gun proper positioning because it turns inwards at the player
        if (weaponPrefab.name.Contains("Bennali_M4") || weaponPrefab.name.Contains("Bennali_M4"))
        {
            currentWeaponObject.transform.localRotation = Quaternion.identity; 
        }
        else
        {
            currentWeaponObject.transform.localRotation = Quaternion.identity;
        }
    }

    public Weapon GetCurrentWeapon()
    {
        if (currentWeaponObject != null)
            return currentWeaponObject.GetComponent<Weapon>();
        return null;
    }
}