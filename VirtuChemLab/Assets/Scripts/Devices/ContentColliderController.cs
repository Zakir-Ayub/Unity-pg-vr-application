using UnityEngine;

/// <summary>
/// Keeps track of chemical gameobjects of ChemicalReactionScript.
/// </summary>
public class ContentColliderController : MonoBehaviour
{
    private ElementContainer Container;
    private ChemicalReactionScript reactionScript;

    // Start is called before the first frame update
    void Start()
    {
        var chemicalContainer = gameObject.transform.parent.parent.gameObject;
        Container = chemicalContainer.GetComponent<IContainerObject>().Container;
        reactionScript = chemicalContainer.GetComponent<ChemicalReactionScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // if it does not have the Chemical tag it should be ignored
        if (!other.gameObject.CompareTag("Chemical")) return; 
        reactionScript.AddChemical(other.gameObject);
    }
    
    private void OnTriggerExit(Collider other)
    {
        // if it does not have the Chemical tag it should be ignored
        if (!other.gameObject.CompareTag("Chemical")) return;
        var amount = ParticleRegistry.Singleton.GetChemicalAmount(other.gameObject);
        if (amount == null)
            return;        
        // Remove from list, since it exited the area
        Container.RemoveAmount(amount);
    }
}
