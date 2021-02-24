using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TownController : USceneController
{
    public TownController() : base(SceneNames.Town) { }

    private TextMeshProUGUI valueLabel;

    private int value;

    public override void SceneDidLoad()
    {
        valueLabel = GameObject.Find("ValueLabel").GetComponent<TextMeshProUGUI>();

        //guard
        if (valueLabel == null) return;

        valueLabel.text = value.ToString();

        var backButton = GameObject.Find("MainMenuButton").GetComponent<Button>();
        //guard
        if (backButton == null) return;

        backButton.onClick.AddListener(() =>
        {
            PopToParentSceneController();
        });


        var inGameMenuButton = GameObject.Find("InGameMenuButton").GetComponent<Button>();

        //should guard

        inGameMenuButton.onClick.AddListener(() => {
            var inGameMenuVC = new InGameMenuController();
            AddChildSceneController(inGameMenuVC);
        });
    }

    public void SetValue(int newValue)
    {
        value = newValue;
    }
}
