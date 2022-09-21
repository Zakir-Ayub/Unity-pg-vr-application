using UnityEngine;

namespace Devices
{
    /// <summary>
    /// Fills the connected hose with gas, if there
    /// is a connected hose.
    /// </summary>
    public class GasTap : MonoBehaviour
    {
        [SerializeField]
        private HoseSocket hoseSocket;
        
        private HoseSegment connectedHose;

        [SerializeField]
        private Valve valve;
        
        [SerializeField]
        private float gasProduction = 10f;
        
        private void Start()
        {
            hoseSocket.OnHoseConnect += (hose) => connectedHose = hose;
            hoseSocket.OnHoseDisconnect += (_) => connectedHose = null;
        }

        private void Update()
        {
            if (connectedHose)
            {
                connectedHose.AddFluids(valve.OpenedBy * gasProduction * Time.deltaTime);
            }
        }
    }
}