using UnityEngine;

public class PipetteController : FluidContainerSystem
{
   [Tooltip("The controller that handles the pipette animation")]
    public Pipette_Animation animationController;

   // the ElementContainer that the pipette is currently colliding with
    private ElementContainer collidedElementContainer;

    // the current trigger value of the controller
    private float currentTriggerValue;
    // the trigger value of the controller in the last update
    private float lastTriggervalue;

    // the amount of (rest) particles that could not be emitted yet
    private float particlesRest = 0;

    private float triggerDifference = 0;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        // get initial components, set initial water, stop initial water emission
        container.maxStorage = 10;

        currentTriggerValue = 0;
        lastTriggervalue = 0;
    }

   
    new void FixedUpdate()
    {
        currentTriggerValue = animationController.triggerValue;        
        triggerDifference = currentTriggerValue - lastTriggervalue;

        float rateOverTime = (container.maxStorage / mlPerParticle) * triggerDifference; // calculate emission rate based on trigger difference and max amount of particles for the pipette
        particlesRest = Mathf.Max(particlesRest + rateOverTime,0); // calculate the number of particles to emit
        
        if (ShouldEmit()) // if trigger is continues being pressed more
        {            
            float emitCooldown = particlesRest > 5 ? 0.04f: 0.2f; // more emissions with more pressure 
            if (Time.time > lastEmit + emitCooldown)
            {
                EmitParticle();
                particlesRest -= 1;
            }
        }
        else
        {
            if (collidedElementContainer != null)
            {
                if (collidedElementContainer.FluidMass() > 0) // if collided WaterController has water in it
                {
                    if (triggerDifference < 0) // if trigger continues being released more
                    {
                        float suctionAmount = container.maxStorage * triggerDifference * -1; // calculate emit amount based on trigger difference and max volume of the pipette, -1 because of release motion
                        ElementAmounts collectedAmounts = collidedElementContainer.RemoveAmount(suctionAmount); // decrease water in hit object
                        container.AddElementAmount(collectedAmounts);
                    }
                }
            }
        }
        lastTriggervalue = currentTriggerValue;
        UpdateWaterShader();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Chemical")) return; // if it does not have the Chemical tag it should be ignored
        if (other.gameObject.GetComponent("PipetteController") != null) return; // if collision with pipette itself, ignore

        ElementContainer elementContainer = other.gameObject.GetComponentInParent<IContainerObject>().Container;
        if (elementContainer == null) return;
        collidedElementContainer = elementContainer;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Chemical")) return; // if it does not have the Chemical tag it should be ignored
        if (other.gameObject.GetComponent("PipetteController") != null) return; // if collision with pipette itself, ignore

        ElementContainer elementContainer = other.gameObject.GetComponentInParent<IContainerObject>().Container;
        if (elementContainer == null) return;
        collidedElementContainer = null;
    }

    protected override bool ShouldEmit()
    {
        if (particlesRest > 0 && container.Mass() > 0) // if trigger is continues being pressed more
        {
            return true;
        }
        return false;
    }
}
