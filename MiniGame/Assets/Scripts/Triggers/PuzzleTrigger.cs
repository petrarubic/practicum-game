using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using Cinemachine;
using VHS;

[RequireComponent(typeof(Collider))]
public abstract class PuzzleTrigger : Trigger {

    #region Variables

    #region Needed settings

    [BoxGroup("Required variables")]
    [Tooltip("Tick: \nDefault\nPuzzleTrigger")] public LayerMask puzzleTriggerLayer;
    [BoxGroup("Required variables")]
    [Tooltip("Tick everything except:\nPlayer\nPuzzleTrigger\nIgnoreRaycast\n")] public LayerMask puzzleFocusLayer;
    public string descriptionOnHover = "Puzzle";

    #endregion

    #region Camera settings
    [BoxGroup("Camera")]
    public bool HaveCameraFocus;
    [BoxGroup("Camera")] private Collider triggerForCameraFocus;
    [BoxGroup("Camera")]
    [ShowIf("HaveCameraFocus")] public CinemachineVirtualCamera puzzleCam;
    [BoxGroup("Camera")]
    [ReadOnly] [ShowIf("HaveCameraFocus")] public bool isFocused;
    #endregion

    #region Needed item settings
    [BoxGroup("Needed item")]
    public bool HaveNeededItem;
    [BoxGroup("Needed item")]
    [ShowIf("HaveNeededItem")] public List<SwapableItems> swapableItems = new List<SwapableItems>();
    [BoxGroup("Needed item")]
    [ReadOnly] [ShowIf("HaveNeededItem")] public bool areAllItemsInPlace = false;

    [BoxGroup("Needed item")]
    [ShowIf("HaveNeededItem")]
    public UnityEvent onItemPlaced;
    [BoxGroup("Needed item")]
    [ShowIf("HaveNeededItem")]
    public bool usePlaceItemSound;
    #endregion

    #region Other settings
    [Space]
    [BoxGroup("Read only puzzle things")]
    [ReadOnly] public bool isSolved;
    [BoxGroup("Read only puzzle things")]
    [Tooltip("Are all of the prerequisits done for the puzzle")] [ReadOnly] public bool isPuzzleUnlocked;
    [BoxGroup("Read only puzzle things")]
    [Tooltip("Restart the puzzle to its original state")] [ReadOnly] public bool restartPuzzle;


    [BoxGroup("Camera")] [ShowIf("HaveCameraFocus")]
    public UnityEvent onPuzzleFocused, onPuzzleUnfocused;
    [BoxGroup("Camera")]
    [ShowIf("HaveCameraFocus")]
    public bool useFocusSound;

    [BoxGroup("Other")]
    public UnityEvent onPuzzleSolve;
    
    private Ray ray;
    //protected Camera playerCam;
    private Collider puzzleCollider;

    #endregion

    #endregion

    private void Awake() {
        SetupPuzzle();
    }

    private void SetupPuzzle() {
        if (HaveCameraFocus) {
            if (puzzleCam) {
                puzzleCam.enabled = false;
            }
            triggerForCameraFocus = GetComponent<Collider>();
            if (triggerForCameraFocus == null) {
                Debug.LogError("Puzzle [" + transform.gameObject.name + "] needs to have a collider on it.");
            }
            Debug.Log("Puzzle [" + transform.gameObject.name + "] collider: " + triggerForCameraFocus);
            if (triggerForCameraFocus.gameObject.layer != LayerMask.NameToLayer(ReadonlyStrings.PuzzleTriggerLayer)) {
                Debug.LogWarning("Object [" + triggerForCameraFocus.gameObject.name + "] for camera trigger is not set to PuzzleTrigger layer, setting...");
                Debug.LogWarning("Object's [" + triggerForCameraFocus.gameObject.name + "] last layer is: " + LayerMask.LayerToName(triggerForCameraFocus.gameObject.layer));
                triggerForCameraFocus.gameObject.layer = LayerMask.NameToLayer(ReadonlyStrings.PuzzleTriggerLayer);
            }
        }
        if (HaveNeededItem) {
            TurnOffPuzzleExistingItems();
        }
    }
    public string GetDescription() {
        return descriptionOnHover;
    }

    private void Update() {
        if (isSolved) {
            return;
        }

        if (isFocused) {
            if (InputManager.IsKeyPressed(KeyCode.F)) {
                UnfocusFromPuzzle();
            }
            InFocus();
        }
    }

    /// <summary>
    /// Serves like an update - it is called from the interaction manager when ray hits this
    /// </summary>
    public virtual void Hover() {      
        if (!isFocused) {
            if (InputManager.IsInteractionKeyPressed()) {
                if (isSolved) {
                    return;
                }
                if (HaveNeededItem) {
                    if (!areAllItemsInPlace) {
                        var neededItemTuple = IsPlayerHavingNeededItem(); // check if the held item is one of the needed items
                        if (neededItemTuple.Item1) {
                            MoveItemToEndPos(InteractionManager.instance.GetHeldItem(), neededItemTuple.Item2); // lerps the object in the given position
                            if (areAllItemsInPlace) {
                                FocusOnPuzzle(true);
                            }
                        }
                    } else {
                        if (InteractionManager.instance.GetHeldItem() == null) {
                            FocusOnPuzzle(true);
                        }
                    }
                } else {
                    if (InteractionManager.instance.GetHeldItem() != null) {
                        return;
                    }
                    FocusOnPuzzle(true);
                }
            }
        }
    }
    /// <summary>
    /// Called every frame when puzzle is in focus
    /// </summary>
    public abstract void InFocus();
    


    #region Components / Methods

    #region Needed item

    /// <summary>
    /// Check if all needed items are found
    /// </summary>
    /// <returns></returns>
    public bool AreAllNeededItemsInPlace() {
        for (int i = 0; i < swapableItems.Count; i++) {
            if (!swapableItems[i].isItemInPlace) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Checks if the player have the needed item for puzzle in hands. 
    /// </summary>
    /// <returns>Returns -1 if it doesn't have needed item, -2 if the item == null and index of the the item if it is the needed one</returns>
    public (bool, int) IsPlayerHavingNeededItem() {
        var heldItem = InteractionManager.instance.GetHeldItem();
        if (heldItem == null) {
            return (false, -2);
        }
        var heldInfoSO = heldItem.GetComponent<Item>().item;
        for (int i = 0; i < swapableItems.Count; i++) {
            if (swapableItems[i].isItemInPlace) {
                areAllItemsInPlace = AreAllNeededItemsInPlace();
                continue;
            }
            if (AreItemsTheSame(heldInfoSO, swapableItems[i].neededItem)) {
                swapableItems[i].isItemInPlace = true;
                areAllItemsInPlace = AreAllNeededItemsInPlace();
                return (true, i);
            }
        }
        return (false, -1);
    }

    /// <summary>
    /// Turns gameobjects on or off
    /// </summary>
    /// <param name="toTurnOn"></param>
    /// <param name="toTurnOff"></param>
    public void TurnOnOffItem(GameObject toTurnOn, GameObject toTurnOff) {
        toTurnOff.SetActive(false);
        toTurnOn.SetActive(true);
    }

    /// <summary>
    /// Calling the coroutine that moves the item to the defined positions
    /// </summary>
    /// <param name="item">Index of the swapable item object</param>
    public void MoveItemToEndPos(GameObject item, int swapableItemIndex) {
        //var a = NormalizeAngle(item.transform.eulerAngles.x);
        //var b = NormalizeAngle(item.transform.eulerAngles.y);
        //var c = NormalizeAngle(item.transform.eulerAngles.z);

        //var newRot = Quaternion.Euler(new Vector3(a, b, c));
        //item.transform.rotation = newRot;
        StartCoroutine(MoveItem(item, swapableItemIndex));
    }
    float NormalizeAngle(float angle) {
        if (angle < 0) {
            angle += 360;
        }
        if (Mathf.Abs(angle) > 359) {
            angle %= 360;
        }
        
        return angle;
    }
    /// <summary>
    /// Moves the held item to the defined positions for the puzzle element and depending on data it does some stuff (as example - turn off the moving item when in place)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private IEnumerator MoveItem(GameObject item, int swapableItemIndex) {
        float elapsedTime = 0;
        var swapableItem = swapableItems[swapableItemIndex];
        var movePositions = swapableItem.neededItemMovePositions;
        swapableItem.startPos = item.transform.position;
        swapableItem.startRot = item.transform.eulerAngles;
        if (swapableItem.disableRigidbodyOnPuzzleItem) {
            Rigidbody rb = null;
            if (item.TryGetComponent(out rb)) {
                rb.isKinematic = true;
            }
        }
        if (swapableItem.disableCollisionOnPuzzleItem) {
            Collider col = null;
            if (item.TryGetComponent(out col)) {
                col.enabled = false;
            }
            Rigidbody rb = null;
            if (item.TryGetComponent(out rb)) {
                rb.detectCollisions = false;
            }
        }
        InteractionManager.instance.DropItem();
        int positionsIndex = 0;


        while (positionsIndex < movePositions.Length) {
            item.transform.position = Vector3.Lerp(swapableItem.startPos, movePositions[positionsIndex].movePosition.position, elapsedTime / movePositions[positionsIndex].neededItemMoveTime);
            item.transform.eulerAngles = Vector3.Lerp(swapableItem.startRot, movePositions[positionsIndex].movePosition.eulerAngles, elapsedTime / movePositions[positionsIndex].neededItemMoveTime);
            swapableItem.currentPos = item.transform.position;
            swapableItem.currentRot = item.transform.eulerAngles;
            if (Vector3.Distance(item.transform.position, movePositions[positionsIndex].movePosition.position) < 0.01f) {
             //if(elapsedTime >= movePositions[positionsIndex].neededItemMoveTime) {
                elapsedTime = 0;
                positionsIndex++;

                swapableItem.startPos = item.transform.position;
                swapableItem.startRot = item.transform.eulerAngles;
                Debug.Log("Position++");
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        onItemPlaced?.Invoke();
        if (swapableItem.swapMovingItemWithExistingPuzzleItem) {
            TurnOnOffItem(swapableItem.existingPuzzleItem, item);
        }
        if (swapableItem.disableRigidbodyOnPuzzleItem) {
            Rigidbody rb = null;
            if (item.TryGetComponent(out rb)) {
                rb.isKinematic = false;
            }
        }
        if (swapableItem.disableCollisionOnPuzzleItem) {
            Collider col = null;
            if (item.TryGetComponent(out col)) {
                col.enabled = true;
            }
            Rigidbody rb = null;
            if (item.TryGetComponent(out rb)) {
                rb.detectCollisions = true;
            }
        }
        yield return null;

    }
    /// <summary>
    /// Checks if the two given ItemSO are the same
    /// </summary>
    /// <param name="heldItem">Item in players hands</param>
    /// <param name="neededItem">Needed item for the puzzle</param>
    /// <returns></returns>
    public bool AreItemsTheSame(ItemSO heldItem, ItemSO neededItem) {
        return (heldItem == neededItem);
    }

    /// <summary>
    /// Sets the gameobject active depending on the bool defined in swapableItem
    /// </summary>
    public void TurnOffPuzzleExistingItems() {
        for (int i = 0; i < swapableItems.Count; i++) {
            var item = swapableItems[i];
            item.existingPuzzleItem.SetActive(!item.swapMovingItemWithExistingPuzzleItem);
        }
    }

    #endregion

    #region Focus on puzzle

    /// <summary>
    /// Moves the camera to the defined puzzle virtual camera and invoking the onFocus event
    /// </summary>
    public void FocusOnPuzzle(bool showMouse = false) {
        if (HaveCameraFocus) {
            InteractionManager.instance.ToggleInteractionIcons(false);
            InteractionManager.instance.ToggleCrosshair(false);
            Utils.ChangeCursorState(showMouse);
            isFocused = true;
            onPuzzleFocused?.Invoke();
            Player.instance.playerVirtualCamera.enabled = false;
            puzzleCam.enabled = true;
            Player.instance.SetCanControl(false);
            Player.instance.SetCanControlCamera(false);
            Camera.main.transform.position = puzzleCam.transform.position;
            Camera.main.transform.rotation = puzzleCam.transform.rotation;
        }
    }

    /// <summary>
    /// Unfocusing from the puzzle virtual camera back to player camera and invoking the onUnfocus event
    /// </summary>
    public void UnfocusFromPuzzle() {
        if (HaveCameraFocus) {
            InteractionManager.instance.ToggleInteractionIcons(true);
            InteractionManager.instance.ToggleCrosshair(true);
            Utils.ChangeCursorState(false);
            isFocused = false;
            onPuzzleUnfocused?.Invoke();
            Player.instance.playerVirtualCamera.enabled = true;
            puzzleCam.enabled = false;
            Player.instance.SetCanControl(true);
            Player.instance.SetCanControlCamera(true);
            Camera.main.transform.position = Vector3.zero;
            Camera.main.transform.eulerAngles = Vector3.zero;
        }
    }

    #endregion

    #region Other custom methods

    /// <summary>
    /// Set the isSolved to true and unfocuse from the puzzle
    /// </summary>
    protected void SetPuzzleSolved() {
        isSolved = true;
        if (isFocused) {
            UnfocusFromPuzzle();
        }
    }

    #endregion


    #endregion


    [System.Serializable]
    public class SwapableItems {
        [ReadOnly] public bool isItemInPlace;
        public ItemSO neededItem;
        public GameObject existingPuzzleItem;
        public MovePosition[] neededItemMovePositions;
        [Tooltip("To turn off the moving held item and to turn on the existing puzzle item")] public bool swapMovingItemWithExistingPuzzleItem = true;
        public bool disableRigidbodyOnPuzzleItem = false;
        public bool disableCollisionOnPuzzleItem = false;
        [HideInInspector] public Vector3 currentPos, startPos, startRot, currentRot;
    }

    [System.Serializable]
    public class MovePosition {
        [Range(0, 15)] [Tooltip("Lenght in seconds in which item will be at the end position")] public float neededItemMoveTime = 1f;
        public Transform movePosition;
    }
}
