using ParrelSync;
using Unity.Netcode;
using UnityEngine;

namespace Network.Manager
{
    /// <summary>
    /// Utility script which auto starts server/client when
    /// pressing play.
    /// </summary>
    public class NetworkAutoStart : MonoBehaviour
    {
        [Tooltip("Automatically start server if we are not a clone (see ParrelSync)")]
        public bool autoStartServer = true;
    
        [Tooltip("Automatically connect to server if we are a clone (see ParrelSync)")]
        public bool autoConnectClient = true;
    
        private void Start()
        {
            NetworkManager m = NetworkManager.Singleton;

            if (!ClonesManager.IsClone() && autoStartServer)
            {
                m.StartHost();
            }
            else if (ClonesManager.IsClone() && autoConnectClient)
            {
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}