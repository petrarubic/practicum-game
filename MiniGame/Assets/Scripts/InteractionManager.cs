using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
//using Knife.HDRPOutline.Core;
using System;
using VHS;
using DG.Tweening;

// Controlling all the interactions with all interactable objects in the scene

public enum InteractionSpriteType { None, Default, Interactable, Pickupable, Focusable, CantPickup, PuzzleTrigger, MemoryTrigger }
public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;

    public bool useNewInteraction;

    [SerializeField] Camera mainCamera;

    public Canvas playerCanvas;

    public LayerMask checkObstacleLayer =~ 0;

    [BoxGroup("New System")] [Tooltip("Tick everything except: Player, PuzzleTrigger, IgnoreRaycast")] public LayerMask mainInteractLayers = ~0;
    [BoxGroup("New System")] [SerializeField] private float mainRayDistance = 3f;
    private Ray mainRay;
    private RaycastHit mainHit;
    private bool mainHitSomething;

    [BoxGroup("New System")] [Tooltip("Tick everything except: Player, IgnoreRaycast")] public LayerMask puzzleTriggerLayers = ~0;
    [BoxGroup("New System")] [SerializeField] private float puzzleTriggerRayDistance = 3f;
    private Ray puzzleTriggerRay;
    private RaycastHit puzzleTriggerHit;
    private bool puzzleTriggerHitSomething;

    [Header("Pickup data")]
    [Tooltip("Smoothing the movement of picking up the object. The lower the number the faster the picking")]
    public float speedOfFocusingObject;
    [Tooltip("Distance of focused object from the camera")]
    public float focusDistance;
    [Tooltip("Distance of picked up object from the camera")]
    public float pickupDistance;
    [Range(0.25f, 3f)] public float pickupMinDistance = 0.5f, pickupMaxDistance = 2.5f;

    private Vector3 currPos;
    private Quaternion currRot;
    private Vector3 startPos;
    private Quaternion startRot;
    private float dur;
    private float minSpeed = 0;
    [SerializeField] private float maxSpeed = 300f;
    [SerializeField] private float maxDist = 10f;
    private float currentSpeed = 0f;
    private float currentDist = 0f;
    public float rotationSpeed = 100f;
    Quaternion lookRot;

    [Header("Held item info")]
    [Range(1f, 10f)] public float heldItemScrollSpeed = 5f;
    [HideInInspector]
    public GameObject heldItem;
    [HideInInspector]
    public Rigidbody heldItemRb;
    [HideInInspector]
    public Collider heldItemCollider;
    private Item heldItemCompInfo;
    [SerializeField] [Range(0, 50)] private float throwForce;
    private float lerpTime;

    [Header("Other stuff")]
    public InteractionIconsSO interactionIcons;

    [Header("Focused object info")]
    private GameObject focusedObject;
    private Rigidbody focusedObjectRb;
    private bool isFocusingHeldItem;
    private bool isFocusing;
    private bool isBeingFocused;
    [Range(50, 500)] public float rotationSpeedOnFocusedObject = 250f;

    private bool showInteractionIcons;

    private bool crosshairActive;
    private bool hitSomethingTags;
    private GameObject levelSelect;
    private Sprite lastInteractionIcon;
    private string lastInteractionDescription;
    private float currentIconColorAlpha;

    // REFERENCE TO LAST HIT OBJECTS
    //private OutlineObject lastObj;
    private IInteractable lastInteractableObj;

    void Awake()
    {
        instance = this;
        DOTween.Init();

        levelSelect = GameObject.Find(ReadonlyStrings.LevelSelect);
        if (!!levelSelect)
        {
            levelSelect.SetActive(false);
        }

        if (!!mainCamera)
        {
            mainCamera = Camera.main;
        }
        ToggleCrosshair(true);
        ToggleInteractionIcons(true);
        lastInteractionIcon = null;
        lastInteractionDescription = "";
        ShowInteractionIcon(InteractionSpriteType.None);

    }

    private void Start()
    {
    }

    void Update()
    {        
        if (useNewInteraction)
        {
            var check1 = CheckMain();
            var check2 = CheckOther();
            if (!check1 && !check2)
            {
                HideIcons();
            }
            HandleHeldItem();
        }
    }


    private void FixedUpdate()
    {
        if (heldItem != null && !isFocusingHeldItem && !isBeingFocused && !isFocusing)
        {
            var scrollValue = InputManager.GetMouseScrollValueY();
            if (scrollValue != 0)
            {
                pickupDistance += scrollValue * heldItemScrollSpeed * Time.fixedDeltaTime;
            }
            pickupDistance = Mathf.Clamp(pickupDistance, pickupMinDistance, pickupMaxDistance);
            Vector3 desiredPos = mainCamera.ScreenToWorldPoint(Utils.GetMousePosition(pickupDistance));
            currentDist = Vector3.Distance(desiredPos, heldItemRb.position);
            currentSpeed = Mathf.SmoothStep(minSpeed, maxSpeed, currentDist / maxDist);
            currentSpeed *= Time.fixedDeltaTime;
            Vector3 direction = desiredPos - heldItemRb.position;
            heldItemRb.velocity = direction.normalized * currentSpeed;
        }
    }

    #region NEW SYSTEM

    private bool CheckMain()
    {
        mainRay = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        mainHitSomething = Physics.Raycast(mainRay, out mainHit, mainRayDistance, mainInteractLayers);

        if (mainHitSomething)
        {
            if (GetHeldItem() != null)
            {
                return false;
            }
            var hitObj = mainHit.collider.transform.gameObject;
            // order matters
            if (CheckCustomScripts(hitObj)) return true;
            if (CheckPickupable(hitObj)) return true;
            if (CheckInteractable(hitObj)) return true;
            if (CheckInteractable(hitObj)) return true;
            if (CheckTrigger(hitObj)) return true;
            if (CheckTags(hitObj)) return true;
        }
        return false;
    }

    private bool CheckOther()
    {
        puzzleTriggerRay = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        puzzleTriggerHitSomething = Physics.Raycast(puzzleTriggerRay, out puzzleTriggerHit, puzzleTriggerRayDistance, puzzleTriggerLayers);

        if (puzzleTriggerHitSomething)
        {
            var hitObj = puzzleTriggerHit.collider.transform.gameObject;
            if (CheckPuzzleTrigger(hitObj)) return true;
        }
        return false;
    }

    private void HideIcons()
    {
        ShowInteractionIcon(InteractionSpriteType.None);
        if (lastInteractableObj != null)
        {
            lastInteractableObj.StopInteraction();
            lastInteractableObj = null;
        }
    }

    private bool CheckPuzzleTrigger(GameObject hitObj)
    {
        bool isHit = false;
        string description = "";
        PuzzleTrigger puzzleScript = null;
        if (hitObj.TryGetComponent<PuzzleTrigger>(out puzzleScript))
        {
            ShowInteractionIcon(InteractionSpriteType.PuzzleTrigger);
            description = puzzleScript.GetDescription();
            puzzleScript.Hover();
            isHit = true;
        }
        return isHit;
    }


    private bool CheckTrigger(GameObject hitObj)
    {
        bool isHit = false;
        Trigger trigScript = null;
        if (hitObj.TryGetComponent<Trigger>(out trigScript))
        {
            if (hitObj.GetComponent<PuzzleTrigger>() != null)
            {
                return false;
            }
            isHit = true;
            if (trigScript.canTriggerWithInteractionPress)
            {
                ShowInteractionIcon(InteractionSpriteType.Interactable);
                if (InputManager.IsInteractionKeyPressed())
                {
                    trigScript.isTriggered = true;
                    trigScript.OnTrigger();
                }
            }
        }
        return isHit;
    }

    private bool CheckInteractable(GameObject hitObj)
    {
        bool isHit = false;
        IInteractable interactableScript = null;
        if (hitObj.TryGetComponent<IInteractable>(out interactableScript))
        {
            isHit = true;
            // Outline turn on/off feedback
            //TurnOutline(interactableHit);

            // saving the lastInteractable obj - so later StopInteraction can be called
            if (interactableScript != lastInteractableObj)
            {
                lastInteractableObj = null;
            }
            lastInteractableObj = interactableScript;
            interactableScript.Hover();
        }
        else
        {
            if (lastInteractableObj != null)
            {
                lastInteractableObj.StopInteraction();
                lastInteractableObj = null;
            }
        }
        return isHit;
    }



    private bool CheckPickupable(GameObject hitObj)
    {
        bool isHit = false;
        Item pickupableScript = null;
        if (hitObj.TryGetComponent<Item>(out pickupableScript))
        {
            isHit = true;

            if (InputManager.IsInteractionKeyPressed())
                Destroy(pickupableScript.gameObject);
            
        }

        return isHit;
    }

    private bool CheckTags(GameObject hitObj)
    {
        if (GetHeldItem() != null)
        {
            return false;
        }
        /* REFERENCE
        if (hitObj.CompareTag("some tag")) {
            if (InputManager.IsInteractionKeyPressed()) {
                //DO smth
            }
            return true;
        }
        */
        return false;
    }
    private bool CheckCustomScripts(GameObject hitObj)
    {
        bool isHit = false;
        string description = "";
        return isHit;
    }
    #endregion

    #region Other methods

    //void TurnOutline(RaycastHit hit) {
    //    OutlineObject outline = hit.collider.GetComponent<OutlineObject>();

    //    if (!!outline) {
    //        if (outline != lastObj && !!lastObj) {
    //            lastObj.enabled = false;
    //            lastObj = null;
    //        }
    //        outline.enabled = true;
    //        lastObj = outline;
    //    }
    //}



    public void ToggleCrosshair(bool show = true)
    {
        if (crosshairActive != show)
        {
            crosshairActive = show;
            //PlayerCanvasGenerated.Crosshair_IMG.gameObject.SetActive(show);
        }
    }

    public void ToggleInteractionIcons(bool on = true)
    {
        showInteractionIcons = on;
        //PlayerCanvasGenerated.Interaction_IMG.transform.DOScale(on ? 1 : 0, 0.3f);
        // PlayerCanvasGenerated.Interaction_IMG.gameObject.SetActive(on);
        //PlayerCanvasGenerated.InteractionDescriptionTMP.transform.DOScale(on ? 1 : 0, 0.3f);
        //PlayerCanvasGenerated.InteractionDescriptionTMP.gameObject.SetActive(on);
    }

    public void ShowInteractionIcon(InteractionSpriteType interactionSpriteType)
    {
        var icon = interactionIcons.GetSprite(interactionSpriteType);
        if (lastInteractionIcon == icon)
        {
            return;
        }
        lastInteractionIcon = icon;

        if (lastInteractionIcon == null)
        {
            //PlayerCanvasGenerated.Interaction_IMG.transform.DOScale(0, 0.3f);
            //PlayerCanvasGenerated.Interaction_IMG.DOColor(Color.clear, 0.2f);
            ToggleCrosshair(true);
        }
        else
        {
            //PlayerCanvasGenerated.Interaction_IMG.DOColor(Color.clear, 0.2f).OnStart(()=> {
            //    PlayerCanvasGenerated.Interaction_IMG.sprite = icon;
            //});
            //PlayerCanvasGenerated.Interaction_IMG.DOColor(Color.white, 0.2f).OnStart(() => {
            //    PlayerCanvasGenerated.Interaction_IMG.sprite = lastInteractionIcon;
            //});
            ToggleCrosshair(false);
        }

    }

    #endregion

    #region Held item

    private void HandleHeldItem()
    {
        if (heldItem != null)
        {
            //Vector3 desiredPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickupDistance));
            //heldItem.transform.position = Vector3.MoveTowards(heldItem.transform.position, desiredPos, moveSpeedOfHeldObject * Time.deltaTime);

            //heldItem.transform.rotation = Quaternion.RotateTowards(heldItem.transform.rotation, Quaternion.LookRotation(mainCamera.transform.forward, mainCamera.transform.up), rotationSpeedOfHeldObject * Time.deltaTime);
            //heldItem.transform.LookAt(mainCamera.transform);
            //if (InputManager.IsInteractionKeyPressed()) {
            //    heldItemRb.useGravity = true;
            //    heldItemRb = null;
            //    heldItemCollider = null;
            //    heldItem = null;
            //}
            if (InputManager.IsKeyPressed(KeyCode.R))
            {
                heldItemRb = null;
                heldItemCollider = null;
                heldItem = null;
                DestroyItemFunctions();
            }

            if (InputManager.IsKeyPressed(KeyCode.F) && heldItemCompInfo.canFocus)
            {
                Player.instance.SetCanControl(false);
                Player.instance.SetCanControlCamera(false);
                focusedObject = heldItem;
                focusedObjectRb = heldItemRb;
                startPos = focusedObject.transform.position;
                startRot = focusedObject.transform.rotation;
                heldItem = null;
                isBeingFocused = true;
                StartCoroutine(FocusObject(1));
            }
            if (InputManager.IsDropButtonPressed() && !isBeingFocused)
            {
                //heldItemRb.AddForce(mainCamera.transform.forward * throwForce, ForceMode.Impulse);
                DropItem();

            }

            if (currentDist > maxDist) DropItem();
        }
        if (InputManager.IsKeyPressed(KeyCode.R) && isFocusingHeldItem)
        {
            focusedObjectRb = null;
            focusedObject = null;
            heldItemRb = null;
            heldItemCollider = null;
            heldItem = null;
            DestroyItemFunctions();
            Player.instance.SetCanControl(true);
            Player.instance.SetCanControlCamera(true);
            isFocusingHeldItem = false;
        }
        if ((isFocusing || isFocusingHeldItem) && !isBeingFocused && focusedObject != null)
        {
            if (InputManager.IsInteractionKey())
            {
                focusedObject.transform.Rotate(-mainCamera.transform.up, InputManager.GetMouseAxisX() * rotationSpeedOnFocusedObject * Time.deltaTime, Space.World);
                focusedObject.transform.Rotate(mainCamera.transform.right, InputManager.GetMouseAxisY() * rotationSpeedOnFocusedObject * Time.deltaTime, Space.World);
            }
        }

        if (focusedObject != null && !isBeingFocused && (InputManager.IsDropButtonPressed() || InputManager.IsKeyPressed(KeyCode.F)))
        {
            isBeingFocused = true;
            Utils.ChangeCursorState(false);
            StartCoroutine(UnfocusObject());
        }
    }
    public void ChangeHeldItem(Collider heldItemToSet)
    {
        heldItem = heldItemToSet.gameObject;
        heldItemCollider = heldItemToSet;
        heldItemRb = heldItem.GetComponent<Rigidbody>();
        heldItemCompInfo = heldItem.GetComponent<Item>();
        lerpTime = 0;

        ShowItemFunctions(heldItemCompInfo);
        PickupItem();
    }
    public GameObject GetHeldItem()
    {
        return heldItem;
    }

    #endregion

    private void DestroyItemFunctions()
    {
        //for (int i = 0; i < PlayerCanvasGenerated.ItemFunctionsPanel.transform.childCount; i++)
        //{
        //    Destroy(PlayerCanvasGenerated.ItemFunctionsPanel.transform.GetChild(i).gameObject);
        //}
    }

    private void ShowItemFunctions(Item heldItemInfo)
    {
        //var itemFunc = Instantiate(UIPlayerManager.instance.itemFunctionTMP, PlayerCanvasGenerated.ItemFunctionsPanel.transform);
        //itemFunc.SetText("RMB - drop");

        //if (heldItemInfo.canFocus)
        //{
        //    var itemFunc1 = Instantiate(UIPlayerManager.instance.itemFunctionTMP, PlayerCanvasGenerated.ItemFunctionsPanel.transform);
        //    itemFunc1.SetText("F - focus");
        //}
    }

    public void DropItem()
    {
        heldItem.layer = LayerMask.NameToLayer(ReadonlyStrings.PickupableLayer);
        heldItemRb.constraints = RigidbodyConstraints.None;
        heldItemRb.useGravity = true;
        heldItemCompInfo.DropItem();
        heldItem = null;
        heldItemRb = null;
        heldItemCollider = null;
        currentDist = 0;
        // destroying childs of itemfunctionpanel after item is dropped
        DestroyItemFunctions();
    }

    public void PickupItem()
    {
        heldItemRb.constraints = RigidbodyConstraints.FreezeRotation;
        heldItemRb.interpolation = RigidbodyInterpolation.Interpolate;
        heldItemRb.useGravity = false;
        heldItem.layer = LayerMask.NameToLayer(ReadonlyStrings.IgnoreRaycastLayer);
        StartCoroutine(heldItemCompInfo.PickUp());
    }

    #region Coroutines
    private IEnumerator FocusObject(int focusType)
    {
        dur = speedOfFocusingObject;
        if (speedOfFocusingObject == 0f)
        {
            dur = 1f;
        }
        currPos = focusedObject.transform.position;
        currRot = focusedObject.transform.rotation;
        //float i = ReMindUtils.InverseLerp(currPos, , focusedObject.transform.position);
        float i = Utils.InverseLerp(currPos, mainCamera.transform.position + mainCamera.transform.forward * focusDistance, focusedObject.transform.position);
        while (i < 1f)
        {
            i += Time.deltaTime / dur;
            Vector3 move = Vector3.Lerp(currPos, mainCamera.transform.position + mainCamera.transform.forward * focusDistance, i);
            var rot = Quaternion.LookRotation(mainCamera.transform.position - focusedObjectRb.position);
            Quaternion rotate = Quaternion.Lerp(currRot, rot, i);


            focusedObject.transform.position = move;
            focusedObject.transform.rotation = rotate;

            yield return new WaitForFixedUpdate();
        }

        focusedObjectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        focusedObjectRb.isKinematic = true;
        focusedObjectRb.useGravity = false;
        if (focusType == 1) isFocusingHeldItem = true;
        else isFocusing = true;
        isBeingFocused = false;

        focusedObject.transform.position = mainCamera.transform.position + mainCamera.transform.forward * focusDistance;
        Utils.ChangeCursorState(true);
    }

    private IEnumerator UnfocusObject()
    {
        dur = speedOfFocusingObject;
        if (speedOfFocusingObject == 0f)
        {
            dur = 1f;
        }
        currPos = focusedObject.transform.position;
        currRot = focusedObject.transform.rotation;

        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        Physics.Raycast(ray, out hit, pickupMaxDistance + 0.5f, checkObstacleLayer);

        if (hit.transform != null)
        {
            var offset = mainCamera.transform.position - hit.point;
            pickupDistance = hit.distance - 0.5f;
            startPos = hit.point - offset / 3;
        }

        float i = Utils.InverseLerp(currPos, startPos, focusedObject.transform.position);
        while (i < 1f)
        {
            i += Time.deltaTime / dur;
            Vector3 move = Vector3.Lerp(currPos, mainCamera.ScreenToWorldPoint(Utils.GetMousePosition(pickupDistance)), i);
            var rot = Quaternion.LookRotation(mainCamera.transform.position - focusedObjectRb.position);
            Quaternion rotate = Quaternion.Lerp(currRot, rot, i);

            focusedObject.transform.position = move;
            focusedObject.transform.rotation = rotate;

            yield return new WaitForFixedUpdate();
        }

        focusedObjectRb.isKinematic = false;
        focusedObjectRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        if (isFocusingHeldItem)
        {
            heldItem = focusedObject;
            isFocusingHeldItem = false;
            focusedObjectRb.useGravity = false;
        }
        else
        {
            focusedObjectRb.useGravity = true;
            isFocusing = false;
        }
        isBeingFocused = false;
        focusedObject = null;
        Player.instance.SetCanControl(true);
        Player.instance.SetCanControlCamera(true);
    }
    #endregion


}
