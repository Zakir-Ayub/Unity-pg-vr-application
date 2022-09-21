using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScaleDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    public InputActionReference inputaction;

    private Image Panel;
    [SerializeField]
    private UnityEvent onInteract = new UnityEvent();
    /* private TextMeshProUGUI text;*/
    public ThermoDisplayPanel script;
  


/*This code will be used to further implemenent the enable/disable function of the dynamic value*/

    void Start()
    {

        Panel = GetComponent<Image>();
        /* text = GetComponent<TextMeshProUGUI>();*/



        Panel.enabled = false;
        script.PanelValue.enabled = false;
        


        /* text.enabled = false;*/

        inputaction.action.performed += ctx => TogglePanel();
    }





    public void TogglePanel()
    {
        Panel.enabled = !Panel.enabled;
        script.PanelValue.enabled =!script.PanelValue.enabled;
       


    }

    void OnEnable()
    {
        inputaction.action.Enable();
    }

    private void OnDisable()
    {
        inputaction.action.Disable();
    }
}
