using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class that is used in Chemical containers to create particles with chemical content.
/// </summary
[RequireComponent(typeof(ParticleTriggerListener))]
public class FixedAmountPouringSystem : PouringSystem
{

    [Tooltip("The chemicals of each particle")]
    public ElementAmount eamount;

    ElementAmounts eamounts; // used to avoid creating new object each time
    protected override ElementAmounts GetNextElementAmount() => eamounts;

    //time between each spawn cicle
    protected float emitCooldown = 0.1f;

    private XRSocketInteractorSelectedDevices m_LidSocketInteractor;

    protected void FixedUpdate()
    {
        if (ShouldEmit() && Time.time > lastEmit + emitCooldown && !m_LidSocketInteractor.hasSelection)
        {
            lastEmit = Time.time;
            var ep = new ParticleSystem.EmitParams
            {
                startColor = GetNextElementAmount().GetColor()
            };
            pouringParticleSystem.Emit(ep, emitCount);
        }
    }

    protected new void Start()
    {
        base.Start();
        eamounts = new ElementAmounts(eamount);
        // Register fixed amount
        ParticleRegistry.Singleton.RegisterDefaultAmount(transform.parent.gameObject, eamount);
        m_LidSocketInteractor = GetComponentInParent<XRSocketInteractorSelectedDevices>();
    }
}
