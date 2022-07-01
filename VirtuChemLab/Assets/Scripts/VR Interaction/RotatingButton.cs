using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class RotatingButton : MonoBehaviour
{
    // The Hand that currently grabbed the button
    XRSimpleInteractable simpleinteractable;

    // The rotation of object when grabbing started
    private float startRotation;

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
        if (simpleinteractable.firstInteractorSelecting != null)
            return;
        // Remember starting rotation
        startRotation = getRotation(transform.rotation.eulerAngles);
    }

    private float getRotation(Vector3 g)
    {
        // return selected axis
        return axis switch
        {
            Axis.X => g.x,
            Axis.Y => g.y,
            Axis.Z => g.z,
            _ => 0
        };
    }

    private Vector3 setRotation(float goalRotation)
    {
        // return vector where the selected axis is modified
        return axis switch
        {
            Axis.X => new Vector3(goalRotation, transform.eulerAngles.y, transform.eulerAngles.z),
            Axis.Y => new Vector3(transform.eulerAngles.x, goalRotation, transform.eulerAngles.z),
            Axis.Z => new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, goalRotation),
            _ => Vector3.zero
        };
    }

    // FixedUpdate is called once per physics tick
    void FixedUpdate()
    {
        var currentInteractor = simpleinteractable.firstInteractorSelecting;
        if (currentInteractor == null)
            return;
        float newHandRotation = getRotation(currentInteractor.transform.eulerAngles);
        float currentRotation = getRotation(transform.eulerAngles);
        float rotationDifference = startRotation - newHandRotation;        
        float goalRotation = rotationDifference + currentRotation;
        transform.eulerAngles = setRotation(goalRotation);
        startRotation = newHandRotation;
    }
}
