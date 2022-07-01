using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Network.Ownership
{
    /// <summary>
    /// Use to hide/disable certain components of a <c>GameObject</c>
    /// depending on whether we are connected or not.
    ///
    /// <example>
    /// Only show connect button when we are not already connected.
    /// Only show disconnect button when we are connected.
    /// </example>
    /// </summary>
    public class NetworkHide : MonoBehaviour
    {
        [Tooltip("Hide components when we are connected or disconnected?")]
        public bool whenConnected = true;
        
        public bool disableRenderers = true;
        public bool disableColliders = true;
        public bool disableUI = true;
        public bool disableChildren = false;
        
        /// <summary>
        /// Used to detect connection state changes
        /// </summary>
        private bool prevIsConnected = false;
        
        private void Start()
        {
            if (IsConnected())
            {
                OnConnect();
            }
            else
            {
                OnDisconnect();
            }
        }

        private void Update()
        {
            bool isConnected = IsConnected();

            if (isConnected && !prevIsConnected)
            {
                OnConnect();
            }
            else if (!isConnected && prevIsConnected)
            {
                OnDisconnect();
            }
            
            prevIsConnected = isConnected;
        }

        private bool IsConnected()
        {
            return NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient;
        }

        private void OnConnect()
        {
            if (whenConnected)
            {
                HideComponents();
            }
            else
            {
                ShowComponents();
            }
        }

        private void OnDisconnect()
        {
            if (whenConnected)
            {
                ShowComponents();
            }
            else
            {
                HideComponents();
            }
        }
        
        private void HideComponents()
        {
            SetRenderersEnabled(!disableRenderers);
            SetCollidersEnabled(!disableColliders);
            SetUIEnabled(!disableUI);
            SetChildrenEnabled(!disableChildren);
        }

        private void ShowComponents()
        {
            SetRenderersEnabled(true);
            SetCollidersEnabled(true);
            SetUIEnabled(true);
            SetChildrenEnabled(true);
        }

        private void SetRenderersEnabled(bool enable)
        {
            Renderer[] childRenderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in childRenderers)
            {
                r.enabled = enable;
            }
        }
        
        private void SetCollidersEnabled(bool enable)
        {
            Collider[] childColliders = GetComponentsInChildren<Collider>();
            foreach (Collider c in childColliders)
            {
                c.enabled = enable;
            }
        }
        
        private void SetUIEnabled(bool enable)
        {
            Text[] childTexts = GetComponentsInChildren<Text>();
            foreach (Text t in childTexts)
            {
                t.enabled = enable;
            }
        }
        
        private void SetChildrenEnabled(bool enable)
        {
            foreach (UnityEngine.Transform t in transform)
            {
                t.gameObject.SetActive(enable);
            }
        }
    }
}