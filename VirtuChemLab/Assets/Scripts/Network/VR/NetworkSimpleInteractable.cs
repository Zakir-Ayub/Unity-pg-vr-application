using Network.Transform;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Network.VR
{
    /// <summary>
    /// When a player interacts with an object, this component will
    /// send a message to the server. The server will then inform it's
    /// listening components that a player interacted with this object.
    /// For now these listeners will only be informed on the server.
    /// </summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkSimpleInteractable : AbstractNetworkInteractable
    {
        private NetworkObject networkObject;
        private XRSimpleInteractable interactable;
        public InteractDelegate OnSelect, OnDeselect, OnActivate, OnDeactivate;
        
        private void Start()
        {
            interactable = GetComponent<XRSimpleInteractable>();
            networkObject = GetComponent<NetworkObject>();
            
            interactable.selectEntered.AddListener(OnSelectEnter);
            interactable.selectExited.AddListener(OnSelectExit);
            
            interactable.activated.AddListener(OnActivateEnter);
            interactable.deactivated.AddListener(OnActivateExit);
        }
    
        private void OnSelectEnter(SelectEnterEventArgs args)
        {
            OnSelectEnterServerRpc();
        }
    
        [ServerRpc(RequireOwnership = false)]
        private void OnSelectEnterServerRpc(ServerRpcParams args = default)
        {
            ulong clientId = args.Receive.SenderClientId;
            
            // if the client wants to move the object, he needs to own it
            // used for rotating button on magnet stirrer
            networkObject.ChangeOwnership(clientId);
            
            OnSelect?.Invoke(GetPlayer(args.Receive.SenderClientId));
        }
        
        private void OnSelectExit(SelectExitEventArgs args)
        {
            OnSelectExitServerRpc();
        }
    
        [ServerRpc(RequireOwnership = false)]
        private void OnSelectExitServerRpc(ServerRpcParams args = default)
        {
            networkObject.ChangeOwnership(NetworkManager.ServerClientId);
            OnDeselect?.Invoke(GetPlayer(args.Receive.SenderClientId));
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
    }
}
