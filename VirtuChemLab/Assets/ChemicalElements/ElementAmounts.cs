using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A list of ElementAmounts. Has helper functions that are "cached".
/// </summary>
public class ElementAmounts
{
    public readonly List<ElementAmount> content = new List<ElementAmount>();

    //Delegate used internally for lazy updates
    protected delegate void LazyChangeDelegte();
    protected LazyChangeDelegte OnContentChanged;

    // lazy update properties
    protected bool outOfDate = true;
    private float totalMass = 0;
    private float weight = 0;
    private float fluidMass = 0;
    private float fluidMassWithoutDissolved = 0;
    private Color color;

    public ElementAmounts()
    {
        OnContentChanged += () => { outOfDate = true; };
    }

    protected void UpdateProperties()
    {
        outOfDate = false;
        weight = content.Select(x => x.amount).DefaultIfEmpty(0).Sum();
        totalMass = content.Select(x => x.ml).DefaultIfEmpty(0).Sum();
        fluidMass = GetLiquids().Select(x => x.ml).DefaultIfEmpty(0).Sum();
        fluidMassWithoutDissolved = GetLiquidsWithoutDissolved().Select(x => x.amount / x.element.density).DefaultIfEmpty(0).Sum();
        //Update Color
        Color result = new Color(0, 0, 0, 0);
        foreach (ElementAmount ea in content)
        {
            result += ea.element.color * ea.amount;
        }
        result /= totalMass;
        color = result;
    }

    public ElementAmounts(ElementAmount amount)
    {
        OnContentChanged += () => { outOfDate = true; };
        content.Add(amount);
        OnContentChanged.Invoke();
    }

    public ElementAmounts(List<ElementAmount> amounts)
    {
        OnContentChanged += () => { outOfDate = true; };
        content.AddRange(amounts);
        OnContentChanged.Invoke();
    }

    protected IEnumerable<ElementAmount> GetLiquids()
    {
        return content.Where(x => x.state == ElementAmount.ElementState.Liquid || x.state == ElementAmount.ElementState.Dissolved);
    }

    protected IEnumerable<ElementAmount> GetLiquidsWithoutDissolved()
    {
        return content.Where(x => x.state == ElementAmount.ElementState.Liquid);
    }

    public bool IsEmpty()
    {
        return content.Count == 0;
    }

    public float Weight()
    {
        if (outOfDate) UpdateProperties();
        return weight;
    }

    public float Mass()
    {
        if (outOfDate) UpdateProperties();
        return totalMass;
    }

    public float FluidMass()
    {
        if (outOfDate) UpdateProperties();
        return fluidMass;
    }

    public float FluidMassWithoutDissolved()
    {
        if (outOfDate) UpdateProperties();
        return fluidMassWithoutDissolved;
    }

    public Color GetColor()
    {
        if (outOfDate) UpdateProperties();
        return color;
    }
}
