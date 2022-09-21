using UnityEngine.XR.Interaction.Toolkit;

namespace Network.VR
{
    /// <summary>
    /// Base class for networked grab and simple
    /// interactables.
    /// </summary>
    public class NetworkBaseInteractable : NetworkInteractionManager
    {
        private XRBaseInteractable interactable;
        
        protected override void Start()
        {
            base.Start();
            
            interactable = GetComponent<XRBaseInteractable>();
            
            interactable.firstHoverEntered.AddListener(OnFirstHoverEnter);
            interactable.lastHoverExited.AddListener(OnLastHoverExit);
            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);
            
            interactable.firstSelectEntered.AddListener(OnFirstSelectEnter);
            interactable.lastSelectExited.AddListener(OnLastSelectExit);
            interactable.selectEntered.AddListener(OnSelectEnter);
            interactable.selectExited.AddListener(OnSelectExit);
            
            interactable.activated.AddListener(OnActivateEnter);
            interactable.deactivated.AddListener(OnActivateExit);
        }

        protected override void OnActivateEnterClient(ActivateEventArgs args)
        {
            base.OnActivateEnterClient(args);
            interactable.activated.Invoke(args);
        }
        
        protected override void OnActivateExitClient(DeactivateEventArgs args)
        {
            base.OnActivateExitClient(args);
            interactable.deactivated.Invoke(args);
        }
    }
}