using System;
using UnityEngine;

public class ScaleTouchController : MonoBehaviour
{
    [NonSerialized]
    public bool touchesScale;

    [NonSerialized]
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
        if(scaleTouchController != null)
        {
            if(scaleTouchController.touchesScale)
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
        if(scaleTouchController != null)
        {
            if(scaleTouchController.scale != null)
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
