using UnityEngine;

namespace VR
{
    /// <summary>
    /// Provides utility functions to interact with
    /// the vr rig (bone structure) of the character
    /// model.
    /// </summary>
    public class VRRig : MonoBehaviour
    {
        public GameObject leftHand, rightHand;

        public void SetLeftHandColliders(bool isEnabled)
        {
            SetCollidersEnabled(leftHand, isEnabled);
        }
        
        public void SetRightHandColliders(bool isEnabled)
        {
            SetCollidersEnabled(rightHand, isEnabled);
        }
        
        private void SetCollidersEnabled(GameObject go, bool isEnabled)
        {
            Collider[] childColliders = go.GetComponentsInChildren<Collider>();
            foreach (Collider c in childColliders)
            {
                c.enabled = isEnabled;
            }
        }
    }
}