using UnityEngine;

namespace Network.Device
{
    /// <summary>
    /// Common interface for networked device interactors.
    ///
    /// <example>
    /// A button which resets the scale's weight.
    /// </example>
    /// </summary>
    public interface NetworkDeviceInteractor
    {
        /// <summary>
        /// Called by <c>NetworkDeviceInteractable</c> when any player interacted with this <c>GameObject</c>
        /// </summary>
        public void OnInteract(GameObject player);
    }
}