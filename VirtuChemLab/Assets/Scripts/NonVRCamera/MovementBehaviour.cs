using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBehaviour : MonoBehaviour
{
    //Keep track of our current state
    private CharacterController characterController;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    private float fallVelocity = 0.0f;

    //Set movement speed and gravity (if gravity is zero, there is no falling)
    public static float MOVEMENT_SPEED = 5f;
    public static float GRAVITY = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get mouse movement for rotation
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        //Add mouse movement to current rotation speed
        yRotation += mouseX;
        xRotation -= mouseY;
        //Make sure we can't break our neck by excessively nodding up or down
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        //Apply rotation around the vertical axis, omit rotation around the horizontal axis (nodding) first for motion calculation
        transform.localRotation = Quaternion.Euler(0.0f, yRotation, 0.0f);

        //Get the movement direction from user input
        float horizontalMovement = Input.GetAxis("Horizontal") * MOVEMENT_SPEED;
        float verticalMovement = Input.GetAxis("Vertical") * MOVEMENT_SPEED;

        //Rotate the movement direction according to camera rotation around the vertical axis to allow the user to move in the direction it is looking
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        //Apply movement
        characterController.Move((right * horizontalMovement + forward * verticalMovement) * Time.deltaTime);

        //Reapply rotation to camera, this time including both rotation axis
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0.0f);

        //Calculate and apply falling (disable by setting gravity to zero)
        if (characterController.isGrounded) {
            fallVelocity = 0;
        } else {
            fallVelocity -= GRAVITY * Time.deltaTime;
            characterController.Move(new Vector3(0, fallVelocity, 0));
        }
    }
}
