using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetStirrer_Controller : MonoBehaviour
{
    [Tooltip("Reference to the left switch of the stirrer for controlling the heat")]
    public MagnetStirrer_left_switch leftSwitch;
    [Tooltip("Set the max temperature of the heat plate in °C")]
    public float maxTemperature;

    // variables to set correct temperature based on ObjectProperties
    private float initialObjectTemperature;

    private ObjectProperties objectProperties;

    private float maxLeftRotation;

    private float checkTemp;

    // Hashtable for monitoring collision
    private Hashtable collisionTable;

    // Start is called before the first frame update
    void Start()
    {
        // Get temperature properties of ObjectProperties script
        objectProperties = gameObject.GetComponent<ObjectProperties>();
        initialObjectTemperature = objectProperties.Temperature;
        maxLeftRotation = leftSwitch.maxRotation;
        checkTemp = 0;
        collisionTable = new Hashtable(); // initialize collision Hashtable
    }

    // Update is called once per frame
    void Update()
    {
        // get the z rotation of the left switch
        var zRotation = leftSwitch.transform.eulerAngles.z;

        checkTemp = zRotation / maxLeftRotation * maxTemperature;

        if (zRotation > 1 && zRotation < maxLeftRotation && checkTemp >= initialObjectTemperature) 
        {
            // assign temperature based as fracture of max temperature
            objectProperties.Temperature = zRotation / maxLeftRotation * maxTemperature;
            //Debug.Log("Rotation based Temperature: " + zRotation / maxLeftRotation * maxTemperature);
        }
        else 
        {
            // go back to the initial temperature of the object to reach origin state
            objectProperties.Temperature = initialObjectTemperature;
        }

        // iterate through collision list and update all temperatures
        foreach (GameObject collisionObject in collisionTable.Keys)
        {
            ObjectProperties collisionProperties = collisionObject.GetComponent("ObjectProperties") as ObjectProperties;
            collisionProperties.Temperature = objectProperties.Temperature; // update temperature
        }
    }

    // this method is called when a collision with the collision object is triggered
    void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("ChemistryObject") && !collision.gameObject.CompareTag("StirringFish")) return; // if the collision object does not have the ChemistryObject or StirringFish tag, it should be ignored

        ObjectProperties collisionProperties = (collision.gameObject.GetComponent("ObjectProperties") as ObjectProperties); // store ObjectProperties for collision object
        if(collisionProperties != null)
        {
            // since the table sometimes deletes itself, create a new one to prevent Nullpointer
            if (collisionTable == null)
            {
                collisionTable = new Hashtable();
            }

            if (!collisionTable.Contains(collision.gameObject))
            {
                // only add collision object if not in table already
                collisionTable.Add(collision.gameObject, collisionProperties.Temperature);
                
                // since the local object properties were sometimes deleted (for some reason), re-create the link here again to prevent Nullpointer :c
                if (objectProperties == null)
                {
                    objectProperties = gameObject.GetComponent<ObjectProperties>();
                }

                // adjust temperature of collision object based on heat of heat plate
                collisionProperties.Temperature = objectProperties.Temperature;
            }
        }
    }

    // this method is called when the collision with the collision object ends
    void OnCollisionExit(Collision collision)
    {
        // Note: this somehow works as expected c:
        if(!collision.gameObject.CompareTag("ChemistryObject") && !collision.gameObject.CompareTag("StirringFish")) return; // if the collision object does not have the ChemistryObject or StirringFish tag, it should be ignored

        ObjectProperties collisionProperties = (collision.gameObject.GetComponent("ObjectProperties") as ObjectProperties);
        if(collisionProperties != null)
        {
            if (collisionTable.Contains(collision.gameObject))
            {
                // remove object from table if its part of it
                collisionProperties.Temperature = (float) collisionTable[collision.gameObject];
                collisionTable.Remove(collision.gameObject);
            }
        }
    }
}
