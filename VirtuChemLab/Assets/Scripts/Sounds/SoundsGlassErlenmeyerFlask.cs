using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: add this script to the prefabs versions of the Erlenmeyer Flasks
public class SoundsGlassErlenmeyerFlask : MonoBehaviour
{
    
    private AudioSource source;

    [Tooltip("Sound that plays when the flask hits a hard surface like the floor or a table")]
    public AudioClip glasHittingHardSurfaceSound;

    [Tooltip("Sound that plays when a stirringfish is put into an empty flask")]
    public AudioClip stirrfishInEmptyFlaskSound;

    [Tooltip("Sound that plays when the flask is getting filled with water")]
    public AudioClip waterFillingFlaskSound;

    [Tooltip("Sound that plays when the flask hits another glas object")]
    public AudioClip glasOnGlasSound;

    [Tooltip("Sound that plays when the flask hits a plastic container")]
    public AudioClip glasOnPlasticContainer;

    [Tooltip("Sound that plays when the flask hits a small plastic object like a lid")]
    public AudioClip glasOnPlasticSmallObject;

    //tells whether there is water in the flask or not
    private bool isEmpty = true;

    //time until sound can be retriggered
    private float retriggerWaitTime = 0.0f;
    VolumeSettings vs = null;


    void Start()
    {
        source = GetComponent<AudioSource>();
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener != null)
        {
            vs = listener.GetComponent<VolumeSettings>();
        }
    }


    void OnCollisionEnter(Collision coll)
    {
        //determining the correct sound to play for the collision

        //tables and washbasin
        if (coll.gameObject.CompareTag("Surface") && retriggerWaitTime == 0.0f && coll.relativeVelocity.magnitude > 1.3f)
        {
            source.PlayOneShot(glasHittingHardSurfaceSound, 0.4F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("Floor") && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(glasHittingHardSurfaceSound, 0.4F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if ((coll.gameObject.CompareTag("Scale") || coll.gameObject.CompareTag("Heatplate")) && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(glasHittingHardSurfaceSound, 0.5F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("PlasticContainer") && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(glasOnPlasticContainer, 1.0F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("PlasticLid") && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(glasOnPlasticSmallObject, 1.0F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("MetalObject") && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(glasHittingHardSurfaceSound, 0.5F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("GlasObject") && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(glasOnGlasSound, 1.0F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("StirringFish") && isEmpty && retriggerWaitTime == 0.0f)
        {
            float otherY = coll.gameObject.transform.position.y;
            float difference = otherY - transform.position.y;
            if ((difference*difference) >= (0.1f * 0.1f ))
            {
                source.PlayOneShot(stirrfishInEmptyFlaskSound, 1.0F);
                if (vs != null)
                {
                    retriggerWaitTime = vs.MaxWaitRetrigger;

                }
            }
            else 
            {
                source.PlayOneShot(glasOnGlasSound, 0.4F);
                if (vs != null)
                {
                    retriggerWaitTime = vs.MaxWaitRetrigger;

                }
            }
        }

            //determing objects that I can't assign another tag to
            Material collMat = null;
            Renderer rendr = coll.gameObject.GetComponent<Renderer>();
            if (rendr != null)
            {
                collMat = rendr.material;
            }
            if (collMat != null && collMat.name.Contains("Material.001") && retriggerWaitTime == 0.0f) 
            {                                
                source.PlayOneShot(glasOnGlasSound, 0.5F);
                if (vs != null)
                {
                   retriggerWaitTime = vs.MaxWaitRetrigger;

                }
            }
            if (collMat != null && collMat.name.Contains("Metall Texture") && retriggerWaitTime == 0.0f)
            {
                source.PlayOneShot(glasHittingHardSurfaceSound, 0.4F);
                if (vs != null)
                {
                   retriggerWaitTime = vs.MaxWaitRetrigger;

                }
            }
            if (coll.gameObject.name.Contains("Beaker") && name.Contains("Beaker") && retriggerWaitTime == 0.0f)
            {
                source.PlayOneShot(glasOnGlasSound, 0.5F);
                if (vs != null)
                {
                   retriggerWaitTime = vs.MaxWaitRetrigger;

                }
            }

        
    }

    void Update()
    {
        if (retriggerWaitTime > 0.0f)
        {
            retriggerWaitTime -= 1.0f;
        }
    }
}
