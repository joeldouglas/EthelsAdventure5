using UnityEngine;

[CreateAssetMenu(fileName = "NewCat", menuName = "Catsino/Cat")]
public class Cat : ScriptableObject
{
    [Header("Bio")]
    public string catName;
    public Sprite catSprite;

    [Header("Base Stats")]
    public int baseCuteness;
    public int baseFear;

    [Header("Equipment")]
    public Mask equippedMask; // The mask currently worn

    // --- CALCULATED STATS ---
    // These automatically check if a mask is equipped and add its stats
    public int TotalCuteness
    {
        get 
        { 
            if (equippedMask != null) return baseCuteness + equippedMask.finalCuteness;
            return baseCuteness;
        }
    }

    public int TotalFear
    {
        get 
        { 
            if (equippedMask != null) return baseFear + equippedMask.finalFear;
            return baseFear;
        }
    }

    // --- METHODS ---
    public void EquipMask(Mask newMask)
    {
        equippedMask = newMask;
        // Logic here if you need to trigger specific effects later
    }
}