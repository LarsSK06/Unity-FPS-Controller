using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour{

    [Header(" - General - ")]
    public CursorLockMode cursorLockMode;
    
    [Header(" - Movement & Camera - ")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;
    public float mouseSensitivity;
    public Camera mainCamera;
    public bool invertCamera;
    public float cameraClampAngle;


    [Header(" - Ground Sensement - ")]
    public float groundSensorRadius;
    public Vector3 groundSensorOffset;


    // Private variables
    float elevation;
    float speed;
    bool isGrounded;


    void Start(){
        // Mouse Confinement
        Cursor.lockState = cursorLockMode;
    }


    void Update(){
        // Inputs
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;


        // Camera
        elevation += invertCamera ? mouseY : -mouseY;
        elevation = Mathf.Clamp(elevation, -cameraClampAngle, cameraClampAngle);
        mainCamera.transform.localRotation = Quaternion.Euler(elevation, 0, 0);
        transform.Rotate(transform.up * mouseX);


        // Movement
        speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        GetComponent<Rigidbody>().drag = isGrounded ? 10 : 0;
        if(Input.GetKeyDown(KeyCode.Space)){
            GetComponent<Rigidbody>().AddForce(isGrounded ? Vector3.up * jumpForce : Vector3.zero, ForceMode.VelocityChange);
        }


        // Ground Sensement
        isGrounded = false;
        foreach(Collider i in Physics.OverlapSphere(transform.position + groundSensorOffset, groundSensorRadius)){
            if(i.transform.tag != "Player"){
                isGrounded = true;
                break;
            }
        }
    }

    void FixedUpdate(){
        // Inputs
        float moveX = Input.GetAxisRaw("Horizontal") * speed;
        float moveZ = Input.GetAxisRaw("Vertical") * speed;


        // Movement
        if(isGrounded){
            Vector3 move = transform.forward * moveZ + transform.right * moveX;
            GetComponent<Rigidbody>().AddForce(move, ForceMode.VelocityChange);
        }
    }

    void OnDrawGizmos(){
        // Camera
        Gizmos.color = Color.red;
        Gizmos.DrawLine(mainCamera.transform.position, mainCamera.transform.forward * 3);

        // Movement
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical"));

        // Ground Sensement
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + groundSensorOffset, groundSensorRadius);
    }

}