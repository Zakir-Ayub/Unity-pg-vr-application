using Unity.Netcode;
using UnityEngine;

namespace Network.VR
{
    /// <summary>
    /// Abstract parent class for grab and simple interactables, which are to be
    /// networked.
    /// </summary>
    public abstract class AbstractNetworkInteractable : NetworkBehaviour
    {
        public delegate void InteractDelegate(GameObject player);
        
        protected static GameObject GetPlayer(ulong clientId)
        {
            return NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject;
        }
    }
}