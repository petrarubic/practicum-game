using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsController : TAction
{

    public override void OnTrigger(Trigger newTrigger)
    {
        var rend = this.gameObject.GetComponent<MeshRenderer>();
        var col = this.gameObject.GetComponent<Collider>();
        col.enabled = false;
        StartCoroutine(Dissolve(rend));
    }

    private IEnumerator Dissolve(Renderer obj)
    {
        int i = 0;
        while (i < 1000)
        {
            i += 2;
            obj.material.SetFloat("Vector1_DAF21F0F", i * 0.001f);
            yield return null;
        }
        yield return null;
    }
}
