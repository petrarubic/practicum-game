using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public struct SaveData
{
    public Dictionary<int, Dictionary<string, object>> snapshot;
}

public interface ISerializationManager
{
    void Serialize(SaveData data, string filePath);
    SaveData Deserialize(string filePath);
}


public class MockSerializationManager : ISerializationManager
{
    public SaveData Deserialize(string filename)
    {
        //do the deserialization
        return new SaveData();
    }

    public void Serialize(SaveData data, string filename)
    {
       
    }
}

public class BinarySerializationManager : ISerializationManager
{
    private BinaryFormatter binaryFormatter = ServiceProvider.BinaryFormatter;

    public SaveData Deserialize(string filePath)
    {
        var fileInfo = new FileInfo(filePath);

        using (var binaryFile = fileInfo.OpenRead())
        {
            var data = (SaveData)binaryFormatter.Deserialize(binaryFile);

            return data;
        }
    }

    public void Serialize(SaveData data, string filePath)
    {
        var fileInfo = new FileInfo(filePath);

        using (var binaryFile = fileInfo.Create())
        {
            binaryFormatter.Serialize(binaryFile, data);
            binaryFile.Flush();
        }
    }
}
