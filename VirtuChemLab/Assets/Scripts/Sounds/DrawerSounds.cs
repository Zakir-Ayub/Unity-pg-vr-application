using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerSounds : MonoBehaviour
{
    private AudioSource source;

    [Tooltip("Sound that plays when the drawer is opened/closed")]
    public AudioClip drawerMovement;

    private float cooldown = 0.0f;

    private float MAXWAIT = 10.0f;

    // Start is called before the first frame update


    void Start()
    {
        source = GetComponent<AudioSource>();
        cooldown = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {

        Rigidbody rb = GetComponent<Rigidbody>();


        if (rb.velocity.magnitude > 2.0f)
        {
            if (cooldown > 0.0)
            {
                cooldown -= 1.0f;
            }
            else 
            {
                cooldown = MAXWAIT;
                source.PlayOneShot(drawerMovement, 1.0F);
            }
      
            
        }
    }
}

