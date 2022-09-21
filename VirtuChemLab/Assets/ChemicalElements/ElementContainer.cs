using System.Linq;
using UnityEngine;
using System;
using UnityEngine.Assertions;

/// <summary>
/// Containers are part of physical objects and contain chemicals.
/// The amount is limited by a ML amount.
/// They have physical properties like weight, a temperature and a PH value.
/// </summary>
public class ElementContainer : ElementAmounts
{
    public float maxStorage; //maximum Storage in ml
    public float temperature = 18; //temperature in C
    public float EmptyWeight = 0; //weight without content
    private float filledPercantage;
    private float phValue;
    public ElementContainer()
    {
        this.maxStorage = 0; // Set value later
    }
    public ElementContainer(float maxStorage)
    {
        this.maxStorage = maxStorage;
    }

    public new float Weight()
    {
        return EmptyWeight + base.Weight();
    }

    protected new void UpdateProperties()
    {
        base.UpdateProperties();
        filledPercantage = Mass() / maxStorage;
        if(FluidMass() > 0)
            phValue = GetLiquids().Average(x => x.element.phValue);
    }

    public float PhValue()
    {
        if (outOfDate) UpdateProperties();
        return phValue;
    }

    public float FilledPercantage()
    {
        if (outOfDate) UpdateProperties();
        return filledPercantage;
    }

    // apply a reaction a certain amount of times to the content.
    public void ApplyReaction(ElementReactions reaction, float amount)
    {
        Assert.IsTrue(amount != 0, "Reaction: Amount 0");
        if (reaction.applyMaxWaterRatio)
        {
            var waterLiters = content.Where(x => x.element.elementName == "Water").Select(x => x.amount).DefaultIfEmpty(0).Sum() / 1000;
            if (waterLiters > 0)
            {
                foreach (var reactant in reaction.reactans)
                {
                    if (reactant.element.maxWaterRatio != 0 && reactant.element.maxWaterRatio != 1)
                    {
                        var dissolvedReactantAmountLiters = content.Where(x => x.element.elementName == reactant.element.elementName && x.state == ElementAmount.ElementState.Dissolved).Select(x => x.amount).DefaultIfEmpty(0).Sum() / 1000;
                        if (dissolvedReactantAmountLiters / waterLiters > reactant.element.maxWaterRatio) return;
                    }
                }
            }
        }
        foreach (var reactant in reaction.reactans)
        {
            RemoveAmount(reactant * amount);
        }
        foreach (var output in reaction.outputs)
        {
            Assert.IsTrue(output.amount != 0, "Reaction: Output 0");
            AddElementAmount(output * amount);
        }

        float energy = amount * reaction.energyResult;
        float temperatureChange = energy / FluidMass() * 0.7f;
        temperature += temperatureChange;
    }

    // return how often the reaction can be applied with the given elements
    public float GetReactionAmount(ElementReactions reaction)
    {
        if (reaction.applyMaxWaterRatio)
        {
            var waterLiters = content.Where(x => x.element.elementName == "Water").Select(x => x.amount).DefaultIfEmpty(0).Sum() / 1000;
            if (waterLiters > 0)
            {
                foreach (var reactant in reaction.reactans)
                {
                    if (reactant.element.maxWaterRatio != 0 && reactant.element.maxWaterRatio != 1)
                    {
                        var dissolvedReactantAmountLiters = content.Where(x => x.element.elementName == reactant.element.elementName && x.state == ElementAmount.ElementState.Dissolved).Select(x => x.amount).DefaultIfEmpty(0).Sum() / 1000;
                        if (dissolvedReactantAmountLiters / waterLiters > reactant.element.maxWaterRatio) return 0;
                    }
                }
            }
        }
        float min_amount = float.MaxValue;
        foreach (var reactant in reaction.reactans)
        {
            float reactant_amount = GetElementAmount(reactant.element, reactant.state) / reactant.amount;
            min_amount = Mathf.Min(min_amount, reactant_amount);
        }
        return min_amount;
    }

    //Return amount of element in container with the specified properties
    public float GetElementAmount(ElementProperties element, ElementAmount.ElementState state, ElementAmount.IonizationState charge = ElementAmount.IonizationState.neutral)
    {
        foreach (var item in content)
        {
            if (item.element == element && item.state == state && item.charge == charge)
                return item.amount;
        }
        return 0;
    }

    //Remove amount of specific Element
    public void RemoveAmount(ElementAmount amount)
    {
        foreach (var eamount in content)
        {
            if(eamount.EqualsElement(amount) && eamount.amount >= amount.amount)
            {
                eamount.amount -= amount.amount;
                if (eamount.amount == 0)
                    content.Remove(eamount);
                return;
            }
        }
        throw new UnityException("Amount not present " + amount.element.elementName + " " + amount.amount);
    }

    //Remove specified amount from the total ElementAmounts that fullfill the filter.
    protected ElementAmounts RemoveAmount(float ml, Func<ElementAmount, bool> filter)
    {
        var filtered = content.Where(x => filter(x)); // ElementsAmounts to consider
        // calculate fraction to remove from each ElementAmount
        float fraction = ml / filtered.Sum(x => x.ml);
        fraction = Mathf.Min(1, fraction); // fraction can be at most 1
        var removed = filtered.Select(x => x * fraction).ToList();
        foreach (var item in removed)
            RemoveAmount(item);
        OnContentChanged.Invoke();
        return new ElementAmounts(removed);
    }

    //Remove specified amount from total content
    public ElementAmounts RemoveAmount(float ml)
    {
        return RemoveAmount(ml, x => true);
    }

    //Remove specified amount from fluid (dissolved or liquid) content only
    public ElementAmounts RemoveFluidAmount(float ml)
    {
        return RemoveAmount(ml, x => x.state == ElementAmount.ElementState.Dissolved || 
                                        x.state == ElementAmount.ElementState.Liquid);
    }


    //Remove specified amount from liquid content, without dissolved.
    public ElementAmounts RemoveFilteredAmount(float ml)
    {
        return RemoveAmount(ml, x => x.state == ElementAmount.ElementState.Liquid);
    }

    public void AddElementAmount(ElementAmounts elementAmounts)
    {
        foreach (var eamount in elementAmounts.content)
            AddElementAmount(eamount);
    }

    //Add amount of specific Element
    public void AddElementAmount(ElementAmount elementAmount)
    {
        // still has space?
        if(maxStorage < Mass())
            return;

        // Add ElementAmount to existing amount, or add as new element
        var existingElement = content.FirstOrDefault(x => x.EqualsElement(elementAmount));
        if(existingElement == default(ElementAmount))
            content.Add(new ElementAmount(elementAmount));
        else
            existingElement.amount += elementAmount.amount;

        OnContentChanged.Invoke();
    }
}
