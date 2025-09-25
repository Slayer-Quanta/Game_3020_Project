using UnityEngine;
public class MouseMovement : MonoBehaviour 
{ 
    public float mouseSensitivity = 100f; 
    
    float xRotate = 0f;
    float yRotate = 0f; 

    public float topClamp = -90f; 
    public float bottomClamp = 90f; 
    void Start() 
    { 
        Cursor.lockState = CursorLockMode.Locked;
    } 

     void Update()
     { 
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; 
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; 
        xRotate -= mouseY; 
        xRotate = Mathf.Clamp(xRotate, topClamp , bottomClamp);
        yRotate += mouseX; 
        transform.localRotation = Quaternion.Euler(xRotate, yRotate, 0f); 
    } 
}