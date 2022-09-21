using Unity.Netcode;
using UnityEngine;

namespace Devices
{
    /// <summary>
    /// The gas burner controller reads the amount
    /// of gas in the connected hose and sets appropriate
    /// values for the <see cref="GasBurnerFire"/> effect.
    /// </summary>
    public class GasBurner : NetworkBehaviour
    {
        [SerializeField]
        private Valve airValve;
        
        [SerializeField]
        private Valve gasValve;
        
        [SerializeField]
        private GasBurnerFire fire;
        
        [SerializeField]
        private HoseSocket hoseSocket;
        
        [SerializeField]
        private HoseSegment connectedHose;

        [SerializeField]
        private float gasConsumption = 10f;

        private void Start()
        {
            hoseSocket.OnHoseConnect += (hose) => connectedHose = hose;
            hoseSocket.OnHoseDisconnect += (_) => connectedHose = null;
        }

        private void Update()
        {
            float gasAmount = 0f;
            float airAmount = airValve.OpenedBy;
            
            /*
            if (connectedHose)
            {
                float consumeGasAmount = gasValve.OpenedBy * gasConsumption * Time.deltaTime;
                if (connectedHose.FluidAmount > consumeGasAmount)
                {
                    gasAmount = gasValve.OpenedBy;
                    connectedHose.RemoveFluids(consumeGasAmount);
                }
            }
            */
            gasAmount = gasValve.OpenedBy; 

            fire.GasAmount = gasAmount;
            fire.AirAmount = airAmount;
        }
    }
}