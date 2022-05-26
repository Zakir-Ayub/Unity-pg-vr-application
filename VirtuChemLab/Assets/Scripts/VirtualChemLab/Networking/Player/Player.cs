using Unity.Netcode;
using UnityEngine;
using VirtualChemLab.Networking.Device;

namespace VirtualChemLab.Networking.Player
{
    public class Player : NetworkBehaviour
    {
        public GameObject devicePrefab;
        public float interactLength = 5f;

        private PlayerHead head;

        private void Start()
        {
            head = GetComponentInChildren<PlayerHead>();
        }

        public void Interact()
        {
            InteractServerRpc();
        }
        
        [ServerRpc]
        private void InteractServerRpc()
        {
            Transform origin = head.transform;
            foreach (RaycastHit hit in Physics.RaycastAll(origin.position, origin.forward, interactLength))
            {
                DeviceToggle possibleDevice = hit.collider.gameObject.GetComponent<DeviceToggle>();
                if (possibleDevice)
                {
                    possibleDevice.Toggle();
                    break;
                }
            }
        }

        public void SpawnDevice()
        {
            SpawnDeviceServerRpc();
        }
    
        [ServerRpc]
        private void SpawnDeviceServerRpc()
        {
            GameObject device = Instantiate(devicePrefab, transform.position, Quaternion.identity);
            device.GetComponent<NetworkObject>().Spawn();
        }
    }
}