using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollisionSounds : MonoBehaviour
{
    private AudioSource source;

    [Tooltip("Sound that plays when the flask hits a hard surface like the floor or a table")]
    public AudioClip HittingHardSurfaceSound;

    [Tooltip("Sound that plays when the flask hits another glas object")]
    public AudioClip OnGlasSound;

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
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener != null)
        {
            vs = listener.GetComponent<VolumeSettings>();
        }
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision coll)
    {
        //determining the correct sound to play for the collision

        //tables and washbasin
        if (coll.gameObject.CompareTag("Surface") && retriggerWaitTime == 0.0f && coll.relativeVelocity.magnitude > 1.3f)
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

        if ((coll.gameObject.CompareTag("Scale") || coll.gameObject.CompareTag("Heatplate")) && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(HittingHardSurfaceSound, 0.5F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("PlasticContainer") && retriggerWaitTime == 0.0f && coll.relativeVelocity.magnitude > 1.1f)
        {
            source.PlayOneShot(OnPlasticContainer, 1.0F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("PlasticLid") && retriggerWaitTime == 0.0f && coll.relativeVelocity.magnitude > 1.1f)
        {
            source.PlayOneShot(OnPlasticSmallObject, 1.0F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("MetalObject") && retriggerWaitTime == 0.0f && coll.relativeVelocity.magnitude > 1.0f)
        {
            source.PlayOneShot(HittingHardSurfaceSound, 0.5F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }

        if (coll.gameObject.CompareTag("GlasObject") && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(OnGlasSound, 1.0F);
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

            //determing objects that I can't assign another tag to
            Material collMat = null;
        Renderer rendr = coll.gameObject.GetComponent<Renderer>();
        if (rendr != null)
        {
            collMat = rendr.material;
        }
        if (collMat != null && collMat.name.Contains("Material.001") && retriggerWaitTime == 0.0f)
        {
            source.PlayOneShot(OnGlasSound, 0.5F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

            }
        }
        if (collMat != null && collMat.name.Contains("Metall Texture") && retriggerWaitTime == 0.0f && coll.relativeVelocity.magnitude > 1.0f)
        {
            source.PlayOneShot(HittingHardSurfaceSound, 0.4F);
            if (vs != null)
            {
                retriggerWaitTime = vs.MaxWaitRetrigger;

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
