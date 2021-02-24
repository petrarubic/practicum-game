using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OddOneOut : PuzzleTrigger {

    [Range(0, 10)] public float timeUntilChange = 2;

    public GameObject cubeParent;

    public bool useShades;
    public bool useRotation;
    public bool useScale;

    public bool darkMode;
    public bool randomOddOne;

    Color[] colors = new Color[6] { Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta, Color.red };
    float[] scale = new float[5] { 0.02f, 0.06f, 0.10f, 0.14f, 0.18f };
    float[] rotation = new float[4] { 30f, 45f, 60f, 90f };

    int index = 0;
    private float timer = 0;
    private GameObject oddOne;
    private int oddOneIndex;
    private Ray ray;

    void Start() {
        if (randomOddOne) {
            GenerateRandNum();
        } else {
            oddOneIndex = 5;
        }

        if (useShades) {
            ChangeColorShade();
        } else if (useScale) {
            ChangeScale();
        } else if (useRotation) {
            ChangeRotation();
        } else {
            ChangeColors();
        }
        //foreach (Transform cubeChild in cubeParent.transform)
        //{
        //    cubeChild.GetComponent<Renderer>().material.color = colors[index];
        //}
    }

    public override void InFocus() {
        ChangeColorsTimer();

        RaycastHit hitFocus;
        Ray ray = Camera.main.ScreenPointToRay(Utils.GetMousePosition());
        if (Physics.Raycast(ray, out hitFocus, 10, puzzleFocusLayer)) {
            if (InputManager.IsInteractionKeyPressed()) {
                if (hitFocus.transform.gameObject == oddOne) {
                    Debug.Log("Correct");
                    if (randomOddOne) {
                        GenerateRandNum();
                    } else {
                        oddOneIndex = 5;
                    }

                    if (useShades) {
                        ChangeColorShade();
                    } else if (useScale) {
                        ChangeScale();
                    } else if (useRotation) {
                        ChangeRotation();
                    } else {
                        ChangeColors();
                    }

                    if (++index == colors.Length) index = 0;
                    if (++index == scale.Length) index = 0;
                    if (++index == rotation.Length) index = 0;
                    timer = 0;
                    isSolved = true;
                    isTriggered = true;
                    UnfocusFromPuzzle();
                    OnTrigger();
                }
            }
        }
    }

    void ChangeRotation() {
        for (int i = 0; i < cubeParent.transform.childCount; i++) {
            if (i == oddOneIndex) {
                int oddOneRotationIndex = index + 1 == rotation.Length ? 0 : index + 1;
                cubeParent.transform.GetChild(i).transform.Rotate(new Vector3(rotation[oddOneRotationIndex], 0));
                oddOne = cubeParent.transform.GetChild(i).gameObject;
                continue;
            }
            cubeParent.transform.GetChild(i).transform.Rotate(new Vector3(rotation[index], 0));
        }
    }

    void ChangeScale() {
        for (int i = 0; i < cubeParent.transform.childCount; i++) {
            if (i == oddOneIndex) {
                int oddOneScaleIndex = index + 1 == scale.Length ? 0 : index + 1;
                cubeParent.transform.GetChild(i).transform.localScale = new Vector3(1.5f, scale[oddOneScaleIndex], scale[oddOneScaleIndex]);
                oddOne = cubeParent.transform.GetChild(i).gameObject;
                continue;
            }
            cubeParent.transform.GetChild(i).transform.localScale = new Vector3(1.5f, scale[index], scale[index]);
        }
    }
    void ChangeColorsTimer() {
        timer += Time.deltaTime;

        if (timer > timeUntilChange) {
            if (++index == colors.Length) index = 0;
            if (++index == scale.Length) index = 0;
            if (++index == rotation.Length) index = 0;
            if (randomOddOne) {
                GenerateRandNum();
            } else {
                oddOneIndex = 5;
            }

            if (useShades) {
                ChangeColorShade();
            } else if (useScale) {
                ChangeScale();
            } else if (useRotation) {
                ChangeRotation();
            } else {
                ChangeColors();
            }

            //foreach (Transform cubeChild in cubeParent.transform)
            //{
            //    cubeChild.GetComponent<Renderer>().material.color = colors[index];
            //}

            timer = 0;
        }

    }

    /// <summary>
    /// Changes colors of all object and sets one to be odd
    /// </summary>
    void ChangeColors() {
        for (int i = 0; i < cubeParent.transform.childCount; i++) {
            if (i == oddOneIndex) {
                int oddOneColorIndex = index + 1 == colors.Length ? 0 : index + 1;
                int oddOneScaleIndex = index + 1 == scale.Length ? 0 : index + 1;
                int oddOneRotationIndex = index + 1 == rotation.Length ? 0 : index + 1;
                cubeParent.transform.GetChild(i).transform.Rotate(new Vector3(rotation[oddOneRotationIndex], 0));
                cubeParent.transform.GetChild(i).GetComponent<Renderer>().material.color = colors[oddOneColorIndex];
                cubeParent.transform.GetChild(i).transform.localScale = new Vector3(1.5f, scale[oddOneScaleIndex], scale[oddOneScaleIndex]);
                oddOne = cubeParent.transform.GetChild(i).gameObject;
                continue;
            }
            cubeParent.transform.GetChild(i).GetComponent<Renderer>().material.color = colors[index];
            cubeParent.transform.GetChild(i).transform.localScale = new Vector3(1.5f, scale[index], scale[index]);
            cubeParent.transform.GetChild(i).transform.Rotate(new Vector3(rotation[index], 0));
        }
    }

    void ChangeColorShade() {
        var color = colors[index];
        bool changeShade = false;
        int len = cubeParent.transform.childCount;
        int ostatak = 0;
        int toTheEnd = 0;
        for (int i = 0; i < len; i++) {
            if (i == oddOneIndex) {
                cubeParent.transform.GetChild(i).GetComponent<Renderer>().material.color = colors[index];
                oddOne = cubeParent.transform.GetChild(i).gameObject;
                toTheEnd = len - i;
                changeShade = true;
            } else {
                if (!changeShade) {
                    float step = 0;
                    if (i == 0) {
                        step = (0.5f / oddOneIndex);
                    } else {
                        step = ((float)i / oddOneIndex);
                    }
                    var colorShade = Color.Lerp(darkMode ? Color.black : Color.white, color, step);
                    cubeParent.transform.GetChild(i).GetComponent<Renderer>().material.color = colorShade;
                } else {
                    float step = 0;
                    //if (i == 0)
                    //{
                    //    step = (oddOneIndex / 0.5f);
                    //}
                    //else
                    //{
                    //    step = (oddOneIndex / (float)i);
                    //}
                    //var colorShade = Color.Lerp(color, darkMode ? Color.black : Color.white, step);
                    //cubeParent.transform.GetChild(i).GetComponent<Renderer>().material.color = colorShade;

                    step = (float)(ostatak) / toTheEnd;
                    //Debug.Log("STEP"+ step);
                    var colorShade = Color.Lerp(color, darkMode ? Color.black : Color.white, step);
                    cubeParent.transform.GetChild(i).GetComponent<Renderer>().material.color = colorShade;
                    ostatak++;
                }
            }
        }
    }

    /// <summary>
    /// Generates a random number between 0 and number of childs in cubeParent
    /// </summary>
    void GenerateRandNum() {
        oddOneIndex = UnityEngine.Random.Range(1, cubeParent.transform.childCount);
    }

    public override void ResetTrigger() {
        throw new System.NotImplementedException();
    }
}
