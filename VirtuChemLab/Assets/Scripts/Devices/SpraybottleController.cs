using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using Network.XR;
using Unity.Netcode;
using System;
using System.Collections.Generic;

public class SpraybottleController : NetworkXRGrabInteractable
{

    [Tooltip("The particle system of the water")]
    [SerializeField] ParticleSystem sprayParticleSystem = null;

    [Tooltip("The maximum emission rate over time for the particle system when held upside-down")]
    public int maxEmissionRateOverTime = 500;

    // the time the trigger has to be held for activation
    private const float heldThreshold = 0.2f;
    // the GrabInteractable of the object
    private XRGrabInteractable interactableBase;
    // tracks the time the trigger is held for
    private float triggerHeldTime;

    // tracks hwo far the trigger is pulled
    private float triggerValue;

    [Tooltip("The WaterlevelSync that deals with the water level of this object")]
    public WaterlevelSync waterlevelSync;

    //for sound purposes
    [HideInInspector]
    public bool isSpraying=false;

    // State transition actions
    [SerializeField]
    InputActionReference triggerAction;

    [SerializeField]
    InputActionReference grabAction;

    //Object Type of Animator
    public Animator SprayBottleAnim;

    protected void Start()
    {
        sprayParticleSystem.Stop(); // should not be running by default
        // subscribe to Interactable Events
        interactableBase = GetComponent<XRGrabInteractable>();
        interactableBase.selectExited.AddListener(Dropped);
        interactableBase.deactivated.AddListener(TriggerReleased);
    }

    protected void Update()
    {
        triggerValue = triggerAction.action.ReadValue<float>(); // get trigger pull value
        
        if(sprayParticleSystem.isPlaying)// SprayBottle squeeze animation plays if "sprayParticleSystem" emits water particles
        {
            SprayBottleAnim.SetFloat("BottleSqueezeSpeed", triggerValue);// Using Trigger value as animation speed and direction 
        }

        if(!sprayParticleSystem.isPlaying)
        {
            SprayBottleAnim.SetFloat("BottleSqueezeSpeed", 0.0f);// Resets and stops bottle animation permanently if water is finished
        }
    }

    // override main interactable from XRGrabInteractable
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase) 
    {
        // if (at least) one hand is grabbing the spraybottle
        if (interactorsSelecting.Count > 0)
        {
            base.ProcessInteractable(updatePhase);
            // true once trigger is pressed slightly
            if (triggerValue > 0 && waterlevelSync.WaterAmount > 0)
            {
                triggerHeldTime += Time.deltaTime;
                // only activate once the trigger is held for at least the specified amount
                if (triggerHeldTime >= heldThreshold)
                {
                    if (!sprayParticleSystem.isPlaying)
                    {
                        // start ParticleSystem
                        sprayParticleSystem.Play();
                        isSpraying = true;
                    }
                    var em = sprayParticleSystem.emission;
                    int rateOverTime = Mathf.RoundToInt(maxEmissionRateOverTime * triggerValue); // calculate emission rate based on trigger value and max rate, round for more consistent calculations
                    em.rateOverTime = rateOverTime;
                    waterlevelSync.DecreaseWaterAmount(rateOverTime); // lower water level
                }
            }
            else {
                // stop emitting particles when trigger not held
                var em = sprayParticleSystem.emission;
                em.rateOverTime = 0;
                if (sprayParticleSystem.isPlaying)
                {
                    // stop ParticleSystem
                    isSpraying = false;
                    sprayParticleSystem.Stop();
                }
            }
        }
    }

    // stops the ParticleSystem
    private void Stop()
    {
        isSpraying = false;
        triggerHeldTime = 0f;
        sprayParticleSystem.Stop();
    }

    // Event Handlers

    void TriggerReleased(DeactivateEventArgs args)
    {
        Stop();
    }

    void Dropped(SelectExitEventArgs args)
    {
        Stop();
    }
}
