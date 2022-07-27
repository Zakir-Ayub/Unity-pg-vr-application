using Network.Device;
using Unity.Netcode;
using UnityEngine;

public class ObjectProperties : NetworkBehaviour
{
    [Tooltip("Enable temperature logging in the console")]
    public bool enableLog;

    [SerializeField, Tooltip("Weight of the object in gram.")]
    public NetworkDeviceValue<float> weight = new NetworkDeviceValue<float>();
    public float Weight
    {
        get => weight.Value;
        set
        {
            if (IsServer)
            {
                weight.Value = value;
            }
        }
    }

    [SerializeField, Tooltip("Initial temperature of the object in °C")]
    private NetworkDeviceValue<float> temperature = new NetworkDeviceValue<float>();
    
    /// <summary>
    /// Actual temperature of the object. Trends towards target <see cref="temperature"/> each frame according to <see cref="heatFlowRate"/>.
    /// </summary>
    private NetworkDeviceValue<float> actualTemperature = new NetworkDeviceValue<float>();
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
    
    /// <summary>
    /// Manages object temperature. Gets the current <see cref="actualTemperature"/> of the object and sets the temperature target,
    /// towards which <see cref="actualTemperature"/> will the trend according to <see cref="heatFlowRate"/>.
    /// </summary>
    public float Temperature
    {
        get => temperature.Value;
        set
        {
            if (IsServer)
            {
                temperature.Value = value;
            }
        }
    }
    
    [SerializeField, Tooltip("The rate with which the object's temperature can change, in °C per second")]
    private NetworkDeviceValue<float> heatFlowRate = new NetworkDeviceValue<float>();
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
    /// The rate with which the object's temperature can change, in °C per frame. Always derived from the per-second <see cref="heatFlowRate"/>.
    /// </summary>
    //private float heatFlowPerFrame;
    private NetworkDeviceValue<float> heatFlowPerFrame = new NetworkDeviceValue<float>();
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


    /// <summary>
    /// The phValue of the object, on the scale of (mostly) 1-14.
    /// </summary>
    //private float heatFlowPerFrame;
    [SerializeField, Tooltip("pH Value.")]
    private NetworkDeviceValue<float> phValue = new NetworkDeviceValue<float>();
    public float PhValue
    {
        get => phValue.Value;
        set
        {
            if (IsServer)
            {
                phValue.Value = value;
            }
        }
    }

    void Awake()
    {
        actualTemperature.Value = temperature.Value;
        heatFlowPerFrame.Value = heatFlowRate.Value / 50f;
    }

    // FixedUpdate is called 50 times per second
    private void FixedUpdate()
    {
        if(enableLog) Debug.Log("actual temp: " + actualTemperature.Value + ", target temp: " + temperature.Value + " on " + gameObject.name);

        if (IsServer)
        {
            if (ActualTemperature != Temperature) // only update if actual and target temperatures differ
            {
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (IsServer)
        {
            Temperature = temperature.Value;
        }
    }
#endif
}
