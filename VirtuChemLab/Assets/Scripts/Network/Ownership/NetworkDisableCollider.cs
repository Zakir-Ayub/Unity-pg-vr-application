using Unity.Netcode;
using UnityEngine;

namespace Network.Ownership
{
    /// <summary>
    /// Disables the renderers of this <c>GameObject</c> depending on
    /// whether we are it's owner or not.
    ///
    /// <example>
    /// Disable our player model, if we only want to see other players
    /// but not ourself.
    /// </example>
    /// </summary>
    public class NetworkDisableCollider : NetworkBehaviour
    {
        public enum Mode {
            Owner, NotOwner 
        }
        
        [Tooltip("Disable our own colliders or those of other players?")]
        public Mode mode = Mode.Owner;

        private void Start()
        {
            if ((IsOwner && mode == Mode.Owner) || (!IsOwner && mode == Mode.NotOwner))
            {
                Collider[] childColliders = GetComponentsInChildren<Collider>();
                foreach (Collider c in childColliders)
                {
                    c.enabled = false;
                }
            }
            
        }
    }
}