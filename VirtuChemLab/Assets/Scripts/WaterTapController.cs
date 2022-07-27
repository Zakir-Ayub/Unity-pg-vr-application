using UnityEngine;

public class WaterTapController : MonoBehaviour
{
    private ParticleSystem waterParticles;

    private float currentRotation;

    [HideInInspector]
    public bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        // get particle system for water tap
        waterParticles = transform.parent.Find("WaterTap").Find("WaterParticles").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        currentRotation = transform.eulerAngles.x; // get durrent x rotation (rotation axis for button)
        if(currentRotation > 15 && currentRotation < 180)
        {
            isOpen = true;
            waterParticles.Play(); // only play if the x rotation is in (45, 180)
        }
        else
        {
            isOpen = false;
            waterParticles.Stop(); // else stop the particle system
        }
        
    }
}
