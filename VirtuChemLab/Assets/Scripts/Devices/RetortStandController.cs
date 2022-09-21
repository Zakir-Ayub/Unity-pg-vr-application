using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Devices
{
    public class RetortStandController : MonoBehaviour
    {
        private BoxCollider colliderLeftClamp;
        private BoxCollider colliderRightClamp;
    
        // Start is called before the first frame update
        void Start()
        {
            var leftClamp = transform.Find("Plane.009");
            var rightClamp = transform.Find("Plane.008");
        
            colliderLeftClamp = leftClamp.gameObject.GetComponent(typeof(BoxCollider)) as BoxCollider;
            colliderRightClamp = rightClamp.gameObject.GetComponent(typeof(BoxCollider)) as BoxCollider;
        }

        //Disable collision boxes of clamps once Burette is attached
        public void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (((XRBaseInteractable)args.interactableObject).name == "Burette")
            {
                colliderLeftClamp.enabled = false;
                colliderRightClamp.enabled = false;
            }
        }
        
        //Reenable collision boxes of clamps once Burette is removed
        public void OnSelectExited(SelectExitEventArgs args)
        {
            if (((XRBaseInteractable)args.interactableObject).name == "Burette")
            {
                colliderLeftClamp.enabled = true;
                colliderRightClamp.enabled = true;
            }
        }
    }
}
