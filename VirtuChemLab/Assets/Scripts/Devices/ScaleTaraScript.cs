using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class ScaleTaraScript : MonoBehaviour
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
        scaleController = transform.parent.GetComponent<ScaleController>();
        Assert.IsTrue(scaleController != null, "Error, scalecontroller not found");
    }

    void OnMouseDown()
    {
        SetInternalWeight();
    }

    private void SetInternalWeight()
    {
        // set the tara of the scale to the current internal weight on mouse click        
        source.PlayOneShot(taraButton, 1.0f);
        scaleController.SetInternalWeight();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if collider is left or right hand via layer
        int layer = other.gameObject.layer;
        if (!other.isTrigger && (layer == 10 || layer == 11))
            SetInternalWeight();
    }
}
