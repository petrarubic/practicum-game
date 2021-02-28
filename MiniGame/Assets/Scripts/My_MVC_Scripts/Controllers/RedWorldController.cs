using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedWorldController : USceneController
{
    public RedWorldController() : base(SceneNames.World2) { }

    // Serves as a start method
    public override void SceneDidLoad()
    {
        Player.Instantiate(AssetProvider.GameObjectForType(GameAsset.Player));
        VHS.FirstPersonController.instance.TeleportPlayer(new Vector3(8f, 1f, -16.3f));
        //var sphere1 = AssetProvider.GameObjectForType(GameAsset.Sphere1);
        //GameObject.Instantiate(
        //GameObject.Instantiate(sphere1);
        //Debug.Log("World2 loaded");
        // do this in every world to load UIScene aditively
        var UIScene = new UIController();
        AddChildSceneController(UIScene);
    }
}
