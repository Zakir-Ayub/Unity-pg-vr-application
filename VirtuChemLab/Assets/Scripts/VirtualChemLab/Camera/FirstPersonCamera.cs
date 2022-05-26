using System;
using UnityEngine;

namespace VirtualChemLab.Camera
{
    public class FirstPersonCamera : MonoBehaviour
    {
        public Transform head;

        private void Update()
        {
            if (head)
            {
                transform.position = head.position;
                transform.rotation = head.rotation;
            }
        }
    }
}