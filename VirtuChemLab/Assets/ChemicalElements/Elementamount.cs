
using UnityEngine.Assertions;
/// <summary>
/// ElementAmount is the class that is used to model an occurence of a chemical.
/// It has a specific state, charge, element and amount in gramm.
/// </summary>
[System.Serializable]
public class ElementAmount
{
    public enum ElementState
    { 
        Solid,
        Dissolved,
        Liquid,
        Gas
    }

    // for easy printing
    public string ChargeAsChar()
    {
        return charge switch
        {
            IonizationState.cation => "+",
            IonizationState.anion => "-",
            _ => "",
        };
    }

    public enum IonizationState
    {
        neutral,
        cation,
        anion
    }

    private void CheckAsserts()
    {
        Assert.IsTrue(amount > 0, "Amount Empty");
        Assert.IsTrue(element.density > 0, "Density 0");
        //Add checks here to validate assumtions about Element amounts.
    }

    public override string ToString()
    {
        return string.Format("{0}{3} {1:.##}g {2}", element.elementName, amount, state.ToString(), ChargeAsChar());
    }
    public ElementAmount(ElementProperties element, ElementState state, float amount, IonizationState charge = IonizationState.neutral)
    {
        this.element = element;
        this.state = state;
        this.amount = amount;
        this.charge = charge;
        CheckAsserts();
    }

    public ElementAmount(ElementAmount elementAmount)
    {
        state = elementAmount.state;
        amount = elementAmount.amount;
        element = elementAmount.element;
        charge = elementAmount.charge;
        CheckAsserts();
    }

    public bool EqualsElement(ElementAmount amount)
    {
        return amount.element == element && amount.state == state && amount.charge == charge;
    }

    // allow multiplication by float
    public static ElementAmount operator *(ElementAmount amount, float multiplier)
    {
        return new ElementAmount(amount.element, amount.state, amount.amount * multiplier, amount.charge);
    }

    public ElementState state; //the state the element is in
    public float amount; //the amount in grams
    public float ml => amount / element.density;
    public ElementProperties element; // the element
    public IonizationState charge;
}
