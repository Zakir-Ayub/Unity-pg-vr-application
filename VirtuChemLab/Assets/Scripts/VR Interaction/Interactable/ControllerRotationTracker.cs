using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerRotationTracker : MonoBehaviour
{
    public delegate void RotateEvent(IXRInteractor interactor, Vector3 rotationDiff);
    public event RotateEvent OnRotate;
    
    private XRSimpleInteractable interactable;
    private IXRInteractor interactor;
    
    private Vector3 rotationPreviousUpdate;
    
    private void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnSelectEnter);
        interactable.selectExited.AddListener(OnSelectExit);
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        if (interactor == null && args.interactorObject is XRBaseControllerInteractor)
        {
            interactor = args.interactorObject;
            rotationPreviousUpdate = GetRotation(args.interactorObject);
        }
    }
    
    private void OnSelectExit(SelectExitEventArgs args)
    {
        if (args.interactorObject == interactor)
        {
            interactor = null;
        }
    }

    private void Update()
    {
        if (IsTrackingRotation())
        {
            Vector3 currentRotation = GetRotation(interactor);
            Vector3 rotationDiff = GetRotationDiff(currentRotation, rotationPreviousUpdate);
            
            if (rotationDiff.magnitude > 0f)
            {
                OnRotate?.Invoke(interactor, rotationDiff);
            }

            rotationPreviousUpdate = currentRotation;
        }
    }

    private Vector3 GetRotation(IXRInteractor xrInteractor)
    {
        return xrInteractor.transform.localRotation.eulerAngles;
    }

    private Vector3 GetRotationDiff(Vector3 current, Vector3 previous)
    {
        float diffX = GetRotationDifference(previous.x, current.x);
        float diffY = GetRotationDifference(previous.y, current.y);
        float diffZ = GetRotationDifference(previous.z, current.z);

        return new Vector3(diffX, diffY, diffZ);
    }

    /// <summary>
    /// Prevents swapping from negative to positive rotations
    /// </summary>
    private float GetRotationDifference(float oldValue, float newValue)
    {
        float diff = newValue - oldValue;
        if (newValue < 0 && oldValue > 0)
        {
            diff = oldValue - newValue;
        }

        float maxRotationDiff = 350f;
        if (diff >= maxRotationDiff)
        {
            diff -= maxRotationDiff;
        }
        else if (diff <= -maxRotationDiff)
        {
            diff += maxRotationDiff;
        }

        return diff;
    }

    private bool IsTrackingRotation()
    {
        return interactor != null;
    }
}