using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ChemicalReactionScript : NetworkBehaviour
{
    public Hashtable reactionIngredients; // public list of all ingredients with their corresponding current values in the container

    private GameObject sodiumHydroxide; // internal reference to first NaOH pellet in the container
    private ObjectProperties water; // internal reference to first water GameObject in the container
    private ObjectProperties ownProperties;
    private float initialContainerWeight;
    private bool startedReaction; // private bool value to prevent multiple reactions from happening, may be changed back after some time later to allow for multiple reactions in one container

    // Start is called before the first frame update
    void Start()
    {
        reactionIngredients = new Hashtable(); // keys: string name of chemical, values: chemical amount in gram
        startedReaction = false; // set to false initially
        ownProperties = GetComponent<ObjectProperties>();
        initialContainerWeight = ownProperties.Weight; // saved for weight changes later
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        
        // check for container contents and start new reaction if corresponding chemicals are contained
        if(reactionIngredients.ContainsKey("Water") && reactionIngredients.ContainsKey("NaOH") && !startedReaction)
        {
            if((float) reactionIngredients["Water"] > 0) 
            {
                Debug.Log("[Log] Found both NaOH and H2O in the container, will start reaction in 5 seconds.."); // Logging reaction start, can be removed at a later point
                StartCoroutine("StartNaOHAfter5Secs"); // wait 5 seconds for chemicals to set in the container, then start corresponding reaction
                startedReaction = true;
            }
        }

        float addtitionalIngredientWeight = 0;
        foreach (var key in reactionIngredients.Keys)
        {
            addtitionalIngredientWeight += (float) reactionIngredients[key]; // compute sum of all ingredients
        }
        if(initialContainerWeight + addtitionalIngredientWeight > ownProperties.Weight) ownProperties.Weight = initialContainerWeight + addtitionalIngredientWeight; // add ingredient weights to initial weight
    }

    // add an ingredient of name ingredientName with its amount ingredientAmount
    public void addIngredient(string ingredientName, float ingredientAmount)
    {
        if(reactionIngredients.ContainsKey(ingredientName))
        {
            // if the ingredient is contained already, only update the amount in the list
            reactionIngredients[ingredientName] = (float) reactionIngredients[ingredientName] + ingredientAmount;
        }
        else
        {
            // if the ingredient is not contained, add it as a new ingredient
            reactionIngredients.Add(ingredientName, ingredientAmount);
        }
    }

    // remove ingredientAmount of ingredientName from the containers contents
    public void removeIngredient(string ingredientName, float ingredientAmount)
    {
        if(reactionIngredients.ContainsKey(ingredientName))
        {
            if((float) reactionIngredients[ingredientName] > ingredientAmount)
            {
                // only remove amount if contained amount is greater than amount to be removed
                reactionIngredients[ingredientName] = (float) reactionIngredients[ingredientName] - ingredientAmount;
            }
            else 
            {
                // remove ingredient completely else
                reactionIngredients.Remove(ingredientName);
                if(ingredientName == "Water")
                {
                    resetWater(); // if no water is left anymore, delete reference to water GameObject
                }
                else if(ingredientName == "NaOH")
                {
                    resetNaOH(); // if no NaOH is left anymore, delete reference to (first) NaOH pellet
                }
            }
        }
        else
        {
            Debug.Log("[ERROR] The specified ingredient (" + ingredientName + ") is not part of the container!"); // log illegal function call
        }
    }

    // clears the whole list of container contents (not used currently, might be removed later!)
    void clearContainer()
    {
        reactionIngredients.Clear();
    }

    // Coroutine to wait 5 seconds first and update the water (solution placeholder) temperature with the reaction process result afterwards
    IEnumerator StartNaOHAfter5Secs() 
    {
        yield return new WaitForSeconds(5);
        float finalTemperature = startNaOHAndWater();
        water.Temperature = finalTemperature;
        ownProperties.Temperature = finalTemperature;
    }

    float startNaOHAndWater()
    {
        // ASSUMPTION: both chemicals have the same initial temperature in the container
        Debug.Log("[Log] Starting NaOH + Water reaction with " + (float) reactionIngredients["NaOH"] + "g of NaOH and " + (float) reactionIngredients["Water"] + "g of H2O."); // Logging specific reaction start, can be removed at a later point
        float initialSolutionTemperature = sodiumHydroxide.GetComponent<ObjectProperties>().Temperature; // get NaOH temperature

        // make sure that really NaOH and water are both contained in the container
        if(reactionIngredients.ContainsKey("Water") && reactionIngredients.ContainsKey("NaOH"))
        {
            // TODO: add events for particles!
            float solutionMoles = (float) reactionIngredients["NaOH"] / ChemicalConstants.NaOH_molarMass; // number of moles of NaOH given
            float solutionHeat = ChemicalConstants.NaOH_molarHeat * solutionMoles; // heat of solution in (negative) kJ
            float solutionMass = ((float) reactionIngredients["NaOH"] + (float) reactionIngredients["Water"]) / 1000; // total mass of the solution(needed in kg)
            float temperatureChange = (- solutionHeat / (solutionMass * ChemicalConstants.Water_specificHeat)) * 0.7f; // Delta temperature of solution (has to be added to the initial temperature)
            Debug.Log("This is the estimated result for the solution: " + (initialSolutionTemperature + temperatureChange)); // Logging reaction temperature result, can be removed at a later point
            return initialSolutionTemperature + temperatureChange;
        }
        else
        {
            Debug.Log("An error occurred! Please contact your system administrator!"); // This should be impossible to reach under normal circumstances due to double check beforehand!
            return initialSolutionTemperature; // dont change anything in this case
        }
    }

    // set internal water GameObject reference
    public void setWater(ObjectProperties waterObject)
    {
        if(water == null)
        {
            water = waterObject;
        }
    }

    // // set internal NaOH GameObject reference
    public void setNaOH(GameObject naOHObject)
    {
        if(sodiumHydroxide == null)
        {
            sodiumHydroxide = naOHObject;
        }
    }

    // reset internal water GameObject reference (set to null)
    void resetWater()
    {
        water = null;
    }

    // reset internal NaOH GameObject reference (set to null)
    void resetNaOH()
    {
        sodiumHydroxide = null;
    }
}