using UnityEngine;

public class ContentColliderController : MonoBehaviour
{

    private GameObject chemicalContainer;
    private ChemicalReactionScript containerReaction;

    // Start is called before the first frame update
    void Start()
    {
        chemicalContainer = gameObject.transform.parent.gameObject.transform.parent.gameObject;
        containerReaction = chemicalContainer.GetComponent<ChemicalReactionScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Chemical")) return; // if it does not have the Chemical tag it should be ignored

        ObjectProperties objectProperties = (other.gameObject.GetComponent("ObjectProperties") as ObjectProperties);
        if(objectProperties != null)
        {
            // add the chemical to the corresponding ingredient list of the container it was put into
            string otherName = other.name;

            if(otherName.Contains("Water"))
            {
                containerReaction.setWater(objectProperties);
                containerReaction.addIngredient(otherName, objectProperties.Weight);
            }
            else if(otherName.Contains("NaOH"))
            {
                containerReaction.setNaOH(other.gameObject);
                containerReaction.addIngredient("NaOH", objectProperties.Weight);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(!other.gameObject.CompareTag("Chemical")) return; // if it does not have the Chemical tag it should be ignored

        string otherName = other.name.Split()[0];
        if(containerReaction.reactionIngredients.ContainsKey(otherName))
        {
            ObjectProperties objectProperties = (other.gameObject.GetComponent("ObjectProperties") as ObjectProperties);
            if(objectProperties != null)
            {
                // remove the chemical from the corresponding ingredient list of the container it was removed from
                containerReaction.removeIngredient(otherName, objectProperties.Weight);
            }
        }
    }
}
