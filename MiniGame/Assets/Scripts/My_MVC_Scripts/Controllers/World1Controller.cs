using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World1Controller : USceneController
{
    public World1Controller() : base(SceneNames.World1) { }

    // Serves as a start method
    public override void SceneDidLoad()
    {
        //var sphere1 = AssetProvider.GameObjectForType(GameAsset.Sphere1);
        //GameObject.Instantiate(sphere1);
        Debug.Log("World1 loaded");
        // do this in every world to load UIScene aditively
        var UIScene = new UIController();
        AddChildSceneController(UIScene);
    }

}
