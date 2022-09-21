using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class PanelDisplay : MonoBehaviour
{
    private ScaleController WeightDisplay;
    public TextMeshProUGUI PanelText;
    public TextMeshProUGUI LabelValue;

    public SwitchHandeler switchHandeler;


    void Start()
    {
        WeightDisplay = GameObject.FindObjectOfType<ScaleController>();
    }

    // Update is called once per frame
    void Update()
    {

        /* Check the Status of the SwitchHandeler to check if the object label shoub be displayed or not*/

        if (switchHandeler.Status == true)
        {
            LabelValue.enabled = true;
            LabelValue.text = "Scale";
        }
        else
        {
            LabelValue.enabled = false;
        }

        if (WeightDisplay.IsDeviceTurnedOn)
        {
            PanelText.text = "Weight: " + (WeightDisplay.Weight - WeightDisplay.TaraOffset) + "g";//Displays the weight on Details Panel
        }
    }
}
