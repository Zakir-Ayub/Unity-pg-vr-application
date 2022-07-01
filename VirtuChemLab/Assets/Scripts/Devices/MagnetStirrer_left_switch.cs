using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetStirrer_left_switch : MonoBehaviour
{
    [Tooltip("Set the rotation speed for the switch")]
    public float rotationSpeed;

    [Tooltip("Set the maximum rotation value for the switch in ° (should be >270 as of now")]
    public float maxRotation;

    // rotation information for switch and mouse input
    private float currentRotation;
    private float xRotationInput;
    private float yRotationInput;

    // int counter indicating the current quarter
    private int zoneCounter;

    // Start is called before the first frame update
    void Start()
    {
        zoneCounter = 0; // initialize zoneCounter with 0 at start (initial position)
    }

    // Update is called once per frame
    void Update()
    {
        currentRotation = gameObject.transform.eulerAngles.z;
        // get current rotation value of the switch
        // compute zoneCounter for current switch position
        zoneCounter = (int)Mathf.Floor(currentRotation / 90);
    }

    void OnMouseDrag() 
    {
        // REMARK: This code might not work properly when turning the object in the scene!

        // get mouse input
        xRotationInput = (Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime);
        yRotationInput = (Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime);
        
        switch (zoneCounter)
        {
            // 0-90 degrees
            case 0:
                // make sure to not "overrotate" to the left side
                if (currentRotation + xRotationInput + yRotationInput < 0)
                {
                    transform.Rotate(0, 0, 0, Space.World);
                }
                // move clockwise if allowed by input
                else if (currentRotation + xRotationInput - yRotationInput > 0)
                {
                    currentRotation = currentRotation + xRotationInput - yRotationInput;
                    gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, currentRotation);
                }
                break;
            // 90-180 degrees
            case 1:
                // move clockwise if allowed by input
                if (currentRotation - xRotationInput - yRotationInput > 89)
                {
                    currentRotation = currentRotation - xRotationInput - yRotationInput;
                    gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, currentRotation);
                }
                break;
            // 180-270 degrees
            case 2:
                // move clockwise if allowed by input
                if (currentRotation - xRotationInput + yRotationInput > 179)
                {
                    currentRotation = currentRotation - xRotationInput + yRotationInput;
                    gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, currentRotation);
                }
                break;
            // 270-360 degrees
            case 3:
                // make sure to not "overrotate" more than max rotation value
                if (currentRotation + xRotationInput + yRotationInput > maxRotation)
                {
                    transform.Rotate(0, 0, 0, Space.World);
                }
                // move clockwise if allowed by input
                else if (currentRotation + xRotationInput + yRotationInput > 269)
                {
                    currentRotation = currentRotation + xRotationInput + yRotationInput;
                    gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, currentRotation);
                }
                break;
        }
    }
}
