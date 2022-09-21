using Network.Manager;
using UnityEditor;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// Utility functions for managing other players
    /// inside the editor.
    /// </summary>
    [CustomEditor(typeof(NetworkPlayer))]
    public class NetworkPlayerEditor : Editor
    {
        /// <inheritdoc cref="Editor"/>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (NetworkLobbyManager.Singleton.IsConnected())
            {
                if (GUILayout.Button("Kick"))
                {
                    KickPlayer();
                }
            }
        }

        /// <summary>
        /// Kicks the player from the session.
        /// </summary>
        private void KickPlayer()
        {
            NetworkPlayer player = (NetworkPlayer) target;
            player.KickPlayer();
        }
    }
}