using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Abstract class that is used for objects that can spawn particles of ElementAmounts
/// </summary>
public abstract class PouringSystem : MonoBehaviour
{
    [Tooltip("The ParticleSystem that spawns the particles")]
    public ParticleSystem pouringParticleSystem;

    [Tooltip("The amount of particles spawned at once")]
    public int emitCount = 1;
    // the time of the last particle emitting
    protected float lastEmit = 0;
    // the desired rotation of the container (for sand)
    protected Vector3 desiredRotation = Vector3.down;
    // the desired minimum rotation until sand is poured
    protected float rotationMinThreshold = 0.5f;

    // should return an element amount that is used for the next particle
    protected abstract ElementAmounts GetNextElementAmount();

    protected void Start()
    {
        Assert.IsNotNull(pouringParticleSystem, name + ": PS null");
        Assert.IsNotNull(pouringParticleSystem.GetComponent<ParticleTriggerListener>(), name + ": PS has no ParticleTriggerListener");
        pouringParticleSystem.Stop();
    }

    // return true if it should emit a particle.
    protected bool ShouldEmit()
    {
        float rotationLevel = (Vector3.Dot(transform.up, desiredRotation) + 1) / 2;
        return rotationLevel > rotationMinThreshold;
    }
    
}
