using UnityEngine;
using UnityEngine.UI;
using TMPro;
public static class WorldSelectCanvasGenerated {
	public static GameObject WorldSelectCanvas;
	public static GameObject WorldSelectPanel;
	public static GameObject WorldSelectTitle;
	public static GameObject BlueWorldButton;
	public static GameObject BlueWorldButtonText;
	public static GameObject RedWorldButton;
	public static GameObject RedWorldButtonText;
	public static GameObject GreenWorldButton;
	public static GameObject GreenWorldButtonText;

	public static void Init() {
		WorldSelectCanvas = GameObject.Find("WorldSelectCanvas");
		WorldSelectPanel = GameObject.Find("WorldSelectCanvas/WorldSelectPanel");
		WorldSelectTitle = GameObject.Find("WorldSelectCanvas/WorldSelectPanel/WorldSelectTitle");
		BlueWorldButton = GameObject.Find("WorldSelectCanvas/WorldSelectPanel/BlueWorldButton");
		BlueWorldButtonText = GameObject.Find("WorldSelectCanvas/WorldSelectPanel/BlueWorldButton/BlueWorldButtonText");
		RedWorldButton = GameObject.Find("WorldSelectCanvas/WorldSelectPanel/RedWorldButton");
		RedWorldButtonText = GameObject.Find("WorldSelectCanvas/WorldSelectPanel/RedWorldButton/RedWorldButtonText");
		GreenWorldButton = GameObject.Find("WorldSelectCanvas/WorldSelectPanel/GreenWorldButton");
		GreenWorldButtonText = GameObject.Find("WorldSelectCanvas/WorldSelectPanel/GreenWorldButton/GreenWorldButtonText");
	
	}
}
