using Network.XR;
using UnityEngine;
using UnityEngine.Assertions;

// This GrabInteractable is not teleported into the hand but instead can be dynamically grabbed
public class DynamicGrabInteractable : NetworkXRGrabInteractable
{
    // Start is called before the first frame update
    void Start()
    {
        //Create DynamicAttachTransform that moves the attachmentpoint to allow dynamic grabbing
        Assert.IsTrue(base.attachTransform == null, "Attachmentpoint already set of " + name);
        var attach = new GameObject("DynamicAttachmentPoint").transform;
        attach.parent = transform;
        base.attachTransform = attach;
        attach.gameObject.AddComponent<DynamicAttachTransform>();
        attachTransform = attach;

    }

}
