using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Swiitch : MonoBehaviour
{
   
    public GameObject[] background;
    int index;
    

    void Start()
    {
        
        string strCmdText;
        //strCmdText= "/C notepad";
        //strCmdText= "/C cd C:/Users/HP/Desktop/Python & jpg.py UML.pdf";
        strCmdText= "/C cd C:/Users/HP/Documents/virtuchemlabunity/virtuchemlabunity/VirtuChemLab/Assets/Scripts/PDF/Python & jpg.py UML.pdf";   //This command to open a new notepad
        System.Diagnostics.Process.Start("CMD.exe",strCmdText); //Start cmd process
        index = 0;
    }
        

    void Update()
    {
        if(index >= 14)
           index = 14 ; 

        if(index < 0)
           index = 0 ;
        


        if(index == 0)
        {
            background[0].gameObject.SetActive(true);
        }
        
    }

    public void Next()
     {
        index += 1;
    
         for(int i = 0 ; i < background.Length; i++)
         {
            background[i].gameObject.SetActive(false);
            background[index].gameObject.SetActive(true);
         }
            //Debug.Log(index);
     }
    
     public void Previous()
     {
          index -= 1;
    
        for(int i = 0 ; i < background.Length; i++)
         {
            background[i].gameObject.SetActive(false);
            background[index].gameObject.SetActive(true);
         }
            //Debug.Log(index);
     }

   
}
