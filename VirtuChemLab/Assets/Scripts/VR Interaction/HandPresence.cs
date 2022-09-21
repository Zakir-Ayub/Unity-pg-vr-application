using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    // State transition actions
    [SerializeField]
    InputActionReference triggerAction;

    [SerializeField]
    InputActionReference grabAction;

    public Animator handAnimator;

    void UpdateHandAnimation()
    {
        var triggerValue = triggerAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);
        var gripValue = grabAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHandAnimation();
    }
}
