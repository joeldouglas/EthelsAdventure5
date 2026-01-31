// using UnityEngine;
// using System.Collections.Generic;

// [CreateAssetMenu(fileName = "Cat", menuName = "Scriptable Objects/Cat")]
// [System.Serializable]
// public class Cat : ScriptableObject
// {

//     [SerializeField] new public string name;
//     [SerializeField] public Sprite sprite;

//     [SerializeField] public int cuteness;
//     [SerializeField] public int fear;
    
//     [SerializeField] public Mask mask;
//     public bool maskActive => mask != null;




//     #region Constructors

//     // Random
//     public Cat(bool startsWithMask = false)
//     {
        
//         name = GetRandomName();
//         cuteness = Random.Range(0, 6);
//         fear = Random.Range(0, 6);
//         mask = startsWithMask ? new Mask() : null;

//     }

//     // Set Values
//     public Cat(string name, Sprite sprite, int cuteness, int fear, Mask mask, List<string> randomNames)
//     {

//         this.name = name;
//         this.sprite = sprite;
//         this.cuteness = cuteness;
//         this.fear = fear;
//         this.mask = mask;
//         this.randomNames = randomNames;

//     }

//     #endregion



//     #region Modifiers

//     public void Cuteness(int i) { cuteness += i; }
//     public void Fear(int i) { fear += i; }

//     #endregion



//     #region Name Pool

//     private List<string> randomNames = new List<string>
//     {
//         "Boris", "Doris", "Humphrey", "Bojack", 
//         "Bingo", "Bango", "Bongo", "Bungo",
//         "Chris", "Steve", "Derek", "Mark",
//         "Paws", "Mittens", "Snowball", "Fluffy",
//         "Ting", "Tang", "Wallawalla", "Bingbang",
//         "Priscilla", "Cruella", "Felicity", "Dog"
//     };

//     private string GetRandomName()
//     {
//         return randomNames[Random.Range(0, randomNames.Count)];
//     }

//     #endregion



// }

using UnityEngine;

[CreateAssetMenu(fileName = "NewCat", menuName = "Gacha/Cat Archetype")]
public class Cat : ScriptableObject
{
    [Header("Identity")]
    public string catName;
    public Sprite catSprite;

    [Header("Base Stats")]
    public int baseCuteness;
    public int baseFear;

    [Header("Equipment Slot")]
    // This is where the Gacha will "plug in" the mask instance
    public Mask equippedMask; 

    // CALCULATED STATS: 
    // These automatically add the mask bonus if one exists.
    public int TotalCuteness => baseCuteness + (equippedMask != null ? equippedMask.finalCuteness : 0);
    public int TotalFear => baseFear + (equippedMask != null ? equippedMask.finalFear : 0);

    // This makes it easy to "Equip" via code
    public void SetMask(Mask newMask)
    {
        equippedMask = newMask;
        Debug.Log($"{catName} is now wearing {newMask.runtimeDisplayName}");
    }
}