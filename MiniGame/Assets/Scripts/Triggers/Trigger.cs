using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Trigger : MonoBehaviour
{
    public TAction[] actions;

    public bool isTriggered;
    [Tooltip("Tick if you want to call OnTrigger when raycast hits this object")]public bool canTriggerWithInteractionPress = false;
    public UnityEvent onTrigger;

    abstract public void ResetTrigger();
    public void OnTrigger() {
        onTrigger?.Invoke();
        foreach (TAction a in actions) {
            a.OnTrigger(this);
        }
    }
}
