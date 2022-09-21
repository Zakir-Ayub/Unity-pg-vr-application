using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class RotatingButton : MonoBehaviour
{
    protected XRSimpleInteractable simpleinteractable;

    protected private Quaternion inverseHandStart;
    protected private Quaternion buttonStart;

    protected enum Axis
    {
        X,
        Y,
        Z
    }

    // The axis along which the hand is used as an input
    [SerializeField]
    Axis input_axis = Axis.Z;

    // The axis along which the button is rotated
    [SerializeField]
    Axis output_axis = Axis.Z;

    [SerializeField]
    bool invert_rotation = false;

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

    /// <summary>
    /// Return vector where the selected axis is modified
    /// </summary>
    protected Vector3 SelectRotation(Vector3 rotationDifference)
    {
        return input_axis switch
        {
            Axis.X => SelectRotation(rotationDifference.x),
            Axis.Y => SelectRotation(rotationDifference.y),
            Axis.Z => SelectRotation(rotationDifference.z),
            _ => Vector3.zero
        };
    }

    /// <summary>
    /// Return vector where the selected axis is modified
    /// </summary>
    protected Vector3 SelectRotation(float rotationDifference)
    {
        if (invert_rotation)
            rotationDifference = -rotationDifference;

        return output_axis switch
        {
            Axis.X => new Vector3(rotationDifference, 0, 0),
            Axis.Y => new Vector3(0, rotationDifference, 0),
            Axis.Z => new Vector3(0, 0, rotationDifference),
            _ => Vector3.zero
        };
    }

    protected virtual void Update()
    {
        var currentInteractor = simpleinteractable.firstInteractorSelecting;
        if (currentInteractor == null)
            return;

        var handDiff = inverseHandStart * currentInteractor.transform.rotation;
        var clean = SelectRotation(handDiff.eulerAngles);

        transform.rotation = buttonStart * Quaternion.Inverse(Quaternion.Euler(clean));
    }
}
