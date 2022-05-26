using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace VirtualChemLab.Networking.GUI
{
    public class ConnectMenu : MonoBehaviour
    {
        public string ipAddress = "127.0.0.1";
        public string port = "7777";

        void OnGUI()
        {
            if (NetworkManager.Singleton.IsClient)
            {
                return;
            }
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            RenderMenu();
            GUILayout.EndArea();
        }

        private void RenderMenu()
        {
            if (GUILayout.Button("Host"))
            {
                NetworkManager.Singleton.StartHost();
            }

            ipAddress = GUILayout.TextField(ipAddress);
            port = GUILayout.TextField(port);

            if (GUILayout.Button("Client"))
            {
                UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.ConnectionData.Address = ipAddress;
                transport.ConnectionData.Port = ushort.Parse(port);

                NetworkManager.Singleton.StartClient();
            }
        }
    }
}