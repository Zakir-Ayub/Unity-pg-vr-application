using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NaOH : MonoBehaviour
{
    // Start is called before the first frame update

    public TextMeshProUGUI LabelValue;

    public SwitchHandeler switchHandeler;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

       

        if (switchHandeler.Status == true) /* To check if the label enable disable is turened on*/
        {
            LabelValue.enabled = true;
            LabelValue.text = "NaOH container";
        }
        else
        {
            LabelValue.enabled = false;
        }

    }
}
