using Unity.Netcode;
using UnityEngine;
using System;
using System.Collections.Generic;

public class WaterlevelSync : NetworkBehaviour
{
    [Tooltip("The max amount of water in the beaker in ml.")]
    public float maxWater;

    [Tooltip("The amount of water in ml each water particle is worth")]
    public float waterPerParticle = 0.25f;

    [Tooltip("The value that the emission rate of the spraybottle is nerfed for, visually")]
    public float sprayBottleNerfer = 100f;

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

    // the material of the water shader
    private Material waterShaderMat;

    // fixed value for current implementation of shader graph slider
    private const float MinSliderVal = 0.453f;
    // fixed value for current implementation of shader graph slider
    private const float MaxSliderVal = 0.58f;
    // maxSliderVal - minSliderVal, to make calculations easier to read
    private float SliderDifference => MaxSliderVal - MinSliderVal;

    // Start is called before the first frame update
    void Start()
    {
        waterShaderMat = gameObject.GetComponent<Renderer>().material;
        waterShaderMat.SetFloat("_WaterAmountBurette", MaxSliderVal);

        WaterAmount = maxWater;
        waterAmount.OnValueChanged += (_, newWaterAmount) =>
        {
            waterShaderMat.SetFloat("_WaterAmountBurette", newWaterAmount / maxWater * SliderDifference + MinSliderVal); // set value shader automatically when the water amount changes
        };
    }

    // this function handles the decreasing of the water level based on the emissionRate and the other set values in this script
    public void DecreaseWaterAmount(int emissionRate)
    {
        if (IsServer) // water amount calculation is server-side
        {
            WaterAmount = Mathf.Max(WaterAmount - emissionRate /sprayBottleNerfer * waterPerParticle, 0); // removes waterPerParticle * numberOfParticlesEmmittedInFixedUpdate water from container
        }
    }
}
