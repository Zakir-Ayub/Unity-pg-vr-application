using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Network.VR
{
    /// <summary>
    /// As the actual <see cref="XRBaseController"/> only exists locally, the other
    /// players get a "fake" controller which invokes the events we received over
    /// the network.
    /// </summary>
    public class NetworkFakeController : MonoBehaviour, IXRSelectInteractor, IXRActivateInteractor, IXRHoverInteractor
    {
        public event Action<InteractorRegisteredEventArgs> registered;
        public event Action<InteractorUnregisteredEventArgs> unregistered;

        [SerializeField]
        private InteractionLayerMask m_interactionLayers;
        public InteractionLayerMask interactionLayers => m_interactionLayers;

        [SerializeField]
        private HoverEnterEvent m_HoverEnterEvent = new HoverEnterEvent();
        public HoverEnterEvent hoverEntered => m_HoverEnterEvent;

        [SerializeField]
        private HoverExitEvent m_HoverExitEvent = new HoverExitEvent();
        public HoverExitEvent hoverExited => m_HoverExitEvent;

        [SerializeReference]
        private List<IXRHoverInteractable> m_interactablesHovered;
        public List<IXRHoverInteractable> interactablesHovered => m_interactablesHovered;
        
        public bool hasHover => interactablesHovered.Count > 0;
        public bool isHoverActive => true;

        [SerializeField]
        private SelectEnterEvent m_SelectEnterEvent = new SelectEnterEvent();
        public SelectEnterEvent selectEntered => m_SelectEnterEvent;

        private SelectExitEvent m_SelectExitEvent = new SelectExitEvent();
        public SelectExitEvent selectExited => m_SelectExitEvent;

        [SerializeReference]
        private List<IXRSelectInteractable> m_interactablesSelected;
        public List<IXRSelectInteractable> interactablesSelected => m_interactablesSelected;
        
        private IXRSelectInteractable m_firstInteractableSelected;
        public IXRSelectInteractable firstInteractableSelected => m_firstInteractableSelected;

        public bool hasSelection => interactablesSelected.Count > 0;
        public bool isSelectActive => true;
        public bool keepSelectedTargetValid => true;

        public bool shouldActivate => false;
        public bool shouldDeactivate => false;

        private void Start()
        {
            m_interactablesHovered = new List<IXRHoverInteractable>();
            m_interactablesSelected = new List<IXRSelectInteractable>();
        }
        
        public UnityEngine.Transform GetAttachTransform(IXRInteractable interactable)
        {
            return transform;
        }

        public void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();
        }

        public void OnRegistered(InteractorRegisteredEventArgs args)
        {
            registered?.Invoke(args);
        }

        public void OnUnregistered(InteractorUnregisteredEventArgs args)
        {
            unregistered?.Invoke(args);
        }

        public void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
        }

        public void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
        }
        
        public bool CanHover(IXRHoverInteractable interactable)
        {
            return false;
        }

        public bool IsHovering(IXRHoverInteractable interactable)
        {
            return m_interactablesHovered.Contains(interactable);
        }

        public void OnHoverEntering(HoverEnterEventArgs args)
        {
            m_interactablesHovered.Add(args.interactableObject);
        }

        public void OnHoverEntered(HoverEnterEventArgs args)
        {
        }

        public void OnHoverExiting(HoverExitEventArgs args)
        {
            m_interactablesHovered.Remove(args.interactableObject);
        }

        public void OnHoverExited(HoverExitEventArgs args)
        {
        }

        public bool CanSelect(IXRSelectInteractable interactable)
        {
            return false;
        }

        public bool IsSelecting(IXRSelectInteractable interactable)
        {
            return m_interactablesSelected.Contains(interactable);
        }

        public Pose GetAttachPoseOnSelect(IXRSelectInteractable interactable)
        {
            return Pose.identity;
        }

        public Pose GetLocalAttachPoseOnSelect(IXRSelectInteractable interactable)
        {
            return Pose.identity;
        }

        public void OnSelectEntering(SelectEnterEventArgs args)
        {
            m_interactablesSelected.Add(args.interactableObject);
            if (interactablesSelected.Count == 1)
            {
                m_firstInteractableSelected = args.interactableObject;
            }
        }

        public void OnSelectEntered(SelectEnterEventArgs args)
        {
        }

        public void OnSelectExiting(SelectExitEventArgs args)
        {
            m_interactablesSelected.Remove(args.interactableObject);
            if (interactablesSelected.Count == 0)
            {
                m_firstInteractableSelected = null;
            }
        }

        public void OnSelectExited(SelectExitEventArgs args)
        {
        }

        public void GetActivateTargets(List<IXRActivateInteractable> targets)
        {
            targets.Clear();
        }
    }
}