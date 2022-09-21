using Unity.Netcode;
using UnityEngine;
using System;
using System.Collections.Generic;

public class FunnelController : FluidContainerSystem
{
    // sets the emission cooldown for water drops
    private const float withoutFilterCooldown = 0.1f;
    private const float withFilterCooldown = 2f;

    // the variable signals if a filer paper is placed upon the funnel or not
    public bool isFilterPlaced;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        container.maxStorage = 20;
        // initially, there is no filter placed upon funnel
        isFilterPlaced = false;
    }

    new private void FixedUpdate()
    {
        if (ShouldEmit()) // if emission should happen
        {
            if (!isFilterPlaced)
            {
                if (Time.time > lastEmit + withoutFilterCooldown)
                {
                    EmitParticle();
                }
            }
            else
            {
                if (Time.time > lastEmit + withFilterCooldown)
                {
                    EmitFilteredParticle();
                }
            }
        }
        UpdateWaterShader();
    }

    protected override bool ShouldEmit()
    {
        if (container.FluidMass() > 0) // if there is any fluid left in the funnel
        {
            return true;
        }
        return false;
    }
}
