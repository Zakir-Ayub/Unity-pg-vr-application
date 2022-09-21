using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public class XRSocketInteractorSelectedDevices : XRSocketInteractor
    {
        [Tooltip("Can multiple devices be attached at the same time?")]
        [SerializeField]
        bool multiSelect = false;
        
        [Tooltip("List of Interactables that can make use of this Socket Interactor.")]
        [SerializeField]
        List<XRGrabInteractable> allowedInteractables = new List<XRGrabInteractable>();

        [Tooltip("Transforms used for the Interactables. The Interactable on Index i uses Transform of Index i of this List.")]
        [SerializeField]
        List<Transform> interactableAttachTransforms = new List<Transform>();
        
        //Cache hover materials for later use
        private Material cachedCantHoverMaterial;
        private Material cachedHoverMaterial;
        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            CreateDefaultHoverMaterials();
            cachedHoverMaterial = interactableHoverMeshMaterial;
            cachedCantHoverMaterial = interactableCantHoverMeshMaterial;
        }

        /// <inheritdoc />
        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            Debug.Assert(allowedInteractables.Count == interactableAttachTransforms.Count);
            //base.CanSelect only checks, if there is only one interactable selected
            return CanInteractWith(interactable) && 
                   (base.CanSelect(interactable)
                    || multiSelect && !interactable.isSelected &&
                    !occupiedAttachmentTransforms.Contains(GetAttachTransform(interactable))) ||
                   interactable.interactorsSelecting.Contains(this);
        }
        
        /// <inheritdoc />
        public override bool CanHover(IXRHoverInteractable interactable)
        {
            Debug.Assert(allowedInteractables.Count == interactableAttachTransforms.Count);
            return CanInteractWith(interactable) && (base.CanHover(interactable) || multiSelect);
        }
        
        /// <summary>
        /// Check, whether a given interactable is in the list of allowed interactables
        /// </summary>
        private bool CanInteractWith(IXRInteractable interactable)
        {
            var baseInteractable = (XRBaseInteractable)interactable;
            return allowedInteractables.Any(interactableFromList => 
                baseInteractable.gameObject.name.ToLower().StartsWith(interactableFromList.gameObject.name.ToLower()));
        }
        
        /// <summary>
        /// Return the Attachment Point's Transform for a given interactable
        /// </summary>
        /// <param name="interactable">The interactable in question</param>
        /// <returns>The Transform to which the interactable can be attached</returns>
        public override Transform GetAttachTransform(IXRInteractable interactable)
        {
            var baseInteractable = (XRBaseInteractable)interactable;    
            foreach (var itemIndex in from interactableFromList in allowedInteractables 
                     where baseInteractable.gameObject.name.ToLower().StartsWith(interactableFromList.gameObject.name.ToLower())
                     select allowedInteractables.IndexOf(interactableFromList))
            {
                Transform interactorTransform = interactableAttachTransforms[itemIndex];
                return interactorTransform;
            }
            
            //If no other attachment point has been defined using our component, revert to the base component's transform
            return base.GetAttachTransform(interactable);
        }
        
        private List<Transform> occupiedAttachmentTransforms = new List<Transform>();

        /// <summary>
        /// Add the now occupied attachment transform to the list
        /// </summary>
        /// <param name="args"></param>
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            occupiedAttachmentTransforms.Add(GetAttachTransform(args.interactableObject));
        }

        /// <summary>
        /// Remove the now no longer occupied attachment transform from the list
        /// </summary>
        /// <param name="args"></param>
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            occupiedAttachmentTransforms.Remove(GetAttachTransform(args.interactableObject));
        }

        /// <inheritdoc />
        /// <summary>
        /// When start hovering and multiselect is true, we need to change the hover material, so the inherited
        /// hover rendering method uses the correct material, even though it does not know about our mutli select
        /// functionality.
        ///
        /// When multiselect is false, restore the materials.
        /// </summary>
        protected override void OnHoverEntering(HoverEnterEventArgs args)
        {
            if (multiSelect)
            {
                if (occupiedAttachmentTransforms.Contains(GetAttachTransform(args.interactableObject)))
                {
                    interactableCantHoverMeshMaterial = cachedCantHoverMaterial;
                    interactableHoverMeshMaterial = cachedCantHoverMaterial;
                }
                else
                {
                
                    interactableCantHoverMeshMaterial = cachedHoverMaterial;
                    interactableHoverMeshMaterial = cachedHoverMaterial;
                } 
            }
            else
            {
                interactableCantHoverMeshMaterial = cachedCantHoverMaterial;
                interactableHoverMeshMaterial = cachedHoverMaterial;
            }
            
            base.OnHoverEntering(args);
        }

    }
}
