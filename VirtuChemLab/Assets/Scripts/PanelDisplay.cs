using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class PanelDisplay : MonoBehaviour
{
    private ScaleController WeightDisplay;
    public TextMeshProUGUI PanelText;
    
    void Start()
    {
        WeightDisplay = GameObject.FindObjectOfType<ScaleController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (WeightDisplay.IsDeviceTurnedOn)
        {
            PanelText.text = "Weight: " + (WeightDisplay.Weight - WeightDisplay.TaraOffset) + "g";//Displays the weight on Details Panel
        }
        else
        {
            PanelText.text = "Turned Off";
        }
    }
}
