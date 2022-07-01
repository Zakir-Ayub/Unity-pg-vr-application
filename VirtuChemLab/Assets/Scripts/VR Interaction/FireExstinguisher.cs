using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class FireExstinguisher : MonoBehaviour
{
    XRGrabInteractable interactableBase;
    
    [Tooltip("The particle system that is controlled")]
    [SerializeField] ParticleSystem bubbleParticleSystem = null;
    
    //Time the trigger has to be held for activation
    const float heldThreshold = 0.1f;

    //Time the trigger is held currently
    float triggerHeldTime;
    //If the trigger is currently down
    bool triggerDown;

    protected void Start()
    {
        // Subscribe to Interactable Events
        interactableBase = GetComponent<XRGrabInteractable>();
        interactableBase.selectExited.AddListener(Dropped);
        interactableBase.activated.AddListener(TriggerPulled);
        interactableBase.deactivated.AddListener(TriggerReleased);
    }

    protected void Update()
    {
        if (triggerDown)
        {
            triggerHeldTime += Time.deltaTime;
            // Only activate once the trigger is held the specified amount
            if (triggerHeldTime >= heldThreshold)
            {
                if (!bubbleParticleSystem.isPlaying)
                {
                    // start particle system
                    bubbleParticleSystem.Play();    
                }
            }
        }
    }

    // Stop Particle System
    void stop(){
        triggerDown = false;
        triggerHeldTime = 0f;
        bubbleParticleSystem.Stop();
    }

    // Event Handlers

    void TriggerReleased(DeactivateEventArgs args)
    {
        stop();
    }

    void TriggerPulled(ActivateEventArgs args)
    {
        triggerDown = true;
    }

    void Dropped(SelectExitEventArgs args)
    {
        stop();
    }

    public void ShootEvent()
    {
        bubbleParticleSystem.Emit(1);
    }
}
