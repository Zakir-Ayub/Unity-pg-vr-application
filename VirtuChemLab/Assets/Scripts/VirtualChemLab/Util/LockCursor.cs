using UnityEngine;

namespace VirtualChemLab.Util
{
    public class LockCursor : MonoBehaviour
    {
        public CursorLockMode mode = CursorLockMode.Locked;
    
        void Start()
        {
            Cursor.lockState = mode; 
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
    }
}