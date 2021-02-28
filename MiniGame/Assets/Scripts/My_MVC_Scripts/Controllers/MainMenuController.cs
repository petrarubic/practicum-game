using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : USceneController
{
    public MainMenuController() : base(SceneNames.MainMenu) { }

    public override void SceneWillAppear()
    {
        Debug.Log("Will appear: " + SceneName);
        var obj = GameObject.Find("MainMenu");
        //Debug.Log("Object grabbed: " + obj.name);
    }

    public override void SceneDidLoad()
    {
        MainMenuCanvasGenerated.Init();
        WorldSelectCanvasGenerated.Init();

        SetButtons();

        Debug.Log("MainMenu loaded");
    }

    //private void SetupLabels()
    //{
    //    valueLabel = GameObject.Find("ValueLabel").GetComponent<TextMeshProUGUI>();

    //    //guard
    //    if (valueLabel == null) return;
    //    valueLabel.text = value.ToString();
    //}

    //private void CreateCube()
    //{
    //    Debug.Log("Did load : " + SceneName);
    //    var obj = GameObject.Find("MainMenu");

    //    //guard
    //    if (obj == null) return;
    //    Debug.Log("Object grabbed: " + obj.name);

    //    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //    cube.transform.SetParent(obj.transform);
    //}

    private void SetButtons()
    {
        MainMenuCanvasGenerated.MainMenuPanel.SetActive(true);
        WorldSelectCanvasGenerated.WorldSelectPanel.SetActive(false);

        MainMenuCanvasGenerated.StartButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            WorldSelectCanvasGenerated.WorldSelectPanel.SetActive(true);
        });

        MainMenuCanvasGenerated.QuitButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            Application.Quit();
        });

        WorldSelectCanvasGenerated.BlueWorldButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            var blueWorld = new BlueWorldController();
            PushSceneController(blueWorld);
        });

        WorldSelectCanvasGenerated.RedWorldButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            var redWorld = new RedWorldController();
            PushSceneController(redWorld);
        });

        WorldSelectCanvasGenerated.GreenWorldButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            var greenWorld = new GreenWorldController();
            PushSceneController(greenWorld);
        });
    }
}
