using UnityEngine;
using UnityEngine.InputSystem;
using VHS;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    // MOVEMENT
    private static KeyCode runKey = KeyCode.LeftShift;
    private static KeyCode upKey = KeyCode.W;
    private static KeyCode leftKey = KeyCode.A;
    private static KeyCode downKey = KeyCode.S;
    private static KeyCode rightKey = KeyCode.D;
    private static KeyCode jumpKey = KeyCode.Space;
    private static KeyCode crouchKey = KeyCode.C;

    // INTERACTIONS
    private static KeyCode interactButton = KeyCode.Mouse0;
    private static KeyCode interact2Button = KeyCode.E;
    private static KeyCode cancelPickupButton = KeyCode.Mouse1;
    private static KeyCode QButton = KeyCode.Q;

    // INVENTORY
    private static KeyCode inventoryButton = KeyCode.V;
    private static KeyCode inventoryButton2 = KeyCode.I;
    private static KeyCode inventory1Button = KeyCode.Alpha1;
    private static KeyCode inventory2Button = KeyCode.Alpha2;
    private static KeyCode inventory3Button = KeyCode.Alpha3;
    private static KeyCode inventory4Button = KeyCode.Alpha4;


    [SerializeField] private MovementInputData movementInputData = null;
    private PlayerInput inputActions;
    private Vector2 moveValues;
    private Vector2 cameraRotInput;
    private bool isCrouchPressed;
    private bool isJumpPressed;


    private void Awake() {
        instance = this;
        inputActions = new PlayerInput();
        inputActions.Player.Move.performed += SetMoveValues;
        inputActions.Player.MouseRotation.performed += SetCameraRotValues;
        inputActions.Player.Run.started += ctx => SetRunValues(true);
        inputActions.Player.Run.canceled += ctx => SetRunValues(false);
        inputActions.Player.Crouch.started += ctx => SetCrouchValues(true);
        inputActions.Player.Crouch.canceled += ctx => SetCrouchValues(false);
        inputActions.Player.Jump.started += ctx => SetJumpValues(true);
        inputActions.Player.Jump.canceled += ctx => SetJumpValues(false);
    }

    private void Start() {
        movementInputData.ResetInput();
    }
    private void Update() {
        GetMovementInputData();
    }
    void GetMovementInputData() {
        //movementInputData.InputVectorX = ReMindInput.GetAxisRawHorizontal();
        //movementInputData.InputVectorY = ReMindInput.GetAxisRawVertical();

        movementInputData.InputVectorX = GetAxisRawHorizontal();
        movementInputData.InputVectorY = GetAxisRawVertical();

        movementInputData.RunClicked = IsRunKeyPressed();
        movementInputData.RunReleased = IsRunKeyReleased();

        if (movementInputData.RunClicked)
            movementInputData.IsRunning = true;

        if (movementInputData.RunReleased)
            movementInputData.IsRunning = false;

        movementInputData.JumpClicked = IsJumpKeyPressed();
        movementInputData.CrouchClicked = IsCrouchKeyPressed();
    }


    // MOVEMENT - NEW INPUT SYSTEM

    private void SetMoveValues(InputAction.CallbackContext ctx) {
        moveValues = ctx.ReadValue<Vector2>();
    }
    private void SetCameraRotValues(InputAction.CallbackContext ctx) {
        cameraRotInput = ctx.ReadValue<Vector2>();
    }
    private void SetRunValues(bool runValues) {
        movementInputData.RunReleased = !runValues;
        movementInputData.RunClicked = runValues;
    }
    private void SetCrouchValues(bool setCrouch) {
        isCrouchPressed = setCrouch;
    }
    private void SetJumpValues(bool setJump) {
        isJumpPressed = setJump;
    }

    public Vector2 GetMoveValue() {
        return moveValues;
    }
    public Vector2 GetCameraRotValue() {
        return cameraRotInput;
    }
    public bool GetRunValue() {
        return movementInputData.RunClicked;
    }
    public bool GetRunReleaseValue() {
        return movementInputData.RunReleased;
    }
    public bool GetCrouchValue() {
        return movementInputData.CrouchClicked;
    }
    public bool GetJumpValue() {
        return movementInputData.JumpClicked;
    }
    
    private void OnEnable() {
        inputActions.Enable();
    }

    private void OnDisable() {
        inputActions.Disable();
    }

    // MOVEMENT - OLD INPUT SYSTEM
    public static bool IsJumpKeyPressed() {
        return Input.GetKeyDown(jumpKey);
    }
    public static bool IsJumpKeyReleased() {
        return Input.GetKeyUp(jumpKey);
    }
    public static bool IsCrouchKeyPressed() {
        return Input.GetKeyDown(crouchKey);
    }
    public static bool IsCrouchKeyReleased() {
        return Input.GetKeyUp(crouchKey);
    }
    public static bool IsMoveUpKeyPressed() {
        return Input.GetKeyDown(upKey);
    }
    public static bool IsMoveUpKeyReleased() {
        return Input.GetKeyUp(upKey);
    }
    public static bool IsMoveLeftKeyPressed() {
        return Input.GetKeyDown(leftKey);
    }
    public static bool IsMoveLeftKeyReleased() {
        return Input.GetKeyUp(leftKey);
    }
    public static bool IsMoveRightKeyPressed() {
        return Input.GetKeyDown(rightKey);
    }
    public static bool IsMoveRightKeyReleased() {
        return Input.GetKeyUp(rightKey);
    }
    public static bool IsMoveDownKeyPressed() {
        return Input.GetKeyDown(downKey);
    }
    public static bool IsMoveDownKeyReleased() {
        return Input.GetKeyUp(downKey);
    }
    public static bool IsRunKeyPressed() {
        return Input.GetKeyDown(runKey);
    }
    public static bool IsRunKeyReleased() {
        return Input.GetKeyUp(runKey);
    }
    public static float GetAxisRawHorizontal() {
        return Input.GetAxisRaw("Horizontal");
    }
    public static float GetAxisRawVertical() {
        return Input.GetAxisRaw("Vertical");
    }

    // INTERACTIONS
    public static bool IsInteractionKeyPressed() {        
        return Input.GetKeyDown(interactButton);
    }
    public static bool IsInteractionKey() {
        return Input.GetKey(interactButton);
    }
    public static bool IsInteractionKeyReleased() {
        return Input.GetKeyUp(interactButton);
    }

    // OTHER
    public static bool IsKeyPressed(KeyCode keyCode) {
        return Input.GetKeyDown(keyCode);
    }

    // INVENTORY

    public static bool IsInventoryKeyPressed() {
        return Input.GetKeyDown(inventoryButton) || Input.GetKeyDown(inventoryButton2);
    }
    public static bool IsInventory1KeyPressed() {
        return Input.GetKeyDown(inventory1Button);
    }
    public static bool IsInventory2KeyPressed() {
        return Input.GetKeyDown(inventory2Button);
    }
    public static bool IsInventory3KeyPressed() {
        return Input.GetKeyDown(inventory3Button);
    }
    public static bool IsInventory4KeyPressed() {
        return Input.GetKeyDown(inventory4Button);
    }

    public static bool IsKey(KeyCode keyCode) {
        return Input.GetKey(keyCode);
    }
    public static bool IsKeyUp(KeyCode keyCode) {
        return Input.GetKeyUp(keyCode);
    }

    // MOUSE
    public static bool IsPickUpButtonPressed() {
        return Input.GetMouseButtonDown(0);
    }
    public static bool IsDropButtonPressed() {
        return Input.GetMouseButtonDown(1);
    }
    public static float GetMouseScrollValueY() {
        return Input.mouseScrollDelta.y;
    }
    public static float GetMouseScrollValueX() {
        return Input.mouseScrollDelta.x;
    }
    public static float GetMouseAxisY() {
        return Input.GetAxis("Mouse Y");
    }
    public static float GetMouseAxisX() {
        return Input.GetAxis("Mouse X");
    }

}
