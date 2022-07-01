using Network.VR;
using UnityEngine;

namespace Network.Device
{
    /// <summary>
    /// Bridge between <c>NetworkSimpleInteractable</c> and
    /// <c>NetworkDeviceInteractor</c>. Tells the <c>NetworkDeviceInteractor</c>
    /// (e.g. scale weight reset button) that is has been interacted with.
    /// </summary>
    [RequireComponent(typeof(NetworkDeviceInteractor))]
    [RequireComponent(typeof(NetworkSimpleInteractable))]
    public class NetworkDeviceInteractable : MonoBehaviour
    {
        private NetworkDeviceInteractor deviceInteractor;
    
        private void Start()
        {
            deviceInteractor = GetComponent<NetworkDeviceInteractor>();
            GetComponent<NetworkSimpleInteractable>().OnSelect += OnSelect;
        }

        private void OnSelect(GameObject player)
        {
            deviceInteractor.OnInteract(player);
        }
    }
}