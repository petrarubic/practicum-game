using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : USceneController
{
    public MainMenuController() : base(SceneNames.MainMenu) { }

    private int value = 0;

    private TextMeshProUGUI valueLabel;

    public override void SceneWillAppear()
    {
        Debug.Log("Will appear: " + SceneName);
        var obj = GameObject.Find("MainMenu");
        //Debug.Log("Object grabbed: " + obj.name);
    }

    public override void SceneDidLoad()
    {
        // CreateCube();

        //HandleButtons();

        //SetupLabels();

        Debug.Log("MainMenu loaded");

        var world1 = new World1Controller();
        PushSceneController(world1);

    }

    private void SetupLabels()
    {
        valueLabel = GameObject.Find("ValueLabel").GetComponent<TextMeshProUGUI>();

        //guard
        if (valueLabel == null) return;
        valueLabel.text = value.ToString();
    }

    private void CreateCube()
    {
        Debug.Log("Did load : " + SceneName);
        var obj = GameObject.Find("MainMenu");

        //guard
        if (obj == null) return;
        Debug.Log("Object grabbed: " + obj.name);

        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.SetParent(obj.transform);
    }

    private void HandleButtons()
    {
        var newGameButton = GameObject.Find("StartGameButton").GetComponent<Button>();

        //guard
        if (newGameButton == null) return;

        newGameButton.onClick.AddListener(() =>
        {
            //Debug.Log("Click on a button");
            var townVC = new TownController();

            townVC.SetValue(value);

            PushSceneController(townVC);
        });

        var addButton = GameObject.Find("AddButton").GetComponent<Button>();

        //guard
        if (addButton == null) return;

        addButton.onClick.AddListener(() =>
        {
            value++;
            valueLabel.text = value.ToString();
        });


        //buttons for level1 level2 level3
        //ovaj btn treba postojat u main menuu
        //level1Button.onClick.AddListener(() =>
        //{
        //    //Debug.Log("Click on a button");
        //    var world1 = new World1Controller();

        //    PushSceneController(world1);
        //});

    }
}
