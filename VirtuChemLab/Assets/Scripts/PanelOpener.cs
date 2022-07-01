using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelOpener : MonoBehaviour
{
    public GameObject DetailsPanel;

    public void OpenPanel()
    {
        if(DetailsPanel != null)
        {
            bool isActive = DetailsPanel.activeSelf;

            DetailsPanel.SetActive(!isActive);
        }
    }
}
