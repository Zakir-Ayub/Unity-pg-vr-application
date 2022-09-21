using Network.XR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CrucibleTongController : MonoBehaviour
{
    // tracks hwo far the trigger is pulled
    private float triggerValue;

    // State transition actions
    [SerializeField]
    InputActionReference triggerAction;


    private XRSocketInteractorSelectedDevices m_InteractorSelectedDevices;

    private NetworkXRGrabInteractable m_NetworkXRGrabInteractable;

    private Transform m_TongArm;

    private const float TriggerThreshold = 0.75f;

    private BoxCollider[] Colliders;
    
    void Start()
    {
        m_InteractorSelectedDevices = GetComponent<XRSocketInteractorSelectedDevices>();
        m_InteractorSelectedDevices.socketActive = false;

        m_TongArm = transform.Find("Cylinder.079");

        m_NetworkXRGrabInteractable = GetComponent<NetworkXRGrabInteractable>();

        Colliders = GetComponentsInChildren<BoxCollider>();
        
        m_NetworkXRGrabInteractable.selectEntered.AddListener(OnTakeInHand);
        m_NetworkXRGrabInteractable.selectExited.AddListener(OnDropOutOfHand);
        /*
        foreach (var boxCollider in Colliders)
        {
            //Disable all colliders except if they are not a trigger (we still need those for the socket interactor)
            boxCollider.enabled = boxCollider.isTrigger;
            
            //The rigidbody should be kinematic while hold, but somehow this doesnt happen. We'll just force that here.
            GetComponent<Rigidbody>().isKinematic = true;
        }
        m_TongArm.Rotate(Vector3.forward, -1 * 9f);*/
    }

    void Update()
    {
        if (m_NetworkXRGrabInteractable.isSelected)
        {
            float triggerOld = triggerValue;
            triggerValue = triggerAction.action.ReadValue<float>(); // get trigger pull value
            m_TongArm.Rotate(Vector3.forward, (triggerOld - triggerValue) * 9f);
        }

        m_InteractorSelectedDevices.socketActive = m_NetworkXRGrabInteractable.isSelected && triggerValue > TriggerThreshold;
        //m_InteractorSelectedDevices.socketActive = true;
    }

    void OnTakeInHand(SelectEnterEventArgs args)
    {
        foreach (var boxCollider in Colliders)
        {
            //Disable all colliders except if they are not a trigger (we still need those for the socket interactor)
            boxCollider.enabled = boxCollider.isTrigger;
            
            //The rigidbody should be kinematic while hold, but somehow this doesnt happen. We'll just force that here.
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    void OnDropOutOfHand(SelectExitEventArgs args)
    {
        foreach (var boxCollider in Colliders)
        {
            boxCollider.enabled = true;
            
            //The rigidbody should be kinematic while hold, but somehow this doesnt happen. We'll just force that here.
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
