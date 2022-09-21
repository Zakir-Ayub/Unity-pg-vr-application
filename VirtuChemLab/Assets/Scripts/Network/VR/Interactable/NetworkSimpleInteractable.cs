using Network.VR.Event;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Network.VR
{
    /// <summary>
    /// Extends the <see cref="NetworkBaseInteractable"/> class, by also
    /// changing the owner of the interactable. This the player can move
    /// and change the interactable however he wants.
    /// </summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class NetworkSimpleInteractable : NetworkBaseInteractable
    {
        private NetworkObject networkObject;

        protected override void Start()
        {
            base.Start();
            networkObject = GetComponentInParent<NetworkObject>();
        }

        
        protected override void OnFirstSelectEnterServer(AbstractNetworkEventArgs args)
        {
            base.OnFirstSelectEnterServer(args);
            
            // if the client wants to rotate the object, he needs to own it
            networkObject.ChangeOwnership(args.ClientId);
            
            // to minimize desync, client can also directly move children network objects
            foreach (UnityEngine.Transform child in networkObject.transform.GetComponentsInChildren<UnityEngine.Transform>())
            {
                NetworkObject childNetworkObject = child.GetComponent<NetworkObject>();
                if (childNetworkObject)
                {
                    childNetworkObject.ChangeOwnership(args.ClientId);
                }
            }
        }
        
        protected override void OnSelectExitServer(NetworkSelectExitEventArgs args)
        {
            base.OnSelectExitServer(args);
            
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
        }
    }
}
