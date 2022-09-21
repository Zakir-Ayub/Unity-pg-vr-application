using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThermoDisplayPanel : MonoBehaviour
{

    private ObjectProperties TempDisplay;
    public TextMeshProUGUI PanelValue;
    public TextMeshProUGUI LabelValue;

    public SwitchHandeler switchHandeler;



    // Start is called before the first frame update
    void Awake()
    {
        TempDisplay = GetComponentInParent<ObjectProperties>(); ;
    }

    // Update is called once per frame
    void Update()
    {
 



        if (switchHandeler.Status == true) /* To check if the label enable disable is turened on*/
        {
            LabelValue.enabled = true;
            LabelValue.text = "Thermometer";
        }
        else
        {
            LabelValue.enabled = false;
        }

        PanelValue.text ="Temperature : " + TempDisplay.Temperature + " °C"; /* Get the latest dynamic value*/
        

        
    }
}
