using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSettings : MonoBehaviour
{
    [Tooltip("Volume of the sce in general")]
    [Range(0.0f, 1.0f)]
    public float currentVolume;

    //mask sounds that play due to collisions when scene is loading
    private float currentWaitTime;
    private float MAXWAITIME = 400.0f;

    public float MaxWaitRetrigger;

    void Start()
    {
        AudioListener.volume = 0.0f;
        currentWaitTime = MAXWAITIME;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWaitTime == 0.0f)
        {
            AudioListener.volume = currentVolume;
        }
        else 
        {
            currentWaitTime -= 1.0f;
        }

    }
}
