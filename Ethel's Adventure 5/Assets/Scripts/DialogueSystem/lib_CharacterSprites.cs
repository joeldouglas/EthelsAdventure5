using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class lib_CharacterSprites : MonoBehaviour
{

    public Sprite Ethel;
    public Sprite Marge;
    public Sprite Sabine;
    public Sprite Robin;
    public Sprite Tabby;
    public Sprite Toby;    
    public Sprite uhoh;

    public Sprite GetSprite(string t)
    {
        return t switch
        {
            "Ethel" => Ethel,
            "Marge" => Marge,
            "Sabine" => Sabine,
            "Robin" => Robin,
            "Tabby" => Tabby,
            "Toby" => Toby,
            _ => uhoh
        };
    }

    /* the way it should be done but cba to do resource path referencing
    public static Dictionary<string, Sprite> libSprites = new Dictionary<string, Sprite>
    {
        { "Ethel", null },
        { "Marge", null }
    };
    */
}
