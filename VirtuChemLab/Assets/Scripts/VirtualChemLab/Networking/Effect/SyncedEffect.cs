using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace VirtualChemLab.Networking.Effect
{
    /**
     * This class demonstrates the usage of LocalTime
     * and ServerTime by spawning an object at
     * the (roughly) same time on clients and server without additional
     * synchronization.
     */
    public class SyncedEffect : NetworkBehaviour
    {
        public GameObject effect;

        public void CreateEffect()
        {
            double localTime = NetworkManager.LocalTime.Time;
            CreateSyncedEffectServerRpc(localTime);
        }

        [ServerRpc]
        private void CreateSyncedEffectServerRpc(double clientTime)
        {
            CreateSyncedEffectClientRpc(clientTime);
        }

        [ClientRpc]
        private void CreateSyncedEffectClientRpc(double otherClientTime)
        {
            double serverTime = NetworkManager.ServerTime.Time;
            double timeToWait = otherClientTime - serverTime;
            StartCoroutine(WaitAndSpawnSyncedEffect((float)timeToWait));
        }
        
        private IEnumerator WaitAndSpawnSyncedEffect(float timeToWait)
        {
            if (timeToWait > 0)
            {
                yield return new WaitForSeconds(timeToWait);
            }

            Instantiate(effect, transform.position, Quaternion.identity);
        }
    }
}