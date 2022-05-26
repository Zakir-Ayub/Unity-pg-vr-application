using Unity.Netcode;

namespace VirtualChemLab.Networking.Player
{
    public class PlayerDance : NetworkBehaviour
    {
        public delegate void OnDanceListener(int dance);
        public OnDanceListener OnDance;
        
        public void Dance(int number)
        {
            OnDance?.Invoke(number);
        }

        [ServerRpc]
        private void DanceServerRpc(int number)
        {
            DanceClientRpc(number);
        }

        [ClientRpc]
        private void DanceClientRpc(int number)
        {
            OnDance?.Invoke(number);
        }
    }
}