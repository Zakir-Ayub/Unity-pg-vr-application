using Network.Device;
using UnityEngine;

public class ScaleTaraScript : MonoBehaviour, NetworkDeviceInteractor
{
    private AudioSource source;

    // this represents the ScaleController from the parent object
    private ScaleController scaleController;

    [Tooltip("Sound that plays when the tare button is pressed")]
    public AudioClip taraButton;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
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
            source.PlayOneShot(taraButton, 1.0f);
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
