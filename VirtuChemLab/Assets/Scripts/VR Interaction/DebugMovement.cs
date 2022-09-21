using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMovement : MonoBehaviour
{
    Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Debug.DrawLine(transform.position, lastPosition, Color.yellow, 50, false);
        lastPosition = transform.position;
    }
}
