using UnityEngine;

namespace Devices
{
    /// <summary>
    /// Can be used by the SpoonItemDispenser to check whether a Spoon entered
    /// the collider. Holds the dispenseSpawnPoint at which the items should
    /// be spawned.
    /// </summary>
    public class Spoon : MonoBehaviour
    {
        [Tooltip("Where the chemicals (e.g. NaOH) should be spawned")]
        public Transform dispenseSpawnPoint;
    }
}