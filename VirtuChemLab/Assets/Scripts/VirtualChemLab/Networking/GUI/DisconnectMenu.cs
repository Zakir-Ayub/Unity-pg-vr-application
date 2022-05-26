using Unity.Netcode;
using UnityEngine;

namespace VirtualChemLab.Networking.GUI
{
    public class DisconnectMenu : MonoBehaviour
    {
        void OnGUI()
        {
            if (!NetworkManager.Singleton.IsClient)
            {
                return;
            }
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            RenderMenu();
            GUILayout.EndArea();
        }

        public void RenderMenu()
        {
            if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Stop Server" : "Disconnect"))
            {
                Disconnect();
            }
        }
        
        public void Disconnect()
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}