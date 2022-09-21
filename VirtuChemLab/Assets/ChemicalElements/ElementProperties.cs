using UnityEngine;

/// <summary>
/// Used to model an element. Contains all needed properties and constants.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ElementScriptableObject", order = 1)]
public class ElementProperties : ScriptableObject
{
    [Tooltip("melting point in Celsius")]
    public float meltingPoint = 0f; //this element melts
    [Tooltip("boiling point in Celsius")]
    public float boilingPoint; //this element boils
    [Tooltip("weight in g/mole")]
    public float molecularWeight; // Weight per mole
    [Tooltip("density in g/ml")]
    public float density; // g per ml
    public float phValue; // ph of element
    public Color color = Color.white; // Color of the element
    public string elementName; // name of the element

    public float maxWaterRatio; // max dissolving ratio
}
