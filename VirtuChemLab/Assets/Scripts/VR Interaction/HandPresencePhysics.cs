using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class HandPresencePhysics : MonoBehaviour
{
    // The controller which the hand follows
    public XRBaseController actionController;
    // The transform of the controller
    public Transform handTarget;
    // The Rigid body of the hand
    private Rigidbody rb;
    // the colliders of the hand
    private Collider[] handColliders;

    [SerializeField]
    LayerMask handInterctions = (1 << 0);
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        handTarget = actionController.modelParent;
        // subscribe to interactor events
        XRBaseControllerInteractor interactor = actionController.GetComponentInChildren<XRBaseControllerInteractor>();
        interactor.selectEntered.AddListener((args) => { DisableHandCollider(); });
        interactor.selectExited.AddListener((args) => { StartCoroutine(EnableHandCollider()); });
        // Get hand colliders
        handColliders = GetComponentsInChildren<Collider>();

        // subscribe to locomotion events
        var lps = FindObjectsOfType<LocomotionProvider>();
        foreach (var lp in lps)
        {
            lp.beginLocomotion += (args) => { DisableHandCollider(); };
            lp.endLocomotion += OnEndLM;
        }
    }

    IEnumerator EnableHandCollider()
    {
        Collider[] collisions;
        do
        {
            yield return new WaitForSeconds(0.1f);
            collisions = Physics.OverlapSphere(transform.position, 0.15f, handInterctions, QueryTriggerInteraction.Ignore);
        } while (collisions.Length > 0);

        foreach (var item in handColliders)
            item.enabled = true;
    }

    void DisableHandCollider()
    {
        foreach (var item in handColliders)
            item.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Show non physics Hands if too far away
        float distance = Vector3.Distance(transform.position, handTarget.position);
        actionController.hideControllerModel = distance < 0.05f;
    }

    public void OnEndLM(LocomotionSystem lms)
    {
        transform.position = handTarget.position;
        rb.velocity = Vector3.zero;
        StartCoroutine(EnableHandCollider());
    }

    private void FixedUpdate()
    {
        //move hand by setting the velocity
        var velocity = (handTarget.position - transform.position) / Time.fixedDeltaTime;
        rb.velocity = velocity;
        //hand rotation
        Quaternion rotDiff = handTarget.rotation * Quaternion.Inverse(transform.rotation);
        rotDiff.ToAngleAxis(out float angleDegree, out Vector3 rotationAxis);
        if (float.IsInfinity(rotationAxis.x))
            return;
        if (angleDegree > 180) angleDegree -= 360;
        Vector3 rotDiffRad = angleDegree * rotationAxis.normalized * Mathf.Deg2Rad;
        rb.angularVelocity = rotDiffRad / Time.fixedDeltaTime;
    }
}
