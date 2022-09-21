using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResetButton : MonoBehaviour
{
    // after pressing resetButton reset all pixel in whiteboard to White 
    private float ResetClickTime = 0f;
    private float Cooldown = 2f; // adding cooldown 2 = two seconds 
    public void resetWhiteboard(Whiteboard _whiteboard)
    {   
        if (Time.time > ResetClickTime + Cooldown)
        {
            for (int w=0; w<_whiteboard.texture.height; ++w) 
            {
                for (int q= 0; q< _whiteboard.texture.width; ++q) 
                {
                    _whiteboard.texture.SetPixel (q, w, Color.white);
                }
            } 
            _whiteboard.texture.Apply();
        
             ResetClickTime= Time.time;
        }
    }
}
