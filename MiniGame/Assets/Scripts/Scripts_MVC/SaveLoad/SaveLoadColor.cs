using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadColor : SaveLoad
{
    private new Renderer renderer;

    private struct OverrideParams
    {
        public static string color = "color";
    }

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    public override void ApplyData(Dictionary<string, object> data)
    {
        renderer.material.color = ((GenericArrayWrapper)data[OverrideParams.color]).ToColor();
        base.ApplyData(data);
    }

    public override Dictionary<string, object> GetData()
    {
        var data = base.GetData();
        data.Add(OverrideParams.color, GenericArrayWrapper.InitFromColor(renderer.material.color));

        return data;
    }
}
