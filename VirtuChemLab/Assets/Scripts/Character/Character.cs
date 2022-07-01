using UnityEngine;

namespace Character
{
    /// <summary>
    /// Simple data structure for body bone positions.
    ///
    /// <example>
    /// Use for synchronizing VR player with character
    /// model.
    /// </example>
    /// </summary>
    public class Character : MonoBehaviour
    {
        public Transform head, rightFoot, leftFoot, hips;
        public CharacterHand rightHand, leftHand;
    }
}