namespace Reactions
{
     /// <summary>
     /// Things that can transfer heat/cold from itself to others
     ///
     /// <example>Gas burner flame or water</example>
     /// </summary>
    public interface ITemperatureSource
    {
        /// <summary>
        /// Should return the current temperature of the object in Celsius.
        /// Can also return negative values to cool the other object.
        /// </summary>
        /// <returns>Temperature in Celsius</returns>
        public float GetTemperature();
    }
}