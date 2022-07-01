using Unity.Netcode;
using UnityEngine;

public class WaterController : NetworkBehaviour
{
    public Material dissolveMat;

    [Tooltip("The max amount of water in the beaker in ml.")]
    public float maxWater;

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

    void Start()
    {
        reactionController = GetComponent<ChemicalReactionScript>();
        reactionController.setWater(water);

        dissolveMat.SetFloat("_WaterAmount", MinSliderVal);
        
        waterAmount.OnValueChanged += (_, newWaterAmount) =>
        {
            dissolveMat.SetFloat("_WaterAmount", newWaterAmount / maxWater * SliderDifference + MinSliderVal);
        };
    }

    private void Update()
    {
        // compute how much water has been added since last update
        float addedWaterWeight = WaterAmount - water.Weight;
        water.Weight = WaterAmount;

        if (reactionController)
        {
            // add corresponding volume to the ingredients
            reactionController.addIngredient("Water", addedWaterWeight);
        }
    }

    void OnParticleCollision(GameObject other)
    {
        IncreaseWaterAmount();
    }

    private void IncreaseWaterAmount()
    {
        if (IsServer)
        {
            if (WaterAmount / maxWater * SliderDifference + MinSliderVal < MaxSliderVal)
            {
                // multiply with deltaTime to ensure same fill rate regardless of frames per second
                WaterAmount = Mathf.Min(WaterAmount + 0.1f * Time.deltaTime * 100f, maxWater);
                reactionController.setWater(water);
            }
        }
    }
}
