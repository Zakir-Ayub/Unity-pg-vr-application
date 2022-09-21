using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_Play : MonoBehaviour
{
    // TODO: This script is not usable and will be removed.

    public ParticleSystem Steam;//object type for particle
    private bool PrefabCheck;// Boolean variable for making condition
    private Dissolve_Effect[] getcode;// Reference of all "Dissolve_Effect" scripts attached to the NaOH blocks
    

    void Start()
    {

        getcode = GameObject.FindObjectsOfType<Dissolve_Effect>();// On start function "Dissolve_Effect" scripts will be assigned to getcode for accessing the variables and functions
        Steam.Stop();// Particle stops at first frame after playing scene
        PrefabCheck = true;
    }

    
    void Update()
    {

        foreach (Dissolve_Effect AllScripts in getcode) // Made a loop of foreach to access variable of all "Dissolve_Effect" scripts present in the scene
        {
            if (PrefabCheck == true) //Made a condition so that the particle will be played once and if the boolean condition is not added then the particle will be played infinite amount of time because the condition is in Update function 
            {
                StartCoroutine(Wait());// "Wait" function is called inside Coroutine function. See Script "Dissolve_Effect" for description of Coroutine
                PrefabCheck = false;
            }
        }
        
    }

    private IEnumerator Wait() // See Script "Dissolve_Effect" for description of this function
    {
        yield return new WaitForSeconds(8.0f);// See Script "Dissolve_Effect" for description of this parameter
        Steam.Play();// Particle plays when the fucntion "Wait()" is called.
    }
}
