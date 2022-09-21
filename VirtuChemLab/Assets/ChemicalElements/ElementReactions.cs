using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Used to model a reaction. Can be instantiated in the menu.
/// Has list of inputs and outputs as well as the resulting energy.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ReactionScriptableObject", order = 1)]
public class ElementReactions : ScriptableObject
{
    public ElementAmount[] reactans;
    
    public ElementAmount[] outputs;
    public bool IsEndothermikReaction() { return energyResult < 0; }
    public bool IsExothermicReaction() { return energyResult > 0; }

    public float energyResult; // amount that is freed by / required for the reaction
    public bool needsStirring; // requires stirring to start reaction
    public float time; // speed of the reaction
    public bool applyMaxWaterRatio;
}
