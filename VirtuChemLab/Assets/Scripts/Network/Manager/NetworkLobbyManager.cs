using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Network.Manager
{
    /// <summary>
    /// NetworkManager adapter used to host, connect and
    /// disconnect. 
    /// </summary>
    public class NetworkLobbyManager : MonoBehaviour
    {
        public void Host()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void Connect(string ip, ushort port)
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.ConnectionData.Address = ip;
            transport.ConnectionData.Port = port;

            NetworkManager.Singleton.StartClient();
        }

        public void Disconnect()
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}