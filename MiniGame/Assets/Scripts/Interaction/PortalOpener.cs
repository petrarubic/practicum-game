using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalOpener : Trigger
{
    [SerializeField]
    GameObject portalSphere;

    [SerializeField]
    GameObject spherePosition;

    public override void ResetTrigger()
    {
        
    }

    public void SpawnPortalSphere()
    {
        Instantiate(portalSphere, spherePosition.transform);
    }
}
