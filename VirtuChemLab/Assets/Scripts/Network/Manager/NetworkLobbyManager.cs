using System;
using System.Collections.Generic;
using System.Text;
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
        public static NetworkLobbyManager Singleton { get; private set; }
        
        /// <summary>
        /// Maps connected client id to username
        /// </summary>
        private Dictionary<ulong, string> usernames = new Dictionary<ulong, string>(); 

        private void Awake()
        {
            if (Singleton != null) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Singleton = this; 
            }
        }

        private void Start()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApproveConnection;
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnect;
        }

        /// <inheritdoc cref="NetworkManager.ConnectionApprovalCallback"/>
        private void ApproveConnection(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            string username = Encoding.UTF8.GetString(request.Payload);
            if (string.IsNullOrEmpty(username))
            {
                username = "";
            }
            usernames.Add(request.ClientNetworkId, username);
                
            response.Pending = false;
            response.CreatePlayerObject = true;
            response.Approved = true;
        }

        /// <summary>
        /// Called on server and the client which connected
        /// </summary>
        /// <param name="clientId">The id of the client who connected</param>
        private void OnConnect(ulong clientId)
        {
            if (IsServer())
            {
                // set the username of the player
                NetworkPlayer player = GetPlayer(clientId);
                player.Username = usernames[clientId];
            }
        }
        
        /// <summary>
        /// Called on server and the client which disconnected
        /// </summary>
        /// <param name="clientId">The id of the client who disconnected</param>
        private void OnDisconnect(ulong clientId)
        {
            usernames.Remove(clientId);
        }

        public void Host(string username)
        {
            SetConnectionData(username);
            NetworkManager.Singleton.StartHost();
        }

        /// <summary>
        /// Connect to the given IP and port.
        /// </summary>
        /// <param name="username">The username to assign our player</param>
        /// <param name="ip">The IP address to connect to</param>
        /// <param name="port">The port at which the host is listening</param>
        public void Connect(string username, string ip, ushort port)
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.ConnectionData.Address = ip;
            transport.ConnectionData.Port = port;
            
            SetConnectionData(username);
            NetworkManager.Singleton.StartClient();
        }

        /// <summary>
        /// Set the data which is sent to the server on
        /// connecting.
        /// </summary>
        private void SetConnectionData(string username)
        {
            byte[] data = Encoding.UTF8.GetBytes(username);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = data;
        }

        /// <summary>
        /// Disconnect from the server or stop the server,
        /// depending on whether we are connected or hosting.
        /// </summary>
        public void Disconnect()
        {
            NetworkManager.Singleton.Shutdown();
            usernames.Clear();
        }

        /// <summary>
        /// Get the <see cref="NetworkPlayer"/> for the
        /// given client id.
        /// </summary>
        public NetworkPlayer GetPlayer(ulong clientId)
        {
            NetworkClient client = GetClient(clientId);
            return client.PlayerObject.transform.GetComponent<NetworkPlayer>();
        }
        
        /// <summary>
        /// Get all connected players.
        /// </summary>
        public List<NetworkPlayer> GetConnectedPlayers()
        {
            List<NetworkPlayer> players = new List<NetworkPlayer>();

            foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
            {
                players.Add(GetPlayer(client.Key));
            }

            return players;
        }

        /// <summary>
        /// Get the <see cref="NetworkClient"/> for the
        /// given client id.
        /// </summary>
        private NetworkClient GetClient(ulong clientId)
        {
            return NetworkManager.Singleton.ConnectedClients[clientId];
        }

        /// <summary>
        /// Return the client id of the given <see cref="NetworkPlayer"/>
        /// </summary>
        /// <returns>Client id or -1 if not found</returns>
        public ulong GetClientIdForPlayer(NetworkPlayer player)
        {
            foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
            {
                NetworkPlayer connectedPlayer = GetPlayer(client.Key);
                if (connectedPlayer == player)
                {
                    return client.Key;
                }
            }

            throw new ArgumentException("Could not find player");
        }

        /// <summary>
        /// Returns true when we are connected to a server
        /// or hosting ourselves.
        /// </summary>
        public bool IsConnected()
        {
            return NetworkManager.Singleton.IsListening;
        }
        
        /// <summary>
        /// Returns whether we are the hosting server.
        /// </summary>
        private bool IsServer()
        {
            return NetworkManager.Singleton.IsServer;
        }

        /// <summary>
        /// Returns whether we are a client.
        /// </summary>
        /// <returns></returns>
        private bool IsClient()
        {
            return NetworkManager.Singleton.IsClient;
        }
    }
}