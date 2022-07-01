using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermometerTip : MonoBehaviour
{

    private ThermometerController thermometer;

    // Start is called before the first frame update
    void Start()
    {
        this.thermometer = transform.parent.gameObject.GetComponent<ThermometerController>();
    }

    // OnTrigger functions are used for the non-digital thermometer
    void OnTriggerEnter(Collider collision)
    {
        if(!collision.gameObject.CompareTag("ChemistryObject") && !collision.gameObject.CompareTag("Chemical")) return; // if it does not have the ChemistryObject tag it should be ignored
        if(collision.gameObject.GetComponent("ThermometerController") != null) return; // if collision with themometer itself, ignore

        ObjectProperties objectProperties = (collision.gameObject.GetComponent("ObjectProperties") as ObjectProperties);
        if(objectProperties != null)
        {
            if(thermometer != null)
            {
                if (!thermometer.collisionList.Contains(collision.gameObject))
                {
                    // save colliding game object
                    thermometer.collisionList.Add(collision.gameObject);
                }
            }
        }
    }

    // OnTrigger functions are used for the non-digital thermometer
    void OnTriggerExit(Collider collision)
    {
        if(!collision.gameObject.CompareTag("ChemistryObject") && !collision.gameObject.CompareTag("Chemical")) return; // if it does not have the ChemistryObject tag it should be ignored
        if(collision.gameObject.GetComponent("ThermometerController") != null) return; // if collision with themometer itself, ignore

        ObjectProperties objectProperties = (collision.gameObject.GetComponent("ObjectProperties") as ObjectProperties);
        if(objectProperties != null)
        {
            if(thermometer != null)
            {
                if (thermometer.collisionList.Contains(collision.gameObject))
                {
                    // forget game object no longer colliding
                    thermometer.collisionList.Remove(collision.gameObject);
                }
            }
        }
    }

    // OnCollision functions are used for the digital thermometer
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("ChemistryObject") && !collision.gameObject.CompareTag("Chemical")) return; // if it does not have the ChemistryObject tag it should be ignored
        if (collision.gameObject.GetComponent("ThermometerController") != null) return; // if collision with themometer itself, ignore

        ObjectProperties objectProperties = (collision.gameObject.GetComponent("ObjectProperties") as ObjectProperties);
        if (objectProperties != null)
        {
            if (thermometer != null)
            {
                if (!thermometer.collisionList.Contains(collision.gameObject))
                {
                    // save colliding game object
                    thermometer.collisionList.Add(collision.gameObject);
                }
            }
        }
    }

    // OnCollision functions are used for the digital thermometer
    void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("ChemistryObject") && !collision.gameObject.CompareTag("Chemical")) return; // if it does not have the ChemistryObject tag it should be ignored
        if (collision.gameObject.GetComponent("ThermometerController") != null) return; // if collision with themometer itself, ignore

        ObjectProperties objectProperties = (collision.gameObject.GetComponent("ObjectProperties") as ObjectProperties);
        if (objectProperties != null)
        {
            if (thermometer != null)
            {
                if (thermometer.collisionList.Contains(collision.gameObject))
                {
                    // forget game object no longer colliding
                    thermometer.collisionList.Remove(collision.gameObject);
                }
            }
        }
    }
}
