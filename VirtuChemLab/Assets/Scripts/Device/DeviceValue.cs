namespace Device
{
    /// <summary>
    /// Wrapper for a simple device value (e.g. temperature).
    /// Other components and the Network can register as listener,
    /// to see when a device value changes.
    ///
    /// <example>
    /// Update temperature across the network when the thermometer
    /// is held over a flame. 
    /// </example>
    /// </summary>
    /// <typeparam name="T">Any value</typeparam>
    public delegate void OnDeviceValueChanged<T>(T oldValue, T newValue);
    
    /// <summary>
    /// Wrapper around any value. Updates listeners when the value changes. 
    /// </summary>
    /// <typeparam name="T">Any value</typeparam>
    public interface DeviceValue<T>
    {
        public event OnDeviceValueChanged<T> OnChanged;
        public T Value
        {
            get;
            set;
        }
    }
}