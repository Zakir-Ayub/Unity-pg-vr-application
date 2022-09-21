using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitalThermometerSounds : MonoBehaviour
{
    private AudioSource source;
    // Start is called before the first frame update

    [Tooltip("Sound that plays when the flask hits this thermometer")]
    public AudioClip glasOnThis;

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

        if (coll.gameObject.CompareTag("ChemistryObject"))
        {
            Material collMat = null;
            Renderer rendr = coll.gameObject.GetComponent<Renderer>();
            if (rendr != null)
            {
                collMat = rendr.material;
            }
            if ( collMat!=null && collMat.name.Contains("Material.001") && retriggerWaitTime == 0.0f)
            {
                source.PlayOneShot(glasOnThis, 0.5F);
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
