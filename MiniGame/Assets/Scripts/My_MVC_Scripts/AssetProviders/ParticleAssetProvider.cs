using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAssetProvider : AssetBaseProvider
{
    public GameObject particle1;
    public GameObject particle2;
    public GameObject billboard1;

    private static ParticleAssetProvider _instance;
    private static ParticleAssetProvider Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<ParticleAssetProvider>("ParticleAssetProvider");
            }

            return _instance;
        }
    }

    protected override AssetBaseProvider GetInstance()
    {
        return Instance;
    }

    public static GameObject GetParticle(ParticleType particle)
    {
        switch (particle)
        {
            case ParticleType.Cube:
                return Instance.GetObjectFromPool(Instance.particle1);
            case ParticleType.Sphere:
                return Instance.GetObjectFromPool(Instance.particle2);
            case ParticleType.Billboard:
                return Instance.GetObjectFromPool(Instance.billboard1);
        }
        return null;
    }

    public static void Prewarm(ParticleType particleType, int numberOfParticles)
    {
        if (_instance == null)
        {
            _instance = Resources.Load<ParticleAssetProvider>("ParticleAssetProvider");
            //prefill pool (this could be done through reflection)

            _instance.poolObject = new GameObject();
            _instance.poolObject.name = "ParticlePool";
            GameObject.DontDestroyOnLoad(_instance.poolObject);
        }

        Instance.InstatiatePool(PrefabForParticleType(particleType), numberOfParticles);
    }

    private static GameObject PrefabForParticleType(ParticleType particleType)
    {
        switch (particleType)
        {
            case ParticleType.Cube:
                return Instance.particle1;
            case ParticleType.Sphere:
                return Instance.particle2;
            case ParticleType.Billboard:
                return Instance.billboard1;
            default:
                break;
        }

        return null;
    }

}

public enum ParticleType
{
    Cube, Sphere, Billboard
}