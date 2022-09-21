using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace Network.Transform
{
    /// <summary>
    /// Similar to the <c>NetworkTransform</c> provided by Unity. The
    /// original <c>NetworkTransform</c> only allows server authoritative
    /// changes. This transform on the other hand can be changed by it's
    /// owner.
    /// </summary>
    [DisallowMultipleComponent]
    public class NetworkOwnerTransform : NetworkTransform
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            CanCommitToTransform = IsOwner;
        }

        protected override void Update()
        {
            CanCommitToTransform = IsOwner;
            base.Update();
            
            if (NetworkManager.Singleton && (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsListening))
            {
                if (CanCommitToTransform)
                {
                    TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
                }
            }
        }
    }
}