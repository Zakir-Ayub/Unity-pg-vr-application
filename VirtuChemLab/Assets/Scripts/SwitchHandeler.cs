using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*using DG.Tweening;
*/

public class SwitchHandeler : MonoBehaviour
{

    [SerializeField] RectTransform HandelRectTransform;
    [SerializeField] Color backgroundActiveColor;
    [SerializeField] Color handelActiveColor;
    [SerializeField] private GameObject[] labels;


    public RawImage backgroundImage, handelImage;
    public Color backgroundDefaultColor, handelDefaultColor;
    



    public Toggle toggle;
    public Vector2 handelPosition;

    public bool Status;
    // Start is called before the first frame update
    private void Awake()
    {

     /*   labels = GameObject.FindGameObjectsWithTag("Labels");*/


        toggle = GetComponent<Toggle>();
        handelPosition = HandelRectTransform.anchoredPosition;
       


        backgroundImage = HandelRectTransform.parent.GetComponent<RawImage>();
        handelImage = HandelRectTransform.GetComponent<RawImage>();

        backgroundDefaultColor = backgroundImage.color;
        handelDefaultColor = handelImage.color;

        toggle.onValueChanged.AddListener(OnSwitch); /*added a listener to the switch button*/
        if (toggle.isOn)
        {
            OnSwitch(true);
           
        }
      
           
    }

    void OnSwitch(bool on)
    {


        HandelRectTransform.anchoredPosition = on ? handelPosition * -1 : handelPosition; /* Move the position of the scrolling button*/


        /*HandelRectTransform.DOAnchorPos(on ? handelPosition * -1 : handelPosition, .4f).SetEase(Ease.InOutBack); (For animation, temporarily commented, will work on it if the liscencing issue is solved)*/
        if (on)
        {
            Status = true;
        }
        else
        {
            Status = false;
        }


       /* foreach (GameObject label in labels) enable / disable all the lables with Lables, this would be usefull in futture implementation of disableing the labels

            {

            if (on)
            {
                label.SetActive(true);
                Status = true;
            }
               
            else
            {
                label.SetActive(false);
                Status = false;
            }
               
        }*/

        backgroundImage.color = on ? backgroundActiveColor : backgroundDefaultColor;


        /* backgroundImage.DOColor(on ? backgroundActiveColor : backgroundDefaultColor, .6f);* (For animation)/

       /*  handelImage.DOColor(on ? handelActiveColor : handelDefaultColor, .4f); (For animation)*/
       
        handelImage.color = on ? handelActiveColor : handelDefaultColor; /* Change the colour if turned on / off*/


    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }



}
