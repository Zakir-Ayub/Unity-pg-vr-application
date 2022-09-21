using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextSlider : MonoBehaviour
{
    // Start is called before the first frame update


    [SerializeField] private float actualValue;
    private Slider sliderFontSize;

    [SerializeField] private GameObject[] textFontSizeToChange;

    //for soundscript
    [HideInInspector]
    public bool hasIncreased = false;

    [HideInInspector]
    public bool hasDecreased = false;

    void Start()
    {
       textFontSizeToChange = GameObject.FindGameObjectsWithTag("Text");
       sliderFontSize = GameObject.Find("SliderText").GetComponent<Slider>(); 

       actualValue = sliderFontSize.value;
    }

     public void FontSizeChange()
     {
         int sameValue = ((int) sliderFontSize.value / (int) sliderFontSize.value); 
         if (sliderFontSize.value > actualValue)  // To increase the size of the text
        {
            hasIncreased = true;
            foreach (GameObject text in textFontSizeToChange)
            {
                var t = text.GetComponent<TMP_Text>(); // For TextMeshPro
                if (t!=null)
                    t.fontSize += sameValue;

                var t2 = text.GetComponent<Text>();  // For Text 
                if (t2 != null)
                    t2.fontSize += sameValue;
            }

            actualValue += sameValue; 
        }
     


      if (sliderFontSize.value < actualValue) // To decrease the size of the text
        {
            hasDecreased = true;
            foreach (GameObject text in textFontSizeToChange)
            {
                var t = text.GetComponent<TMP_Text>();
                if (t != null)
                    t.fontSize -= sameValue;

                var t2 = text.GetComponent<Text>();
                if (t2 != null)
                    t2.fontSize -= sameValue;
            }
            
            actualValue -= sameValue;
        }

     }

   
}
