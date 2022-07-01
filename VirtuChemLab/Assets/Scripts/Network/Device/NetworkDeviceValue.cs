using System;
using Device;
using Unity.Netcode;

namespace Network.Device
{
    /// <summary>
    /// Monitors a device value (e.g. weight) and updates its on all
    /// clients. The value can only be updated by server or owner of
    /// the object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class NetworkDeviceValue<T> : NetworkVariable<T>, DeviceValue<T> where T : unmanaged
    {
        public event OnDeviceValueChanged<T> OnChanged;

        public NetworkDeviceValue(T value = default) : base(value)
        {
            OnValueChanged += (oldValue, newValue) =>
            {
                OnChanged?.Invoke(oldValue, newValue);
            };
        }
    }
}