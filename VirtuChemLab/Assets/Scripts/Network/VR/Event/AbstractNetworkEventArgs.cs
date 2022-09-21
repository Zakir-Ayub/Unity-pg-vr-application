using Unity.Netcode;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

namespace Network.VR.Event
{
    /// <summary>
    /// The base class for network events. Stores who interacted and
    /// with which object the player interacted with.
    /// </summary>
    public class AbstractNetworkEventArgs : INetworkSerializable
    {
        /// <summary>The client id of the player who interacted</summary>
        public ulong ClientId => clientId;
        private ulong clientId;

        /// <summary>The <see cref="NetworkObject"/> connected to the interactor</summary>
        public NetworkObject Interactor => interactor;
        private NetworkObjectReference interactor;
        
        public IXRInteractor LocalInteractor => 
            Interactor.transform.GetComponentsInChildren<IXRInteractor>()[InteractorIndex];

        /// <summary>The position of the interactor, used to find the original interactor</summary>
        public int InteractorIndex => interactorIndex;
        private int interactorIndex;

        /// <summary>The <see cref="NetworkObject"/> connected to the interactable</summary>
        public NetworkObject Interactable => interactable;
        private NetworkObjectReference interactable;
        
        public IXRInteractable LocalInteractable => 
            Interactable.transform.GetComponentsInChildren<IXRInteractable>()[InteractableIndex];

        /// <summary>The position of the interactor, used to find the original interactor</summary>
        public int InteractableIndex => interactableIndex;
        private int interactableIndex;

        /// <summary>
        /// Empty constructor required for deserialization! 
        /// </summary>
        public AbstractNetworkEventArgs()
        {
        }

        public AbstractNetworkEventArgs(ulong clientId, NetworkObject interactor, int interactorIndex, NetworkObject interactable, int interactableIndex)
        {
            this.clientId = clientId;
            
            this.interactor = interactor;
            this.interactorIndex = interactorIndex;
            
            Assert.IsNotNull(interactor, "Interactor should not be null");
            Assert.AreNotEqual(-1, interactorIndex, "interactorIndex should not be -1");
            
            this.interactable = interactable;
            this.interactableIndex = interactableIndex;
            
            Assert.IsNotNull(interactable, "Interactable should not be null");
            Assert.AreNotEqual(-1, interactableIndex, "interactableIndex should not be -1");
        }

        /// <inheritdoc cref="INetworkSerializable.NetworkSerialize{T}"/>
        public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref clientId);
            serializer.SerializeNetworkSerializable(ref interactor);
            serializer.SerializeValue(ref interactorIndex);
            serializer.SerializeNetworkSerializable(ref interactable);
            serializer.SerializeValue(ref interactableIndex);
        }
    }
}