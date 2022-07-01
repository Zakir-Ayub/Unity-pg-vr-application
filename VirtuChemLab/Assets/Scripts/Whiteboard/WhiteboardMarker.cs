using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

public class WhiteboardMarker : NetworkBehaviour
{
 //setting up pen size and the array to color the pixel which we touch using the marker
[SerializeField] private Transform _tip;
[SerializeField] private int _penSize = 5;
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
        // get access to the color and getting color array and get the link of the tip
        _renderer = _tip.GetComponent<Renderer>();
        _colors = Enumerable.Repeat(_renderer.material.color,_penSize*_penSize).ToArray();
        _tipHeight = _tip.localScale.y;
        
    }

   
    void Update()
    {
        // every frame calling this method that keep the update simple
        Draw();
    }

    private void Draw()
    {
        //Drawing is only calculated Server-Side and then communicated to the clients (See Whiteboard.cs)
        if (!IsServer) return;

        if(Physics.Raycast(_tip.position, transform.up, out _touch,_tipHeight))//check if we touch any thing
        {
            if(_touch.transform.CompareTag("Whiteboard")) //if we touch the whiteboard
            {
                _whiteboard=_touch.transform.GetComponent<Whiteboard>();//catch whiteboard script
            }
            //TODO This code is not good, _whiteboard may be null, which would need to go to the else case of the outermost if-statement (or rather, the block afterwards, which effectively _is_ the else)
            if (!_whiteboard) return;
            _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);//getting the touch position

            var x =(int)(_touchPos.x *_whiteboard.textureSize.x - (_penSize / 2));//getting pixel which we are touching
            var y =(int)(_touchPos.y *_whiteboard.textureSize.y - (_penSize / 2));

            if (y < 0 || y > _whiteboard.textureSize.y || x < 0 || x > _whiteboard.textureSize.x)// if we are out of whiteboard
            {
                return;   
            }
            //color the pixel we touch and the pixels between the last fram and the current.
            if (_touchedLastFrame)
            {
                _whiteboard.setPixelsToTexture(x,y,_penSize,_penSize, _renderer.material.color);

                for (float f = 0.01f; f<1.00f; f+=0.03f)
                {
                    var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x,f);
                    var lerpY =(int)Mathf.Lerp(_lastTouchPos.y, y,f);
                    _whiteboard.setPixelsToTexture(lerpX, lerpY, _penSize, _penSize, _renderer.material.color);

                }
                //lock the rotation of the marker
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
