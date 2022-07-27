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
    public Material dissolveMat;

    [Tooltip("The max amount of water in the beaker in ml.")]
    public float maxWater;

    [Tooltip("Desired rotation of when to dispense the object")]
    public Vector3 desiredRotation = Vector3.down;

    [Tooltip("The minimum likeness between our transform rotation and rotation at which to dispense the item"), Range(0f, 1f)]
    public float rotationMinLikeness = 0.5f;

    [SerializeField]
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

    public ObjectProperties water;
    private ChemicalReactionScript reactionController;

    //Audio components
    private AudioSource audi;

    [Tooltip("Sound that plays when the flask is getting filled with water")]
    public AudioClip waterFillingFlaskSound;

    //necessary to play loop properly
    private bool isFillingUp = false;

    //necessary to play loop properly
    private bool startFillingUp = false;

    private float maxWaitTime;
    private float currentWaitTime;

    private GameObject[] valves;
    private float distanceNearestValve=0.0f;
    private bool openValve = false;
    float particleTick = 0.0f;
    float particlePrev = 0.0f;


    void Start()
    {
        waterParticleSystem.Stop();
        reactionController = GetComponent<ChemicalReactionScript>();
        reactionController.setWater(water);
        dissolveMat = waterObject.GetComponent<Renderer>().material;
        dissolveMat.SetFloat("_WaterAmount", MinSliderVal);
        
        waterAmount.OnValueChanged += (_, newWaterAmount) =>
        {
            dissolveMat.SetFloat("_WaterAmount", newWaterAmount / maxWater * SliderDifference + MinSliderVal);
        };

        audi = GetComponent<AudioSource>();

        //Sets the main AudioClip to the water filling one in order to enable looping
        audi.clip = waterFillingFlaskSound;
        audi.loop = false;
        valves = GameObject.FindGameObjectsWithTag("Valve");

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
        if (rotationLevel > rotationMinLikeness)
        {
            if(WaterAmount > 0)
            {
                if (!waterParticleSystem.isPlaying)
                {
                    waterParticleSystem.Play();
                }
                var em = waterParticleSystem.emission;
                int rateOverTime = Mathf.RoundToInt(maxEmissionRateOverTime * rotationLevel);
                em.rateOverTime = rateOverTime; // round for more consistent calculations
                DecreaseWaterAmount(rateOverTime);
            }
            else
            {
                if (waterParticleSystem.isPlaying)
                {
                    waterParticleSystem.Stop();
                }
            }
        }
        else
        {
            if (waterParticleSystem.isPlaying)
            {
                waterParticleSystem.Stop();
            }
        }


       
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


        if (isFillingUp && !startFillingUp && openValve)
        {
            startFillingUp = true;
            audi.loop = true;
            audi.Play();
        }
        else
        {
            if (!(isFillingUp && openValve))
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
            IncreaseWaterAmount();
            particleTick += 1.0f;
        }
    }


    public void IncreaseWaterAmount()
    {
        if (IsServer)
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
        if (IsServer)
        {
            // had to remove deltaTime because of consistency loss, emissionRate = particle / sec -> 200 emissionRate = 200/50 particles per FixedUpdate
            WaterAmount = Mathf.Max(WaterAmount - ((float)emissionRate / 50f)* waterPerParticle, 0); // removes waterPerParticle * numberOfParticlesEmmittedInFixedUpdate water from container
            reactionController.setWater(water);
        }
    }
}
