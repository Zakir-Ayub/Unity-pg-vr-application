using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetruehrerSounds : MonoBehaviour
{
    private AudioSource source;

    [Tooltip("Sound that plays when a glass object hits this object")]
    public AudioClip glasHittingThisSound;

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

    void OnCollisionEnter(Collision coll)
    {
        //determining the correct sound to play for the collision
        Material collMat = null;
        Renderer rendr = coll.gameObject.GetComponent<Renderer>();
        if (rendr != null)
        {
            collMat = rendr.material;
        }
        if (coll.gameObject.CompareTag("ChemistryObject"))
        {
            if (collMat != null && collMat.name.Contains("Material.001") && retriggerWaitTime == 0.0f)
            {
                source.PlayOneShot(glasHittingThisSound, 0.5F);
                if (vs != null)
                {
                    retriggerWaitTime = vs.MaxWaitRetrigger;

                }
            }
            if (coll.gameObject.name.Contains("Beaker") && retriggerWaitTime == 0.0f)
            {
                source.PlayOneShot(glasHittingThisSound, 0.5F);
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
