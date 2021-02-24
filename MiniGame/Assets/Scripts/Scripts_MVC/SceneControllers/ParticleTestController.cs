using UnityEngine;

public class ParticleTestController : USceneController
{
    public ParticleTestController() : base(SceneNames.ParticleTest)
    {

    }

    public override void SceneDidLoad()
    {
        var particleSystem = new MyFirstParticleSystem();
        particleSystem.Activate();
    }
}

public class MyFirstParticleSystem : UParticleSystem
{
    public MyFirstParticleSystem()
    {
        var emitter = new MyFirstParticleEmitter();
        AddEmitter(emitter);

        var emitter2 = new MySecondParticleEmitter();
        AddEmitter(emitter2);

        emitter2.gameObject.transform.position += HelperFunctions.RandomVector(10, 10, 10);
    }
}

public class MyFirstParticleEmitter : UParticleEmitter
{
    private float startSize = 1;
    private float endSize = 0.05f;

    public MyFirstParticleEmitter()
    {
        initialNumber = 300;
        particleType = ParticleType.Cube;
        SetParticlesPerSecond(100);
        Prewarm();
    }

    public override void SetupParticle(UParticle particle)
    {
        particle.lifespan = 3f;
        particle.initialForce = HelperFunctions.RandomVector(5, 15, 5);
    }

    public override void UpdateParticle(UParticle particle, float deltaTime)
    {
        particle.ApplyForce(new Vector3(0, 10 * deltaTime, 0));
        float size = Mathf.Lerp(startSize, endSize, particle.currentAgePercentage);
        particle.transform.localScale = Vector3.one * size;
    }
}

public class MySecondParticleEmitter : UParticleEmitter
{
    private float startSize = 2;
    private float endSize = 0.05f;

    public MySecondParticleEmitter()
    {
        initialNumber = 500;
        particleType = ParticleType.Sphere;
        SetParticlesPerSecond(50);

        UParticleBurst[] bursts = new[] {new UParticleBurst(1, 100), new UParticleBurst(2, 150) };
        SetBurst(bursts, 4, 3);

        Prewarm();
    }

    public override void SetupParticle(UParticle particle)
    {
        particle.lifespan = 3f;
        particle.initialForce = HelperFunctions.RandomVector(5, 15, 5);
        //particle.gameObject.GetComponent<Renderer>().material.color = Color.red;
    }

    public override void UpdateParticle(UParticle particle, float deltaTime)
    {
        particle.ApplyForce(new Vector3(0, 10 * deltaTime, 0));
        float size = Mathf.Lerp(startSize, endSize, particle.currentAgePercentage);
        particle.transform.localScale = Vector3.one * size;
    }
}