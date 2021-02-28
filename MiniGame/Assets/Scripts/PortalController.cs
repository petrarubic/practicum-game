using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PortalController : TAction
{
    private void Awake()
    {
        this.GetComponent<VisualEffect>().Stop();
    }

    public override void OnTrigger(Trigger newTrigger)
    {
        this.GetComponent<VisualEffect>().Play();
    }
}
