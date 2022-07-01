using UnityEngine;
using UnityEngine.UI;

namespace Network.UI.Connect
{
    /// <summary>
    /// Controller which switches the keypad between ip and
    /// port input mode. Updates the button text to the current
    /// input mode.
    /// </summary>
    public class ConnectSwitchKeypadModeButton : MonoBehaviour
    {
        public ConnectKeypadController keypad;
        public string ipModeText = "Switch to Port";
        public string portModeText = "Switch to IP";

        private Text buttonText;

        private void Start()
        {
            buttonText = GetComponentInChildren<Text>();
        }

        private void Update()
        {
            switch (keypad.inputMode)
            {
                case ConnectKeypadController.InputMode.IP:
                    buttonText.text = ipModeText;
                    break;
                case ConnectKeypadController.InputMode.Port:
                    buttonText.text = portModeText;
                    break;
            }
        }
    }
}