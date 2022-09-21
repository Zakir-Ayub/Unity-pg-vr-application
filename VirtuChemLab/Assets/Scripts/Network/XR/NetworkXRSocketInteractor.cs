using UnityEngine.XR.Interaction.Toolkit;

namespace Network.XR
{
    /// <summary>
    /// <inheritdoc cref="XRSocketInteractor"/> but should be used
    /// in conjunction with the other network components, as we might
    /// need to make changes to how networked sockets work in the future.
    /// </summary>
    public class NetworkXRSocketInteractor : XRSocketInteractor
    {
    }
}