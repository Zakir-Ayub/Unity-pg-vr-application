using System;
using Network.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Network.UI.Connect
{
    /// <summary>
    /// Controller for the VR IP/port input keypad, used to connect or
    /// host. Receives inputs from the UI and updates the IP/port values.
    /// Calls <c>NetworkLobby</c> to host or connect to a server.
    /// </summary>
    public class ConnectKeypadController : MonoBehaviour
    {
        public NetworkLobbyManager networkLobby;

        public string defaultIPAddress = "127.0.0.1";
        public string defaultPort = "7777";
        
        public enum InputMode
        {
            IP,
            Port,
        }
        public InputMode inputMode = InputMode.IP;
        
        public Color inputActiveColor = Color.gray, inputInactiveColor =  Color.white;
        private ColorBlock activeColors, inactiveColors;
        
        public InputField ipAddressInput, portInput;

        private void Start()
        {
            activeColors = ipAddressInput.colors;
            activeColors.disabledColor = inputActiveColor;

            inactiveColors = ipAddressInput.colors;
            inactiveColors.disabledColor = inputInactiveColor;

            ipAddressInput.text = defaultIPAddress;
            portInput.text = defaultPort;
            
            UpdateInputFields();
        }

        public void AddNumber(int value)
        {
            switch (inputMode)
            {
                case InputMode.IP:
                    ipAddressInput.text += value;
                    break;
                case InputMode.Port:
                    portInput.text += value;
                    break;
            }
        }
        
        public void AddDot()
        {
            switch (inputMode)
            {
                case InputMode.IP:
                    ipAddressInput.text += '.';
                    break;
            }
        }

        public void RemoveLast()
        {
            switch (inputMode)
            {
                case InputMode.IP:
                    ipAddressInput.text = ipAddressInput.text.Substring(0, Math.Max(0, ipAddressInput.text.Length - 1));
                    break;
                case InputMode.Port:
                    portInput.text = portInput.text.Substring(0, Math.Max(0, portInput.text.Length - 1));
                    break;
            }
        }

        public void SwitchMode()
        {
            switch (inputMode)
            {
                case InputMode.IP:
                    inputMode = InputMode.Port;
                    break;
                case InputMode.Port:
                    inputMode = InputMode.IP;
                    break;
            }
            UpdateInputFields();
        }

        private void UpdateInputFields()
        {
            switch (inputMode)
            {
                case InputMode.IP:
                    ipAddressInput.ActivateInputField();
                    ipAddressInput.colors = activeColors;
                    
                    portInput.DeactivateInputField();
                    portInput.colors = inactiveColors;
                    break;
                case InputMode.Port:
                    ipAddressInput.DeactivateInputField();
                    ipAddressInput.colors = inactiveColors;
                    
                    portInput.ActivateInputField();
                    portInput.colors = activeColors;
                    break;
            }
        }

        public void Host()
        {
            networkLobby.Host();
        }

        public void Connect()
        {
            string ip = ipAddressInput.text;
            ushort port = ushort.Parse(portInput.text);
            
            networkLobby.Connect(ip, port);
        }

        public void Disconnect()
        {
            networkLobby.Disconnect();
        }
    }
}