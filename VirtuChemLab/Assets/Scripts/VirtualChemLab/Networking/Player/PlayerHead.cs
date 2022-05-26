using Unity.Netcode;
using UnityEngine;
using VirtualChemLab.Camera;

namespace VirtualChemLab.Networking.Player
{
    public class PlayerHead : NetworkBehaviour
    {
        public bool hideHead = true;
        private Transform body;
        
        private void Start()
        {
            body = transform.parent;
            
            if (IsOwner)
            {
                UnityEngine.Camera.main.GetComponent<FirstPersonCamera>().head = transform;
            }
        }

        public void Rotate(float vertical, float horizontal)
        {
            if (body)
            {
                float rotationY = body.localEulerAngles.y + horizontal * Time.deltaTime;
                float rotationX = transform.localEulerAngles.x - vertical * Time.deltaTime;
            
                transform.localEulerAngles = new Vector3(rotationX, 0, 0);
                body.eulerAngles = new Vector3(0, rotationY, 0);
            }
        }
    }
}