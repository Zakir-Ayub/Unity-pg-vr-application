namespace Device
{
    /// <summary>
    /// Simple Device value implementation which is not networked,
    /// but informs listeners when the value is changed.
    /// </summary>
    /// <typeparam name="T">Any unmanaged value (float, int, bool, ...)</typeparam>
    public class SimpleDeviceValue<T> : DeviceValue<T> where T : unmanaged
    {
        public event OnDeviceValueChanged<T> OnChanged;
        public T Value
        {
            get => internalValue;
            set
            {
                OnChanged?.Invoke(internalValue, value);
                internalValue = value;
            }
        }
        
        private T internalValue;

        public SimpleDeviceValue(T defaultValue = default)
        {
            internalValue = defaultValue;
        }
    }
}