using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsideCollisionSounds : MonoBehaviour
{
    private AudioSource source;

    [Tooltip("Sound that plays when an object enters/leaves the beaker")]
    public AudioClip EnterExitSound;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = EnterExitSound;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //turns off collision sounds of collisions between beaker and an object placed inside
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlasticObject"))
        {
            InsideObjectSoundControl ocs =other.gameObject.GetComponent<InsideObjectSoundControl>();
            if (ocs != null)
            {
                ocs.isInside = true;
                GameObject parent = ocs.gameObject.transform.parent.gameObject;
                GameObject arent = gameObject.transform.parent.gameObject.transform.parent.gameObject;
                if (parent!= null)
                {
                    ObjectCollisionSounds obcs= parent.GetComponent<ObjectCollisionSounds>();
                    if (obcs!=null)
                    {
                        obcs.isInside = true;
                    }
                }
                if (arent != null)
                {
                    SoundsGlassErlenmeyerFlask sgef = arent.GetComponent<SoundsGlassErlenmeyerFlask>();
                    if (sgef != null)
                    {
                        sgef.isInside = true;
                        source.Play();
                    }
                }
            }
           
        }
    }

    //turns on collision sounds of collisions between beaker and an object placed inside
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PlasticObject"))
        {
            InsideObjectSoundControl ocs = other.gameObject.GetComponent<InsideObjectSoundControl>();
            if (ocs != null)
            {
                ocs.isInside = false;
                GameObject parent = ocs.gameObject.transform.parent.gameObject;
                GameObject arent = gameObject.transform.parent.gameObject.transform.parent.gameObject;
                if (parent != null)
                {
                    ObjectCollisionSounds obcs = parent.GetComponent<ObjectCollisionSounds>();
                    if (obcs != null)
                    {
                        obcs.isInside = false;
                    }
                }
                if (arent != null)
                {
                    SoundsGlassErlenmeyerFlask sgef = arent.GetComponent<SoundsGlassErlenmeyerFlask>();
                    if (sgef != null)
                    {
                        sgef.isInside = false;
                        source.Play();
                    }
                }
            }
        }
    }
}
