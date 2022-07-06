using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class RotatingButton : MonoBehaviour
{
    // The Hand that currently grabbed the button
    XRSimpleInteractable simpleinteractable;

    private Quaternion inverseHandStart;
    private Quaternion buttonStart;

    enum Axis
    {
        X,
        Y,
        Z
    }

    // The axis along which the button is rotated
    [SerializeField]
    Axis axis = Axis.Z;

    // Start is called before the first frame update
    void Start()
    {
        simpleinteractable = GetComponent<XRSimpleInteractable>();
        simpleinteractable.selectEntered.AddListener(ButtonGrabbed);
    }

    public void ButtonGrabbed(SelectEnterEventArgs args)
    {
        inverseHandStart = Quaternion.Inverse(args.interactorObject.transform.rotation);
        buttonStart = transform.rotation;
    }

    private Vector3 setRotation(Vector3 goalRotation)
    {
        // return vector where the selected axis is modified
        return axis switch
        {
            Axis.X => new Vector3(goalRotation.x, transform.eulerAngles.y, transform.eulerAngles.z),
            Axis.Y => new Vector3(transform.eulerAngles.x, goalRotation.y, transform.eulerAngles.z),
            Axis.Z => new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, goalRotation.z),
            _ => Vector3.zero
        };
    }

    // FixedUpdate is called once per physics tick
    void FixedUpdate()
    {
        var currentInteractor = simpleinteractable.firstInteractorSelecting;
        if (currentInteractor == null)
            return;
        var handDiff = inverseHandStart * currentInteractor.transform.rotation;
        var newRotation = buttonStart * Quaternion.Inverse(handDiff);
        transform.eulerAngles = setRotation(newRotation.eulerAngles);
    }
}
