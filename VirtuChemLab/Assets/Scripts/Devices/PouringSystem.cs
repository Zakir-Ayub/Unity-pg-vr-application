using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PouringSystem : MonoBehaviour
{
    public enum LabMaterial
    {
        Water,
        Sand
    }

    [Tooltip("The ParticleSystem that spawns the particles")]
    public ParticleSystem pouringParticleSystem;

    [Tooltip("The Material that should be poured")]
    public LabMaterial material;

    // the desired rotation of the container (for sand)
    private Vector3 desiredRotation = Vector3.down;
    // the desired minimum rotation until sand is poured
    private float rotationMinLikeness = 0.5f;

    void Start()
    {
        int index = 0;

        switch (material)
        {
            case LabMaterial.Water:
                {
                    WaterController ownWaterController = gameObject.GetComponentInParent<WaterController>();
                    foreach (GameObject chemObj in GameObject.FindGameObjectsWithTag("Chemical")) // iterate through all GameObjects in scene with chemical tag
                    {
                        if (chemObj.name == "Water") // check if name is equal to "Water" --> our naming convention
                        {
                            if (ownWaterController == chemObj.GetComponentInParent<WaterController>()) continue; // if collider is from same GameObject, ignore
                            var trigger = pouringParticleSystem.trigger;
                            trigger.SetCollider(index, chemObj.GetComponent<Collider>());
                            index++;
                        }
                    }
                    break;
                }
            case LabMaterial.Sand:
                {
                    // sand should collide with water as well
                    foreach (GameObject chemObj in GameObject.FindGameObjectsWithTag("Chemical")) // iterate through all GameObjects in scene with chemical tag
                    {
                        if (chemObj.name == "Water") // check if name is equal to "Water" --> our naming convention
                        {
                            var trigger = pouringParticleSystem.trigger;
                            trigger.SetCollider(index, chemObj.GetComponent<Collider>());
                            index++;
                        }
                    }
                    break;
                }
            default:
                {
                    throw new ArgumentException("No Material selected in PouringSystem script");
                }
        }
    }

    void FixedUpdate()
    {
        switch (material)
        {
            case LabMaterial.Water:
                {
                    // nothing for water
                    break;
                }
            case LabMaterial.Sand:
                {
                    // enable sand particle system once the threshold is reached
                    float rotationLevel = (Vector3.Dot(transform.up, desiredRotation) + 1) / 2;
                    if (rotationLevel > rotationMinLikeness)
                    {
                        if (!pouringParticleSystem.isPlaying)
                        {
                            pouringParticleSystem.Play();
                        }
                    }
                    else
                    {
                        if (pouringParticleSystem.isPlaying)
                        {
                            pouringParticleSystem.Stop();
                        }
                    }
                    break;
                }
            default:
                {
                    throw new ArgumentException("No Material selected in PouringSystem script");
                }
        }
    }

    void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> entered = new List<ParticleSystem.Particle>();
        int numEntered = pouringParticleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, entered, out var enteredData);

        switch (material)
        {
            case LabMaterial.Water:
                {
                    for (int i = 0; i < numEntered; i++)
                    {
                        var other = enteredData.GetCollider(i, 0);
                        WaterController otherWaterController = other.GetComponentInParent<WaterController>();
                        otherWaterController.IncreaseWaterAmount(); // increase water amount on hit object
                        ParticleSystem.Particle p = entered[i];
                        p.remainingLifetime = -1; // below 0 -> destroys particle immediately
                        entered[i] = p;
                    }
                    break;
                }
            case LabMaterial.Sand:
                {
                    for (int i = 0; i < numEntered; i++)
                    {
                        var other = enteredData.GetCollider(i, 0);
                        if (other.name == "Water") // check if name is equal to "Water" --> our naming convention
                        {
                            Material waterShaderMat = other.GetComponent<Renderer>().material;
                            WaterController otherWaterController = other.GetComponentInParent<WaterController>();
                            if (otherWaterController.currentSandParticles < otherWaterController.sandParticlesUntilSaturation)
                            {
                                otherWaterController.currentSandParticles++;
                                // if we need color from ParticleSystem: particleSystem.main.startColor.colorMax
                                // change the color of the water shader depending on mixture ratio and lerp color from initial color to sand-ish
                                waterShaderMat.SetColor("_SideColor", Color.Lerp(otherWaterController.initialWaterColorSide, new Color32(122, 71, 18, 1), (float) otherWaterController.currentSandParticles / (float) otherWaterController.sandParticlesUntilSaturation));
                                waterShaderMat.SetColor("_TopColor", Color.Lerp(otherWaterController.initialWaterColorTop, new Color32(122, 71, 18, 1), (float)otherWaterController.currentSandParticles / (float)otherWaterController.sandParticlesUntilSaturation));
                                ParticleSystem.Particle p = entered[i];
                                p.remainingLifetime = -1; // below 0 -> destroys particle immediately
                                entered[i] = p;
                            }
                        }
                    }
                    break;
                }
            default:
                {
                    throw new ArgumentException("No Material selected in PouringSystem script");
                }
        }

        pouringParticleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, entered); // apply the changes to the particle system
    }
}
