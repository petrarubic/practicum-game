using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName ="Item", menuName ="ScriptableObjects/Item")]
public class ItemSO : ScriptableObject
{
    public int itemID;
    public string itemName;
    [Multiline] public string itemDescription;
    [ShowAssetPreview] public Sprite itemImage;
    [ShowAssetPreview] public GameObject itemObject;
}
