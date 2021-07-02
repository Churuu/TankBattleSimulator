using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Camera_Controller : MonoBehaviour
{
    public float normalSpeed;
    public float fastSpeed;

    public float movementSpeed;
    public float movementTime;

    public float rotationAmount;

    public bool isControllingTank = false;

    public CinemachineVirtualCamera virtualCamera;
    
    Vector3 newPosition;
    Quaternion newRotation;
    public Vector3 zoomAmount;
    Vector3 newZoom;

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        newPosition = virtualCamera.transform.position;
        newRotation = virtualCamera.transform.rotation;
        newZoom = virtualCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
    }
    void HandleMovementInput()
    {
        if (isControllingTank)
            return;


        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        if (Input.GetKey(KeyCode.R))
        {
            newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= zoomAmount;
        }


        virtualCamera.transform.position = Vector3.Lerp(virtualCamera.transform.position, newPosition, Time.deltaTime * movementTime);
        virtualCamera.transform.rotation = Quaternion.Lerp(virtualCamera.transform.rotation, newRotation, Time.deltaTime * movementTime);
        //virtualCamera.transform.position = Vector3.Lerp(virtualCamera.transform.position, newZoom, Time.deltaTime * movementTime);
    }

}
