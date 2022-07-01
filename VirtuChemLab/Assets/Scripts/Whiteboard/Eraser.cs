using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

public class Eraser : NetworkBehaviour
{
//setting up pen size and the array to color the pixel which we touch using the Eraser to white 
[SerializeField] private Transform _tip;
[SerializeField] private int _EraserWidth = 80;
[SerializeField] private int _EraserLength = 40;
private Renderer _renderer;
private Color[] _colors;
private float _tipHeight;
private  RaycastHit _touch;

private Whiteboard _whiteboard;
private Vector2 _touchPos, _lastTouchPos;
private bool _touchedLastFrame;
private Quaternion _lastTouchRot;
    void Start()
    {
        // getting color array and get the link of the tip
        _renderer = _tip.GetComponent<Renderer>();
        _colors = Enumerable.Repeat(Color.white,_EraserLength*_EraserWidth).ToArray();
        _tipHeight = _tip.localScale.y;
        
    }

   
    void Update()
    {
        // every frame calling this method that keep the update simple
        Draw();
    }

    private void Draw()
    {
        if (!IsServer) return;
        //check if we touch any thing
        if(Physics.Raycast(_tip.position, transform.up, out _touch,_tipHeight))
        {
            //if we touch the whiteboard
        if(_touch.transform.CompareTag("Whiteboard")) 
        {
            //catch whiteboard script
            _whiteboard=_touch.transform.GetComponent<Whiteboard>();
        }
            //TODO This code is not good, _whiteboard may be null, which would need to go to the else case of the outermost if-statement (or rather, the block afterwards, which effectively _is_ the else)
            if (!_whiteboard) return;

        _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);//getting the touch position

        var x =(int)(_touchPos.x *_whiteboard.textureSize.x - (_EraserWidth / 2));//getting pixel which we are touching
        var y =(int)(_touchPos.y *_whiteboard.textureSize.y - (_EraserLength / 2));

        if (y < 0 || y > _whiteboard.textureSize.y || x < 0 || x > _whiteboard.textureSize.x)// if we are out of whiteboard
        {
         return;   
        }
        if (_touchedLastFrame)
        {
            //color the pixel we touch and the pixels between the last fram and the current.
            _whiteboard.setPixelsToTexture(x,y,_EraserWidth,_EraserLength, Color.white);

            for (float f = 0.01f; f<1.00f; f+=0.1f)
            {
                var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x,f);
                var lerpY =(int)Mathf.Lerp(_lastTouchPos.y, y,f);
                _whiteboard.setPixelsToTexture(lerpX, lerpY, _EraserWidth, _EraserLength, Color.white);

            }
            //lock the rotation of the Eraser

            transform.rotation= _lastTouchRot;
            _whiteboard.applyToTexture();
             }
             //values from the last fram to access them
             _lastTouchPos = new Vector2(x,y);
             _lastTouchRot = transform.rotation;
             _touchedLastFrame = true;
             return;

        }
        //if it's not a whiteboard
        _whiteboard =null;
        _touchedLastFrame = false;

    }
}