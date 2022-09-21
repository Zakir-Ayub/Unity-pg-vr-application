using Network.VR;
using Network.VR.Event;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Devices
{
    /// <summary>
    /// Provides callback listeners to detect when a hose
    /// has been connected. Also aligns the position of the hose
    /// segment.
    /// </summary>
    public class HoseSocket : MonoBehaviour
    {
        private XRSocketInteractor interactor;

        private HoseEnd currentHose;
        
        public delegate void HoseEvent(HoseEnd hoseEnd);
        public event HoseEvent OnHoseConnect, OnHoseDisconnect;
        
        [SerializeField, Tooltip("The position offset for the connected segment segment, relative to the attach transform")]
        private Vector3 bottomRotation, topRotation;
        
        [SerializeField, Tooltip("The rotation offset for the connected segment segment, relative to the attach transform")]
        private Vector3 bottomPosition, topPosition;

        private Vector3 originalAttachPosition;
        private Quaternion originalAttachRotation;
        
        private void Start()
        {
            interactor = GetComponent<XRSocketInteractor>();
            originalAttachPosition = interactor.attachTransform.localPosition;
            originalAttachRotation = interactor.attachTransform.localRotation;
            
            NetworkSocketInteractor networkSocketInteractor = GetComponent<NetworkSocketInteractor>();
            networkSocketInteractor.SelectEnter += OnConnect;
            networkSocketInteractor.SelectExit += OnDisconnect;
        }

        private void OnConnect(AbstractNetworkEventArgs args)
        {
            HoseEnd hoseEnd = args.LocalInteractable.transform.GetComponent<HoseEnd>();
            if (hoseEnd)
            {
                Connect(hoseEnd);
            }
        }
        
        private void Connect(HoseEnd hoseEnd)
        {
            if (currentHose)
            {
                Disconnect();
            }
            
            OnHoseConnect?.Invoke(hoseEnd);
            hoseEnd.Connect(this);
            
            // depending on whether the bottom or top
            // part of the hose was connected, the segment
            // has a different rotation and position
            Transform attachTransform = interactor.attachTransform;
            
            Vector3 attachPosition = originalAttachPosition;
            Vector3 attachRotation = originalAttachRotation.eulerAngles;

            if (hoseEnd.isBottom)
            {
                attachPosition += bottomPosition;
                attachRotation += bottomRotation;
            }
            else
            {
                attachPosition += topPosition;
                attachRotation += topRotation;
            }

            attachTransform.localPosition = attachPosition;
            attachTransform.localEulerAngles = attachRotation;

            currentHose = hoseEnd;
        }
        
        private void OnDisconnect(AbstractNetworkEventArgs args)
        {
            HoseEnd hoseEnd = args.LocalInteractable.transform.GetComponent<HoseEnd>();
            if (hoseEnd == currentHose)
            {
                Disconnect();
            }
        }

        private void Disconnect()
        {
            if (currentHose)
            {
                Transform attachTransform = interactor.attachTransform;
                attachTransform.localPosition = originalAttachPosition;
                attachTransform.localRotation = originalAttachRotation;
            
                OnHoseDisconnect?.Invoke(currentHose);
                currentHose.Disconnect();
                currentHose = null;
            }
        }
    }
}