using Unity.Netcode;
using UnityEngine;

namespace VirtualChemLab.Networking.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : NetworkBehaviour
    {
        public float moveSpeed = 500f;
        public float jumpStrength = 500f;

        private Rigidbody body;
        private float height = 0f;

        private PlayerAnimation animation;
        private float lastVertical = 0, lastHorizontal;
        
        private void Start()
        {
            body = GetComponent<Rigidbody>();
            height = GetComponent<Collider>().bounds.size.y;

            animation = GetComponentInChildren<PlayerAnimation>();
        }

        public void Move(float vertical, float horizontal)
        {
            animation.SetForwardVelocity(vertical);
            animation.SetSidewayVelocity(horizontal);

            Vector3 moveForward = transform.forward * vertical;
            Vector3 moveSideways = transform.right * horizontal;
            Vector3 move = ((moveForward + moveSideways)).normalized * Time.deltaTime * moveSpeed;

            if (body)
            {
                body.velocity = new Vector3(move.x, body.velocity.y, move.z);
            }
        }

        public void Jump()
        {
            if (IsGrounded())
            {
                body.AddForce(Vector3.up * jumpStrength);
            }
        }
        
        public bool IsGrounded()
        {
            return Physics.Raycast(transform.position, -transform.up * height / 2);
        }
    }
}