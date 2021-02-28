using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEntrance : MonoBehaviour
{
    public bool playerEnteredPortal;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player entered portal");
            playerEnteredPortal = true;
            return;
        }
    }
}
