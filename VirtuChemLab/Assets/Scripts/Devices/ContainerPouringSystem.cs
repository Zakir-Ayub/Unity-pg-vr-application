using UnityEngine;

/// <summary>
/// Class that is used for objects with a container. Can spawn particles from its container.
/// </summary>
public class ContainerPouringSystem : PouringSystem, IContainerObject
{
    [Tooltip("The amount of water in ml each water particle is worth")]
    public float mlPerParticle = 1.0f;
    [Tooltip("The maximum emission rate over time for the particle system when held upside-down")]
    public int maxEmissionRateOverTime = 200;

    protected ElementContainer container = new ElementContainer();
    public ElementContainer Container => container;

    protected override ElementAmounts GetNextElementAmount()
    {
        return Container.RemoveAmount(mlPerParticle);
    }

    protected ElementAmounts GetNextLiquidElementAmountWithoutDissolved()
    {
        return Container.RemoveFilteredAmount(mlPerParticle);
    }

    protected virtual new bool ShouldEmit()
    {
        return base.ShouldEmit() && !container.IsEmpty();
    }

    protected void EmitParticle()
    {
        var nextParticle = GetNextElementAmount();
        if (nextParticle.IsEmpty()) return;
        var ep = new ParticleSystem.EmitParams
        {
            startColor = nextParticle.GetColor()
        };
        pouringParticleSystem.Emit(ep, 1); // create particle and register
        ParticleRegistry.Singleton.RegisterNewParticles(pouringParticleSystem, nextParticle);
        lastEmit = Time.time;
    }

    protected void EmitFilteredParticle()
    {
        var nextParticle = GetNextLiquidElementAmountWithoutDissolved();
        if (nextParticle.IsEmpty()) return;
        var ep = new ParticleSystem.EmitParams
        {
            startColor = nextParticle.GetColor()
        };
        pouringParticleSystem.Emit(ep, 1); // create particle and register
        ParticleRegistry.Singleton.RegisterNewParticles(pouringParticleSystem, nextParticle);
        lastEmit = Time.time;
    }

    protected void FixedUpdate()
    {
        float rotationLevel = (Vector3.Dot(transform.up, desiredRotation) + 1) / 2;
        if (ShouldEmit() && rotationLevel > 0)
        {
            // calculate emission rate based on rotation degree and max rate
            float emitCooldown = 1.0f / (maxEmissionRateOverTime * rotationLevel); 
            if (Time.time > lastEmit + emitCooldown)
            {
                // start pouring water if not empty and object is rotated above threshold
                EmitParticle();
            }
        }
    }
}
