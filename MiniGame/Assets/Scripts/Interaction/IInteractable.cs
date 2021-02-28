using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Hover();
    void Interact(int param = 0);
    void StopInteraction();
}
