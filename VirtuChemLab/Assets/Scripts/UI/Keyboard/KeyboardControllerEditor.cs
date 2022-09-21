using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Provides convenience functions for the
    /// <see cref="KeyboardController"/>, such as
    /// automatically laying out the default keys.
    /// </summary>
    [CustomEditor(typeof(KeyboardController))]
    public class KeyboardControllerEditor : Editor
    {
        // characters of our keyboard
        private static readonly char[][] KEYBOARD_LAYOUT =
        {
            new[] { 'Q', 'W', 'E', 'R', 'T', 'Z', 'U', 'I', 'O', 'P' },
            new[]   { 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L' },
            new[]      { 'Y', 'X', 'C', 'V', 'B', 'N', 'M' }
        };
        // left padding of each row
        private static readonly int[] ROW_OFFSETS = { 0, 55, 155 };
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Add QWERTZ keys"))
            {
                CreateKeys();
            }
        }

        /// <summary>
        /// Create and layout the character keys according
        /// to <see cref="KEYBOARD_LAYOUT"/> and <see cref="ROW_OFFSETS"/>.
        /// </summary>
        private void CreateKeys()
        {
            KeyboardController controller = (KeyboardController) target;
            
            for (int row = 0; row < KEYBOARD_LAYOUT.Length; row++)
            {
                GameObject keyRow = Instantiate(controller.keyRowPrefab, controller.transform);
                
                LayoutGroup rowLayoutGroup = keyRow.GetComponent<LayoutGroup>();
                rowLayoutGroup.padding.left = ROW_OFFSETS[row];

                for (int col = 0; col < KEYBOARD_LAYOUT[row].Length; col++)
                {
                    // create key and set it's text to character in KEYBOARD_LAYOUT
                    GameObject key = Instantiate(controller.keyPrefab, Vector3.zero, Quaternion.identity, keyRow.transform);

                    Text text = key.GetComponentInChildren<Text>();
                    text.text = KEYBOARD_LAYOUT[row][col].ToString();

                    CharacterKey characterKey = key.GetComponent<CharacterKey>();
                    characterKey.keyboard = controller;
                }
            }
        }
    }
}