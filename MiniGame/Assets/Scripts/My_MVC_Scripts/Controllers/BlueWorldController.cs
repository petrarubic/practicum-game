using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueWorldController : USceneController
{
    public BlueWorldController() : base(SceneNames.World1) { }

    // Serves as a start method
    public override void SceneDidLoad()
    {
        Player.Instantiate(AssetProvider.GameObjectForType(GameAsset.Player));
        Player.instance.transform.position = new Vector3(8f, 1f, -17f);
        //var playerPosition = AssetProvider.GameObjectForType(GameAsset.PlayerStartingPositionBlue);
        //GameObject.Instantiate(player);
        //player.transform.position = new Vector3(8f, 1f, -17f);
        VHS.FirstPersonController.instance = new VHS.FirstPersonController();
        VHS.FirstPersonController.instance.TeleportPlayer(new Vector3(8f, 1f, -17f));
        


        if (GameManager.Instance.nextSceneReadyToLoad)
        {
            UNavigationController.RemoveViewController();
            var RedScene = new RedWorldController();
            UNavigationController.PresentViewController(RedScene);
        }
        //GameObject.Instantiate(sphere1);
        Debug.Log("World1 loaded");
        // do this in every world to load UIScene aditively
        var UIScene = new UIController();
        AddChildSceneController(UIScene);
    }
}
