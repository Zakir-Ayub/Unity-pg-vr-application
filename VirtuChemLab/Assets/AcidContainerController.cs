using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidContainerController : FluidContainerSystem
{
    protected new void Start()
    {
        base.Start();
        container.maxStorage = 5000;
    }
}
