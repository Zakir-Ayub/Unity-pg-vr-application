using UnityEngine;
using UnityEngine.Assertions;

public class ScaleOnOffScript : MonoBehaviour
{

    // this represents the ScaleController from the parent object
    private ScaleController scaleController;

    // Start is called before the first frame update
    void Start()
    {
        scaleController = transform.parent.GetComponent<ScaleController>();
        Assert.IsTrue(scaleController != null, "Error, scalecontroller not found");
    }

    void OnMouseDown()
    {
        ToggleScale();
    }

    private void ToggleScale()
    {
        // simply toggle on/off status of scale on mouse click
        scaleController.ToggleDeviceOn();
        
    }

    private void OnTriggerEnter(Collider other) 
    {
        // Check if collider is left or right hand via layer
        int layer = other.gameObject.layer;
        if (!other.isTrigger && (layer == 10 || layer == 11))
            ToggleScale();
    }
}
