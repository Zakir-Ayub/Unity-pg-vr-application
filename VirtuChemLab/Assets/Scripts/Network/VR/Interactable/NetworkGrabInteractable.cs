using System.Collections;
using Network.Transform;
using Network.VR.Event;
using Network.VR.Hand;
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
    ///
    /// <example>
    /// Another player grabs an object and I (the local player) will see
    /// the object in the other players hand. Or another player grabs an
    /// object and a sound should be played.
    /// </example>
    /// </summary>
    [RequireComponent(typeof(NetworkXRGrabInteractable))]
    [RequireComponent(typeof(NetworkOwnerTransform))]
    public class NetworkGrabInteractable : NetworkBaseInteractable
    {
        private XRGrabInteractable grabInteractable;
        private NetworkObject networkObject;
        private new Rigidbody rigidbody;
        
        // rigidbody settings before grab 
        private bool wasKinematic, usedGravity;
        private float oldDrag, oldAngularDrag;

        protected override void Start()
        {
            base.Start();
            
            rigidbody = GetComponent<Rigidbody>();
            networkObject = GetComponentInParent<NetworkObject>();
            
            grabInteractable = GetComponent<XRGrabInteractable>();
            grabInteractable.selectExited.AddListener(OnDrop);
        }

        protected override void OnFirstSelectEnterServer(AbstractNetworkEventArgs args)
        {
            base.OnFirstSelectEnterServer(args);
            
            // store current rigidbody settings
            // must be called before setting ownership
            SaveRigidbodySettings();
            
            // if the client wants to move the object, he needs to own it
            networkObject.ChangeOwnership(args.ClientId);
            
            // to minimize desync, client can also move children network objects
            foreach (UnityEngine.Transform child in networkObject.transform.GetComponentsInChildren<UnityEngine.Transform>())
            {
                NetworkObject childNetworkObject = child.GetComponent<NetworkObject>();
                if (childNetworkObject)
                {
                    childNetworkObject.ChangeOwnership(args.ClientId);
                }
            }
        }

        protected override void OnSelectEnterServer(AbstractNetworkEventArgs args)
        {
            base.OnSelectEnterServer(args);
            
            // temporarily disable players hand collider to avoid collisions
            SetPlayerHandCollider(args.ClientId, GetHand(args), false);
        }
        
        private void OnDrop(SelectExitEventArgs args)
        {
            if (IsClient)
            {
                // set velocity on client to minimize desync
                rigidbody.velocity = GetHandVelocity(args.interactorObject);
                rigidbody.angularVelocity = Vector3.zero;
            }
        }

        protected override void OnSelectExitServer(NetworkSelectExitEventArgs args)
        {
            base.OnSelectExitServer(args);
            
            // re-enable hand collider after after a short while, so dropped object can drop without colliding
            StartCoroutine(SetPlayerHandColliderDelay(args.ClientId, GetHand(args), true));
            
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

            networkObject.transform.position = args.LastPosition;
            networkObject.transform.rotation = args.LastRotation; 
            
            // restore old rigidbody settings
            RestoreRigidbodySettings(args.HandVelocity);
        }

        /// <summary>
        /// Saves current rigidbody state and stops rigidbody
        /// from moving.
        /// </summary>
        private void SaveRigidbodySettings()
        {
            wasKinematic = rigidbody.isKinematic;
            usedGravity = rigidbody.useGravity;
            oldDrag = rigidbody.drag;
            oldAngularDrag = rigidbody.angularDrag;
            rigidbody.isKinematic = grabInteractable.movementType == XRBaseInteractable.MovementType.Instantaneous ||
                                    grabInteractable.movementType == XRBaseInteractable.MovementType.Kinematic;
            rigidbody.useGravity = false;
            rigidbody.drag = 0f;
            rigidbody.angularDrag = 0f;
        }

        /// <summary>
        /// Restores the old rigidbody state.
        /// </summary>
        private void RestoreRigidbodySettings(Vector3 handVelocity)
        {
            rigidbody.isKinematic = wasKinematic;
            rigidbody.useGravity = usedGravity;
            rigidbody.drag = oldDrag;
            rigidbody.angularDrag = oldAngularDrag;

            rigidbody.velocity = handVelocity;
            rigidbody.angularVelocity = Vector3.zero;
        }

        /// <summary>
        /// See <see cref="SetPlayerHandCollider"/> but with a small delay.
        /// Used so that the objects which is dropped doesn't immediately
        /// collide with the players hand.
        /// </summary>
        /// <returns></returns>
        private IEnumerator SetPlayerHandColliderDelay(ulong clientId, VRHand hand, bool isEnabled)
        {
            yield return new WaitForSeconds(0.5f);
            SetPlayerHandCollider(clientId, hand, isEnabled);
        }

        /// <summary>
        /// Enables or disables the players hand hand colliders on the
        /// server.
        /// </summary>
        /// <param name="clientId">The client id of the player for which to toggle the colliders</param>
        /// <param name="hand">Left or right hand?</param>
        /// <param name="isEnabled">Enable or disable colliders</param>
        private void SetPlayerHandCollider(ulong clientId, VRHand hand, bool isEnabled)
        {
            // only set hand colliders on server and on other players
            if (IsServer && clientId != NetworkManager.Singleton.LocalClientId)
            {
                VRRig rig = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<NetworkPlayer>().vrRig;
                switch (hand)
                {
                    case VRHand.Left:
                        rig.SetLeftHandColliders(isEnabled);
                        break;
                    case VRHand.Right:
                        rig.SetRightHandColliders(isEnabled);
                        break;
                }
            }
        }

        private VRHand GetHand(AbstractNetworkEventArgs args)
        {
            NetworkPlayer player = NetworkManager.LocalClient.PlayerObject.GetComponent<NetworkPlayer>();
            IXRInteractor interactor = args.LocalInteractor;

            if (interactor is NetworkFakeController controller)
            {
                if (controller == player.LeftHandInteractor)
                {
                    return VRHand.Left;
                }
                if (controller == player.RightHandInteractor)
                {
                    return VRHand.Right;
                }
            }

            return VRHand.Unknown;
        }
    }
}