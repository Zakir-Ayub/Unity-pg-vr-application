using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ObjectProperties : NetworkObjectproperties
{
    NetworkObjectproperties[] networkObjectproperties = {};

    [SerializeField, Tooltip("Weight of the object in gram.")]
    public NetworkVariable<float> weight = new NetworkVariable<float>();
    public override float Weight
    {
        get 
        {
            return weight.Value + networkObjectproperties.Sum(x => x.Weight); 
        }
        set
        {
            if (IsServer)
            {
                weight.Value = value;
            }
        }
    }

    [SerializeField, Tooltip("Initial temperature of the object in °C")]
    private NetworkVariable<float> temperature = new NetworkVariable<float>();
            
    /// <summary>
    /// Manages object temperature. Gets the current <see cref="actualTemperature"/> of the object and sets the temperature target,
    /// towards which <see cref="actualTemperature"/> will the trend according to <see cref="heatFlowRate"/>.
    /// </summary>
    public override float Temperature
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
   
    /// <summary>
    /// The phValue of the object, on the scale of (mostly) 1-14.
    /// </summary>
    //private float heatFlowPerFrame;
    [SerializeField, Tooltip("pH Value.")]
    private NetworkVariable<float> phValue = new NetworkVariable<float>();
    public override float PhValue
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

    protected new void Awake()
    {
        base.Awake();
        actualTemperature.Value = temperature.Value;
        networkObjectproperties = GetComponentsInChildren<NetworkObjectproperties>().Where(x => x != this).ToArray();
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
