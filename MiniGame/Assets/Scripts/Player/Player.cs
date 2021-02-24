using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using NaughtyAttributes;

public class Player : MonoBehaviour
{
    public static Player instance;

    public CinemachineVirtualCamera playerVirtualCamera;

    [SerializeField] [ReadOnly] private bool canControl, canControlCamera;
    [SerializeField] private bool noCrouch, noRun, isSlowMovement, isInvertedMovements;
    [SerializeField] [ReadOnly] private bool noMoveCameraHorizontal, noMoveCameraVertical, cameraInvertedX, cameraInvertedY, cameraWeirdSmoothing;

    public bool isMemorySceneTrampolin;
    public GameObject sphereRb;
    public Rigidbody subtitlesSource;

    private void Awake() {
        instance = this;
        //TODO: change this to destory when you go to main menu and stuff - save load...
        DontDestroyOnLoad(this);
        if (playerVirtualCamera == null) {
            playerVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        }
        InitValues();
    }

    private void InitValues() {
        SetCanControl(true);
        SetCanControlCamera(true);
        noCrouch = false;
        noRun = false;
        isSlowMovement = false;
        isInvertedMovements = false;
        noMoveCameraHorizontal = false;
        noMoveCameraVertical = false;
        cameraInvertedX = false;
        cameraInvertedY = false;
        cameraWeirdSmoothing = false;
    }

    private void Update() {
        if (isMemorySceneTrampolin) {
            sphereRb.transform.position = transform.position + Vector3.up * 0.5f;
        }
    }
    // MOVEMENT
    public bool NoCrouch() {
        return noCrouch;
    }
    public bool NoRun() {
        return noRun;
    }
    public bool IsSlowMovement() {
        return isSlowMovement;
    }
    public bool IsInvertedMovements() {
        return isInvertedMovements;
    }
    public bool CanControl() {
        return canControl;
    }

    // CAMERA
    public bool NoMoveCameraHorizontal() {
        return noMoveCameraHorizontal;
    }
    public bool NoMoveCameraVertical() {
        return noMoveCameraVertical;
    }
    public bool CameraInvertedX() {
        return cameraInvertedX;
    }
    public bool CameraInvertedY() {
        return cameraInvertedY;
    }
    public bool CameraWeirdSmoothing() {
        return cameraWeirdSmoothing;
    }
    public bool CanControlCamera() {
        return canControlCamera;
    }

    // Setters
    public void SetNoRun(bool setRun) {
        noRun = setRun;
    }
    public void SetCanControl(bool setControl) {
        canControl = setControl;
    }
    public void SetCanControlCamera(bool setControlCam) {
        canControlCamera = setControlCam;
    }

}
