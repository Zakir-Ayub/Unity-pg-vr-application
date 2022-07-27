using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR;

[RequireComponent(typeof(XRGrabInteractable))]
public class SpraybottleController : MonoBehaviour
{

    [Tooltip("The particle system of the water")]
    [SerializeField] ParticleSystem particleSystem = null;

    [Tooltip("The maximum emission rate over time for the particle system when held upside-down")]
    public int maxEmissionRateOverTime = 500;

    //the time the trigger has to be held for activation
    private const float heldThreshold = 0.2f;
    //the GrabInteractable of the object
    private XRGrabInteractable interactableBase;
    //tracks the time the trigger is held for
    private float triggerHeldTime;
    //true if the trigger is currently pressed, false otherwise
    private bool triggerDown;

    // State transition actions
    [SerializeField]
    InputActionReference triggerAction;

    [SerializeField]
    InputActionReference grabAction;

    protected void Start()
    {
        particleSystem.Stop();
        // Subscribe to Interactable Events
        interactableBase = GetComponent<XRGrabInteractable>();
        interactableBase.selectExited.AddListener(Dropped);
        interactableBase.activated.AddListener(TriggerPulled);
        interactableBase.deactivated.AddListener(TriggerReleased);
    }

    protected void Update()
    {
        var triggerValue = triggerAction.action.ReadValue<float>();
        
        // if (triggerDown)
        if (triggerValue > 0)
        {
            triggerHeldTime += Time.deltaTime;
            // Only activate once the trigger is held the specified amount
            if (triggerHeldTime >= heldThreshold)
            {
                if (!particleSystem.isPlaying)
                {
                    // start particle system
                    particleSystem.Play();
                }
                var em = particleSystem.emission;
                em.rateOverTime = maxEmissionRateOverTime * triggerValue;
            }
        }
        else {
            var em = particleSystem.emission;
            em.rateOverTime = 0;
        }
    }

    // Stop Particle System
    void stop()
    {
        triggerDown = false;
        triggerHeldTime = 0f;
        particleSystem.Stop();
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
}
