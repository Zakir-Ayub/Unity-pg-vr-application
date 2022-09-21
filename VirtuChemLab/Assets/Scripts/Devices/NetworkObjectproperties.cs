using Reactions;
using Unity.Netcode;
using UnityEngine;

public abstract class NetworkObjectproperties : NetworkBehaviour
{
    [Tooltip("Enable temperature logging in the console")]
    public bool enableLog;

    // Implement these in children
    public abstract float Weight { get; set; }
    public abstract float Temperature { get; set; }
    public abstract float PhValue { get; set; }

    protected float getWeightInChildren()
    {
        float sum = 0;
        foreach (var item in GetComponentsInChildren<NetworkObjectproperties>())
        {
            if (item == this) continue;
            sum += item.Weight;
        }
        return sum;
    }
    /// <summary>
    /// The rate with which the object's temperature can change, in �C per frame. Always derived from the per-second <see cref="heatFlowRate"/>.
    /// </summary>
    //private float heatFlowPerFrame;
    protected NetworkVariable<float> heatFlowPerFrame = new NetworkVariable<float>();
    public float HeatFlowPerFrame
    {
        get => heatFlowPerFrame.Value;
        set
        {
            if (IsServer)
            {
                heatFlowPerFrame.Value = value;
            }
        }
    }

    [SerializeField, Tooltip("The rate with which the object's temperature can change, in �C per second")]
    protected NetworkVariable<float> heatFlowRate = new NetworkVariable<float>();
    public float HeatFlowRate
    {
        get => heatFlowRate.Value;
        set
        {
            if (IsServer)
            {
                heatFlowRate.Value = value;
            }
        }
    }

    /// <summary>
    /// Actual temperature of the object. Trends towards target <see cref="temperature"/> each frame according to <see cref="heatFlowRate"/>.
    /// </summary>
    protected NetworkVariable<float> actualTemperature = new NetworkVariable<float>();
    public float ActualTemperature
    {
        get => actualTemperature.Value;
        set
        {
            if (IsServer)
            {
                actualTemperature.Value = value;
            }
        }
    }

    protected NetworkVariable<float> initialHeatFlowRate = new NetworkVariable<float>();

    public float InitialHeatFlowRate
    {
        get => initialHeatFlowRate.Value;
        set
        {
            if (IsServer)
            {
                initialHeatFlowRate.Value = value;
            }
        }
    }

    protected void Awake()
    {
        heatFlowPerFrame.Value = heatFlowRate.Value / 50f;
        initialHeatFlowRate.Value = heatFlowRate.Value;
    }

    // FixedUpdate is called 50 times per second
    private void FixedUpdate()
    {
        if (enableLog) Debug.Log("actual temp: " + ActualTemperature + ", target temp: " + Temperature + ", weight: " + Weight + " on " + gameObject.name);

        if (IsServer)
        {
            if (ActualTemperature != Temperature) // only update if actual and target temperatures differ
            {
                float tempDiff = Mathf.Abs(ActualTemperature - Temperature) * 0.05f; // use 5% of temperature difference
                if (tempDiff > initialHeatFlowRate.Value)
                {
                    HeatFlowRate = tempDiff; // use corresponding fracture of temperature difference as heatflow rate if it exceeds the initial heatflow rate of the object
                }
                else
                {
                    HeatFlowRate = InitialHeatFlowRate;
                }
                HeatFlowPerFrame = HeatFlowRate / 50f; // always adjust the HeatFlowPerFrame value for the FixedUpdate() function
                if (ActualTemperature < Temperature) // if actualTemperature is lower then target temperature, it should increase, and decrease otherwise.
                {
                    ActualTemperature += HeatFlowPerFrame;
                    if (ActualTemperature > Temperature)
                    {
                        ActualTemperature = Temperature; // if target has been overshot, correct it
                    }
                }
                else
                {
                    ActualTemperature -= HeatFlowPerFrame;
                    if (ActualTemperature < Temperature)
                    {
                        ActualTemperature = Temperature; // if target has been overshot, correct it
                    }
                }
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        // TODO how fast should target temperature be increased? using dummy calculations for now

        ITemperatureSource temperatureSource = other.GetComponent<ITemperatureSource>();
        if (temperatureSource != null)
        {
            float otherTemp = temperatureSource.GetTemperature();
            Temperature += (otherTemp - Temperature) * 0.01f * Time.deltaTime;
        }
    }
}
