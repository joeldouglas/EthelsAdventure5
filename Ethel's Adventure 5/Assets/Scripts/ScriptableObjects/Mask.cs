
using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Mask", menuName = "Scriptable Objects/Mask")]
public class Mask : ScriptableObject
{

    public string adjective;
    public string maskName;
    new public string name => adjective + " " + maskName;



    public enum Type
    {
        Ballroom, Monocle, TopHat, Beard
    }
    public Type type;

    public Action<Cat> effect;



    public enum Rarity
    {
        Common, Uncommon, Rare, VeryRare, Legendary
    }
    public Rarity rarity;

    public int effectValue;



    #region Constructors

    // Random
    public Mask()
    {
        type = (Type)Random.Range(0, Enum.GetValues(typeof(Type)).Length);
        effect = GetEffect(type);

        rarity = (Rarity)Random.Range(0, Enum.GetValues(typeof(Rarity)).Length);
        effectValue = GetEffectValue(rarity);

        adjective = GetAdjective(rarity);
        maskName = type.ToString();
    }

    // Defined
    public Mask(Type type, Rarity rarity)
    {

        this.type = type;
        effect = GetEffect(type);

        this.rarity = rarity;
        effectValue = GetEffectValue(rarity);
                
        adjective = GetAdjective(rarity);
        maskName = type.ToString();

    }    

    #endregion



    #region Effects Functions

    public Action<Cat> GetEffect(Type t)
    {
        return t switch
        {
            Type.Ballroom => FX_Ballroom,
            Type.Monocle => FX_Monocle,
            Type.TopHat => FX_TopHat,
            Type.Beard => FX_Beard,
            _ => FX_None
        };
    }

    public void FX_None(Cat cat)
    {
        Destroy(cat);
    }

    public void FX_Ballroom(Cat cat)
    {
        cat.cuteness += effectValue;
    }

    public void FX_Monocle(Cat cat)
    {
        cat.fear += effectValue;
    }

    public void FX_TopHat(Cat cat)
    {
        cat.cuteness -= effectValue;
    }

    public void FX_Beard(Cat cat)
    {
        cat.fear -= effectValue;
    }

    #endregion



    #region Rarity Getter

    private int GetEffectValue(Rarity r)
    {
        return r switch
        {
            Rarity.Common => 1,
            Rarity.Uncommon => 2,
            Rarity.Rare => 3,
            Rarity.VeryRare => 4,
            Rarity.Legendary => 5,
            _ => 0
        };
    }

    #endregion



    #region Names

    public string GetAdjective(Rarity r)
    {
        var pool = AdjectivesByRarity[r];
        return pool[Random.Range(0, pool.Count)];
    }

    private Dictionary<Rarity, List<string>> AdjectivesByRarity =
        new Dictionary<Rarity, List<string>>
        {
            { Rarity.Common, new List<string>{ "Smelly", "Rubbish", "Silly", "Stupid" } },
            { Rarity.Uncommon, new List<string> { "Alright", "Fine", "Not Bad", "Average" } },
            { Rarity.Rare, new List<string> { "Pretty Good", "Quite Nice", "Noticeable" } },
            { Rarity.VeryRare, new List<string> { "Snazzy", "Cool", "So Good", "Nice Smelling" } },
            { Rarity.Legendary, new List<string> { "Awesome", "The Bee's Knees", "Radical", "Absolutely Rippin'" } }
        };

    #endregion



}
