using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName ="InteractionIcons", menuName = "ScriptableObjects/Interaction Icons")]
public class InteractionIconsSO : ScriptableObject
{
    [ShowAssetPreview]
    public Sprite interactionIcon;
    [ShowAssetPreview]
    public Sprite pickupableIcon;
    [ShowAssetPreview]
    public Sprite focusableIcon;
    [ShowAssetPreview]

    public Sprite doorIcon;

    [ShowAssetPreview]
    public Sprite puzzleTriggerIcon;


    public Sprite GetSprite(InteractionSpriteType interactionSpriteType) {
        switch (interactionSpriteType) {
            case InteractionSpriteType.None:
                return null;
            case InteractionSpriteType.Pickupable:
                return pickupableIcon;
            case InteractionSpriteType.Interactable:
                return interactionIcon;
            case InteractionSpriteType.Focusable:
                return focusableIcon;
            case InteractionSpriteType.Door:
                return doorIcon;
            default:
                return null;
        }
    }
}
