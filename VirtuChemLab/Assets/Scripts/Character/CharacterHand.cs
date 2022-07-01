using UnityEngine;

namespace Character
{
    /// <summary>
    /// Simple storage for hand bone positions.
    ///
    /// <example>
    /// Used for synchronizing VR player hand with
    /// character model hands.
    /// </example>
    /// </summary>
    [System.Serializable]
    public class CharacterHand
    {
        [System.Serializable]
        public class Finger
        {
            public Transform tip;
            public Transform middle;
            public Transform root;

            public Finger(Transform tip, Transform middle, Transform root)
            {
                this.tip = tip;
                this.middle = middle;
                this.root = root;
            }
        }

        public Transform root;
        public Finger thumb, index, middle, ring, pinky;
    }
}