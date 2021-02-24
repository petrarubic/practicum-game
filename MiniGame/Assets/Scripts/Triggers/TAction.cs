using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class TAction : MonoBehaviour
{
    protected Trigger lastTrigger;
    abstract public void OnTrigger(Trigger newTrigger);
}
