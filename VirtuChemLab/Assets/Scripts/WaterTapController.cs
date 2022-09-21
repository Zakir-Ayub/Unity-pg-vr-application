using UnityEngine;

public class WaterTapController : MonoBehaviour
{
    private ParticleSystem waterParticles;

    [Tooltip("The maximum emission rate over time for the water tap particle system")]
    public int maxEmissionRateOverTime;

    [Tooltip("The minimum rotation angle to let the water flow")]
    public int minRotation;

    [Tooltip("The maximum rotation angle to let the water flow")]
    public int maxRotation;

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
        currentRotation = UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).x; // get current x rotation (rotation axis for button)
        var waterEmission = waterParticles.emission; // the emission system of the water tap particle system
        if(currentRotation > minRotation)
        {
            if(currentRotation > maxRotation)
            {
                // Note: The structure of this method may also be changed in a way that the emission rate is changed directly intead of only the currentRotation here
                currentRotation = maxRotation; // emit water with max emission rate after reaching max rotation angle
            }
            int rateOverTime = Mathf.RoundToInt(maxEmissionRateOverTime * (currentRotation - minRotation) / (maxRotation - minRotation)); // calculate emission rate over time by multiplying with the rotation based influence
            waterEmission.rateOverTime = rateOverTime;
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
