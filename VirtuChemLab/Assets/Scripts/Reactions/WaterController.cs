using Unity.Netcode;
using UnityEngine;
using System;
using System.Collections.Generic;

public class WaterController : NetworkBehaviour
{
    [Tooltip("The GameObject of the water")]
    public GameObject waterObject;
    [Tooltip("The ParticleSystem that spawns the water particles")]
    public ParticleSystem waterParticleSystem;
    [Tooltip("The maximum emission rate over time for the particle system when held upside-down")]
    public int maxEmissionRateOverTime = 200;
    [Tooltip("The amount of water in ml each water particle is worth")]
    public float waterPerParticle = 0.25f;

    [NonSerialized]
    // the material of the applied water shader
    public Material waterShaderMat;

    [Tooltip("The max amount of water in the beaker in ml.")]
    public float maxWater;

    [Tooltip("Desired rotation of when to dispense the object")]
    public Vector3 desiredRotation = Vector3.down;

    [Tooltip("The minimum rotation degree necessary between transform rotation and rotation at which to emit particles"), Range(0f, 1f)]
    public float rotationMinThreshold = 0.5f;

    [NonSerialized]
    // the current amount of sand particles present
    public int currentSandParticles = 0;

    [Tooltip("The amount of sand particles that is needed for the water to saturate")]
    public int sandParticlesUntilSaturation = 2000;

    [NonSerialized]
    // the initial color of the water shader on the side
    public Color initialWaterColorSide;

    [NonSerialized]
    // the initial color of the water shader on the top
    public Color initialWaterColorTop;

    [SerializeField]
    // network variable for the water amount
    private NetworkVariable<float> waterAmount = new NetworkVariable<float>(0);
    public float WaterAmount
    {
        get => waterAmount.Value;
        private set
        {
            if (IsServer)
            {
                waterAmount.Value = value;
            }
        }
    }

    // fixed value for current implementation of shader graph slider
    private const float MinSliderVal = 0.495f; 
    // fixed value for current implementation of shader graph slider
    private const float MaxSliderVal = 0.567f;
    // maxSliderVal - minSliderVal, to make calculations easier to read
    private float SliderDifference => MaxSliderVal - MinSliderVal;

    [Tooltip("The ObjectProperties of the corresponding Water object")]
    public ObjectProperties water;

    // the ChemicalReactionScript handling the reaction for this object
    private ChemicalReactionScript reactionController;

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
    private float distanceNearestValve=0.0f;
    private bool openValve = false;
    private SpraybottleController[] spraybottles;
    private float distanceNearestSBottle = 0.0f;
    private bool sprayingBottle = false;
    private WaterController[] beakers;
    private float distanceNearestBeaker = 0.0f;
    private bool pouringBeaker = false;
    float particleTick = 0.0f;
    float particlePrev = 0.0f;


    void Start()
    {
        // stop ParticleSystem, get initial components, set initial water
        waterParticleSystem.Stop();
        reactionController = GetComponent<ChemicalReactionScript>();
        reactionController.setWater(water);
        waterShaderMat = waterObject.GetComponent<Renderer>().material;
        waterShaderMat.SetFloat("_WaterAmount", MinSliderVal);
        
        waterAmount.OnValueChanged += (_, newWaterAmount) =>
        {
            waterShaderMat.SetFloat("_WaterAmount", newWaterAmount / maxWater * SliderDifference + MinSliderVal);
        };

        // get initial colors from water shader
        initialWaterColorSide = waterShaderMat.GetColor("_SideColor");
        initialWaterColorTop = waterShaderMat.GetColor("_TopColor");

        audi = GetComponent<AudioSource>();
        //Sets the main AudioClip to the water filling one in order to enable looping
        audi.clip = waterFillingFlaskSound;
        audi.loop = false;
        valves = GameObject.FindGameObjectsWithTag("Valve");
        spraybottles= FindObjectsOfType<SpraybottleController>();
        beakers= FindObjectsOfType<WaterController>();
    }

    private void FixedUpdate()
    {
        // compute how much water has been added since last update
        float addedWaterWeight = WaterAmount - water.Weight;
        water.Weight = WaterAmount;

        if (reactionController)
        {
            // add corresponding volume to the ingredients
            reactionController.addIngredient("Water", addedWaterWeight);
        }

        float rotationLevel = (Vector3.Dot(transform.up, desiredRotation) + 1) / 2;
        if (rotationLevel > rotationMinThreshold) // if rotation is above the threshold
        {
            if(WaterAmount > 0)
            {
                // start pouring water if there is water and object is rotated above threshold
                if (!waterParticleSystem.isPlaying)
                {
                    isPouring = true;
                    waterParticleSystem.Play();
                }
                var em = waterParticleSystem.emission;
                int rateOverTime = Mathf.RoundToInt(maxEmissionRateOverTime * rotationLevel); // calculate emission rate based on rotation degree and max rate, round for more consistent calculations
                em.rateOverTime = rateOverTime; 
                DecreaseWaterAmount(rateOverTime);
            }
            else
            {
                // stop ParticleSystem if empty
                if (waterParticleSystem.isPlaying)
                {
                    isPouring = false;
                    waterParticleSystem.Stop();
                }
            }
        }
        else
        {
            // stop ParticleSystem if not tilted enough
            if (waterParticleSystem.isPlaying)
            {
                isPouring = false;
                waterParticleSystem.Stop();
            }
        }

        // below necessary logic for sound
        isFillingUp = false;
        if (particleTick!= particlePrev)
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
                    distanceNearestValve = tmp2;
                    sprayingBottle = spray;
                }
            }
        }

        if (beakers != null && isFillingUp)
        {
            foreach (WaterController beaker in beakers)
            {
                if(beaker.gameObject.GetInstanceID()!= gameObject.GetInstanceID())
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


        if (isFillingUp && !startFillingUp && (openValve|| sprayingBottle|| pouringBeaker))
        {
            startFillingUp = true;
            audi.loop = true;
            audi.Play();
        }
        else
        {
            if (!(isFillingUp && (openValve|| sprayingBottle|| pouringBeaker)))
            {
                audi.loop = false;
                startFillingUp = false;
            }
        }

        distanceNearestValve = 0.0f;
    }

    // used for water tap collisions
    void OnParticleCollision(GameObject other)
    {
        if (other.name.Contains("Water") && !waterParticleSystem.isPlaying)
        { 
            IncreaseWaterAmount(); // increase water amount if hit by water particle
            particleTick += 1.0f; // for the sound
        }
    }


    public void IncreaseWaterAmount()
    {
        if (IsServer) // water amount calculation is server-side
        {
            if (WaterAmount / maxWater * SliderDifference + MinSliderVal < MaxSliderVal)
            {
                // had to remove deltaTime because of consistency loss
                WaterAmount = Mathf.Min(WaterAmount + waterPerParticle, maxWater); // adds waterPerParticle amount per particle
                reactionController.setWater(water);
            }
        }
    }


    // this is called 50 times each sec in FixedUpdate
    private void DecreaseWaterAmount(int emissionRate)
    {
        if (IsServer) // water amount calculation is server-side
        {
            // had to remove deltaTime because of consistency loss, emissionRate = particle / sec -> 200 emissionRate = 200/50 particles per FixedUpdate
            WaterAmount = Mathf.Max(WaterAmount - ((float)emissionRate / 50f)* waterPerParticle, 0); // removes waterPerParticle * numberOfParticlesEmmittedInFixedUpdate water from container
            reactionController.setWater(water);
        }
    }
}
