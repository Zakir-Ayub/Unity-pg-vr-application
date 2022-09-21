using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MagneticDisplayPanel : MonoBehaviour

{
    // Start is called before the first frame update
    private ObjectProperties MagTemp;
    
    public TextMeshProUGUI DisplayValue;

    /* public string var;*/

    public TextMeshProUGUI LabelValue;

    public SwitchHandeler switchHandeler;
  


    void Awake()
    {


        /* MagTemp = GameObject.FindObjectOfType<ObjectProperties>();*/
        MagTemp = GetComponentInParent<ObjectProperties>();

 
      
        
        
    }

    // Update is called once per frame
    void Update()
    {

    /* Check the Status of the SwitchHandeler to check if the object label shoub be displayed or not*/

        if (switchHandeler.Status == true)
        {
            LabelValue.enabled = true;
            LabelValue.text = "Magnetic Stirrer";
        }
        else
        {
            LabelValue.enabled = false;
        }

/*Display the dynamic properties including heatflow and the temperatro*/
        DisplayValue.text = "Temperature : " + MagTemp.Temperature + " Heatflow : " + MagTemp.HeatFlowRate;
      
        
        

    }
}
