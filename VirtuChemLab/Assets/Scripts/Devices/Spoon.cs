using UnityEngine;

namespace Devices
{
    /// <summary>
    /// Spoon class is child of the ContainerPouringSystem class.
    /// Used by the SpoonItemDispenser to check whether a Spoon entered the collider.
    /// </summary>
    public class Spoon : FluidContainerSystem
    {
        [Tooltip("Where the chemicals (e.g. NaOH) should be spawned")]
        public Transform dispenseSpawnPoint;

        protected new void Start()
        {
            base.Start();
            container.maxStorage = 50;
        }
    }
}