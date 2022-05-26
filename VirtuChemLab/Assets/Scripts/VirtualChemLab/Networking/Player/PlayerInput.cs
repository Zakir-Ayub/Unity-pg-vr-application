using Unity.Netcode;
using UnityEngine;

namespace VirtualChemLab.Networking.Player
{
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerInput : NetworkBehaviour
    {
        public float mouseSensitivity = 500f;
        
        private Player player;
        private PlayerMovement movement;
        private PlayerHead head;
        private PlayerDance dance;

        private void Start()
        {
            player = GetComponent<Player>();
            movement = GetComponent<PlayerMovement>();
            head = GetComponentInChildren<PlayerHead>();
            dance = GetComponent<PlayerDance>();
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }
            
            movement.Move(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
            if (Input.GetKeyDown(KeyCode.Space))
            {
                movement.Jump();
            }
            head.Rotate(Input.GetAxis("Mouse Y") * mouseSensitivity, Input.GetAxis("Mouse X") * mouseSensitivity);
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                player.SpawnDevice();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                player.Interact();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                dance.Dance(1);
            }
        }
    }
}