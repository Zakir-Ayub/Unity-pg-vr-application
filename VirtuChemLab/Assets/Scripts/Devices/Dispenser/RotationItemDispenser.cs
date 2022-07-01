using UnityEngine;

namespace Devices
{
    /// <summary>
    /// Spawns an item when a certain rotation is reached. This can be used
    /// to for example spawn items when turning an object upside down.
    ///
    /// <example>The NaOH container spawns NaOH when flipped upside down</example>
    /// </summary>
    public class RotationItemDispenser : AbstractItemDispenser
    {
        [Tooltip("Where to dispense the object")]
        public Transform spawnPoint;

        [Tooltip("Desired rotation of when to dispense the object")]
        public Vector3 desiredRotation = Vector3.down;

        [Tooltip("The minimum likeness between our transform rotation and rotation at which to dispense the item"), Range(0f, 1f)]
        public float rotationMinLikeness = 0.8f;
        
        [Tooltip("Minimum delay between item dispenses")]
        public float delay = 0.5f;
        
        // variables used to control how often items should be spawned
        private float delayTimer = 0f;
        private bool canDispense = true;
        
        [Tooltip("The number of items to dispense at once")]
        public int numItemsToDispenseAtOnce = 1;

        private void Update()
        {
            if (!canDispense)
            {
                delayTimer += Time.deltaTime;
                if (delayTimer >= delay)
                {
                    canDispense = true;
                    delayTimer = 0f;
                }
            }

            if (canDispense)
            {
                float rotationLikeness = (Vector3.Dot(transform.up, desiredRotation) + 1) / 2;
                if (rotationLikeness > rotationMinLikeness)
                {
                    DispenseItems(spawnPoint.position, Quaternion.identity, numItemsToDispenseAtOnce);
                    canDispense = false;
                }
            }
        }
    }
}