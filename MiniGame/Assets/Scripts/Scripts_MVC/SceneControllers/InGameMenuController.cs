using UnityEngine;
using UnityEngine.UI;

public class InGameMenuController : USceneController
{
    public InGameMenuController() : base(SceneNames.InGameMenu) { }

    public override void SceneDidLoad()
    {
        var resumeButton = GameObject.Find("ResumeButton").GetComponent<Button>();


        var mainMenuButton = GameObject.Find("MenuButton").GetComponent<Button>();

        Debug.Log("Button: " + mainMenuButton.name);

        //should guard

        mainMenuButton.onClick.AddListener(() =>
        {
            UNavigationController.PopToRootViewController();
        });

        resumeButton.onClick.AddListener(() =>
        {
            RemoveFromParentSceneController();
        });
    }
}