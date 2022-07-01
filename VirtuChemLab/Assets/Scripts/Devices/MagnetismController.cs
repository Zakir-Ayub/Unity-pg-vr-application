using System;
using Unity.Netcode;
using UnityEngine;

public class MagnetismController : NetworkBehaviour
{

    [Tooltip("The forceFactor indicates the strength of the magnetic force")]
    public float forceFactor = 1000f;

    private Rigidbody stirringFish;

    private Transform magnetPoint;

    private float maxRightRotation;

    [NonSerialized]
    public bool magnetize;

    [Tooltip("Reference to the right switch of the stirrer for controlling the heat")]
    public MagnetStirrer_right_switch rightSwitch;

    [Tooltip("Set the max spin speed of the stirring fish in rpm")]
    public int maxSpinSpeed;

    [Tooltip("Set the step size for the RPM")]
    public int stepSize;

    private float stirFishAPF; // the APF = angle per fixedupdate the stirringFish should rotate

    // Start is called before the first frame update
    void Start()
    {
        magnetPoint = GetComponent<Transform>();
        magnetize = false;
        maxRightRotation = rightSwitch.maxRotation;
        stirFishAPF = 0;
    }

    // Update is called 50 times per second
    void FixedUpdate()
    {
        if (IsServer)
        {
            var zRotation = rightSwitch.transform.eulerAngles.z;
            if (magnetize && stirringFish != null)
            {
                stirringFish.AddForce(
                    (magnetPoint.position - stirringFish.position) * forceFactor *
                    Time.fixedDeltaTime); // this creates the magnetism
                if (zRotation > 30)
                {
                    stirFishAPF =
                        Mathf.Round(Mathf.Round((zRotation - 30) / (maxRightRotation - 30) *
                                                (maxSpinSpeed / 60f / 50f * 360f)) / stepSize) *
                        stepSize; // /60 for seconds, /50 for updates per second, * 360 for angle, - 30 for starting offset
                    //Debug.Log("This is the current RPM of the stirring fish: " + Mathf.Round(Mathf.Round((zRotation - 30) / (maxRightRotation - 30) * maxSpinSpeed) / stepSize) * stepSize);
                    stirringFish.transform.Rotate(0, 0, stirFishAPF); // this handles the rotation of the stirring fish
                }
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("StirringFish"))
        {
            stirringFish = collider.GetComponent<Rigidbody>();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("StirringFish"))
        {
            stirringFish = null;
        }
    }
}
