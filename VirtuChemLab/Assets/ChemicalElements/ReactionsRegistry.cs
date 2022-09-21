using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Keeps list of reactions that are allowed in the scene.
/// </summary>
public class ReactionsRegistry : MonoBehaviour
{
    public static ReactionsRegistry Singleton = null;

    public ElementReactions[] knownReactions; // List of known reactions. should be serealized in Inspector

    void Awake()
    {
        Assert.IsNull(Singleton, "Singleton ReactionsRegistry already exists");
        Singleton = this;
    }


}
