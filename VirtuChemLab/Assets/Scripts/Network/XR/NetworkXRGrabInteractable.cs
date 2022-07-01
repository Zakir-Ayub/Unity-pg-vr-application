using UnityEngine.XR.Interaction.Toolkit;

namespace Network.XR
{
    /// <summary>
    /// The original XRGrabInteractable is not always network compatible
    /// e.g. we cannot simply change the parent of a networked object.
    /// This class should remove/adjust code that is incompatible.
    /// </summary>
    public class NetworkXRGrabInteractable : XRGrabInteractable
    {
        protected override void Grab()
        {
            // do nothing on purpose, logic moved to NetworkGrabInteractable
        }

        protected override void Drop()
        {
            // do nothing on purpose, logic moved to NetworkGrabInteractable
        }
    }
}