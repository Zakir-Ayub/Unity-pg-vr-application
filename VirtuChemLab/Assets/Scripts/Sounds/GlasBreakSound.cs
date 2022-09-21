using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlasBreakSound : MonoBehaviour
{
    private AudioSource source;

    [Tooltip("Sound that plays when glas breaks and this Object spawns")]
    public AudioClip spawnSound;

    private bool exists = false;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = spawnSound;
        exists = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (exists)
        {
            source.PlayOneShot(spawnSound, 0.1f);
            exists = false;
        }
    }
}
