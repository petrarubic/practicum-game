using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    [Required] public ItemSO item;

    public bool canFocus;
    public bool isInventoryItem;

    [Header("Pickup")]
    public float waitOnPickup = 0.2f;
    public float breakForce = 25f;

    public float itemWeight = 1f;

    private Vector3 startPos, startRot;

    [HideInInspector] public bool pickedUp = false;

    public UnityEvent onItemPickup, onItemDrop;


    private void Start() {
        startPos = transform.position;
        startRot = transform.eulerAngles;
    }

    private void Update() {
        if(Time.frameCount % 180 == 0) {
            if (transform.position.y < -150) {
                transform.position = startPos;
                transform.eulerAngles = startRot;
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (pickedUp) {
            if(collision.relativeVelocity.magnitude > breakForce) {
                DropItem();
            }
        }
    }

    public void DropItem() {
        pickedUp = false;
        onItemDrop?.Invoke();
    }

    public IEnumerator PickUp() {
        yield return new WaitForSecondsRealtime(waitOnPickup);
        pickedUp = true;
        onItemPickup?.Invoke();
    }
}
