using Unity.Netcode;
using UnityEngine;

namespace VirtualChemLab.Networking.Server
{
    public class Server : MonoBehaviour
    {
        private void Start()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += AuthorizeConnection;
        }

        private void AuthorizeConnection(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            // here we could for example limit the max number of players
            
            bool allowConnection = true;
            bool createPlayerObject = true;
            
            Vector3 position = new Vector3(0, 5, 0);
            Quaternion rotation = Quaternion.identity;

            callback(createPlayerObject, null, allowConnection, position, rotation);
        }
    }
}