using Unity.Netcode;
using UnityEngine;

namespace Network.VR.Event
{
    /// <summary>
    /// <inheritdoc cref="AbstractNetworkEventArgs"/>
    /// Additionally stores the last known position, rotation and
    /// hand velocity of the player, when he stopped interacting with
    /// the interactable.
    /// </summary>
    public class NetworkSelectExitEventArgs : AbstractNetworkEventArgs, INetworkSerializable
    {
        private Vector3 lastPosition;
        public Vector3 LastPosition => lastPosition;
        
        private Quaternion lastRotation;
        public Quaternion LastRotation => lastRotation;
        
        private Vector3 handVelocity;
        public Vector3 HandVelocity => handVelocity;

        public NetworkSelectExitEventArgs() : base()
        {
        }

        public NetworkSelectExitEventArgs(AbstractNetworkEventArgs eventArgs, Vector3 handVelocity) : base(eventArgs.ClientId, eventArgs.Interactor, eventArgs.InteractorIndex, eventArgs.Interactable, eventArgs.InteractableIndex)
        {
            this.handVelocity = handVelocity;
            lastPosition = eventArgs.Interactable.transform.position;
            lastRotation = eventArgs.Interactable.transform.rotation;
        }

        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            base.NetworkSerialize(serializer);
            
            serializer.SerializeValue(ref handVelocity);
            serializer.SerializeValue(ref lastPosition);
            serializer.SerializeValue(ref lastRotation);
        }
    }
}