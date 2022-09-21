using Network.VR;
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

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            bool oldTrackPosition = trackPosition;
            bool oldTrackRotation = trackRotation;
            
            // if interactable is grabbed by another player, don't update position
            if (interactorsSelecting.Count > 0)
            {
                IXRInteractor first = interactorsSelecting[0];
                if (first is NetworkFakeController)
                {
                    trackPosition = false;
                    trackRotation = false;
                }
            }
            
            base.ProcessInteractable(updatePhase);

            trackPosition = oldTrackPosition;
            trackRotation = oldTrackRotation;
        }
    }
}