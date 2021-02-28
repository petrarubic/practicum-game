using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : USceneController
{
    public UIController() : base(SceneNames.UIScene)
    {
    }
    public override void SceneDidLoad()
    {
        //UIGeneratedThis.Init();
        //SetButtons();
        Debug.Log("UISceneController loaded");
    }

    private void SetButtons()
    {
        var pause = GameObject.Find("PauseButton").GetComponent<Button>();
        Debug.Log(pause);
        //var pauseBtn = new UButton(pause);

        //pauseBtn.OnClick(() =>
        //{
        //    Debug.Log("Popping to root");
        //    UNavigationController.PopToRootViewController();
        //});
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pause.onClick.AddListener(() =>
        {
            Debug.Log("Popping to root");
            UNavigationController.PopToRootViewController();
        });



        //UIGeneratedThis.MainMenu.onClick.AddListener(() =>
        //{
        //    UNavigationController.PopToRootViewController();
        //    GameManager.isPaused = !GameManager.isPaused;
        //});
    }
}