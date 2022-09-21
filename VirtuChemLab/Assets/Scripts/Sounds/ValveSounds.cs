using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValveSounds : MonoBehaviour
{
    private AudioSource source;

    [Tooltip("Sound that plays when the valve is opened/closed")]
    public AudioClip valveMovement;


    [HideInInspector]
    public bool isOpen = false;

    // initial rotation of the valve
    private float InitX;
    private float InitY;
    private float InitZ;



    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();

        InitX = transform.rotation.eulerAngles.x;
        InitY = transform.rotation.eulerAngles.y;
        InitZ = transform.rotation.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {

        float diffX = InitX - transform.rotation.eulerAngles.x;

        float diffY = InitY - transform.rotation.eulerAngles.y;

        float diffZ = InitZ - transform.rotation.eulerAngles.z;

        if ((!((diffX <= 1.0f && diffX >= -1.0f) || (diffX <= 361.0f && diffX >= 359.0f)) || !((diffY <= 1.0f && diffY >= -1.0f) || (diffY <= 361.0f && diffY >= 359.0f)) || !((diffZ <= 1.0f && diffZ >= -1.0f) || (diffZ <= 361.0f && diffZ >= 359.0f))))
        {
            if (isOpen == false)
            {
                isOpen = true;
                source.PlayOneShot(valveMovement, 1.0F);
            }
        }
        else 
        {
            if (isOpen == true)
            {
                isOpen = false;
                source.PlayOneShot(valveMovement, 1.0F);
            }
        }
    }

}
