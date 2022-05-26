using Unity.Netcode;
using UnityEngine;

namespace VirtualChemLab.Networking.Device
{
    [RequireComponent(typeof(Renderer))]
    public class DeviceToggle : NetworkBehaviour
    {
        public delegate void OnStateChangedDelegate(bool isTurnedOn);
        public OnStateChangedDelegate OnStateChanged;

        private NetworkVariable<bool> state = new NetworkVariable<bool>();
        private new Renderer renderer;

        public Material turnedOnMaterial, turnedOffMaterial;

        private void Start()
        {
            renderer = GetComponent<Renderer>();
            SetMaterialFromState(state.Value, state.Value);
        }

        public override void OnNetworkSpawn()
        {
            state.OnValueChanged += InvokeOnStateChangedListeners;
            state.OnValueChanged += SetMaterialFromState;
        }

        public override void OnNetworkDespawn()
        {
            state.OnValueChanged -= SetMaterialFromState;
        }

        private void InvokeOnStateChangedListeners(bool newState, bool oldState)
        {
            OnStateChanged?.Invoke(newState);
        }

        private void SetMaterialFromState(bool newState, bool oldState)
        {
            renderer.material = newState ? turnedOnMaterial : turnedOffMaterial;
        }

        public void Toggle()
        {
            if (IsServer)
            {
                state.Value = !state.Value;
            }
        }
    }
}