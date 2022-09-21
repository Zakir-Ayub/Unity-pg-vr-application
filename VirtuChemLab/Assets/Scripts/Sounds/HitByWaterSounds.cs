using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitByWaterSounds : MonoBehaviour
{

    private AudioSource source;

    [Tooltip("Sound that plays when this Object is hit by water")]
    public AudioClip waterHitSound;

    //just to make the sound cut off at the right time
    private GameObject[] valves;
    private float distanceNearestValve = 0.0f;
    private bool openValve = false;
    private SpraybottleController[] spraybottles;
    private float distanceNearestSBottle = 0.0f;
    private bool sprayingBottle = false;
    private WaterController[] beakers;
    private float distanceNearestBeaker = 0.0f;
    private bool pouringBeaker = false;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip= waterHitSound;
        valves = GameObject.FindGameObjectsWithTag("Valve");
        spraybottles = FindObjectsOfType<SpraybottleController>();
        beakers = FindObjectsOfType<WaterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //detect water hitting the object
    void OnParticleCollision(GameObject other)
    {
        if (other.name.Contains("Water"))
        {
            if (valves != null )
            {
                foreach (GameObject valve in valves)
                {
                    bool open = valve.GetComponent<WaterTapController>().isOpen;
                    float tmp = Vector3.Distance(transform.position, valve.transform.position);
                    if (distanceNearestValve == 0.0f || tmp <= distanceNearestValve)
                    {
                        distanceNearestValve = tmp;
                        openValve = open;
                    }
                }
            }

            if (spraybottles != null )
            {
                foreach (SpraybottleController spraybottle in spraybottles)
                {
                    bool spray = spraybottle.isSpraying;
                    float tmp2 = Vector3.Distance(transform.position, spraybottle.gameObject.transform.position);
                    if (distanceNearestSBottle == 0.0f || tmp2 <= distanceNearestSBottle)
                    {
                        distanceNearestValve = tmp2;
                        sprayingBottle = spray;
                    }
                }
            }

            if (beakers != null )
            {
                foreach (WaterController beaker in beakers)
                {
                    if (beaker.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                    {
                        bool pour = beaker.GetComponent<WaterController>().isPouring;
                        float tmp3 = Vector3.Distance(transform.position, beaker.gameObject.transform.position);
                        if (tmp3 != 0.0f && (distanceNearestBeaker == 0.0f || tmp3 <= distanceNearestBeaker))
                        {
                            distanceNearestBeaker = tmp3;
                            pouringBeaker = pour;
                        }
                    }
                }
            }
            if (openValve || sprayingBottle || pouringBeaker)
            {
                if (!source.isPlaying)
                {
                    source.Play();
                }
            }
            else
            {
                if (!( openValve || sprayingBottle || pouringBeaker))
                {
                    source.Stop();
                }
            }

            distanceNearestValve = 0.0f;
        }
    }
}
