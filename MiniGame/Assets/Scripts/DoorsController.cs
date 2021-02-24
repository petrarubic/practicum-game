using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using VHS;
using System.Xml.Linq;
using System;

public class DoorsController : MonoBehaviour {
    public enum RotateAxis { X, Y, Z };
    public enum MovementAxis { X, Y, Z };
    public enum AngleTransform { right, forward, up };
    [Header("Rigidbody object")]
    [Required]public Rigidbody rb;
    [Tooltip("Put Default, Pickupable, Interactable and Focusable")][SerializeField] private LayerMask raycastLayer = ~0;
    [BoxGroup("Rotation")]
    public bool RotatingChange;
    [BoxGroup("Rotation")]
    [ShowIf("RotatingChange")]
    public RotateAxis rotateAxis;
    [BoxGroup("Rotation")]
    [ShowIf("RotatingChange")]
    [Tooltip("How much the object can be rotated")] public float minRot, maxRot;
    [BoxGroup("Rotation")]
    [ShowIf("RotatingChange")]
    public bool reverseRotateDirection;
    [BoxGroup("Rotation")]
    [ShowIf("RotatingChange")]
    public float rotateSensitivity = 150;

    [BoxGroup("Movement")]
    public bool MovingChange;
    [BoxGroup("Movement")]
    [ShowIf("MovingChange")]
    public MovementAxis movementAxis;
    [BoxGroup("Movement")]
    [ShowIf("MovingChange")]
    public AngleTransform angleTransform;
    [BoxGroup("Movement")]
    [ShowIf("MovingChange")]
    [Tooltip("How much should the object move (relative)")] public float minPos, maxPos;
    [BoxGroup("Movement")]
    [ShowIf("MovingChange")]
    public bool reverseMoveDirection;
    [BoxGroup("Movement")]
    [ShowIf("MovingChange")]
    public float moveSensitivity = 1;
    [BoxGroup("Movement")]
    [ShowIf("MovingChange")]
    [Tooltip("From which angle should it change how you drag the mouse to open a drawer")] public float changeAngle;

    [BoxGroup("Other settings")]
    [Tooltip("If Movement, should be smaller, if Rotation, slightly bigger")] public float maxSpeedOfOpening = 0.05f;
    [BoxGroup("Other settings")]
    [ReadOnly] public bool isHandEmpty;
    [BoxGroup("Other settings")]
    public bool isLocked;

    private bool opening;
    private float move = 0, rotate = 0;
    private float angle;
    private float startX;
    private float startY;
    private float startZ;
    private Vector2 lastCursorPosition;
    private new Camera camera;
    private float currRot;
    public string descriptionOnHover = "Doors";
    private void Start() {
        camera = Camera.main;
        startX = transform.position.x;
        startY = transform.position.y;
        startZ = transform.position.z;
        StartCoroutine(OpenControl());
    }

    private void Update() {
        if (InteractionManager.instance.heldItem == null) isHandEmpty = true;
        else isHandEmpty = false;


        if (InputManager.IsInteractionKeyReleased() && opening) {
            opening = false;
        }
    }
    internal void Hover() {
        if (isHandEmpty) {
            InteractionManager.instance.ShowInteractionIcon(InteractionSpriteType.Door);
            if (InputManager.IsInteractionKeyPressed() && !isLocked) {
                if (MovingChange) {
                    Vector3 playerDir = Player.instance.transform.position - transform.position;
                    switch (angleTransform) {
                        case AngleTransform.forward:
                            angle = Vector3.SignedAngle(playerDir, transform.forward, Vector3.up);
                            break;
                        case AngleTransform.up:
                            angle = Vector3.SignedAngle(playerDir, transform.up, Vector3.up);
                            break;
                        case AngleTransform.right:
                            angle = Vector3.SignedAngle(playerDir, transform.right, Vector3.up);
                            break;
                    }
                    Debug.Log("Move Angle: " + angle);
                } else if (RotatingChange) {
                    Vector3 playerDir = Player.instance.transform.position - transform.position;
                    angle = Vector3.SignedAngle(playerDir, transform.parent.right, Vector3.up);
                    Debug.Log("Rotate Angle: " + angle);
                    lastCursorPosition = Input.mousePosition;
                    if (rotateAxis == RotateAxis.Z) Utils.ChangeCursorState(true);
                }
                opening = true;
                Player.instance.SetCanControl(false);
                Player.instance.SetCanControlCamera(false);
            } else if (InputManager.IsInteractionKeyPressed() && isHandEmpty && isLocked && Player.instance.CanControlCamera()) {
                //play locked sound etc
            }
        }

    }

    public string GetDescription() {
        return descriptionOnHover;
    }


    public IEnumerator OpenControl() {
        bool stoppedBefore = false;
        move = 0;
        while (true) {
            if (opening) {
                stoppedBefore = false;
                if (MovingChange) {        
                    if(minPos > maxPos) move = Mathf.Clamp(move, maxPos, minPos);
                    else move = Mathf.Clamp(move, minPos, maxPos);
                    //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, move);
                    switch (movementAxis) {
                        case MovementAxis.X:
                            rb.MovePosition(new Vector3(startX + move, transform.position.y, transform.position.z));
                            break;
                        case MovementAxis.Y:
                            rb.MovePosition(new Vector3(transform.position.x, startY + move, transform.position.z));
                            break;
                        case MovementAxis.Z:
                            rb.MovePosition(new Vector3(transform.position.x, transform.position.y, startZ + move));
                            break;
                    }
                } 
            } else {
                if (!stoppedBefore) {
                    stoppedBefore = true;
                    Player.instance.SetCanControl(true);
                    Player.instance.SetCanControlCamera(true);
                    Utils.ChangeCursorState(false);
                }
            }
            yield return null;
        }
    }
    private void HandleDrag(Vector2 position) {
        if ((position - lastCursorPosition).magnitude < 0.1) return;

        Vector2 center = camera.WorldToScreenPoint(transform.position);

        var firstPoint = lastCursorPosition - center;
        var secondPoint = position - center;

        var a = Vector2.Distance(secondPoint, firstPoint);
        var c = secondPoint.magnitude;
        var b = firstPoint.magnitude;

        var rotation = (float)Mathf.Acos((a * a - b * b - c * c) / (-2 * b * c));
        rotation *= (float)(180 / Math.PI);
        //print("Rotation:" + rotation);
        rotation = InvertDirection(firstPoint, secondPoint) ? rotation : rotation * -1;
        rotation *= maxSpeedOfOpening;
        //if (rotation > 12) {
        //    return;
        //}
        if(minRot > maxRot) currRot = Mathf.Clamp(currRot + rotation, maxRot, minRot);
        else currRot = Mathf.Clamp(currRot + rotation, minRot, maxRot);
        //if (currRot + rotation < 0) {
        //    currRot = 0;
        //} else if (currRot + rotation > maxRot) {
        //    currRot = maxRot;
        //} else if(currRot + rotation < minRot) {
        //    currRot = minRot;
        //} else {
        //    currRot += rotation;
        //}
        if (reverseRotateDirection) {
            //transform.Rotate(new Vector3(0, 0, -rotation));
            rb.MoveRotation(transform.parent.rotation * Quaternion.Euler(new Vector3(0, 0, -currRot)));
        } else {
            //transform.Rotate(new Vector3(0, 0, rotation));
            rb.MoveRotation(transform.parent.rotation * Quaternion.Euler(new Vector3(0, 0, currRot)));
        }
        lastCursorPosition = position;
    }

    private bool InvertDirection(Vector2 first, Vector2 second) {
        bool verticalMovement = Math.Abs(first.x - second.x) < Math.Abs(first.y - second.y);
        if (verticalMovement) {
            if (first.x < 0) {
                return first.y < second.y;
            } else {
                return first.y > second.y;
            }
        } else {
            if (first.y < 0) {
                return first.x > second.x;
            } else {
                return first.x < second.x;
            }
        }
    }

    public void Unlock() {
        isLocked = false;
    }
}
