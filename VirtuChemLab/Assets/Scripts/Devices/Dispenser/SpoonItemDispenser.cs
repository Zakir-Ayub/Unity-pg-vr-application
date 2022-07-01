using UnityEngine;

namespace Devices
{
    /// <summary>
    /// Spawns an item when a <c>Spoon</c> has entered our collider
    /// and then exits.
    /// </summary>
    public class SpoonItemDispenser : AbstractItemDispenser
    {
        private bool isSpoonInside;

        private void OnTriggerEnter(Collider other)
        {
            Spoon spoon = other.GetComponentInParent<Spoon>();
            if (spoon)
            {
                isSpoonInside = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            Spoon spoon = other.GetComponentInParent<Spoon>();
            if (spoon && isSpoonInside)
            {
                DispenseItems(spoon.dispenseSpawnPoint.position, Quaternion.identity, Random.Range(4, 8));
                isSpoonInside = false;
            }
            
        }
    }
}