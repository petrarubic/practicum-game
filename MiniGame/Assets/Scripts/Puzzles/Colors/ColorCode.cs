using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class ColorCode : PuzzleTrigger {
    public enum ButtonType { Red, Blue, Green, Purple, Yellow, Default };
    public enum PuzzleType { Easy, Medium, Hard};

    [BoxGroup("Puzzle specific")]
    public Material defaultProgressMat, redMat, blueMat, greenMat, purpleMat, yellowMat;

    [BoxGroup("Puzzle specific")]
    public List<MeshRenderer> progressCubes = new List<MeshRenderer>();

    [BoxGroup("Puzzle specific")]
    public GameObject resetButton;

    [BoxGroup("Puzzle specific")]
    public PuzzleType puzzleType;

    [BoxGroup("Puzzle specific")]
    public MeshRenderer changingButton;

    [BoxGroup("Puzzle specific")] [InfoBox("Set if easy puzzle type")]
    public List<ButtonType> ezy = new List<ButtonType> { ButtonType.Blue, ButtonType.Red, ButtonType.Blue, ButtonType.Blue, ButtonType.Red };

    [BoxGroup("Puzzle specific")]
    [InfoBox("Set if easy puzzle type")]
    public List<ButtonType> medium = new List<ButtonType> { ButtonType.Blue, ButtonType.Red, ButtonType.Green, ButtonType.Red, ButtonType.Blue, ButtonType.Green, ButtonType.Green };

    [BoxGroup("Puzzle specific")]
    [InfoBox("Set if hard puzzle type")]
    public List<ButtonType> hard = new List<ButtonType> { ButtonType.Red, ButtonType.Green, ButtonType.Blue, ButtonType.Red, ButtonType.Red, ButtonType.Blue, ButtonType.Green };


    List<ButtonType> clicked_buttons = new List<ButtonType>();
    
    int times_clicked = 0;
    float changeColorTimer = 0f;
    private ColorButton changingButtonScript;

    private void Start() {
        foreach (var cube in progressCubes) {
            cube.material = GetMat(ButtonType.Default);
        }
        if (puzzleType == PuzzleType.Hard) {
            changingButtonScript = changingButton.GetComponent<ColorButton>();
        }
    }


    public override void InFocus() {
        if (puzzleType == PuzzleType.Hard) {
            changeColorTimer += Time.deltaTime;
            if (changeColorTimer > 1f) {
                changeColorTimer = 0;
                ButtonType mat = (ButtonType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ButtonType)).Length-1);
                changingButton.material = GetMat(mat);
                changingButtonScript.buttonColor = mat;
            }
            
        }

        RaycastHit hitFocus;
        Ray ray2 = Camera.main.ScreenPointToRay(Utils.GetMousePosition());
        if (Physics.Raycast(ray2, out hitFocus, 3, puzzleFocusLayer)) {
            if (InputManager.IsInteractionKeyPressed()) {
                ColorButton colorButton = null;
                if (hitFocus.transform.TryGetComponent<ColorButton>(out colorButton)) {
                    times_clicked++;
                    clicked_buttons.Add(colorButton.buttonColor);
                }
                if (puzzleType == PuzzleType.Easy) {
                    Check_clicked(ezy);
                } else if (puzzleType == PuzzleType.Medium) {
                    Check_clicked(medium);
                } else if (puzzleType == PuzzleType.Hard) {
                    Check_clicked(hard);
                }
                if (hitFocus.transform.gameObject == resetButton) {
                    foreach (var cube in progressCubes) {
                        cube.material = defaultProgressMat;
                    }
                    Reset();
                }
            }
        }
    }

    void Check_clicked(List<ButtonType> order) {
        if (times_clicked <= order.Count && times_clicked > 0) {
            var isCorrect = SetMatToProgressCubes(order);

            if (times_clicked == order.Count) {
                if (isCorrect) {
                    isSolved = true;
                    isTriggered = true;
                    onPuzzleSolve?.Invoke();
                    UnfocusFromPuzzle();
                    OnTrigger();
                }
            }
        }
    }
    private bool SetMatToProgressCubes(List<ButtonType> order) {
        if (clicked_buttons[times_clicked - 1] != order[times_clicked - 1]) {
            foreach (var cube in progressCubes) {
                cube.material = defaultProgressMat;
            }
            Reset();
            return false;
        } else {
            Material mat = GetMat(clicked_buttons[times_clicked - 1]);
            progressCubes[times_clicked - 1].material = mat;
        }
        return true;
    }
    private Material GetMat(ButtonType buttonType) {
        switch (buttonType) {
            case ButtonType.Blue:
                return blueMat;
            case ButtonType.Red:
                return redMat;
            case ButtonType.Green:
                return greenMat;
            case ButtonType.Purple:
                return purpleMat;
            case ButtonType.Yellow:
                return yellowMat;
            default:
                return defaultProgressMat;
        }
    }

    private void Reset() {
        times_clicked = 0;
        clicked_buttons.Clear();
    }
    public override void ResetTrigger() {
        throw new System.NotImplementedException();
    }
}
