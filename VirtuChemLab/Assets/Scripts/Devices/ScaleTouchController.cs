using System;
using UnityEngine;

public class ScaleTouchController : MonoBehaviour
{
    [NonSerialized]
    // true if this object touches a scale, false if not
    public bool touchesScale;

    [NonSerialized]
    // the scale that this GameObject touches
    public GameObject scale;

    // Start is called before the first frame update, initializes our variables with default values
    void Start()
    {
        touchesScale = false;
        scale = null;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("ChemistryObject") && !collision.gameObject.CompareTag("StirringFish")) return; // if it does not have the ChemistryObject or StirringFish tag it should be ignored

        ScaleTouchController scaleTouchController = collision.gameObject.GetComponent<ScaleTouchController>();
        if(scaleTouchController != null) // if there is a collision with another object with a ScaleTouchController
        {
            if(scaleTouchController.touchesScale) // if the other object is touching a scale
            {
                // if there is a collision with a scale-touching object, add the object with this script to the collisionList as well and fill corresponding variables accordingly
                touchesScale = true;
                scale = scaleTouchController.scale;
                ScaleController scaleController = scale.GetComponent<ScaleController>();
                scaleController.SetOnTop(gameObject, true);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if(!collision.gameObject.CompareTag("ChemistryObject") && !collision.gameObject.CompareTag("StirringFish")) return; // if it does not have the ChemistryObject or StirringFish tag it should be ignored

        ScaleTouchController scaleTouchController = collision.gameObject.GetComponent<ScaleTouchController>();
        if(scaleTouchController != null) // if the collision was with another object with a ScaleTouchController
        {
            if(scaleTouchController.scale != null) // if the other object has a value for the scale attached to it
            {
                ScaleController scaleController = scaleTouchController.scale.GetComponent<ScaleController>();
                if(scaleController.IsOnTop(gameObject))
                {
                    // removes the scale from this object if it no longer touches a scale-touching object and removes it from the collisionList
                    touchesScale = false;
                    scale = null;
                    scaleController.SetOnTop(gameObject, false);
                }
            }
        }
    }
}
