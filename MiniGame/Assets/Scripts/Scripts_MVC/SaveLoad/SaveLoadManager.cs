using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveLoadManager
{
    private static ISerializationManager serializer = ServiceProvider.SerializationManager;

    private static readonly string SaveGameDirectory = "/SaveGame/";

    private static List<ISaveLoadable> activeObjects = new List<ISaveLoadable>();
    private static int IDCount = 0;

    private static Dictionary<int, Dictionary<string, object>> snapshot = new Dictionary<int, Dictionary<string, object>>();

    public static int AutogenerateID()
    {
        return IDCount++;
    }

    public static void RegisterObject(ISaveLoadable newObject)
    {
        if (activeObjects.Contains(newObject)) return;

        activeObjects.Add(newObject);
    }

    public static void DeregisterObject(ISaveLoadable objectToRemove)
    {
        activeObjects.Remove(objectToRemove);
    }

    public static void ClearAll()
    {
        activeObjects.Clear();
    }

    public static void CreateSnapshot()
    {
        snapshot.Clear();

        foreach (var element in activeObjects)
        {
            snapshot.Add(element.ObjectID, element.GetData());
        }
    }

    public static void ApplySnapshot()
    {
        //go through activeObjects
        //if it doesn't exist in snapshot - destroy it (back to pool)
        //if exists - apply data
        //on the end, if any objects left in snapshot - create new object and apply data to it

        //perhaps cache this for extra performance
        var snapshotCopy = new Dictionary<int, Dictionary<string, object>>(snapshot);
        var toRemove = new List<ISaveLoadable>();

        foreach (var element in activeObjects)
        {
            Dictionary<string, object> savedData;
            if (snapshot.TryGetValue(element.ObjectID, out savedData))
            {
                element.ApplyData(savedData);
                snapshotCopy.Remove(element.ObjectID);
            }
            else
            {
                toRemove.Add(element);
            }
        }

        //create elements missing (snapshotCopy)

        foreach (var snapshot in snapshotCopy.Values)
        {
            //var newElement = LoadableAssetsProvider.GenerateLoadableObjectFromSnapshot(snapshot);
            //newElement.Activate();
        }

        //remove objects created after taking a snapshot (toRemove)
        foreach (var element in toRemove)
        {
            activeObjects.Remove(element);
            element.GameObject.GetComponent<PoolableObject>().ReturnToPool();
        }
    }

    //persistance
    public static void Load(string filename)
    {
        var data = serializer.Deserialize(FullPath(filename));
        CreateSnapshotFromSaveData(data);
        ApplySnapshot();
    }

    private static void CreateSnapshotFromSaveData(SaveData data)
    {
        //create snapshot
        snapshot = data.snapshot;
    }

    public static void Save(string filename)
    {
        var data = GenerateSaveDataFromSnapshot();
        serializer.Serialize(data, FullPath(filename));
    }

    private static SaveData GenerateSaveDataFromSnapshot()
    {
        var data = new SaveData
        {
            snapshot = snapshot
        };

        return data;
    }

    private static string FullPath(string filename)
    {
        var path = Application.dataPath + SaveGameDirectory;

        //move this to the game init controller
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path + filename;
    }
}
