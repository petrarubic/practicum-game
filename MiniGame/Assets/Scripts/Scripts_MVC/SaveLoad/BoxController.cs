using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public PoolableObject poolable;
    public ISaveLoadable saveLoadable;

    private void Start()
    {
        saveLoadable = GetComponent<ISaveLoadable>();
        //register this as poolable
        //var gameAssetType = LoadableAssetsProvider.GameAssetTypeFromLoadableObject(saveLoadable.Type);
        //poolable = AssetProvider.Instance.RegisterObjectAsPoolable(gameObject, gameAssetType);
    }

    private void OnMouseUp()
    {
        SaveLoadManager.DeregisterObject(saveLoadable);
        poolable.ReturnToPool();
    }
}
