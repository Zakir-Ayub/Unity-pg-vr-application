using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitalPhMeterTip : MonoBehaviour
{

    private DigitalPhMeterController phMeter;

    // Start is called before the first frame update
    void Start()
    {
        this.phMeter = transform.parent.gameObject.GetComponent<DigitalPhMeterController>();
    }
    
    void OnTriggerEnter(Collider collision)
    {
        if (!collision.gameObject.CompareTag("ChemistryObject") && !collision.gameObject.CompareTag("Chemical")) return; // if it does not have the ChemistryObject tag it should be ignored
        if (collision.gameObject.GetComponent("DigitalPhMeterController") != null) return; // if collision with ph meter itself, ignore

        ObjectProperties objectProperties = (collision.gameObject.GetComponent("ObjectProperties") as ObjectProperties);
        if (objectProperties != null)
        {
            if (phMeter != null)
            {
                if (!phMeter.collisionList.Contains(collision.gameObject))
                {
                    // save colliding game object
                    phMeter.collisionList.Add(collision.gameObject);
                }
            }
        }
    }
    
    void OnTriggerExit(Collider collision)
    {
        if (!collision.gameObject.CompareTag("ChemistryObject") && !collision.gameObject.CompareTag("Chemical")) return; // if it does not have the ChemistryObject tag it should be ignored
        if (collision.gameObject.GetComponent("DigitalPhMeterController") != null) return; // if collision with ph meter itself, ignore

        ObjectProperties objectProperties = (collision.gameObject.GetComponent("ObjectProperties") as ObjectProperties);
        if (objectProperties != null)
        {
            if (phMeter != null)
            {
                if (phMeter.collisionList.Contains(collision.gameObject))
                {
                    // forget game object no longer colliding
                    phMeter.collisionList.Remove(collision.gameObject);
                }
            }
        }
    }
}
