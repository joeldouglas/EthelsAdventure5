using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

using static AudioGrandad;
using FMODUnity;


public class TransitionTester : MonoBehaviour
{

    int buildIndex = -1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        buildIndex = SceneManager.GetActiveScene().buildIndex;

        StartCoroutine(Init_Audio());

    }


    IEnumerator Init_Audio()
    {

        AudioGrandad.Init();

        LoadBanks_bySceneIndex(buildIndex, -1, out var banks);

        foreach (var bank in banks)
            while (!isBankLoaded(bank))
                yield return null;        

        SceneStartup(buildIndex);

    }


    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)        
            SceneTransitions.Instance.TransitionTo(1);        
    }
}
