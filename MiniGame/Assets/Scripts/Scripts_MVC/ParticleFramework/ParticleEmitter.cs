using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Make Particle pooling system use UParticle not GameObjects
//TODO: Separate all classes to its own files


//emit particles (create them, destroy them... pooling system)
//initialize particles with initial values
//update particles...
//Particle emitter to behave as a particle
//different emition modes - constatnt rate, burst... (rate over distance)
public class UParticleEmitter
{
    public float timeScale = 1f;
    public List<UParticle> particles = new List<UParticle>();

    private bool isPrewarmed;

    public GameObject gameObject;

    //Initialization
    public int initialNumber = 10;
    public int maximumNumber = 1000;
    public Vector3 spawnBlock = new Vector3(5, 5, 5);

    //rate over time
    public float particlesPerSecond = 0f;
    private int milisecondsBetweenParticlesEmitted;

    private float timeBetweenParticlesEmitted;
    private float spawnTimer;

    //
    private DelayedExecutionTicket ticket;

    //burts
    private UParticleBurst[] bursts;
    private float burstCycleLenght;
    private int numberOfBurstCycles;
    private int currentBurstCycle = 0;
    private float lastBurstTimer;
    private int lastBurstIndex = 0;

    //Render/Output
    public ParticleType particleType = ParticleType.Cube;


    public UParticleEmitter()
    {
        gameObject = new GameObject("Particle Emitter");
    }

    public UParticleEmitter(ParticleType type, int initialNumberOfParticlesInstatiated, int maximumNumberOfParticles = -1)
    {
        gameObject = new GameObject("Particle Emitter");

        this.particleType = type;
        this.initialNumber = initialNumberOfParticlesInstatiated;
        this.maximumNumber = maximumNumberOfParticles > 0 ? maximumNumberOfParticles : initialNumberOfParticlesInstatiated;
    }

    public void Prewarm()
    {
        isPrewarmed = true;
        ParticleAssetProvider.Prewarm(particleType, initialNumber);
    }

    public virtual void Activate()
    {
        if (!isPrewarmed)
        {
            isPrewarmed = true;
            ParticleAssetProvider.Prewarm(particleType, 1);
        }
        ExecuteBurst();
    }

    public virtual void Update(float deltaTime)
    {
        var scaledDeltaTime = deltaTime * timeScale;

        spawnTimer += scaledDeltaTime;
        Spawn();
        Burst(scaledDeltaTime);

        for (int i = 0; i < particles.Count; i++)
        {
            UpdateParticle(particles[i], scaledDeltaTime);
            particles[i].Update(scaledDeltaTime);
        }
    }

    public void Burst(float deltaTime)
    {
        //check if our current cycle is less then numberOfCycles
        if (currentBurstCycle >= numberOfBurstCycles) { return; }

        lastBurstTimer += deltaTime;

        //if (lastBurstIndex < bursts.Length)
        //{
        //    var i = lastBurstIndex;
        //    while (i < bursts.Length && bursts[i].timestamp < lastBurstTimer)
        //    {
        //        for (int j = 0; j < bursts[i].numberOfParticles; j++)
        //        {
        //            GenerateParticle();
        //        }
        //        i++;
        //    }
        //    lastBurstIndex = i;
        //}

        //if (lastBurstTimer > burstCycleLenght)
        //{
        //    lastBurstIndex = 0;
        //    lastBurstTimer -= burstCycleLenght;
        //    currentBurstCycle++;
        //}

        if (lastBurstTimer > burstCycleLenght)
        {
            lastBurstTimer -= burstCycleLenght;
            currentBurstCycle++;
            ExecuteBurst();
        }
    }

    public void ExecuteBurst()
    {
        if (bursts == null || bursts.Length == 0) { return; }

        foreach (var burst in bursts)
        {
            //TODO: make timestampInMiliseconds property on burst object
            //TODO: Change Generate particle method: GenerateParticles(int numberOfParticles)
            DelayedExecutionManager.ExecuteActionAfterDelay((int)(burst.timestamp * 1000), () => {
                for (int j = 0; j < burst.numberOfParticles; j++)
                {
                    GenerateParticle();
                }
            });
        }
    }

    public void Spawn()
    {
        if (spawnTimer < timeBetweenParticlesEmitted) { return; }

        var numberOfParticleToSpawn = (int)(spawnTimer / timeBetweenParticlesEmitted);

        //TODO: Optimize this
        spawnTimer -= numberOfParticleToSpawn * timeBetweenParticlesEmitted;

        for (int i = 0; i < numberOfParticleToSpawn; i++)
        {
            GenerateParticle();
        }
    }

    private void GenerateParticle()
    {
        var particleObject = ParticleAssetProvider.GetParticle(particleType);
        var particle = new UParticle(particleObject);

        particles.Add(particle);

        particle.OnDestroyed = (destroyedParticle) => { particles.Remove(destroyedParticle); };

        particle.transform.SetParent(gameObject.transform);
        particle.initialPosition = gameObject.transform.position + GetPositionOffset();

        SetupParticle(particle);

        particle.Activate();
    }

    private Vector3 GetPositionOffset()
    {
        return new Vector3(spawnBlock.x * (float)HelperFunctions.randomizer.NextDouble(),
            spawnBlock.y * (float)HelperFunctions.randomizer.NextDouble(),
            spawnBlock.z * (float)HelperFunctions.randomizer.NextDouble());
    }

    public virtual void SetupParticle(UParticle particle)
    {

    }

    public virtual void UpdateParticle(UParticle particle, float deltaTime)
    {

    }

    public void SetParticlesPerSecond(float particlesPerSecond)
    {
        this.particlesPerSecond = particlesPerSecond;
        if (particlesPerSecond > 0)
        {
            timeBetweenParticlesEmitted = 1f / particlesPerSecond;
            //TODO: remove if not needed
            milisecondsBetweenParticlesEmitted = (int)(1000f / particlesPerSecond);
        }
    }

    public void SetBurst(UParticleBurst[] bursts, float burstCycle, int numberOfCycles = 1) 
    {
        this.bursts = bursts;
        this.burstCycleLenght = burstCycle;
        this.numberOfBurstCycles = numberOfCycles;
    }
}

//game object(basically a view)
//color
//current age....
//applying forces
//two modes - rigid body based; transform based
public class UParticle
{
    public GameObject gameObject;
    public Transform transform;
    public Rigidbody rigidbody;
    public PoolableObject poolableObject;

    public Vector3 initialPosition = Vector3.zero;
    public Quaternion initialRotation = Quaternion.identity;
    public Vector3 initialScale = Vector3.one;

    public Vector3 initialForce = Vector3.zero;

    public float lifespan = 0f;
    private float currentAge = 0f;

    public float currentAgePercentage => currentAge / lifespan;

    public Action<UParticle> OnDestroyed;


    public UParticle(GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.transform = gameObject.transform;
        var rigidbody = gameObject.GetComponent<Rigidbody>();

        if (rigidbody != null)
        {
            this.rigidbody = rigidbody;
        }

        this.poolableObject = gameObject.GetComponent<PoolableObject>();
    }

    public virtual void Activate()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;

        currentAge = 0f;
        rigidbody.velocity = Vector3.zero;

        ApplyForce(initialForce);
    }

    public virtual void ApplyForce(Vector3 force)
    {
        rigidbody.AddForce(force, ForceMode.Impulse);
    }

    public virtual void Update(float deltaTime)
    {
        currentAge += deltaTime;

        if (currentAge > lifespan)
        {
            //kill particle
            OnDestroyed?.Invoke(this);
            poolableObject.ReturnToPool();
        }
    }
}

//hold a list of emitters
//update emitters
//handle time scale
public class UParticleSystem
{
    public List<UParticleEmitter> emitters = new List<UParticleEmitter>();
    public float timeScale = 1f;

    public virtual void Update()
    {
        var scaledDeltaTime = GameTicker.DeltaTime * timeScale;

        foreach (var emitter in emitters)
        {
            emitter.Update(scaledDeltaTime);
        }
    }

    public void Activate()
    {
        foreach (var emitter in emitters)
        {
            emitter.Activate();
        }

        GameTicker.SharedInstance.Update += Update;
    }

    public void AddEmitter(UParticleEmitter emitter)
    {
        emitters.Add(emitter);
    }
}

public struct UParticleBurst
{
    public float timestamp;
    public int numberOfParticles;

    public UParticleBurst(float timestamp, int numberOfParticles)
    {
        this.timestamp = timestamp;
        this.numberOfParticles = numberOfParticles;
    }
}