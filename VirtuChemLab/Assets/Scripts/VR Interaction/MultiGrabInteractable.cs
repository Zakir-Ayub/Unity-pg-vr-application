using Network.XR;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

// This script lets an object be moveable only when grabbed by 2 hands
[CanSelectMultiple(true)]
public class MultiGrabInteractable : NetworkXRGrabInteractable
{
    Vector3 lastUp = Vector3.up; // cached rotation

    readonly Pose[] InteractorPoses = new Pose[2]; // array to store poses of hands

    // First and second interactor
    IXRSelectInteractor PrimaryInteractor = null;
    IXRSelectInteractor SecondaryInteractor = null;

    protected override void Awake()
    {
        base.Awake();

        // add attach point if missing
        if (attachTransform == null || attachTransform == transform)
        {
            var attach = new GameObject("Attatch").transform;
            attach.parent = transform;
            attach.gameObject.AddComponent<DynamicAttachTransform>();
            attachTransform = attach;
        }

        Assert.IsTrue(base.selectMode == InteractableSelectMode.Multiple, "SelectMode of MultiGrabInteractable " + name + " not set to Multiple");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (PrimaryInteractor != null && SecondaryInteractor == null)
        {
            // Cache last up vector if second interactor is goig to be added
            lastUp = PrimaryInteractor.GetAttachTransform(this).up;
        }

        base.OnSelectEntered(args);

        // save to correct interactor variable
        if (PrimaryInteractor == null)
        {
            PrimaryInteractor = args.interactorObject;
        }
        else
        {
            SecondaryInteractor = args.interactorObject;
            RecenterDynamicAttach();
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (PrimaryInteractor != null && SecondaryInteractor == null)
        {
            // Cache last up vector if dropping last interactor
            lastUp = PrimaryInteractor.GetAttachTransform(this).up;
        }

        base.OnSelectExited(args);

        // remove correct interactor from variables
        if (args.interactorObject.Equals(SecondaryInteractor))
        {
            SecondaryInteractor = null;
            RecenterDynamicAttach();
        }
        else
        {
            PrimaryInteractor = SecondaryInteractor;
            SecondaryInteractor = null;
        }
    }

    // set attachment points correctly
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        // Cache local position + rotation
        var localAttachPosition = attachTransform.localPosition;
        var localAttachRotation = attachTransform.localRotation;

        if (isSelected)
        {
            // If grabbed by 2 perform multi-grab processing
            if (SecondaryInteractor != null)
            {
                var primaryTransform = PrimaryInteractor.GetAttachTransform(this);

                InteractorPoses[0] = new Pose(primaryTransform.position, primaryTransform.rotation);

                var secondaryTransform = SecondaryInteractor.GetAttachTransform(this);
                InteractorPoses[1] = new Pose(secondaryTransform.position, secondaryTransform.rotation);

                var finalPose = ProcessesMultiGrab();

                // set final pose
                attachTransform.SetPositionAndRotation(attachTransform.position + (primaryTransform.position - finalPose.position), 
                    attachTransform.rotation * Quaternion.Inverse(Quaternion.Inverse(primaryTransform.rotation) * finalPose.rotation));
            }
            else
                return; // Do nothing if not grabbed by two hands
        }

        base.ProcessInteractable(updatePhase);

        // Restore the attach transform back to normal
        attachTransform.localPosition = localAttachPosition;
        attachTransform.localRotation = localAttachRotation;
    }

    void RecenterDynamicAttach()
    {
        // If using a dynamic attach, trigger recentering code
        var dynamicAttach = attachTransform.GetComponent<DynamicAttachTransform>();
        if (dynamicAttach != null && dynamicAttach.enabled)
        {
            var primaryTransform = PrimaryInteractor.GetAttachTransform(this);
            if (SecondaryInteractor != null)
            {
                InteractorPoses[0] = new Pose(primaryTransform.position, primaryTransform.rotation);

                var secondaryTransform = SecondaryInteractor.GetAttachTransform(this); ;
                InteractorPoses[1] = new Pose(secondaryTransform.position, secondaryTransform.rotation);

                var finalPose = ProcessesMultiGrab();
                dynamicAttach.transform.SetPositionAndRotation(finalPose.position, finalPose.rotation);
            }
            else
                dynamicAttach.transform.SetPositionAndRotation(primaryTransform.position, primaryTransform.rotation);
        }
    }

    // combine multiple grab point to a single pose
    public Pose ProcessesMultiGrab()
    {
        // Forward is the difference from second hand to first
        var forward = (InteractorPoses[1].position - InteractorPoses[0].position).normalized;

        // Get the unified up value from controllers
        var up = Vector3.Slerp(InteractorPoses[0].up, InteractorPoses[1].up, 0.5f);
        if (Vector3.Dot(up, lastUp) < 0.0f)
            up = -up;        

        var left = Vector3.Cross(forward, up);
        up = Vector3.Cross(left, forward);
        var rotation = Quaternion.LookRotation(forward, up);
        var finalPose = new Pose(InteractorPoses[0].position, rotation);
        lastUp = finalPose.up;

        return finalPose;
    }
}
