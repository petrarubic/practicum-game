using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ISaveLoadable
{
    Dictionary<string, object> GetData();
    void ApplyData(Dictionary<string, object> data);
    int ObjectID { get; }
    LoadableGameObject Type { get; }
    void Activate();
    GameObject GameObject { get; }
}

public class SaveLoad : MonoBehaviour, ISaveLoadable
{
    public int ObjectID { get; private set; } = SaveLoadManager.AutogenerateID();

    [SerializeField]
    private LoadableGameObject type;

    public LoadableGameObject Type => type;

    public GameObject GameObject => gameObject;

    private Dictionary<string, object> data = new Dictionary<string, object>();

    private struct Params
    {
        public static string id = "id";
        public static string position = "position";
        public static string rotation = "rotation";
        public static string scale = "scale";
        public static string type = "type";
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        Activate();
    }

    public virtual void ApplyData(Dictionary<string, object> data)
    {
        transform.position = (Vector3)data[Params.position];
        transform.rotation = (Quaternion)data[Params.rotation];
        transform.localScale = (Vector3)data[Params.scale];


        //By using Wrapper class
        //transform.position = ((GenericArrayWrapper)data[Params.position]).ToVector3();
        //transform.rotation = ((GenericArrayWrapper)data[Params.rotation]).ToQuaternion();
        //transform.localScale = ((GenericArrayWrapper)data[Params.scale]).ToVector3();

        type = (LoadableGameObject)data[Params.type];
    }

    public virtual Dictionary<string, object> GetData()
    {
        data.Clear();
        data.Add(Params.id, ObjectID);
        data.Add(Params.position, transform.position);
        data.Add(Params.rotation, transform.rotation);
        data.Add(Params.scale, transform.localScale);


        //By using Wrapper class
        //data.Add(Params.position, GenericArrayWrapper.InitFromVector3(transform.position));
        //data.Add(Params.rotation, GenericArrayWrapper.InitFromQuaternion(transform.rotation));
        //data.Add(Params.scale, GenericArrayWrapper.InitFromVector3(transform.localScale));
        data.Add(Params.type, type);

        return data;
    }

    public void Activate()
    {
        SaveLoadManager.RegisterObject(this);
    }
}

[Serializable]
public struct GenericArrayWrapper
{
    public float[] values;

    //vector3
    public static GenericArrayWrapper InitFromVector3(Vector3 vector3)
    {
        var serializable = new GenericArrayWrapper();

        serializable.values = new float[3];
        serializable.values[0] = vector3.x;
        serializable.values[1] = vector3.y;
        serializable.values[2] = vector3.z;

        return serializable;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(values[0], values[1], values[2]);
    }

    //quaternion
    public static GenericArrayWrapper InitFromQuaternion(Quaternion quaternion)
    {
        var serializable = new GenericArrayWrapper();

        serializable.values = new float[4];
        serializable.values[0] = quaternion.x;
        serializable.values[1] = quaternion.y;
        serializable.values[2] = quaternion.z;
        serializable.values[3] = quaternion.w;

        return serializable;
    }

    public Quaternion ToQuaternion()
    {
        return new Quaternion(values[0], values[1], values[2], values[3]);
    }

    //color
    public static GenericArrayWrapper InitFromColor(Color color)
    {
        var serializable = new GenericArrayWrapper();

        serializable.values = new float[4];
        serializable.values[0] = color.r;
        serializable.values[1] = color.g;
        serializable.values[2] = color.b;
        serializable.values[3] = color.a;

        return serializable;
    }

    public Color ToColor()
    {
        return new Color(values[0], values[1], values[2], values[3]);
    }
}