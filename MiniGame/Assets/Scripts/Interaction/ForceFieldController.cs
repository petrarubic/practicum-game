using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldController : TAction
{
    public override void OnTrigger(Trigger newTrigger)
    {
        Destroy(this.gameObject);
    }
}
