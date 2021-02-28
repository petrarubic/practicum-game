using UnityEngine;
using UnityEditor;

public class TeleportPlayer : EditorWindow {
    [MenuItem("Custom Tools/Teleport Player")]
    static void TeleportPlayerFunc() {
        GameObject player = GameObject.FindGameObjectWithTag(ReadonlyStrings.player) != null ? GameObject.FindGameObjectWithTag(ReadonlyStrings.player).gameObject : null;

        Camera sceneCam = SceneView.GetAllSceneCameras()[0];
        Vector3 spawnPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 5f));
        Ray ray = sceneCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit)) {
            spawnPos = raycastHit.point;
        }
        spawnPos.x = Mathf.FloorToInt(spawnPos.x + 0.49f);
        spawnPos.z = Mathf.FloorToInt(spawnPos.z + 0.51f);


        if (player != null) {
            player.transform.position = spawnPos;
            Selection.activeGameObject = player;
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

    [MenuItem("Custom Tools/Teleport Object")]
    static void TeleportObjectFunc() {
        //GameObject player = GameObject.FindGameObjectWithTag(ReadonlyStrings.player) != null ? GameObject.FindGameObjectWithTag(ReadonlyStrings.player).gameObject : null;
        GameObject obj = Selection.activeGameObject;

        Camera sceneCam = SceneView.GetAllSceneCameras()[0];
        Vector3 spawnPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 5f));
        Ray ray = sceneCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit)) {
            spawnPos = raycastHit.point;
        }

        if (obj != null) {
            obj.transform.position = spawnPos;
            Selection.activeGameObject = obj;
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
}
