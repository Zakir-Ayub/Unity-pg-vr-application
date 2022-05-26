using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualChemLab.Networking.Device
{
    public class DeviceValue : NetworkBehaviour
    {
        private DeviceToggle deviceToggle;
        private Text text;

        private NetworkVariable<int> value = new NetworkVariable<int>(0);
        private float increaseValueTimer = 0f;
        private bool shouldIncreaseValue = false;

        private void Start()
        {
            deviceToggle = GetComponent<DeviceToggle>();
            text = GetComponentInChildren<Text>();

            deviceToggle.OnStateChanged += DeviceToggled;
        }

        private void DeviceToggled(bool deviceState)
        {
            if (IsServer)
            {
                value.Value = 0;
                shouldIncreaseValue = deviceState;
            }
        }

        private void Update()
        {
            text.text = value.Value.ToString();

            if (IsServer)
            {
                if (shouldIncreaseValue)
                {
                    increaseValueTimer += Time.deltaTime;

                    if (increaseValueTimer >= 1f)
                    {
                        increaseValueTimer = 1f - increaseValueTimer;
                        value.Value += 10;
                    }
                }
            }
        }
    }
}