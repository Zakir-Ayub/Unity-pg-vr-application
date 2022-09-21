using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Network.UI
{
    /// <summary>
    /// Copies the username of the player to the
    /// <see cref="Text"/> label.
    /// </summary>
    public class NetworkUsername : NetworkBehaviour
    {
        [SerializeField]
        private NetworkPlayer player;

        private Image background;
        private Text username;

        private void Start()
        {
            background = GetComponentInChildren<Image>();
            username = GetComponentInChildren<Text>();
            
            // we don't want to see our own username
            if (IsOwner)
            {
                background.enabled = false;
                username.enabled = false;
            }
        }

        private void Update()
        {
            username.text = player.Username;
        }
    }
}