using Network.Device;
using UnityEngine;

public class ScaleTaraScript : MonoBehaviour, NetworkDeviceInteractor
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
        SetInternalWeight();
    }

    public void OnInteract(GameObject player)
    {
        SetInternalWeight();
    }

    private void SetInternalWeight()
    {
        // set the tara of the scale to the current internal weight on mouse click
        if(scaleController != null)
        {
            scaleController.TaraOffset = scaleController.Weight;
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
