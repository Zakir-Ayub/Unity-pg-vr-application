using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipetteEmissionSystem : MonoBehaviour
{
    [Tooltip("The ParticleSystem that spawns the particles")]
    public ParticleSystem pouringParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        int index = 0;

        foreach (GameObject chemObj in GameObject.FindGameObjectsWithTag("Chemical")) // iterate through all GameObjects in scene with chemical tag
        {
            if (chemObj.name == "Water") // check if name is equal to "Water" --> our naming convention
            {
                var trigger = pouringParticleSystem.trigger;
                trigger.SetCollider(index, chemObj.GetComponent<Collider>()); // add found GameObject to trigger list
                index++;
            }
        }
    }

    void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> entered = new List<ParticleSystem.Particle>();
        int numEntered = pouringParticleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, entered, out var enteredData);

        for (int i = 0; i < numEntered; i++)
        {
            var other = enteredData.GetCollider(i, 0);
            WaterController otherWaterController = other.GetComponentInParent<WaterController>();
            // otherWaterController.IncreaseWaterAmount(); // increase water amount on hit object
            ParticleSystem.Particle p = entered[i];
            p.remainingLifetime = -1; // below 0 -> destroys particle immediately
            entered[i] = p;
        }

        pouringParticleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, entered); // apply the changes to the particle system
    }
}
