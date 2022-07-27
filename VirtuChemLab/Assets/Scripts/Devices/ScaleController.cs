using System.Collections.Generic;
using Network.Device;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ScaleController : NetworkBehaviour
{
    private AudioSource source;

    [Tooltip("This is the text object that the weight will be displayed in.")]
    public Text textObject;

    [Tooltip("Sound that plays when the on/off button is pressed")]
    public AudioClip OnOffButton;

    // this list contains all objects that are used for the weight calculation of the scale
    private List<GameObject> collisionList = new List<GameObject>();

    // necessary network variables below
    private NetworkDeviceValue<bool> isDeviceTurnedOn = new NetworkDeviceValue<bool>(true);
    public bool IsDeviceTurnedOn
    {
        get
        {
            return isDeviceTurnedOn.Value;
        }
        set
        {
            if(IsServer)
                isDeviceTurnedOn.Value = value;
        }
    }

    private NetworkDeviceValue<float> weight = new NetworkDeviceValue<float>(0);
    public float Weight => weight.Value;
    
    private NetworkDeviceValue<float> taraOffset = new NetworkDeviceValue<float>(0);
    public float TaraOffset
    {
        get => taraOffset.Value;
        set
        {
            if (IsServer)
                taraOffset.Value = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        // weight calculation should be server authoritative
        if (IsServer)
        {
            weight.Value = CalculateWeight();
        }
        
        // adds the weight to the display
        if(IsDeviceTurnedOn)
        {
            float displayWeight = Weight - TaraOffset;
            
            if(displayWeight >= 100) // if the displayed weight is over 100g --> display g by default instead of mg
            {
                textObject.text = (Mathf.Round((displayWeight) * 10000.0f) / 10000.0f) + "g";
            }
            else
            {
                textObject.text = (Mathf.Round(((displayWeight) * 1000) * 100.0f) / 100.0f) + "mg"; // mg is the default unit (for low weight)
            }
        }
        else
        {
            textObject.text = "";
        }
    }

    private float CalculateWeight()
    {
        float currentWeight = 0;
        foreach (GameObject collisionObject in collisionList)
        {
            ObjectProperties objectProperties = collisionObject.GetComponent<ObjectProperties>();
            if(objectProperties)
            {
                // sum up the weight for each object with ObjectProperties attached to it in the collisionList
                currentWeight += objectProperties.Weight;
            }
        }
        return currentWeight;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("ChemistryObject") && !other.gameObject.CompareTag("StirringFish")) return; // if it does not have the ChemistryObject or StirringFish tag it should be ignored

        ScaleTouchController scaleTouchController = other.gameObject.GetComponent<ScaleTouchController>();
        if(scaleTouchController) // if object with a ScaleTouchController
        {
            // update the touching gameobject's ScaleTouchController fields and add it to the collisionList
            scaleTouchController.scale = gameObject;
            scaleTouchController.touchesScale = true;
            if(!collisionList.Contains(other.gameObject)) // if object not contained in list yet
            {
                collisionList.Add(other.gameObject);
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.gameObject.CompareTag("ChemistryObject") && !other.gameObject.CompareTag("StirringFish")) return; // if it does not have the ChemistryObject or StirringFish tag it should be ignored

        if(collisionList.Contains(other.gameObject)) // if other object is contained in the collisionList
        {
            ScaleTouchController scaleTouchController = other.gameObject.GetComponent<ScaleTouchController>();
            if(scaleTouchController != null) // if object with a ScaleTouchController
            {
                // update the no longer touching gameobject's ScaleTouchController fields and remove it from the collisionList
                scaleTouchController.scale = null;
                scaleTouchController.touchesScale = false;  
                collisionList.Remove(other.gameObject);
            }
        }
    }

    // toggles the scale on and off
    public void ToggleDeviceOn()
    {
        ToggleDeviceOnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleDeviceOnServerRpc()
    {
        IsDeviceTurnedOn = !IsDeviceTurnedOn;
        source.PlayOneShot(OnOffButton, 1.0f);
    }

    public void SetInternalWeight()
    {
        SetInternalWeightServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetInternalWeightServerRpc()
    {
        TaraOffset = Weight;
    }


    // handles the adding and removing of objects to the collisionList accordingly
    public void SetOnTop(GameObject gameObject, bool isOnTop)
    {
        if (!IsOnTop(gameObject) && isOnTop)
        {
            collisionList.Add(gameObject);
        }
        else if (IsOnTop(gameObject) && !isOnTop)
        {
            collisionList.Remove(gameObject);
        }
    }
    
    // checks if object is contained in the collisionList
    public bool IsOnTop(GameObject gameObject)
    {
        return collisionList.Contains(gameObject);
    }
}
