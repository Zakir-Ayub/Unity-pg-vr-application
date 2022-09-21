using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Pipette_Animation : MonoBehaviour
{
    
    public Animator PipetteAnim;// Animator data type holding animation control and animation
    
    public IndexControlRemap IndexControlReference;// Reference of custom input remap

    [NonSerialized]
    public float triggerValue;
    
    void Awake()// The Awake function is called on all objects in the scene before any object's 
    {
        IndexControlReference = new IndexControlRemap();// New instance of script "IndexControlRemap"
    }

    void Update()// The Update Function executes on every frame after playing the scene
    { 
         IndexControlReference.ValveIndexGamepad.ButtonA.performed += ctx => OnDisable();// This parameter is responsible for disabling input if "Button A" is pressed any time
         triggerValue = IndexControlReference.ValveIndexGamepad.GripAndTrigger.ReadValue<float>();// "TriggerValue" is storing the raw input value of Grip and Trigger
         PipetteAnim.SetFloat("AnimationSpeed", triggerValue);// Previously stored value is being used as animation speed for the Pipette
    }
    void OnTriggerEnter(Collider other)// Built-in function to check if the object where this script is attached staying inside the triggered object or not
    {
        if(other.gameObject.CompareTag("VRRightHand"))// Condition made to check if the triggered object is tagged as "VRRightHand" or not
        {
            IndexControlReference.ValveIndexGamepad.Enable();// This parameter is responsible for enabling input inside the triggered object
        }
    }
    public void OnDisable()// This built-in function is called when the object becomes enabled and active
    {
        IndexControlReference.ValveIndexGamepad.Disable();// This parameter is resposible for disabling input.
    }

    
  
}
