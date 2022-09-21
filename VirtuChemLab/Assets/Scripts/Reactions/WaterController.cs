using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controller of the Beaker. Is a ContainerPouringSystem.
/// Responsible for setting the water shader and playing sounds.
/// </summary>
[RequireComponent(typeof(ChemicalReactionScript))]
[RequireComponent(typeof(ObjectProperties))]
public class WaterController : FluidContainerSystem
{
    // audio source for sound
    private AudioSource audi;

    [Tooltip("Sound that plays when the flask is getting filled with water")]
    public AudioClip waterFillingFlaskSound;

    // necessary to play loop properly
    private bool isFillingUp = false;

    // necessary to play loop properly
    private bool startFillingUp = false;

    // below further necessary things for the sound
    private float maxWaitTime;
    private float currentWaitTime;

    //for sound purposes
    [HideInInspector]
    public bool isPouring = false;

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
    float particleTick = 0.0f;
    float particlePrev = 0.0f;


    protected new void Start()
    {
        base.Start();
        container.maxStorage = 350;

        Assert.IsNotNull(transform.Find("Water"), name + ": PS has no Water");
        Assert.IsTrue(transform.Find("Water").CompareTag("Chemical"), name + ": PS Water has wrong Tag");

        audi = GetComponent<AudioSource>();
        //Sets the main AudioClip to the water filling one in order to enable looping
        audi.clip = waterFillingFlaskSound;
        audi.loop = false;
        valves = GameObject.FindGameObjectsWithTag("Valve");
        spraybottles = FindObjectsOfType<SpraybottleController>();
        beakers = FindObjectsOfType<WaterController>();
    }

    protected new void FixedUpdate()
    {
        isPouring = ShouldEmit();
        base.FixedUpdate();
        FixedUpdateSound();
    }

    void FixedUpdateSound()
    {
        // below necessary logic for sound
        isFillingUp = false;
        if (particleTick != particlePrev)
        {
            isFillingUp = true;
        }

        particlePrev = particleTick;

        if (valves != null && isFillingUp)
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

        if (spraybottles != null && isFillingUp)
        {
            foreach (SpraybottleController spraybottle in spraybottles)
            {
                bool spray = spraybottle.isSpraying;
                float tmp2 = Vector3.Distance(transform.position, spraybottle.gameObject.transform.position);
                if (distanceNearestSBottle == 0.0f || tmp2 <= distanceNearestSBottle)
                {
                    distanceNearestSBottle = tmp2;
                    sprayingBottle = spray;
                }
            }
        }

        if (beakers != null && isFillingUp)
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


        if (isFillingUp && !startFillingUp && (openValve || sprayingBottle || pouringBeaker))
        {
            startFillingUp = true;
            audi.loop = true;
            audi.Play();
        }
        else
        {
            if (!(isFillingUp && (openValve || sprayingBottle || pouringBeaker)))
            {
                audi.loop = false;
                startFillingUp = false;
            }
        }

        distanceNearestValve = 0.0f;
        distanceNearestBeaker = 0.0f;
        distanceNearestSBottle = 0.0f;
    }

}
