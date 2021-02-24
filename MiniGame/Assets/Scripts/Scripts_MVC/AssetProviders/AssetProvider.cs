using System.Collections;
using UnityEngine;

public class AssetProvider : AssetBaseProvider
{
    public GameObject sphere1, sphere2, sphere3;


    public GameObject bullet;
    public int bulletPoolSize;

    [Header("Enemies")]
    public int defaultPoolSize = 10;
    public GameObject enemy1;
    public GameObject enemy2;

    [Header("LoadSave test")]
    public GameObject box;

    private static AssetProvider _instance;
    public static AssetProvider Instance
    {
        get {
            if (_instance == null) {
                _instance = Resources.Load<AssetProvider>("AssetProvider");
            }

            return _instance;
        }
    }

    protected override AssetBaseProvider GetInstance()
    {
        return Instance;
    }

    public static GameObject GetAsset(GameAsset asset)
    {
        return Instance.GetObjectFromPool(GameObjectForType(asset));
    }

    public static void Prewarm()
    {
        if (_instance == null)
        {
            _instance = Resources.Load<AssetProvider>("AssetProvider");
            //prefill pool (this could be done through reflection)

            _instance.poolObject = new GameObject();
            _instance.poolObject.name = "Pool";
            GameObject.DontDestroyOnLoad(_instance.poolObject);

            //bullet pool
            Instance.InstatiatePool(_instance.bullet, _instance.bulletPoolSize);

            Instance.InstatiatePool(_instance.enemy1, _instance.defaultPoolSize);
            Instance.InstatiatePool(_instance.enemy2, _instance.defaultPoolSize);

            //load save
            Instance.InstatiatePool(_instance.box, _instance.defaultPoolSize);
        }
    }

    public PoolableObject RegisterObjectAsPoolable(GameObject gameObject, GameAsset type) 
    {
        var poolable = gameObject.GetComponent<PoolableObject>();
        if (poolable == null)
        {
            poolable = gameObject.AddComponent<PoolableObject>();
        }

        poolable.SetPool(GameObjectForType(type), Instance);

        return poolable;
    }

    public static GameObject GameObjectForType(GameAsset type)
    {
        switch (type)
        {
            case GameAsset.Bullet:
                return Instance.bullet;
            case GameAsset.Footman:
                return Instance.enemy1;
            case GameAsset.Archer:
                return Instance.enemy2;
            case GameAsset.Box:
                return Instance.box;
            case GameAsset.Sphere1:
                return Instance.sphere1;
            case GameAsset.Sphere2:
                return Instance.sphere2;
            case GameAsset.Sphere3:
                return Instance.sphere3;
            default:
                return null;
        }
    }
}

public enum GameAsset
{
    Bullet, Footman, Archer, Box, Sphere1, Sphere2, Sphere3
}

