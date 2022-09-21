using Network.XR;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Can be used by GrabInteractable objects to carry objects on or in them.
/// For this all chemicals inside the radius of the center position are considered.
/// </summary>
public class CarryEnhancer : MonoBehaviour
{
    [Tooltip("The position of the center")]
    [SerializeField]
    Transform center;

    [Tooltip("The radius in which objects are scanned")]
    [SerializeField]
    float radius = 0.02f;

    [Tooltip("The layers in which objects have to be")]
    [SerializeField]
    LayerMask interactionLayer = (1 << 0);

    [Tooltip("The movement distance at which carring starts")]
    [SerializeField]
    float movementThreshhold = 0.01f;

    [Tooltip("The number of ticks the objects continue to be carried (25/second)")]
    [SerializeField]
    int waitTicks = 25;

    [SerializeField]
    [Tooltip("Visualize actions")]
    bool drawDebugLines = false;


    // number of ticks left in which objects are still carried
    int currentWait = 0;

    Vector3 lastPosition;
    Vector3 currentPos;
    Collider[] carriedObjects; // The objects that are currently carried
    Vector3[] offsets; // The relative offsets of the objects to the center

    enum CarryStates
    {
        Idle,
        Started,
        Moving
    }
    // The current carry state
    CarryStates state = CarryStates.Idle;

    NetworkXRGrabInteractable grabInteractable;

    void Start()
    {
        // subscribe to locomotion events
        var lps = FindObjectsOfType<LocomotionProvider>();
        foreach (var lp in lps)
            lp.beginLocomotion += onBeginLocomotion;
        grabInteractable = GetComponent<NetworkXRGrabInteractable>();
        currentPos = center.position;
    }

    void onBeginLocomotion(LocomotionSystem ls)
    {
        // start tracking movement until teleportation        
        if (movementStart())
            state = CarryStates.Started;
        else if(state == CarryStates.Moving)
        {
            // reset counter even when already in Moving mode
            currentWait = waitTicks;
            state = CarryStates.Started;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!drawDebugLines)
            return;
        // Draw a yellow sphere at the transform's position
        Gizmos.color = state==CarryStates.Idle ? Color.yellow : Color.blue;
        Gizmos.DrawWireSphere(center.position, radius);
    }

    private bool movementStart()
    {
        if (state != CarryStates.Idle)
            return false;

        // get chemicals near center
        var colliders = Physics.OverlapSphere(center.position, radius, interactionLayer, QueryTriggerInteraction.Ignore);
        carriedObjects = colliders.Where(x => x.CompareTag("Chemical") && x.GetComponent<Rigidbody>() != null).ToArray();

        // Abort when no objects or not getting carried
        if (carriedObjects.Length == 0 || grabInteractable.firstInteractorSelecting == null)
        {
            return false;
        }

        // get relative offsets
        offsets = carriedObjects.Select(x => Quaternion.Inverse(transform.rotation) * (x.transform.position - currentPos)).ToArray();

        // disable physics of objects
        foreach (var o in carriedObjects)
        {
            o.GetComponent<Rigidbody>().isKinematic = true;
            o.GetComponent<Collider>().enabled = false;
        }
        currentWait = waitTicks;
        return true;
    }

    private void doMovement()
    {
        if(drawDebugLines)
            Debug.DrawLine(lastPosition, currentPos, state == CarryStates.Started ? Color.red : Color.blue, 10f);
        // update the position of the carried objects to stay the same relative to center
        foreach (var oo in carriedObjects.Zip(offsets, Tuple.Create))
        {
            var currentObject = oo.Item1;
            var offset = oo.Item2;
            currentObject.transform.position = currentPos + center.rotation * offset;
        }
    }

    private void movementEnd()
    {
        // reenable physics of pbjects
        foreach (var o in carriedObjects)
        {
            var rb = o.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            o.GetComponent<Collider>().enabled = true;
        }

        state = CarryStates.Idle;
        carriedObjects = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lastPosition = currentPos;
        currentPos = center.position;
        Vector3 diff = currentPos - lastPosition;
        bool belowThreshold = diff.magnitude < movementThreshhold;

        switch (state)
        {
            case CarryStates.Idle:
                // start carrying when movement above threshold
                if (belowThreshold)
                    return;
                if (!movementStart())
                    return;                    
                state = CarryStates.Moving;
                break;              
                
            case CarryStates.Started:
                // change state when teleportation occures
                if (!belowThreshold)
                    state = CarryStates.Moving;
                break;

            case CarryStates.Moving:
                // Stay active until movement is below threshold for the specified number of ticks
                if (belowThreshold)
                    currentWait -= 1;
                // end tracking when waitTicks is 0
                if (currentWait <= 0)                    
                {
                    movementEnd();
                    return;
                }
                break;
        }
        doMovement();
    }
}
