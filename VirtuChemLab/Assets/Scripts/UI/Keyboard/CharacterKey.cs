using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Has a character (e.g. 'Q') and when clicked
    /// adds the character to the output of the
    /// <see cref="KeyboardController"/>.
    /// </summary>
    public class CharacterKey : MonoBehaviour
    {
        [SerializeField, Tooltip("The keyboard of this key")]
        public KeyboardController keyboard;
        
        private Button button;
        private Text text;
        
        private void Start()
        {
            button = GetComponent<Button>();
            text = GetComponentInChildren<Text>();

            button.onClick.AddListener(OnClick);
        }

        /// <summary>
        /// Called when the player presses
        /// the button.
        /// </summary>
        private void OnClick()
        {
            keyboard.AddText(text.text);
        }

        public void SetUpperCase(bool isUpperCase)
        {
            text.text = isUpperCase ? text.text.ToUpper() : text.text.ToLower();
        }
    }
}