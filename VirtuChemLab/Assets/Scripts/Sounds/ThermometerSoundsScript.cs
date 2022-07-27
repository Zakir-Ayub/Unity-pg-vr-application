using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermometerSoundsScript : MonoBehaviour
{
    private AudioSource source;

    [Tooltip("Sound that plays when the flask hits a hard surface like the floor or a table")]
    public AudioClip HittingHardSurfaceSound;


    [Tooltip("Sound that plays when the thermometer hits another glas object")]
    public AudioClip glasOnGlasSound;

    //the base pitch of the sounds
    [HideInInspector]
    public const float STARTPITCH = 1.0f;

    [Tooltip("Sound that plays when the flask hits a plastic container")]
    public AudioClip OnPlasticContainer;

    [Tooltip("Sound that plays when the flask hits a small plastic object like a lid")]
    public AudioClip OnPlasticSmallObject;

    //time until sound can be retriggered
    private float retriggerWaitTime = 0.0f;
    VolumeSettings vs = null;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.pitch = STARTPITCH;
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener != null)
        {
            vs = listener.GetComponent<VolumeSettings>();
        }
    }


    void OnCollisionEnter(Collision coll)
    {

        if (coll.gameObject.CompareTag("Surface") && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(HittingHardSurfaceSound, 0.4F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("Floor") && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(HittingHardSurfaceSound, 0.4F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("Scale") && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(HittingHardSurfaceSound, 0.5F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("PlasticContainer") && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(OnPlasticContainer, 1.0F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("PlasticLid") && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(OnPlasticSmallObject, 1.0F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("MetalObject") && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(HittingHardSurfaceSound, 0.5F);
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

        if (coll.gameObject.CompareTag("StirringFish") && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(OnPlasticSmallObject, 1.0F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("ChemistryObject")) 
        {
            //determining the correct sound to play for the collision

            Material collMat = null;
            Renderer rendr = coll.gameObject.GetComponent<Renderer>();
            if (rendr != null)
            {
                collMat = rendr.material;
            }
            if (collMat!= null && collMat.name.Contains("Material.001") && retriggerWaitTime == 0.0f)
            {
                source.pitch = STARTPITCH;
                source.PlayOneShot(glasOnGlasSound, 0.5F);
            }

            if (coll.gameObject.name.Contains("Thermometer") && !coll.gameObject.name.Contains("Digital") && retriggerWaitTime == 0.0f)
            {
                source.pitch = 1.5f;
                source.PlayOneShot(glasOnGlasSound, 0.3F);
                if (vs != null)
                {
                    retriggerWaitTime = vs.MaxWaitRetrigger;

                }
            }
            if (coll.gameObject.name.Contains("Beaker") && retriggerWaitTime == 0.0f)
            {
                source.pitch = STARTPITCH;
                source.PlayOneShot(glasOnGlasSound, 0.5F);
                if (vs != null)
                {
                    retriggerWaitTime = vs.MaxWaitRetrigger;

                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (retriggerWaitTime > 0.0f)
        {
            retriggerWaitTime -= 1.0f;
        }
    }
}
