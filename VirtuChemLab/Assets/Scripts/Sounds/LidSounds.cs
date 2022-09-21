using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidSounds : MonoBehaviour
{
    private AudioSource source;

    [Tooltip("Sound that plays when the lid is opened/closed")]
    public AudioClip lidRotating;


    //check if already is moving to prevent sound from being started again
    private bool isMoving = false;

    //flag telling whether lid id on container or not
    private bool onContainer = false;

    //to prevent sound from being played upon loading in
    private bool isInit = false;

    // last rotation of the door
    private float lastX;
    private float lastY;
    private float lastZ;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        lastX = transform.rotation.eulerAngles.x;
        lastY = transform.rotation.eulerAngles.y;
        lastZ = transform.rotation.eulerAngles.z;
        source.clip= lidRotating;
        source.loop = false;
    }

    // Update is called once per frame
    void Update()
    {
        //don't play sound if lid is moving too slow
        if (((!(transform.rotation.eulerAngles.x >= lastX - 45 && transform.rotation.eulerAngles.x <= lastX + 45)) || (!(transform.rotation.eulerAngles.y >= lastY - 45 && transform.rotation.eulerAngles.y <= lastY + 45)) || (!(transform.rotation.eulerAngles.z >= lastZ - 45 && transform.rotation.eulerAngles.z <= lastZ + 45))) && !isMoving && onContainer)
        {
            if (isInit)
            {
                isMoving = true;
                source.loop = true;
                source.Play();
            }
            else 
            {
                isInit = true;
            }
        }
        else
        {
            isMoving = false;
            source.loop = false;

        }
        lastX = transform.rotation.eulerAngles.x;
        lastY = transform.rotation.eulerAngles.y;
        lastZ = transform.rotation.eulerAngles.z;
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("PlasticContainer"))
        {
            onContainer = true;
        }
    }

    void OnCollisionExit(Collision coll)
    {
        if (coll.gameObject.CompareTag("PlasticContainer"))
        {
            onContainer = false;
        }
    }

}
