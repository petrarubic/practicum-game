using UnityEngine;
using UnityEngine.UI;
using TMPro;
public static class MainMenuCanvasGenerated {
	public static GameObject MainMenuCanvas;
	public static GameObject MainMenuPanel;
	public static GameObject MainMenuTitle;
	public static GameObject StartButton;
	public static GameObject StartButtonText;
	public static GameObject QuitButton;
	public static GameObject QuitButtonText;

	public static void Init() {
		MainMenuCanvas = GameObject.Find("MainMenuCanvas");
		MainMenuPanel = GameObject.Find("MainMenuCanvas/MainMenuPanel");
		MainMenuTitle = GameObject.Find("MainMenuCanvas/MainMenuPanel/MainMenuTitle");
		StartButton = GameObject.Find("MainMenuCanvas/MainMenuPanel/StartButton");
		StartButtonText = GameObject.Find("MainMenuCanvas/MainMenuPanel/StartButton/StartButtonText");
		QuitButton = GameObject.Find("MainMenuCanvas/MainMenuPanel/QuitButton");
		QuitButtonText = GameObject.Find("MainMenuCanvas/MainMenuPanel/QuitButton/QuitButtonText");
	
	}
}
