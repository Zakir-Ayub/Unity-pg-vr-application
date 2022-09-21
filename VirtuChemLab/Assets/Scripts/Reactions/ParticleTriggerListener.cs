using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to check if the particles of the particle system hit a container.
/// If it does, add the particles content to the container.
/// </summary>
public class ParticleTriggerListener : MonoBehaviour
{
    ParticleSystem pouringParticleSystem;
    GameObject root;

    private void Start()
    {
        pouringParticleSystem = GetComponent<ParticleSystem>();
        root = transform.parent.gameObject;

        // set particle system to trigger when hitting "water" containers
        int triggerIndex = 0;
        IContainerObject ownContainerController = root.GetComponent<IContainerObject>();
        var trigger = pouringParticleSystem.trigger;
        foreach (GameObject chemObj in GameObject.FindGameObjectsWithTag("Chemical")) // iterate through all GameObjects in scene with chemical tag
        {
            if (chemObj.name == "Water") // check if name is equal to "Water" --> our naming convention for the trigger
            {
                if (ownContainerController == chemObj.GetComponentInParent<IContainerObject>()) continue; // if collider is from same GameObject, ignore
                trigger.SetCollider(triggerIndex, chemObj.GetComponent<Collider>());
                triggerIndex++;
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
            IContainerObject otherController = other.GetComponentInParent<IContainerObject>();
            ParticleSystem.Particle p = entered[i];
            var amount = ParticleRegistry.Singleton.GetParticleAmounts(root, p.randomSeed);
            otherController.Container.AddElementAmount(amount);
            p.remainingLifetime = -1; // below 0 -> destroys particle immediately
            entered[i] = p;
        }
        ParticleRegistry.Singleton.UnregisterParticle(entered);
        pouringParticleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, entered); // apply the changes to the particle system
    }
}
