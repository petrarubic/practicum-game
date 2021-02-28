using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject portal;
    public bool nextSceneReadyToLoad;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameManager>("GameManager");
            }

            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (portal.GetComponent<PortalEntrance>().playerEnteredPortal)
        {
            nextSceneReadyToLoad = true;
        }
    }
}
