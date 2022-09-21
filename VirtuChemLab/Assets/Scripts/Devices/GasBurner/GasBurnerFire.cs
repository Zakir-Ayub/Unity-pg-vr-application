using Reactions;
using UnityEngine;
using Random = System.Random;

namespace Devices
{
    /// <summary>
    /// Controls the particle system and sets
    /// values for the particles depending on
    /// the air and gas amount.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class GasBurnerFire : MonoBehaviour, ITemperatureSource
    {
        private float gas = 0f;
        public float GasAmount
        {
            set => gas = value;
        }

        private float air = 0f;
        public float AirAmount
        {
            set => air = value;
        }

        [SerializeField]
        private float fireSpeed = 1f;
        
        [SerializeField]
        private float maxHorizontalVelocity = 0.01f;
        
        [SerializeField]
        private Gradient colorRange;
        
        private new ParticleSystem particleSystem;

        private ParticleSystem.MainModule mainModule;
        private ParticleSystem.EmissionModule emissionModule;
        private ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule;
        private ParticleSystem.CollisionModule collisionModule;
        
        private ParticleSystem.MinMaxCurve fireVelocityX, fireVelocityZ;

        private Random random;

        private void Start()
        {
            random = new Random();

            particleSystem = GetComponent<ParticleSystem>();
            mainModule = particleSystem.main;
            emissionModule = particleSystem.emission;
            velocityOverLifetimeModule = particleSystem.velocityOverLifetime;
            collisionModule = particleSystem.collision;

            fireVelocityX = new ParticleSystem.MinMaxCurve(-0.01f, 0.01f)
            {
                mode = ParticleSystemCurveMode.TwoConstants
            };
            fireVelocityZ = new ParticleSystem.MinMaxCurve(-0.01f, 0.01f)
            {
                mode = ParticleSystemCurveMode.TwoConstants
            };

            velocityOverLifetimeModule.x = fireVelocityX;
            velocityOverLifetimeModule.z = fireVelocityZ;
        }

        private void Update()
        {
            emissionModule.rateOverTime = GetNumFireParticlesToSpawn();
            mainModule.startColor = colorRange.Evaluate(air);
            mainModule.startSpeed = GetFireSpeed();
            mainModule.startSize = GetFireSize();
            
            float horizontalVelocity = GetHorizontalVelocity();
            fireVelocityX.constantMin = -horizontalVelocity;
            fireVelocityX.constantMax = horizontalVelocity;

            fireVelocityZ.constantMin = -horizontalVelocity;
            fireVelocityZ.constantMax = horizontalVelocity;
            
            velocityOverLifetimeModule.x = fireVelocityX;
            velocityOverLifetimeModule.z = fireVelocityZ;

            collisionModule.maxKillSpeed = GetMaxKillSpeed();
        }
        
        private float GetFireSpeed()
        {
            float multiplier = fireSpeed;
            float lifetime = Mathf.Lerp(0.01f, 0.3f, gas);
            float randomOffset = (float) random.NextDouble() * Mathf.Lerp(0.05f, 0.2f, gas);
            
            return (lifetime + randomOffset) * multiplier;
        }

        private float GetFireSize()
        {
            return 0.05f * Mathf.Lerp(0.4f, 1f, gas);
        }

        private int GetNumFireParticlesToSpawn()
        {
            return gas >= 0.01f ? 100 : 0;
        }

        private float GetHorizontalVelocity()
        {
            return maxHorizontalVelocity * gas;
        }

        private float GetMaxKillSpeed()
        {
            return IsBlueFlame() ? 0f : 1000f;
        }

        private bool IsBlueFlame()
        {
            return air > 0.5f;
        }

        public float GetTemperature()
        {
            // TODO: calculate actual temperature of flame
            return 900f + (500f * air);
        }
    }
}