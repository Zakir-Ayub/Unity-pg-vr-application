using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DigitalPhMeterController : MonoBehaviour
{

    private Text textObject;

    [System.NonSerialized]
    public List<GameObject> collisionList;

    private float realPhValue = 0.0f;
    private float displayedPhValue = 7.0f;
    private float timeOfLastPhValueChange = 0;

    private const float displayUpdateInterval = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        collisionList = new List<GameObject>();
        this.textObject = (Text)transform.Find("Canvas/Display").gameObject.GetComponent("Text");
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject g in collisionList)
        {
            var objProps = g.GetComponent<NetworkObjectproperties>();
            if(objProps.PhValue != 0 && objProps.Weight > 0)
            {
                if(objProps.PhValue != realPhValue)
                {
                    realPhValue = objProps.PhValue;
                    timeOfLastPhValueChange = Time.realtimeSinceStartup;
                }
                //Assume that we dont measure to liquids with different pH Values at any time
                break;
            }
        }
        if(realPhValue > 0 && timeOfLastPhValueChange < Time.realtimeSinceStartup- displayUpdateInterval)
        {
            displayedPhValue += + (realPhValue - displayedPhValue) / 2.1981f;
            timeOfLastPhValueChange += displayUpdateInterval;
            textObject.text = displayedPhValue.ToString("n2");
        }
    }
}
