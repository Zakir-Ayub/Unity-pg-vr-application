using ParrelSync;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
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
            if (!ClonesManager.IsClone() && autoStartServer)
            {
                NetworkLobbyManager.Singleton.Host("Host");
            }
            else if (ClonesManager.IsClone() && autoConnectClient)
            {
                UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                NetworkLobbyManager.Singleton.Connect("Client", transport.ConnectionData.Address, transport.ConnectionData.Port);
            }
        }
    }
}