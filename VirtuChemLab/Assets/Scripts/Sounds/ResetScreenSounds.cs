using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetScreenSounds : MonoBehaviour
{
    
    private Canvas cn;
    private bool prevState;

    private AudioSource source;
    public AudioClip openUISound;
    public AudioClip closeUISound;


    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        cn = GetComponent<Canvas>();
        prevState = cn.enabled;
    }

    // Update is called once per frame
    void Update()
    {
        if (prevState != cn.enabled)
        {
            if (cn.enabled)
            {
                source.PlayOneShot(openUISound, 0.5F);
                prevState = cn.enabled;
            }
            else 
            {
                source.PlayOneShot(closeUISound, 0.5F);
                prevState = cn.enabled;
            }
        }
       
    }
}
