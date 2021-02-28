using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class UIGenerator : Editor
{
    public static GameObject selectedCanvas;
    private static string canvasName;

    [MenuItem(itemName: "Custom Tools/Generate UI")]
    public static void GenerateMainMenu()
    {
        selectedCanvas = Selection.activeGameObject;
        if (selectedCanvas.GetComponent<Canvas>() == null)
        {
            Debug.LogWarning("You need to select object of type [Canvas]");
        }
        else
        {
            canvasName = selectedCanvas.name;

            var filePath = Application.dataPath + "/Scripts/GeneratedUI/" + canvasName + "Generated.cs";
            var canvas = selectedCanvas;
            var content = new List<string>();

            content.Add("using UnityEngine;");
            content.Add("using UnityEngine.UI;");
            content.Add("using TMPro;");
            content.Add("public static class " + canvasName + "Generated {");

            AddUIData(canvas.gameObject, content);

            content.Add("");
            content.Add("\tpublic static void Init() {");

            AddUIInit(canvas.gameObject, content, "");

            content.Add("\t");
            content.Add("\t}");
            content.Add("}");

            File.WriteAllLines(filePath, content);

            AssetDatabase.Refresh();
        }
    }

    public static void AddUIData(GameObject go, List<string> content)
    {
        string type = GetType(go.name);

        content.Add("\tpublic static " + type + " " + go.name + ";");

        for (int i = 0; i < go.transform.childCount; i++)
        {
            AddUIData(go.transform.GetChild(i).gameObject, content);
        }

    }

    public static void AddUIInit(GameObject go, List<string> content, string prefix)
    {
        string type = GetType(go.name);
        if (type == "GameObject")
        {
            content.Add("\t\t" + go.name + " = GameObject.Find(\"" + prefix + go.name + "\");");

        }
        else
        {
            content.Add("\t\t" + go.name + " = GameObject.Find(\"" + prefix + go.name + "\").GetComponent<" + type + ">();");
        }
        for (int i = 0; i < go.transform.childCount; i++)
        {
            var newPrefix = prefix;

            if (newPrefix == "")
            {
                newPrefix = go.name + "/";
            }
            else
            {
                newPrefix += go.name + "/";
            }

            AddUIInit(go.transform.GetChild(i).gameObject, content, newPrefix);
        }
    }
    private static string GetType(string name)
    {
        if (name.Contains("TXT"))
        {
            return "Text";
        }
        else if (name.Contains("TMP"))
        {
            return "TextMeshProUGUI";
        }
        else if (name.Contains("BTN"))
        {
            return "Button";
        }
        else if (name.Contains("IMG"))
        {
            return "Image";
        }
        else if (name.Contains("SLIDER"))
        {
            return "Slider";
        }
        else if (name.Contains("IF"))
        {
            return "InputField";
        }
        else if (name.Contains("IF_TMP"))
        {
            return "TMP_InputField";
        }
        else if (name.Contains("Panel"))
        {
            return "GameObject";
        }
        return "GameObject";
    }
}
