using System;
using Unity.Netcode;
using UnityEngine;

public class VRPlayer : NetworkBehaviour
{
    public Transform vrCamera;

    private void Start()
    {
        if (IsOwner)
        {
            vrCamera = GameObject.FindWithTag("VRPlayer").transform;
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            if (vrCamera)
            {
                Vector3 vrRotation = vrCamera.eulerAngles;
                transform.eulerAngles = new Vector3(0, vrRotation.y, 0);
                transform.position = vrCamera.position;
            }
        }
    }
}
