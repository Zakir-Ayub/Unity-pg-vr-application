using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTurningSounds : MonoBehaviour
{
    private AudioSource source;

    [Tooltip("Sound that plays when the button is turned")]
    public AudioClip buttonMovement;

    private bool isMoving = false;

    //to prevent the sound from distorting due to being triggered to quickly again
    private const float MAXWAIT = 120.0f;
    private float currentWaitTime;


    // last rotation of the button
    private float lastX;
    private float lastY;
    private float lastZ;

    // last position of the button
    private float lastXpos;
    private float lastYpos;
    private float lastZpos;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        lastX = transform.rotation.eulerAngles.x;
        lastY = transform.rotation.eulerAngles.y;
        lastZ = transform.rotation.eulerAngles.z;
        lastXpos = transform.position.x;
        lastYpos = transform.position.y;
        lastZpos = transform.position.z;
        currentWaitTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //don't play sound if door is moving too slow
        if (((!(transform.rotation.eulerAngles.x >= lastX - 45 && transform.rotation.eulerAngles.x <= lastX + 45)) || (!(transform.rotation.eulerAngles.y >= lastY - 45 && transform.rotation.eulerAngles.y <= lastY + 45)) || (!(transform.rotation.eulerAngles.z >= lastZ - 45 && transform.rotation.eulerAngles.z <= lastZ + 45))) 
            && (((Mathf.Pow(transform.position.x - lastXpos, 2.0f) < Mathf.Pow(0.01f, 2.0f))) || ((Mathf.Pow(transform.position.y - lastYpos, 2.0f) < Mathf.Pow(0.01f, 2.0f))) || ((Mathf.Pow(transform.position.z - lastZpos, 2.0f) < Mathf.Pow(0.01f, 2.0f)))) && !isMoving)
        {
            isMoving = true;
            source.PlayOneShot(buttonMovement, 1.0F);
            currentWaitTime = MAXWAIT;
        }
        else
        {
            if ((((transform.rotation.eulerAngles.x >= lastX - 45 && transform.rotation.eulerAngles.x <= lastX + 45)) || ((transform.rotation.eulerAngles.y >= lastY - 45 && transform.rotation.eulerAngles.y <= lastY + 45)) || ((transform.rotation.eulerAngles.z >= lastZ - 45 && transform.rotation.eulerAngles.z <= lastZ + 45))) && isMoving)
            {
                //prevent sound from being played to quickly back to back 
                if (currentWaitTime == 0)
                {
                    isMoving = false;
                }
                else
                {
                    currentWaitTime -= 1.0f;
                }
            }
        }
        lastX = transform.rotation.eulerAngles.x;
        lastY = transform.rotation.eulerAngles.y;
        lastZ = transform.rotation.eulerAngles.z;
        lastXpos = transform.position.x;
        lastYpos = transform.position.y;
        lastZpos = transform.position.z;
    }
}
