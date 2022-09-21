using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PhIndicatorPaper : NetworkBehaviour
{
    //Once we used that paper strip, it is to be thrown out and cat be reused
    private bool isUsed = false;

    private Color targetColor;
    private float startTime;

    private const float FADE_TIME = 9f;

    private Renderer paperRenderer;
    private Color baseMaterialColor;

    private ObjectProperties objectProperties;

    // Start is called before the first frame update
    void Start()
    {
        //Get Paper Renderer for later coloring
        this.paperRenderer = this.transform.GetChild(0).GetChild(0).GetComponent<Renderer>();
        this.baseMaterialColor = this.paperRenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if(isUsed)
        {
            //Has it gotten contact with a chemical? Than color the paper
            this.paperRenderer.material.color = Color.Lerp(this.baseMaterialColor, targetColor, Mathf.Clamp01(Mathf.Pow((Time.time - startTime) / FADE_TIME, 2f)));
        } 
        else if(objectProperties)
        {
            //If the paper is in a chemical trigger, check whether it is empty or not
            if (objectProperties.Weight > 0)
            {
                //If not empty, get ph Value and set properties to start coloring the paper
                targetColor = calculateColor(objectProperties.PhValue);
                isUsed = true;
                startTime = Time.time;
                this.objectProperties = null;
            }
        }
    }


    //When entering a trigger with a chemical, we store the object properties for use in Update()
    private void OnTriggerEnter(Collider other)
    {
        if (isUsed) return;

        if (!other.gameObject.CompareTag("Chemical")) return; // if it does not have the ChemistryObject or StirringFish tag it should be ignored

        ObjectProperties objectProperties = other.gameObject.GetComponent<ObjectProperties>();
        if (objectProperties)
        {
            this.objectProperties = objectProperties;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Chemical"))
        {
            this.objectProperties = null;
        }
    }

    //Calculate the color of universal indicator according to ph Value
    private Color calculateColor(float phValue)
    {
        if(phValue < 1)
        {
            return new Color32(246,2,64,255);
        }
        if (phValue >= 1 && phValue < 2)
        {
            return Color.Lerp(new Color32(246,2,64,255),  new Color32(253,54,59,255), (phValue - 1));
        }
        if (phValue >= 2 && phValue < 3)
        {
            return Color.Lerp(new Color32(253,54,59,255),  new Color32(254,78,28,255), (phValue - 2));
        }
        if (phValue >= 3 && phValue < 5)
        {
            return Color.Lerp(new Color32(254,78,28,255),  new Color32(250,110,13,255), (phValue - 3)/2f);
        }
        if (phValue >= 5 && phValue < 6)
        {
            return Color.Lerp(new Color32(250,110,13,255), new Color32(251,149,15,255), (phValue - 5));
        }
        if (phValue >= 6 && phValue < 7)
        {
            return Color.Lerp(new Color32(251,149,15,255), new Color32(254,188,4,255), (phValue - 6));
        }
        if (phValue >= 7 && phValue < 8)
        {
            return Color.Lerp(new Color32(254,188,4,255), new Color32(212,168,0,255), (phValue - 7));
        }
        if (phValue >= 8 && phValue < 9)
        {
            return Color.Lerp(new Color32(212,168,0,255), new Color32(132,164,87,255), (phValue - 8));
        }
        if (phValue >= 9 && phValue < 10)
        {
            return Color.Lerp(new Color32(132,164,87,255), new Color32(64,99,95,255), (phValue - 9));
        }
        if (phValue >= 10 && phValue < 12)
        {
            return Color.Lerp(new Color32(64,99,95,255),  new Color32(32,70,107,255), (phValue - 10)/2f);
        }
        if (phValue >= 12 && phValue < 14)
        {
            return Color.Lerp(new Color32(32,70,107,255),  new Color32(8,40,125,255), (phValue - 12)/2f);
        }
        if(phValue >= 14)
        {
            return new Color32(8,40,125,255);
        }
        return Color.white;
    }
}
