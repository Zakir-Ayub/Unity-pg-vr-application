using UnityEngine.XR.Interaction.Toolkit;

namespace Network.VR
{
    /// <summary>
    /// Base class for network interactors (controllers and network
    /// sockets etc.)
    /// </summary>
    public class NetworkBaseInteractor : NetworkInteractionManager
    {
        private XRBaseInteractor interactor;
        
        protected override void Start()
        {
            base.Start();

            interactor = GetComponent<XRBaseInteractor>();
  
            interactor.hoverEntered.AddListener(OnHoverEnter);
            interactor.hoverExited.AddListener(OnHoverExit);
            
            interactor.selectEntered.AddListener(OnSelectEnter);
            interactor.selectExited.AddListener(OnSelectExit);
        }
    }
}