using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUI : MonoBehaviour
{
    private Canvas _ResetWindow;

    private void Start()
    {
        _ResetWindow = GetComponent<Canvas>();
        _ResetWindow.enabled = false; //Reset window disabled at first
    }
    public void ToggleMenu()
    {
        _ResetWindow.enabled = !_ResetWindow.enabled; //Enables the reset window if primary button toggled
    }
}