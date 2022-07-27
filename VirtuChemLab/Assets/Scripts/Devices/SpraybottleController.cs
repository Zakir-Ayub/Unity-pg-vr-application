using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR;

[RequireComponent(typeof(XRGrabInteractable))]
public class SpraybottleController : MonoBehaviour
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

    //for sound purposes
    [HideInInspector]
    public bool isSpraying=false;

    // State transition actions
    [SerializeField]
    InputActionReference triggerAction;

    [SerializeField]
    InputActionReference grabAction;

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
        var triggerValue = triggerAction.action.ReadValue<float>();
        
        // true once trigger is pressed slightly
        if (triggerValue > 0)
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
                em.rateOverTime = maxEmissionRateOverTime * triggerValue; // change emission rate of ParticleSystem based on how much the trigger is held
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
