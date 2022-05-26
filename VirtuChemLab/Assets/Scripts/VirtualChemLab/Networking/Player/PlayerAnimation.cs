using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace VirtualChemLab.Networking.Player
{
    public class PlayerAnimation : NetworkBehaviour
    {
        private Animator animator;
        private PlayerDance playerDance;
        private Rigidbody rigidbody;
        
        private int dance1Hash = Animator.StringToHash("Dance1");
        
        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            rigidbody = GetComponent<Rigidbody>();
            
            playerDance = GetComponent<PlayerDance>();
            playerDance.OnDance += StartDance;
        }

        private void Update()
        {
            Vector3 velocity = transform.InverseTransformDirection(rigidbody.velocity);

            float xVelocity = velocity.x;
            float yVelocity = velocity.y;
            float zVelocity = velocity.z;

            bool isDancing = animator.GetCurrentAnimatorStateInfo(0).fullPathHash == dance1Hash;
            if (isDancing)
            {
                Debug.Log("dancing");
            }
            
            animator.SetFloat("Upward Velocity", yVelocity);
        }

        public void SetForwardVelocity(float velocity)
        {
            if (IsOwner)
            {
                SetForwardVelocityServerRpc(velocity);
            }
        }
        
        [ServerRpc]
        private void SetForwardVelocityServerRpc(float velocity)
        {
            animator.SetFloat("Forward Velocity", velocity);
        }

        public void SetSidewayVelocity(float velocity)
        {
            if (IsOwner)
            {
                SetSidewayVelocityServerRpc(velocity);
            }
        }
        
        [ServerRpc]
        private void SetSidewayVelocityServerRpc(float velocity)
        {
            animator.SetFloat("Sideways Velocity", velocity);
        }
        
        private void StartDance(int number)
        {
            if (IsOwner)
            {
                StartDanceServerRpc(number);
            }
        }
        
        [ServerRpc]
        private void StartDanceServerRpc(int number)
        {
            if (number == 1)
            {
                animator.SetTrigger("Dance1");
            }
        }
    }
}
