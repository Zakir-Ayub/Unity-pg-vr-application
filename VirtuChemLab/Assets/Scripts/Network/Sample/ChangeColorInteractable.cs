using Network.VR;
using Unity.Netcode;
using UnityEngine;
using Random = System.Random;

namespace Network.Sample
{
    /// <summary>
    /// Sample code which changes the color of a material
    /// when a player interacts with it  using the XR
    /// controls.
    /// </summary>
    [RequireComponent(typeof(NetworkSimpleInteractable))]
    [RequireComponent(typeof(Renderer))]
    public class ChangeColorInteractable : NetworkBehaviour
    {
        private Color[] colors = { Color.red, Color.grey, Color.black, Color.green };
        private NetworkVariable<int> colorIndex = new NetworkVariable<int>();

        private new Renderer renderer;
    
        private void Start()
        {
            GetComponent<NetworkSimpleInteractable>().OnSelect = OnSelect;
            renderer = GetComponent<Renderer>();
        }

        private void OnSelect(GameObject player)
        {        
            Random r = new Random();
            colorIndex.Value = r.Next(0, colors.Length);
        }

        private void Update()
        {
            renderer.material.SetColor("_Color", colors[colorIndex.Value]);
        }
    }
}

