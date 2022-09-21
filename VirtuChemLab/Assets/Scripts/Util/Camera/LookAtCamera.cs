using UnityEngine;

namespace Util
{
    /// <summary>
    /// Rotate the object such that it
    /// always looks towards the main
    /// camera. Can also add a rotation
    /// offset.
    /// </summary>
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField, Tooltip("Additionally rotate after looking at the camera")] 
        private Vector3 rotationOffset;
        
        private Camera camera;

        private void Start()
        {
            camera = Camera.main;
        }

        private void Update()
        {
            transform.LookAt(camera.transform);
            transform.rotation *= Quaternion.Euler(rotationOffset);
        }
    }
}