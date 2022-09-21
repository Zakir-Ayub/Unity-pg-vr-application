using System;
using Unity.Netcode;
using UnityEngine;

namespace Devices
{
    /// <summary>
    /// Controls how "open" (0.0-1.0) the valve is. Also moves and
    /// rotates the object depending on <see cref="targetPosition"/>
    /// and <see cref="targetRotation"/>.
    /// </summary>
    public class Valve : NetworkBehaviour
    {
        /// <summary>
        /// The controller rotation controls opening
        /// and closing the valve.
        /// </summary>
        private ControllerRotationTracker rotationTracker;

        [SerializeField] 
        private NetworkVariable<float> open = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);
        public float OpenedBy
        {
            set
            {
                if (IsOwner)
                {
                    open.Value = Mathf.Max(Math.Min(value, 1f), 0f);
                }
            }
            get => open.Value;
        }

        [SerializeField, Tooltip("The position offset when fully opened")]
        private Vector3 targetPosition = Vector3.zero;
        private Vector3 originalPosition;
        
        [SerializeField, Tooltip("The rotation offset when fully opened")]
        private Vector3 targetRotation = Vector3.zero;
        private Vector3 originalRotation;
        
        private void Start()
        {
            rotationTracker = GetComponent<ControllerRotationTracker>();
            rotationTracker.OnRotate += (controller, rotationDiff) =>
            {
                OpenedBy += rotationDiff.z * Time.deltaTime;
            };

            originalPosition = transform.localPosition;
            originalRotation = transform.localEulerAngles;
        }

        private void Update()
        {
            transform.localPosition = originalPosition + targetPosition * OpenedBy;
            transform.localEulerAngles = originalRotation + targetRotation * OpenedBy;
        }
    }
}