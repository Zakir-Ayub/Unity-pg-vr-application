using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandPresencePhysics : MonoBehaviour
{
    // The controller which the hand follows
    public XRBaseController actionController;
    // The transform of the controller
    private Transform handTarget;
    // The Rigid body of the hand
    private Rigidbody rb;
    // the colliders of the hand
    private Collider[] handColliders;

    private bool teleport = false;

    [SerializeField]
    LayerMask handInterctions = (1 << 0);
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        handTarget = actionController.modelParent;
        // subscribe to interactor events
        XRBaseControllerInteractor interactor = actionController.GetComponentInChildren<XRBaseControllerInteractor>();
        interactor.selectEntered.AddListener(DisableHandCollider);
        interactor.selectExited.AddListener(EnableHandColliderDelay);
        // Get hand colliders
        handColliders = GetComponentsInChildren<Collider>();
        // subscribe to teleport events
        var tp_areas = FindObjectsOfType<TeleportationArea>();
        foreach (var tp_area in tp_areas)
            tp_area.teleporting.AddListener(OnTeleport);
    }

    void EnableHandColliderDelay(SelectExitEventArgs args) {
        // wait 0.5 seconds before enabling to avoid the instant collision
        Invoke("EnableHandCollider", 0.1f);
    }

    void EnableHandCollider(){
        var collisions = Physics.OverlapSphere(transform.position, 0.15f, handInterctions,QueryTriggerInteraction.Ignore);
        if(collisions.Length > 0)
            Invoke("EnableHandCollider", 0.1f);
        else
            foreach (var item in handColliders)
                item.enabled = true;
    }

    void DisableHandCollider(SelectEnterEventArgs args){
        foreach(var item in handColliders)
            item.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Show non physics Hands if too far away
        float distance = Vector3.Distance(transform.position, handTarget.position);
        actionController.hideControllerModel = distance < 0.05f;
    }

    public void OnTeleport(TeleportingEventArgs args) { teleport = true; }

    private void FixedUpdate() {
        //move hand by setting the velocity
        var velocity = (handTarget.position - transform.position) / Time.fixedDeltaTime;
        if (teleport)
        { 
            transform.position = handTarget.position;
            rb.velocity = Vector3.zero;
            teleport = false;
        }
        else
            rb.velocity = velocity;
        //hand rotation
        Quaternion rotDiff = handTarget.rotation * Quaternion.Inverse(transform.rotation);
        rotDiff.ToAngleAxis(out float angleDegree, out Vector3 rotationAxis);
        Vector3 rotDiffDegree = angleDegree * rotationAxis;
        // correct rotation
        if (rotDiffDegree.x > 180)
            rotDiffDegree.x -= 360;
        if (rotDiffDegree.y > 180)
            rotDiffDegree.y -= 360;
        if (rotDiffDegree.z > 180)
            rotDiffDegree.z -= 360;
        rb.angularVelocity = rotDiffDegree * Mathf.Deg2Rad / Time.fixedDeltaTime;
    }
}
