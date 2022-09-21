using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Keeps track of the ElementAmounts of each particle. 
/// Spaweners can also register directly to save memory.
/// The particles are identified using the randomSeed value.
/// </summary>
public class ParticleRegistry : MonoBehaviour
{
    //Singleton pattern
    public static ParticleRegistry Singleton = null;

    //Dictionary for ElementAmounts of particle, uses random seed as key
    private readonly Dictionary<uint, ElementAmounts> particleAmountDictionary = new Dictionary<uint, ElementAmounts>();
    //Dictionary for the ElementAmounts of spawners that always dispense the same amount.
    private readonly Dictionary<GameObject, ElementAmounts> defaultAmountDictionary = new Dictionary<GameObject, ElementAmounts>();
    //Dictionary for the ElementAmounts of chemical gameobjects
    private readonly Dictionary<GameObject, ElementAmount> chemicalAmountDictionary = new Dictionary<GameObject, ElementAmount>();

    //Used to store particles without reallocation
    private ParticleSystem.Particle[] new_particles = new ParticleSystem.Particle[1000];

    void Awake()
    {
        Assert.IsNull(Singleton, "Singleton ParticleRegistry already exists");
        Singleton = this;
    }

    public ElementAmounts GetParticleAmounts(GameObject source, uint ParticleSeed)
    {
        if (defaultAmountDictionary.ContainsKey(source))
            return defaultAmountDictionary[source];
        return particleAmountDictionary[ParticleSeed];
    }

    public ElementAmount GetChemicalAmount(GameObject chemical)
    {
        if (chemicalAmountDictionary.TryGetValue(chemical, out ElementAmount amount))
            return amount;
        return null;
    }

    public void RegisterChemical(GameObject chemical, ElementAmount amount)
    {
        chemicalAmountDictionary[chemical] = amount;
    }

    public void RegisterDefaultAmount(GameObject source, ElementAmount amount)
    {
        defaultAmountDictionary[source] = new ElementAmounts(amount);
    }

    public void RegisterNewParticles(ParticleSystem ps, ElementAmounts eamounts)
    {
        var size = ps.particleCount;
        if (size > new_particles.Length) // only new allocate if too small
            new_particles = new ParticleSystem.Particle[size];

        size = ps.GetParticles(new_particles, size); // Get particle data from GPU
        for (int i = 0; i < size; i++) 
        {
            var p = new_particles[i]; // iterate over all particles of the PS
            var key = p.randomSeed;
            if (!particleAmountDictionary.ContainsKey(key)) // if not yet registered
            {
                particleAmountDictionary[key] = eamounts; // remember
            }
        }
    }

    public void UnregisterParticle(List<ParticleSystem.Particle> p)
    {
        foreach (var particle in p)
            particleAmountDictionary.Remove(particle.randomSeed);
    }

    public void UnregisterChemical(GameObject g)
    {
        chemicalAmountDictionary.Remove(g);
    }

    //Not currently used
    public void RegisterNewParticles(ParticleSystem ps, ElementContainer container, float mlPerParticle)
    {
        var size = ps.particleCount;
        if(size > new_particles.Length) // Only create new array if too small
            new_particles = new ParticleSystem.Particle[size];

        size = ps.GetParticles(new_particles, size);
        for(int i = 0; i < size; i++)
        {
            var p = new_particles[i];
            var key = p.randomSeed;
            if (!particleAmountDictionary.ContainsKey(key))
            {
                particleAmountDictionary[key] = container.RemoveAmount(mlPerParticle);

            }
        }           
    }

    public int GetRegisteredParticles()
    {
        return particleAmountDictionary.Count;
    }

    public int GetRegisteredDefaults()
    {
        return defaultAmountDictionary.Count;
    }

}
