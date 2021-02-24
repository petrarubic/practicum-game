using UnityEngine;
using System;

public class MVCInputManager : MonoBehaviour
{
    public static event Action OnPressedFire;
    public static event Action<Vector3> OnMovement;

    private void Update()
    {
        //fire
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            OnPressedFire?.Invoke();
        }

        //move
        var inputX = Input.GetAxis(InputStrings.axisX);
        var inputY = Input.GetAxis(InputStrings.axixY);

        OnMovement?.Invoke(new Vector3(inputX, 0, inputY).normalized);
    }

    public static void Activate()
    {
        var manager = GameObject.Instantiate(new GameObject());

        manager.AddComponent<MVCInputManager>();
        manager.name = "InputManager";

        GameObject.DontDestroyOnLoad(manager);
    }
}

public struct InputStrings
{
    public static string axisX = "Horizontal";
    public static string axixY = "Vertical";
}
