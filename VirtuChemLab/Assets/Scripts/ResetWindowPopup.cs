using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetWindowPopup : MonoBehaviour
{

       public GameObject ResetPopup;
         public bool activeResetPopup = true;
    void Start()
    {
     
        DisplayResetPopup();
    }

    public void MenuPressed(InputAction.CallbackContext context){
        if(context.performed)
            DisplayResetPopup();
    }



    public void DisplayResetPopup()
    {
        if(activeResetPopup) // to close the window
        {
            ResetPopup.SetActive(false);
            activeResetPopup = false;
        }
        else if (!activeResetPopup) // to open the window
        {
            ResetPopup.SetActive(true);
            activeResetPopup = true;
        }
    }
}