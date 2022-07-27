using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

// Helper Script for DynamicGrabInteractable that moves the attachmentpoint to the controller position when grabbing
public class DynamicAttachTransform : MonoBehaviour
{
    XRGrabInteractable grabInteractable;

    void Start()
    {
        grabInteractable = GetComponentInParent<XRGrabInteractable>();
        Assert.IsTrue(grabInteractable != null, "No grab interactable " + transform.name);
        Assert.IsTrue(grabInteractable.attachTransform == transform, "Attachment point not configured! " + name);
        grabInteractable.selectEntered.AddListener(SelectEntered);
    }

    void SelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor || args.interactorObject is XRRayInteractor)
        {

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            return;
        }
        Transform interactor = args.interactorObject.transform;
        transform.SetPositionAndRotation(interactor.position, interactor.rotation);
    }

}
