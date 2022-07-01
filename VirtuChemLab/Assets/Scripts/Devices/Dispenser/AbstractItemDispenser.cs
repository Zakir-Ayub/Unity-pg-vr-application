using Unity.Netcode;
using UnityEngine;

namespace Devices
{
    /// <summary>
    /// Can be used to instantiate and spawn items like NaOH. The concrete
    /// subclasses decide when and how many should be spawned.
    /// </summary>
    public abstract class AbstractItemDispenser : NetworkBehaviour
    {
        [Tooltip("The Object that is created and put on the spoon (e.g. NaOH)")]
        public GameObject Item;

        protected void DispenseItems(Vector3 position, Quaternion rotation, int count)
        {
            // only server can spawn items (for now)
            if (!IsServer) return;

            for (int i = 0; i < count; i++)
            {
                DispenseItem(position, rotation);
            }
        }
        
        protected void DispenseItem(Vector3 position, Quaternion rotation)
        {
            // only server can spawn items (for now)
            if (!IsServer) return;
            
            GameObject item = Instantiate(Item, position, rotation);
                    
            NetworkObject networkItem = item.GetComponent<NetworkObject>();
            if (networkItem)
            {
                networkItem.Spawn();
            }
        }
    }
}