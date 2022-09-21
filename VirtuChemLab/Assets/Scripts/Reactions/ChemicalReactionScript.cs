using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Checks if there are reactions that can be applied on the chemicals in the container.
/// </summary>
public class ChemicalReactionScript : NetworkBehaviour
{
    private ElementContainer container;

    // non dissolved chemicals in the Trigger
    private readonly List<GameObject> chemicals = new List<GameObject>(); 

    // Start is called before the first frame update
    void Start()
    {
        container = GetComponent<IContainerObject>().Container;
    }

    public void AddChemical(GameObject chemical)
    {
        var amount = ParticleRegistry.Singleton.GetChemicalAmount(chemical);
        if (amount == null) return;
        container.AddElementAmount(amount);
        chemicals.Add(chemical);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsServer) return;

        // Apply reactions
        foreach (var reaction in ReactionsRegistry.Singleton.knownReactions)
        {
            float amount = container.GetReactionAmount(reaction);
            if (amount == 0) continue;
            container.ApplyReaction(reaction, amount);
        }

        // Play Dissolve effect of chemicals that are gameobjects 
        var to_remove = new List<GameObject>();
        foreach (var chemical in chemicals)
        {
            var amount = ParticleRegistry.Singleton.GetChemicalAmount(chemical);
            if (amount == null)
                continue;
            //Check if chemical still in original state
            if (container.GetElementAmount(amount.element, amount.state, amount.charge) >= amount.amount) continue;
            //Play Dissolve animation
            chemical.GetComponent<Dissolve_Effect>().StartDissolveAnimation();
            ParticleRegistry.Singleton.UnregisterChemical(chemical);
            to_remove.Add(chemical);
        }
        foreach (var item in to_remove) chemicals.Remove(item);
    }
    
}