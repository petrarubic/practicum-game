using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntryPointController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif

        //preloading of music and sound
        //common assets

        //setup singletons
        //AssetProvider.Prewarm();
        //MVCInputManager.Activate();
        //ULocalization.Init();

        //network connections

        //create main controller
        var mainMenuVC = new MainMenuController();
        UNavigationController.SetRootViewController(mainMenuVC);

        var dungeonVC = new DungeonController();
        //UNavigationController.SetRootViewController(dungeonVC);

        var particleTest = new ParticleTestController();
        //UNavigationController.SetRootViewController(particleTest);
        

        var saveLoadVC = new LoadSaveTestController();
        //UNavigationController.SetRootViewController(saveLoadVC);

        var localizationVC = new LocalizationController();
        //UNavigationController.SetRootViewController(localizationVC);
    }
}

public class LocalizationController : USceneController
{
    public LocalizationController() : base(SceneNames.LocalizationTest) { }

    public override void SceneDidLoad()
    {
        var engButton = new UButton("Button1");
        var hrButton = new UButton("Button2");
        var titleLabel = new ULabel("TitleLabel");


        engButton.OnClick(() => ULocalization.SetLanguage(LocalizationLanguage.en));

        hrButton.OnClick(() => ULocalization.SetLanguage(LocalizationLanguage.hr));

        engButton.label.Localize("button1");
        hrButton.label.Localize("button2");
        titleLabel.Localize("title");


    }
}

public class LoadSaveTestController : USceneController
{
    public LoadSaveTestController() : base(SceneNames.SaveLoadTest) { }

    public override void SceneWillAppear()
    {
        base.SceneWillAppear();
        SaveLoadManager.ClearAll();
    }

    public override void SceneDidLoad()
    {
        base.SceneDidLoad();

        DelayedExecutionManager.ExecuteActionAfterDelay(1000, () => {
            Debug.Log("1000 miliseconds");
        });
        var firstTicket =  DelayedExecutionManager.ExecuteActionAfterDelay(1001, () => {
            Debug.Log("1001 miliseconds");
        });

        DelayedExecutionManager.ExecuteActionAfterDelay(500, () => {
            Debug.Log("500 miliseconds");
        });

        DelayedExecutionManager.ExecuteActionAfterDelay(1500, () => {
            Debug.Log("1500 miliseconds");
        });

        DelayedExecutionManager.CancelTicket(firstTicket);


        var quickLoadButton = GameObject.Find("QuickLoadButton").GetComponent<Button>();

        if (quickLoadButton == null) return;

        quickLoadButton.onClick.AddListener(() => {
            Debug.Log("Load!");
            SaveLoadManager.ApplySnapshot();
        });

        var quickSaveButton = new UButton("QuickSaveButton");

        quickSaveButton.OnClick(() => {
            Debug.Log("Save!");
            SaveLoadManager.CreateSnapshot();
        });

        var loadButton = new UButton("LoadButton");

        loadButton.OnClick(() => {
            Debug.Log("Load from harddrive!");
            SaveLoadManager.Load("one.txt");

        });

        var saveButton = new UButton("SaveButton");

        saveButton.OnClick(() => {
            Debug.Log("Save to hard drive!");
            SaveLoadManager.CreateSnapshot();
            SaveLoadManager.Save("one.txt");
        });

        var generateBoxButton = new UButton("GenerateButton");

        generateBoxButton.OnClick(() => {
            //var box = AssetProvider.GetAsset(GameAsset.Box);
            //box.transform.position = HelperFunctions.RandomVector(5, 5, 5);
        });

        //saveButton.SetText("New text");
    }

    public override void SceneWillDisappear()
    {
        base.SceneWillDisappear();

        //cancel out all tickets
    }
}

public enum LoadableGameObject
{
    Box
}

public class LoadableAssetsProvider
{
    //public static GameObject GetLoadableGameObject(LoadableGameObject loadable)
    //{
    //    return AssetProvider.GetAsset(GameAssetTypeFromLoadableObject(loadable));
    //}

    //public static GameAsset GameAssetTypeFromLoadableObject(LoadableGameObject loadable)
    //{
    //    //switch (loadable)
    //    //{
    //    //    case LoadableGameObject.Box:
    //    //        return GameAsset.Box;
    //    //    default:
    //    //        return GameAsset.Archer;
    //    //}
    //}

    //public static ISaveLoadable GenerateLoadableObjectFromSnapshot(Dictionary<string, object> snapshotData)
    //{
    //    var type = (LoadableGameObject)snapshotData["type"];
    //    var newObject = GetLoadableGameObject(type).GetComponent<ISaveLoadable>();
    //    newObject.ApplyData(snapshotData);

    //    return newObject;
    //}
}