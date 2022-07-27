using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public class XRSocketInteractableSelectedDevices : XRSocketInteractor
    {
        [SerializeField]
        List<XRGrabInteractable> allowedInteractables = new List<XRGrabInteractable>();

        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            return CanInteractWith(interactable) && base.CanSelect(interactable);
        }

        public override bool CanHover(IXRHoverInteractable interactable)
        {
            return CanInteractWith(interactable) && base.CanHover(interactable);
        }

        private bool CanInteractWith(IXRInteractable interactable)
        {
            foreach(XRGrabInteractable interactableFromList in allowedInteractables)
            {
                //Nothing else worked aside from converting this things to Strings in a dirty way and comparing them. I don't like it either but nothing else seemed to work :-(
                if(interactableFromList+"" == interactable+"")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
