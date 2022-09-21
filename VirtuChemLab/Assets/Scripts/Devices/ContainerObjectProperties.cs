using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// ObjecProperties of an object with an ElementContainer. Added to the Water gameobject.
/// </summary>
public class ContainerObjectProperties : NetworkObjectproperties
{
    private ElementContainer container;

    private void Start()
    {
        Assert.IsTrue(name.Equals("Water"));
        container = transform.parent.GetComponent<IContainerObject>().Container;
    }

    public bool hasContainer()
    {
        return container != null;
    }

    public override float Weight
    {
        get => container.Weight();
        set => throw new System.NotSupportedException();
    }

    public override float PhValue
    {
        get => container.PhValue();
        set => throw new System.NotSupportedException();
    }

    public override float Temperature
    {
        get => container.temperature;
        set
        {
            if (IsServer)
            {
                container.temperature = value;
            }
        }
    }

}
