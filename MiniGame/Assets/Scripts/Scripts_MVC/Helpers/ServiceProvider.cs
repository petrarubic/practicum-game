using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class ServiceProvider : MonoBehaviour
{
    public static readonly ISerializationManager SerializationManager = new BinarySerializationManager();

    private static BinaryFormatter binaryFormatter;
    public static BinaryFormatter BinaryFormatter
    {
        get
        {
            if (binaryFormatter == null)
            {
                binaryFormatter = new BinaryFormatter();

                SurrogateSelector surrogateSelector = new SurrogateSelector();

                //add some surrogates
                var contex = new StreamingContext(StreamingContextStates.All);

                //vector3
                var vector3Surrogate = new Vector3Surrogate();
                surrogateSelector.AddSurrogate(typeof(Vector3), contex, vector3Surrogate);

                //quaternion
                var quaternionSurrogate = new QuaternionSurrogate();
                surrogateSelector.AddSurrogate(typeof(Quaternion), contex, quaternionSurrogate);

                //attach them to formatter
                binaryFormatter.SurrogateSelector = surrogateSelector;
            }

            return binaryFormatter;
        }
    }
}