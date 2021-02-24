using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utils : MonoBehaviour {
    /// <summary>
    /// Used for calculating step for lerping in corutines
    /// </summary>
    /// <param name="a">from</param>
    /// <param name="b">to</param>
    /// <param name="value">step</param>
    /// <returns></returns>
    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value) {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Mathf.Clamp01(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB));
    }
    public static float InverseLerpFloat(float a, float b, float value) {
        return Mathf.InverseLerp(a, b, value);
    }
    /* EXAMPLE OF USE
    TestUtils script
    */

    public static void PingPongObj(GameObject obj, float scale = 2f, float speed = 3f) {
        obj.transform.localScale = Vector3.one + (1f - Mathf.PingPong(speed * Time.time, scale)) * 0.06f * Vector3.one;
    }
    public static Vector3 CalculateWorldPositionFromCamera(Vector2 screenPosition, Camera camera) {
        Ray ray = camera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            return hit.point;
        }

        return Vector3.zero;
    }

    public static void ChangeCursorState(bool turnOn) {
        Cursor.visible = turnOn;
        Cursor.lockState = turnOn ? CursorLockMode.None : CursorLockMode.Locked;
    }

    /// <summary>
    /// Returns the vector3 that contains the mouse position on the given zDistance
    /// </summary>
    /// <param name="zDistance"></param>
    /// <returns></returns>
    public static Vector3 GetMousePosition(float zDistance = 0) {
        return new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance);
    }

    public static LayerMask GetLayerFromName(string layerName) {
        return LayerMask.NameToLayer(layerName);
    }


    public static IEnumerator FadeOut(float startAlpha, float endAlpha, float fadeDuration) {
        var startTime = Time.time;
        var endTime = Time.time + fadeDuration;
        var elapsedTime = 0f;

        while (Time.time <= endTime) {
            elapsedTime = Time.time - startTime;
            var percentage = 1 / (fadeDuration / elapsedTime);
            if (startAlpha > endAlpha)
            {
                //PlayerCanvasGenerated.Fade_IMG.color = new Color(0, 0, 0, startAlpha - percentage);
            } else
              {
                //PlayerCanvasGenerated.Fade_IMG.color = new Color(0, 0, 0, startAlpha + percentage);
            }

            yield return new WaitForEndOfFrame();
        }

        //PlayerCanvasGenerated.Fade_IMG.color = new Color(0, 0, 0, endAlpha);
    }

    public static IEnumerator FadeIn(float startAlpha, float endAlpha, float fadeDuration) {
        var startTime = Time.time;
        var endTime = Time.time + fadeDuration;
        var elapsedTime = 0f;

        while (Time.time <= endTime) {
            elapsedTime = Time.time - startTime;
            var percentage = 1 / (fadeDuration / elapsedTime);
            if (startAlpha > endAlpha) {
                //PlayerCanvasGenerated.Fade_IMG.color = new Color(0, 0, 0, startAlpha - percentage);
            } else {
                //PlayerCanvasGenerated.Fade_IMG.color = new Color(0, 0, 0, startAlpha + percentage);
            }

            yield return new WaitForEndOfFrame();
        }

        //PlayerCanvasGenerated.Fade_IMG.color = new Color(0, 0, 0, endAlpha);
    }
}
