using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PouringParticles : MonoBehaviour
{
    [Tooltip("The ParticleSystem that spawns the water particles")]
    public ParticleSystem waterParticleSystem;

    void Start()
    {
        WaterController ownWaterController = gameObject.GetComponentInParent<WaterController>();
        int index = 0;
        foreach (GameObject chemObj in GameObject.FindGameObjectsWithTag("Chemical")) // iterate through all GameObjects in scene with chemical tag
        {
            if (chemObj.name == "Water") // check if name is equal to "Water" --> our naming convention
            {
                if (ownWaterController == chemObj.GetComponentInParent<WaterController>()) continue; // if collider is from same GameObject, ignore
                var trigger = waterParticleSystem.trigger;
                trigger.SetCollider(index, chemObj.GetComponent<Collider>());
                index++;
            }
        }
    }

    void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> entered = new List<ParticleSystem.Particle>();

        int numEntered = waterParticleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, entered, out var enteredData);

        for (int i = 0; i < numEntered; i++)
        {
            var other = enteredData.GetCollider(i, 0);
            WaterController otherWaterController = other.GetComponentInParent<WaterController>();
            otherWaterController.IncreaseWaterAmount(); // increase water amount on hit object
            ParticleSystem.Particle p = entered[i];
            p.remainingLifetime = -1; // below 0 -> destroys particle immediately
            entered[i] = p;
        }

        waterParticleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, entered); // apply the changes to the particle system
    }
}
