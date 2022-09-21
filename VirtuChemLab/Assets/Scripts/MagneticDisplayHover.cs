using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class MagneticDisplayHover : MonoBehaviour

{
    // Start is called before the first frame update
    public InputActionReference inputaction;

   
    private TextMeshProUGUI MPanel;
    [SerializeField]
    private UnityEvent onInteract = new UnityEvent();
  

    public MagneticDisplayPanel mscript;




    void Start()
    {



/* Here the dynamic value of the lable is disabled in the start and will be enabled on input action which is triggered by calling the toggle panel function */

        MPanel = GetComponent<TextMeshProUGUI>();


        mscript.DisplayValue.enabled = false;
       


        inputaction.action.performed += ctx => TogglePanel();
    }


 



    public void TogglePanel()
    {
       
       
        mscript.DisplayValue.enabled = !mscript.DisplayValue.enabled;
       

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



