using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Has keys and can add or remove text
    /// from an output (e.g. <see cref="InputField"/>).
    /// </summary>
    public class KeyboardController : MonoBehaviour
    {
        [SerializeField]
        public GameObject keyRowPrefab;
        
        [SerializeField]
        public GameObject keyPrefab;

        [SerializeField]
        private InputField input;

        [SerializeField]
        private List<CharacterKey> characterKeys = new List<CharacterKey>();

        [SerializeField]
        private bool isUpperCase = true;

        private void Start()
        {
            SetUpperCase(isUpperCase);
        }

        /// <summary>
        /// Add given text to the output.
        /// </summary>
        public void AddText(string text)
        {
            input.text += text;
        }

        /// <summary>
        /// Delete the last character of the output.
        /// </summary>
        public void DeleteLastCharacter()
        {
            input.text = input.text.Substring(0, Math.Max(0, input.text.Length - 1));
        }

        /// <summary>
        /// Toggles upper case mode of keyboard, e.g. "Q" -> "q"
        /// and "q" -> "Q".
        /// </summary>
        public void ToggleUpperCase()
        {
            SetUpperCase(!isUpperCase);
        }
        
        /// <summary>
        /// Set characters of keyboard to upper or lower
        /// case.
        /// </summary>
        private void SetUpperCase(bool isUpperCase)
        {
            this.isUpperCase = isUpperCase;

            foreach (CharacterKey key in characterKeys)
            {
                key.SetUpperCase(isUpperCase);
            }
        }
    }
}