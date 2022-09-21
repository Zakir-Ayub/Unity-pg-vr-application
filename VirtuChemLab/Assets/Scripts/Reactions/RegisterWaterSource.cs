using UnityEngine;

/// <summary>
/// Registers WaterTap at ParticleRegistry and sets the PS trigger
/// </summary>
[RequireComponent(typeof(ParticleTriggerListener))]
public class RegisterWaterSource : MonoBehaviour
{
    public ElementAmount amount;

    void Start()
    {
        ParticleRegistry.Singleton.RegisterDefaultAmount(transform.parent.gameObject, amount);
    }
}
