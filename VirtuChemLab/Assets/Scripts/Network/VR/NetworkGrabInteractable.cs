using System.Collections;
using System.Linq;
using Network.Transform;
using Network.XR;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VR;

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
        public InteractDelegate OnFirstGrab, OnGrab, OnDrop, OnLastDrop, OnActivate, OnDeactivate;
        
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
            
            interactable.selectEntered.AddListener(OnSelectEnter);
            interactable.selectExited.AddListener(OnSelectExit);
            
            interactable.activated.AddListener(OnActivateEnter);
            interactable.deactivated.AddListener(OnActivateExit);

            physicsHands = FindObjectsOfType<HandPresencePhysics>();
        }

        private Vector3 GetVelocity(GameObject handGameObject)
        {
            var hand = physicsHands.First(x => x.actionController.Equals(handGameObject.GetComponent<XRBaseController>()));
            return hand.GetComponent<Rigidbody>().velocity;
        }

        private void OnSelectEnter(SelectEnterEventArgs args)
        {
            Hand hand = GetHand(args.interactorObject.transform.gameObject);

            if (interactable.interactorsSelecting.Count == 1)
            {
                OnFirstSelectEnterServerRpc(hand);
            }
            OnSelectEnterServerRpc(hand);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void OnFirstSelectEnterServerRpc(Hand hand, ServerRpcParams args = default)
        {
            ulong clientId = args.Receive.SenderClientId;

            // must be called before setting ownership
            SetupRigidBodyGrab();
            
            // if the client wants to move the object, he needs to own it
            networkObject.ChangeOwnership(clientId);
            
            // to minimize desync, client can also move children network objects
            foreach (UnityEngine.Transform child in transform.GetComponentsInChildren<UnityEngine.Transform>())
            {
                NetworkObject childNetworkObject = child.GetComponent<NetworkObject>();
                if (childNetworkObject)
                {
                    childNetworkObject.ChangeOwnership(clientId);
                }
            }
            
            OnFirstGrab?.Invoke(GetPlayer(args.Receive.SenderClientId));
        }
    
        [ServerRpc(RequireOwnership = false)]
        private void OnSelectEnterServerRpc(Hand hand, ServerRpcParams args = default)
        {
            // temporarily disable players hand collider
            SetPlayerHandCollider(args.Receive.SenderClientId, hand, false);
            
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
            rigidbody.isKinematic = interactable.movementType == XRBaseInteractable.MovementType.Instantaneous ||
                                    interactable.movementType == XRBaseInteractable.MovementType.Kinematic;
            rigidbody.useGravity = false;
            rigidbody.drag = 0f;
            rigidbody.angularDrag = 0f;
        }

        private void OnSelectExit(SelectExitEventArgs args)
        {
            GameObject handGameObject = args.interactorObject.transform.gameObject;
            Hand hand = GetHand(handGameObject);
            var velocity = GetVelocity(handGameObject);
            if (IsClient)
            {
                // reset velocity on client, otherwise the object might fall
                // rapidly on the client which can cause de-sync
                rigidbody.velocity = velocity;
                rigidbody.angularVelocity = Vector3.zero;
            }
            
            OnSelectExitServerRpc(hand, transform.position, transform.rotation, velocity);
            if (interactable.interactorsSelecting.Count == 0)
            {
                OnLastSelectExitServerRpc();
            }
        }
    
        [ServerRpc(RequireOwnership = false)]
        private void OnSelectExitServerRpc(Hand hand, Vector3 position, Quaternion rotation, Vector3 velocity, ServerRpcParams args = default)
        {
            ulong clientId = args.Receive.SenderClientId;
            
            // re-enable hand collider after after a short while, so dropped object can drop without colliding
            StartCoroutine(SetPlayerHandColliderDelay(clientId, hand, true));
            
            // set owner back to server
            networkObject.ChangeOwnership(NetworkManager.ServerClientId);
            
            // also set children owner back to server
            foreach (UnityEngine.Transform child in transform.GetComponentsInChildren<UnityEngine.Transform>())
            {
                NetworkObject childNetworkObject = child.GetComponent<NetworkObject>();
                if (childNetworkObject)
                {
                    childNetworkObject.ChangeOwnership(NetworkManager.ServerClientId);
                }
            }
            
            // set last known position to minimize de-sync
            networkObject.transform.position = position;
            networkObject.transform.rotation = rotation;
            
            SetupRigidBodyDrop();
            rigidbody.velocity = velocity;
            
            OnDrop?.Invoke(GetPlayer(args.Receive.SenderClientId));
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void OnLastSelectExitServerRpc(ServerRpcParams args = default)
        {
            OnLastDrop?.Invoke(GetPlayer(args.Receive.SenderClientId));
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

        private Hand GetHand(GameObject handGameObject)
        {
            Hand hand = Hand.Unknown;
            if (handGameObject.CompareTag(VRLeftHandTag))
            {
                hand = Hand.Left;
            }
            else if (handGameObject.CompareTag(VRRightHandTag))
            {
                hand = Hand.Right;
            }

            return hand;
        }
    }
}