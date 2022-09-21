using System;
using System.Collections.Generic;
using System.Linq;
using Network.VR.Event;
using Network.VR.Hand;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

namespace Network.VR
{
    /// <summary>
    /// Provides listeners for when others players (or sockets on other clients)
    /// interact with a <see cref="XRBaseInteractable"/>. These events will only
    /// be invoked on the server, but clients can just register to the "normal"
    /// XR events, as events are sent to all other clients and then "fake invoked".
    /// </summary>
    public abstract class NetworkInteractionManager : NetworkBehaviour
    {
        [SerializeField, Tooltip("Hover events can cause a network bottleneck because many events will be sent")]
        private bool sendHoverEvents = false;
        
        [SerializeField]
        private bool sendSelectEvents = true;
        
        [SerializeField]
        private bool sendActivateEvents = true;
        
        private const string VRLeftHandTag = "VRLeftHand";
        private const string VRRightHandTag = "VRRightHand";
        
        private HandPresencePhysics[] physicsHands;

        // register listeners on these events to know when any player hovers
        public NetworkVREvent<AbstractNetworkEventArgs> FirstHoverEnter;
        public NetworkVREvent<AbstractNetworkEventArgs> LastHoverExit;
        public NetworkVREvent<AbstractNetworkEventArgs> HoverEnter;
        public NetworkVREvent<AbstractNetworkEventArgs> HoverExit;
        
        // register listeners on these events to know when any player grabs
        public NetworkVREvent<AbstractNetworkEventArgs> FirstSelectEnter;
        public NetworkVREvent<NetworkSelectExitEventArgs> LastSelectExit;
        public NetworkVREvent<AbstractNetworkEventArgs> SelectEnter;
        public NetworkVREvent<NetworkSelectExitEventArgs> SelectExit;
        
        // register listeners on these events to know when any player activates
        public NetworkVREvent<AbstractNetworkEventArgs> ActivateEnter;
        public NetworkVREvent<AbstractNetworkEventArgs> ActivateExit;

        // store events recevied, in order not to rebroadcast them again (prevent infinite loop)
        private List<BaseInteractionEventArgs> eventsReceivedThisFrame = new List<BaseInteractionEventArgs>();
        
        protected virtual void Start()
        {
            physicsHands = FindObjectsOfType<HandPresencePhysics>();
        }

        private void LateUpdate()
        {
            eventsReceivedThisFrame.Clear();
        }

        /// <summary>
        /// Converts an XR event, to an event which can be sent
        /// over the network.
        /// </summary>
        /// <param name="args">The original XR event</param>
        /// <returns></returns>
        private AbstractNetworkEventArgs GetDefaultNetworkEventArgs(BaseInteractionEventArgs args)
        {
            return new AbstractNetworkEventArgs(
                NetworkManager.LocalClientId,
                GetNetworkInteractor(args.interactorObject),
                FindInteractorIndex(args.interactorObject),
                GetNetworkInteractable(args.interactableObject),
                FindInteractableIndex(args.interactableObject)
            );
        }
        
        /// <summary>
        /// Return whether this event should be sent over the network
        /// </summary>
        /// <param name="args">The event</param>
        /// <returns>True if the event should be sent to other players</returns>
        protected bool ShouldSendEvent(BaseInteractionEventArgs args)
        {
            if (!NetworkManager.Singleton.IsListening)
            {
                return false;
            }

            if (!sendHoverEvents && (args is HoverEnterEventArgs || args is HoverExitEventArgs))
            {
                return false;
            }
            if (!sendSelectEvents && (args is SelectEnterEventArgs || args is SelectExitEventArgs))
            {
                return false;
            }
            if (!sendActivateEvents && (args is ActivateEventArgs || args is DeactivateEventArgs))
            {
                return false;
            }
            
            // don't resend events which we received from other players!
            if (DidReceiveEventThisFrame(args))
            {
                return false;
            }
            
            // the interactor needs to be networked
            NetworkObject networkInteractor = GetNetworkInteractor(args.interactorObject);
            if (!networkInteractor || !networkInteractor.IsSpawned)
            {
                return false;
            }

            // the interactable needs to be networked
            NetworkObject networkInteractable= GetNetworkInteractable(args.interactableObject);
            if (!networkInteractable || !networkInteractable.IsSpawned)
            {
                return false;
            }
            
            return true;
        }

        protected void OnFirstHoverEnter(HoverEnterEventArgs args)
        {
            if (!ShouldSendEvent(args))
                return;
            OnFirstHoverEnterServerRpc(GetDefaultNetworkEventArgs(args));
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnFirstHoverEnterServerRpc(AbstractNetworkEventArgs args)
        {
            OnFirstHoverEnterServer(args);
        }

        protected virtual void OnFirstHoverEnterServer(AbstractNetworkEventArgs args)
        {
            FirstHoverEnter?.Invoke(args);
        }
        
        protected void OnLastHoverExit(HoverExitEventArgs args)
        {
            if (!ShouldSendEvent(args))
                return;
            OnLastHoverExitServerRpc(GetDefaultNetworkEventArgs(args));
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void OnLastHoverExitServerRpc(AbstractNetworkEventArgs args)
        {
            OnLastHoverExitServer(args);
        }
        
        protected virtual void OnLastHoverExitServer(AbstractNetworkEventArgs args)
        {
            LastHoverExit?.Invoke(args);
        }
        
        protected void OnHoverEnter(HoverEnterEventArgs args)
        {
            if (!ShouldSendEvent(args))
                return;
            OnHoverEnterServerRpc(GetDefaultNetworkEventArgs(args));
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnHoverEnterServerRpc(AbstractNetworkEventArgs args)
        {
            OnHoverEnterServer(args);
            OnHoverEnterClientRpc(args, GetClientRpcParams(args));
        }
        
        protected virtual void OnHoverEnterServer(AbstractNetworkEventArgs args)
        {
            HoverEnter?.Invoke(args);
        }
        
        [ClientRpc]
        private void OnHoverEnterClientRpc(AbstractNetworkEventArgs args, ClientRpcParams rpcParams = default)
        {
            IXRHoverInteractor xrInteractor = args.LocalInteractor as IXRHoverInteractor;
            IXRHoverInteractable xrInteractable = args.LocalInteractable as IXRHoverInteractable;

            // the event has to be added before executing the event!
            eventsReceivedThisFrame.Add(new HoverEnterEventArgs
            {
                manager = GetXRInteractionManager(),
                interactorObject = xrInteractor,
                interactableObject = xrInteractable
            });

            if (!xrInteractor.IsHovering(xrInteractable))
            {
                GetXRInteractionManager().HoverEnter(xrInteractor, xrInteractable);
            }
        }
        
        protected void OnHoverExit(HoverExitEventArgs args)
        {
            if (!ShouldSendEvent(args))
                return;
            OnHoverExitServerRpc(GetDefaultNetworkEventArgs(args));
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void OnHoverExitServerRpc(AbstractNetworkEventArgs args)
        {
            OnHoverExitServer(args);
            OnHoverExitClientRpc(args, GetClientRpcParams(args));
        }
        
        protected virtual void OnHoverExitServer(AbstractNetworkEventArgs args)
        {
            HoverExit?.Invoke(args);
        }
        
        [ClientRpc]
        private void OnHoverExitClientRpc(AbstractNetworkEventArgs args, ClientRpcParams rpcParams = default)
        {
            IXRHoverInteractor xrInteractor = args.LocalInteractor as IXRHoverInteractor;
            IXRHoverInteractable xrInteractable = args.LocalInteractable as IXRHoverInteractable;

            // the event has to be added before executing the event!
            eventsReceivedThisFrame.Add(new HoverExitEventArgs
            {
                manager = GetXRInteractionManager(),
                interactorObject = xrInteractor,
                interactableObject = xrInteractable
            });


            if (xrInteractor.IsHovering(xrInteractable))
            {
                GetXRInteractionManager().HoverExit(xrInteractor, xrInteractable);
            }
        }

        protected void OnFirstSelectEnter(SelectEnterEventArgs args)
        {
            if (!ShouldSendEvent(args))
                return;
            OnFirstSelectEnterServerRpc(GetDefaultNetworkEventArgs(args));
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnFirstSelectEnterServerRpc(AbstractNetworkEventArgs args)
        {
            OnFirstSelectEnterServer(args);
        }
        
        protected virtual void OnFirstSelectEnterServer(AbstractNetworkEventArgs args)
        {
            FirstSelectEnter?.Invoke(args);
        }
        
        protected void OnLastSelectExit(SelectExitEventArgs args)
        {
            if (!ShouldSendEvent(args))
                return;
            Vector3 handVelocity = GetHandVelocity(args.interactorObject);
            OnLastSelectExitServerRpc(new NetworkSelectExitEventArgs(GetDefaultNetworkEventArgs(args), handVelocity));
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void OnLastSelectExitServerRpc(NetworkSelectExitEventArgs args)
        {
            OnLastSelectExitServer(args);
        }
        
        protected virtual void OnLastSelectExitServer(NetworkSelectExitEventArgs args)
        {
            LastSelectExit?.Invoke(args);
        }

        protected void OnSelectEnter(SelectEnterEventArgs args)
        {
            if (!ShouldSendEvent(args))
                return;
            OnSelectEnterServerRpc(GetDefaultNetworkEventArgs(args));
        }
    
        [ServerRpc(RequireOwnership = false)]
        private void OnSelectEnterServerRpc(AbstractNetworkEventArgs args)
        {
            OnSelectEnterServer(args);
            OnSelectEnterClientRpc(args, GetClientRpcParams(args));
        }
        
        protected virtual void OnSelectEnterServer(AbstractNetworkEventArgs args)
        {
            SelectEnter?.Invoke(args);
        }

        [ClientRpc]
        private void OnSelectEnterClientRpc(AbstractNetworkEventArgs args, ClientRpcParams rpcParams = default)
        {
            IXRSelectInteractor xrInteractor = args.LocalInteractor as IXRSelectInteractor;
            IXRSelectInteractable xrInteractable = args.LocalInteractable as IXRSelectInteractable;

            // the event has to be added before executing the event!
            eventsReceivedThisFrame.Add(new SelectEnterEventArgs
            {
                manager = GetXRInteractionManager(),
                interactorObject = xrInteractor,
                interactableObject = xrInteractable
            });

            if (!xrInteractor.IsSelecting(xrInteractable))
            {
                GetXRInteractionManager().SelectEnter(xrInteractor, xrInteractable);
            }
        }

        protected void OnSelectExit(SelectExitEventArgs args)
        {
            if (!ShouldSendEvent(args))
                return;
            Vector3 handVelocity = GetHandVelocity(args.interactorObject);
            OnSelectExitServerRpc(new NetworkSelectExitEventArgs(GetDefaultNetworkEventArgs(args), handVelocity));
        }
    
        [ServerRpc(RequireOwnership = false)]
        private void OnSelectExitServerRpc(NetworkSelectExitEventArgs args)
        {
            OnSelectExitServer(args);
            OnSelectExitClientRpc(args, GetClientRpcParams(args));
        }
        
        protected virtual void OnSelectExitServer(NetworkSelectExitEventArgs args)
        {
            SelectExit?.Invoke(args);
        }
        
        [ClientRpc]
        private void OnSelectExitClientRpc(AbstractNetworkEventArgs args, ClientRpcParams rpcParams = default)
        {
            IXRSelectInteractor xrInteractor = args.LocalInteractor as IXRSelectInteractor;
            IXRSelectInteractable xrInteractable = args.LocalInteractable as IXRSelectInteractable;

            // the event has to be added before executing the event!
            eventsReceivedThisFrame.Add(new SelectExitEventArgs
            {
                manager = GetXRInteractionManager(),
                interactorObject = xrInteractor,
                interactableObject = xrInteractable
            });

            if (xrInteractor.IsSelecting(xrInteractable))
            {
                GetXRInteractionManager().SelectExit(xrInteractor, xrInteractable);
            }
        }
        
        protected void OnActivateEnter(ActivateEventArgs args)
        {
            if (!ShouldSendEvent(args))
                return;
            OnActivateEnterServerRpc(GetDefaultNetworkEventArgs(args));
        }
    
        [ServerRpc(RequireOwnership = false)]
        private void OnActivateEnterServerRpc(AbstractNetworkEventArgs args)
        {
            OnActivateEnterServer(args);
            OnActivateEnterClientRpc(args, GetClientRpcParams(args));
        }
        
        protected virtual void OnActivateEnterServer(AbstractNetworkEventArgs args)
        {
            ActivateEnter?.Invoke(args);
        }
        
        [ClientRpc]
        private void OnActivateEnterClientRpc(AbstractNetworkEventArgs args, ClientRpcParams rpcParams = default)
        {
            IXRActivateInteractor xrInteractor = args.LocalInteractor as IXRActivateInteractor;
            IXRActivateInteractable xrInteractable = args.LocalInteractable as IXRActivateInteractable;
            
            ActivateEventArgs evt = new ActivateEventArgs
            {
                interactorObject = xrInteractor,
                interactableObject = xrInteractable,
            };
            
            // has to be added before invoking the event!
            eventsReceivedThisFrame.Add(evt);
            OnActivateEnterClient(evt);
        }

        protected virtual void OnActivateEnterClient(ActivateEventArgs args)
        {
        }
        
        protected void OnActivateExit(DeactivateEventArgs args)
        {
            if (!ShouldSendEvent(args))
                return;
            OnActivateExitServerRpc(GetDefaultNetworkEventArgs(args));
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnActivateExitServerRpc(AbstractNetworkEventArgs args)
        {
            OnActivateExitServer(args);
            OnActivateExitClientRpc(args, GetClientRpcParams(args));
        }
        
        protected virtual void OnActivateExitServer(AbstractNetworkEventArgs args)
        {
            ActivateExit?.Invoke(args);
        }
        
        [ClientRpc]
        private void OnActivateExitClientRpc(AbstractNetworkEventArgs args, ClientRpcParams rpcParams = default)
        {
            IXRActivateInteractor xrInteractor = args.LocalInteractor as IXRActivateInteractor;
            IXRActivateInteractable xrInteractable = args.LocalInteractable as IXRActivateInteractable;
            
            DeactivateEventArgs evt = new DeactivateEventArgs
            {
                interactorObject = xrInteractor,
                interactableObject = xrInteractable,
            };
            
            // has to be added before invoking the event!
            eventsReceivedThisFrame.Add(evt);
            OnActivateExitClient(evt);
        }

        protected virtual void OnActivateExitClient(DeactivateEventArgs args)
        {
        }

        /// <summary>
        /// Returns whether we received this event from another
        /// player during the current frame.
        /// </summary>
        /// <param name="args">The XR event</param>
        private bool DidReceiveEventThisFrame(BaseInteractionEventArgs args)
        {
            foreach (BaseInteractionEventArgs received in eventsReceivedThisFrame)
            {
                if (args.GetType() == received.GetType())
                {
                    if (args.interactorObject == received.interactorObject && args.interactableObject == received.interactableObject)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the client rpc params for the given event. 
        /// </summary>
        protected ClientRpcParams GetClientRpcParams(AbstractNetworkEventArgs args)
        {
            Assert.IsTrue(IsServer);

            List<ulong> allClients = new List<ulong>(NetworkManager.ConnectedClients.Keys);
            List<ulong> otherClients = new List<ulong>();
            foreach (ulong client in allClients)
            {
                if (client != args.ClientId)
                {
                    otherClients.Add(client);
                }
            }

            return new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = otherClients
                }
            };
        }
        
        /// <summary>
        /// Get the <see cref="XRInteractionManager"/> singleton
        /// </summary>
        protected XRInteractionManager GetXRInteractionManager()
        {
            return FindObjectOfType<XRInteractionManager>();
        }
        
        /// <summary>
        /// Get the network object of the interactor
        /// </summary>
        protected virtual NetworkObject GetNetworkInteractor(IXRInteractor interactor)
        {
            if (interactor.transform.CompareTag(VRLeftHandTag) || interactor.transform.CompareTag(VRRightHandTag))
            {
                return NetworkManager.LocalClient.PlayerObject;
            }
            
            return interactor.transform.GetComponentInParent<NetworkObject>();
        }

        /// <summary>
        ///  Find the "child position" in the scene graph for this interactor in our
        /// scene tree
        /// </summary>
        private int FindInteractorIndex(IXRInteractor interactor)
        {
            IXRInteractor actualInteractor = interactor;
            
            // the controllers are special, as they don't exist on other clients
            // only the local player has actual XR controllers, therefore we use
            // "fake" controllers (see NetworkFakeController)
            if (interactor.transform.CompareTag(VRLeftHandTag))
            {
                actualInteractor = NetworkManager.LocalClient.PlayerObject.GetComponent<NetworkPlayer>()
                    .LeftHandInteractor;
            }
            else if (interactor.transform.CompareTag(VRRightHandTag))
            {
                actualInteractor = NetworkManager.LocalClient.PlayerObject.GetComponent<NetworkPlayer>()
                    .RightHandInteractor;
            }
            
            return Array.IndexOf(GetNetworkInteractor(interactor).GetComponentsInChildren<IXRInteractor>(), actualInteractor);
        }

        /// <summary>
        /// Get the network object of the interactable
        /// </summary>
        protected virtual NetworkObject GetNetworkInteractable(IXRInteractable interactable)
        {
            return interactable.transform.GetComponentInParent<NetworkObject>();
        }

        /// <summary>
        /// Find the "child position" in the scene graph for this interactable in our
        /// scene tree
        /// </summary>
        private int FindInteractableIndex(IXRInteractable interactable)
        {
            return Array.IndexOf(GetNetworkInteractable(interactable).GetComponentsInChildren<IXRInteractable>(), interactable);
        }

        /// <summary>
        /// Returns the movement direction of the controller.
        ///
        /// <example>Can be used for throwing objects</example>
        /// </summary>
        protected Vector3 GetHandVelocity(IXRInteractor interactor)
        {
            XRBaseController controller = interactor.transform.GetComponent<XRBaseController>();
            if (!controller)
            {
                // a XRSocketInteractor does not have a velocity
                return Vector3.zero;
            }
            
            HandPresencePhysics hand = physicsHands.First(x => x.actionController.Equals(controller));
            return hand.GetComponent<Rigidbody>().velocity;
        }

        /// <summary>
        /// Is this controller for the left or right hand?
        /// </summary>
        protected VRHand GetVRHand(IXRInteractor interactor)
        {
            if (interactor.transform.CompareTag(VRLeftHandTag))
            {
                return VRHand.Left;
            }
            if (interactor.transform.CompareTag(VRRightHandTag))
            {
                return VRHand.Right;
            }
            return VRHand.Unknown;
        }
    }
}