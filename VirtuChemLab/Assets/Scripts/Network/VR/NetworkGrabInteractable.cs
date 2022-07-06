using System.Collections;
using Network.Transform;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VR;
using Network.XR;
using System.Linq;

namespace Network.VR
{ 
    /// <summary>
    /// When a player grabs an object, this component will
    /// send a message to the server. The server will then inform it's
    /// listening components that a player grabbed this object.
    /// For now these listeners will only be informed on the server.
    ///
    /// <example>
    /// Another player grabs an object and I (the local player) will see
    /// the object in the other players hand.
    /// </example>
    /// </summary>
    [RequireComponent(typeof(NetworkXRGrabInteractable))]
    [RequireComponent(typeof(NetworkOwnerTransform))]
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkGrabInteractable : AbstractNetworkInteractable
    {
        public const string VRLeftHandTag = "VRLeftHand";
        public const string VRRightHandTag = "VRRightHand";
        private enum Hand
        {
            Unknown, Left, Right
        }
        
        private NetworkObject networkObject;
        private XRGrabInteractable interactable;
        
        // register listeners on these events to know when another player interacted
        public InteractDelegate OnGrab, OnDrop, OnActivate, OnDeactivate;
        
        private new Rigidbody rigidbody;
        
        // rigidbody settings before grab 
        private bool wasKinematic, usedGravity;
        private float oldDrag, oldAngularDrag;

        private HandPresencePhysics[] physicsHands;
            
        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            interactable = GetComponent<XRGrabInteractable>();
            networkObject = GetComponent<NetworkObject>();

            interactable.selectEntered.AddListener(OnSelectEntered);
            interactable.selectExited.AddListener(OnSelectExit);
            
            interactable.activated.AddListener(OnActivateEnter);
            interactable.deactivated.AddListener(OnActivateExit);

            physicsHands = FindObjectsOfType<HandPresencePhysics>();
        }

        private Vector3 getVelocity(GameObject handGameObject)
        {
            var hand = physicsHands.Where(
                x => x.actionController.Equals(handGameObject.GetComponent<XRBaseController>())
                ).First();
            return hand.GetComponent<Rigidbody>().velocity;
        }
        
        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            GameObject handGameObject = args.interactorObject.transform.gameObject;
            Hand hand = Hand.Unknown;
            if (handGameObject.CompareTag(VRLeftHandTag))
            {
                hand = Hand.Left;
            }
            else if (handGameObject.CompareTag(VRRightHandTag))
            {
                hand = Hand.Right;
            }
            
            OnSelectEnteredServerRpc(hand);
        }
    
        [ServerRpc(RequireOwnership = false)]
        private void OnSelectEnteredServerRpc(Hand hand, ServerRpcParams args = default)
        {
            ulong clientId = args.Receive.SenderClientId;
            
            // temporarily disable players hand collider
            SetPlayerHandCollider(clientId, hand, false);

            // must be called before setting ownershi
            SetupRigidBodyGrab();
            
            // if the client wants to move the object, he needs to own it
            networkObject.ChangeOwnership(clientId);
            
            OnGrab?.Invoke(GetPlayer(args.Receive.SenderClientId));
        }

        /// <summary>
        /// Saves current rigidbody state and stops rigidbody
        /// from moving.
        /// </summary>
        private void SetupRigidBodyGrab()
        {
            wasKinematic = rigidbody.isKinematic;
            usedGravity = rigidbody.useGravity;
            oldDrag = rigidbody.drag;
            oldAngularDrag = rigidbody.angularDrag;
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
            rigidbody.drag = 0f;
            rigidbody.angularDrag = 0f;
        }

        private void OnSelectExit(SelectExitEventArgs args)
        {
            GameObject handGameObject = args.interactorObject.transform.gameObject;
            Hand hand = Hand.Unknown;
            if (handGameObject.CompareTag(VRLeftHandTag))
            {
                hand = Hand.Left;
            }
            else if (handGameObject.CompareTag(VRRightHandTag))
            {
                hand = Hand.Right;
            }
            var velocity = getVelocity(handGameObject);
            if (IsClient)
            {
                // reset velocity on client, otherwise the object might fall
                // rapidly on the client which can cause de-sync
                rigidbody.velocity = velocity;
                rigidbody.angularVelocity = Vector3.zero;
            }
            
            OnSelectExitServerRpc(transform.position, transform.rotation, hand, velocity);
        }
    
        [ServerRpc(RequireOwnership = false)]
        private void OnSelectExitServerRpc(Vector3 position, Quaternion rotation, Hand hand, Vector3 velocity, ServerRpcParams args = default)
        {
            ulong clientId = args.Receive.SenderClientId;
            
            // re-enable hand collider after after a short while, so dropped object can drop without colliding
            StartCoroutine(SetPlayerHandColliderDelay(clientId, hand, true));
            
            // set owner back to server
            networkObject.ChangeOwnership(NetworkManager.ServerClientId);
            
            // set last known position to minimize de-sync
            networkObject.transform.position = position;
            networkObject.transform.rotation = rotation;
            
            SetupRigidBodyDrop();
            rigidbody.velocity = velocity;

            OnDrop?.Invoke(GetPlayer(args.Receive.SenderClientId));
        }
        
        /// <summary>
        /// Restores the old rigidbody state.
        /// </summary>
        private void SetupRigidBodyDrop()
        {
            rigidbody.isKinematic = wasKinematic;
            rigidbody.useGravity = usedGravity;
            rigidbody.drag = oldDrag;
            rigidbody.angularDrag = oldAngularDrag;
            
            rigidbody.angularVelocity = Vector3.zero;
        }
        
        private void OnActivateEnter(ActivateEventArgs args)
        {
            OnActivateEnterServerRpc();
        }
    
        [ServerRpc(RequireOwnership = false)]
        private void OnActivateEnterServerRpc(ServerRpcParams args = default)
        {
            OnActivate?.Invoke(GetPlayer(args.Receive.SenderClientId));
        }
        
        private void OnActivateExit(DeactivateEventArgs args)
        {
            OnActivateExitServerRpc();
        }
    
        [ServerRpc(RequireOwnership = false)]
        private void OnActivateExitServerRpc(ServerRpcParams args = default)
        {
            OnDeactivate?.Invoke(GetPlayer(args.Receive.SenderClientId));
        }

        private IEnumerator SetPlayerHandColliderDelay(ulong clientId, Hand hand, bool isEnabled)
        {
            yield return new WaitForSeconds(0.5f);
            SetPlayerHandCollider(clientId, hand, isEnabled);
        }

        private void SetPlayerHandCollider(ulong clientId, Hand hand, bool isEnabled)
        {
            // only set hand colliders on server and on other players
            if (IsServer && clientId != NetworkManager.Singleton.LocalClientId)
            {
                VRRig rig = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<NetworkCharacter>().vrRig;
                switch (hand)
                {
                    case Hand.Left:
                        rig.SetLeftHandColliders(isEnabled);
                        break;
                    case Hand.Right:
                        rig.SetRightHandColliders(isEnabled);
                        break;
                }
            }
        }
    }
}