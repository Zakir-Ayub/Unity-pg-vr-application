using System;
using Network.XR;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

// This GrabInteractable is not teleported into the hand but instead can be dynamically grabbed
public class DynamicGrabInteractable : NetworkXRGrabInteractable
{
    private Transform attach;
    // Start is called before the first frame update
    void Start()
    {
        //Create DynamicAttachTransform that moves the attachmentpoint to allow dynamic grabbing
        Assert.IsTrue(base.attachTransform == null, "Attachmentpoint already set of " + name);
        attach = new GameObject("DynamicAttachmentPoint").transform;
        attach.parent = transform;
        attach.gameObject.AddComponent<DynamicAttachTransform>();

    }

    public override Transform GetAttachTransform(IXRInteractor interactor)
    {
        if(interactor is XRDirectInteractor) return attach;
        return base.GetAttachTransform(interactor);
    }
}
