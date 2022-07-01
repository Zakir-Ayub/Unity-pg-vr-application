using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class ThermometerController : NetworkBehaviour
{

    [Tooltip("Reference to the ObjectProperties of the thermometer")]
    public ObjectProperties objectProperties;
    [Tooltip("Set the minimum temperature the thermometer can display in °C")]
    public float minTemperature;
    [Tooltip("Set the maximum temperature the thermometer can display in °C")]
    public float maxTemperature;
    [Tooltip("Check this if thermometer this is applied to is non-digital (uses red cylinders)")]
    public bool isNotDigital;
    [Tooltip("[Only necessary if digital!] This is the text object that the temperature will be displayed in. ")]
    public Text textObject;

    /// <summary>
    /// Actually dispalyed temperature
    /// </summary>
    private float currentTemperature;

    /// <summary>
    /// Initial / default temperature to return to
    /// </summary>
    private float initialTemperature;

    /// <summary>
    /// Previously displayed temperature, for efficiency
    /// </summary>
    private float previousTemperature = -500;

    /// <summary>
    /// The thermometer's innermost child cylinder, representing displayed temperature
    /// </summary>
    private Transform innerCylinder;

    /// <summary>
    /// Original y scale factor of the inner cylinder
    /// </summary>
    private float initialYScale = 0;

    /// <summary>
    /// Original y size of the inner cylinder
    /// </summary>
    private float initialYSize = 0;

    /// <summary>
    /// Original local position of the inner cylinder in y dimension
    /// </summary>
    private float initialYPos = 0;

    /// <summary>
    /// List of colliding objects
    /// </summary>
    [System.NonSerialized]
    public List<GameObject> collisionList;

    void Start()
    {
        collisionList = new List<GameObject>(); // initialize collision Hashtable
        initialTemperature = objectProperties.Temperature;
        currentTemperature = objectProperties.Temperature;
        if(!isNotDigital)
        {
            textObject.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            if (collisionList.Count != 0)
            {
                float temperatureSum = 0;
                foreach (var collider in collisionList)
                {
                    temperatureSum += (collider.gameObject.GetComponent("ObjectProperties") as ObjectProperties).ActualTemperature;
                }
                objectProperties.Temperature = temperatureSum / collisionList.Count;

            }
            else
            {
                objectProperties.Temperature = initialTemperature;
            }
        }

        currentTemperature = objectProperties.Temperature;

        // update only if temperature has actually changed and if not digital
        if(currentTemperature != previousTemperature && isNotDigital)
        {
            float filledPortion;

            filledPortion = (currentTemperature - minTemperature) / (maxTemperature - minTemperature);

            // keep filledPortion within a sensible range
            filledPortion = filledPortion > 1 ? 1 : filledPortion < 0 ? 0 : filledPortion;

            if(innerCylinder != null)
            {
                // adjust the inner cylinders position to accomodate the new size, since it should be "filling up from the bottom"
                innerCylinder.transform.localPosition = new Vector3(
                    innerCylinder.transform.localPosition.x,
                    initialYPos - (initialYSize * ((1f - filledPortion) / 2f)) - (initialYSize * (1f - filledPortion)) * 0.09f,
                    innerCylinder.transform.localPosition.z
                    );

                // adjust the inner cylinders scaling based the portion of the thermometer which should be filled
                innerCylinder.transform.localScale = new Vector3(
                    innerCylinder.transform.localScale.x,
                    initialYScale * filledPortion,
                    innerCylinder.transform.localScale.z
                    );
            }

            previousTemperature = currentTemperature;
        }
        if(currentTemperature != previousTemperature && !isNotDigital)
        {
            textObject.text = (Mathf.Round((currentTemperature) * 10.0f) / 10.0f) + "°C";
            previousTemperature = currentTemperature;
        }

    }

    void Awake()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform currentItem = transform.GetChild(i);

            // search for inner cylinder by name and save it, as well as it's original scale factor and size in y dimension
            if (currentItem.name.Equals("CylinderInner"))
            {
                innerCylinder = currentItem;
                initialYScale = innerCylinder.transform.localScale.y;
                initialYSize = innerCylinder.transform.GetComponent<Renderer>().bounds.size.y;
                initialYPos = innerCylinder.transform.localPosition.y;
            }
        }


    }
}
