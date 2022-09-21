using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;


public class ScaleHover : MonoBehaviour
{
    // Start is called before the first frame update
    public InputActionReference inputaction;

    /*private Image Panel;*/
    public TextMeshProUGUI text;

    [SerializeField]
    private UnityEvent onInteract = new UnityEvent();
    /* private TextMeshProUGUI text;*/
    public PanelDisplay mscript;





    void Start()
    {
        /* Here the dynamic value of the lable is disabled in the start and will be enabled on input action which is triggered by calling the toggle panel function */
        text = GetComponent<TextMeshProUGUI>();
        mscript.PanelText.enabled = false;

        inputaction.action.performed += ctx => TogglePanel();


    }


   


    public void TogglePanel()
    {
     

        mscript.PanelText.enabled = !mscript.PanelText.enabled;
        




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

