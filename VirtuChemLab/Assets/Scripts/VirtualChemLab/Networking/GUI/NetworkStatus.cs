using System;
using Unity.Netcode;
using UnityEngine;

namespace VirtualChemLab.Networking.GUI
{
    public class NetworkStatus : MonoBehaviour
    {
        private void OnGUI()
        {
            if (!NetworkManager.Singleton.IsClient)
            {
                return;
            }
            
            GUILayout.BeginArea(new Rect(350, 10, 300, 300));
            RenderNetworkStatus();
            GUILayout.EndArea();
        }

        private void RenderNetworkStatus()
        {
            NetworkManager network = NetworkManager.Singleton;

            string mode = network.IsClient ?  "Client" : "Host";
            string transport = network.NetworkConfig.NetworkTransport.GetType().Name;

            GUILayout.Label("Transport: " + transport);
            GUILayout.Label("Mode:" + mode);
        }
    }
}