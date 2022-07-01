using Network.Device;
using UnityEngine;

public class ScaleOnOffScript : MonoBehaviour, NetworkDeviceInteractor
{

    // this represents the ScaleController from the parent object
    private ScaleController scaleController;

    // Start is called before the first frame update
    void Start()
    {
        scaleController = transform.parent.gameObject.GetComponent<ScaleController>();
    }

    void OnMouseDown()
    {
        ToggleScale();
    }

    public void OnInteract(GameObject player)
    {
        ToggleScale();
    }

    private void ToggleScale()
    {
        // simply toggle on/off status of scale on mouse click
        if(scaleController != null)
        {
            scaleController.ToggleDeviceOn();
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        // Check if collider is left or right hand via layer
        int layer = other.gameObject.layer;
        if (!other.isTrigger && (layer == 10 || layer == 11))
            OnMouseDown();
    }
}
