using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
using System.Collections;

using rtm = FMODUnity.RuntimeManager;
using STOP = FMOD.Studio.STOP_MODE;
using System;


public class _test_FMODEvent : MonoBehaviour
{

    public string testEvent;
    public string testMusic;

    private EventInstance TEST_Repeat;
    private EventInstance TEST_Stream;
    //private EventInstance TEST_OneShot;

    private List<EventInstance> allInstances = new();


    private List<string> banks = new List<string> 
    {
        "Master.strings", "Core", "Music"
    };



    private void Awake()
    {
        
        

    }


    private IEnumerator Start()
    {

        gameObject.name = "AudioGrandad";



        foreach (var b in banks)
            rtm.LoadBank(b, loadSamples: true);
        yield return null;



        var listener = FindAnyObjectByType<StudioListener>();
        Debug.Log(listener 
            ? $"Listener found on {listener.gameObject.name}" 
            : "NO LISTENER FOUND");



        rtm.PlayOneShot(_Path(testEvent));
        TryStart(testMusic, ref TEST_Stream);

        TEST_Stream.getPlaybackState(out var musicState);
        
        StartCoroutine(PauseAfter(TEST_Stream, 5));

    }



    private void TryStart(string eventRef, ref EventInstance inst, 
        bool triggerOnStop = false, Action OnStop = null)
    {

        var path = _Path(eventRef);

        // who?
        if (string.IsNullOrWhiteSpace(path))
        { Debug.LogError("Path is empty"); return; }

        // Create dat boy
        Debug.Log($"Attempting CreateInstance: '{path}'");                
        inst = _Create(path);

        // bad boy?
        if (!inst.isValid())
        { Debug.LogError($"CreateInstance({path}) = INVALID"); return; }               

        // good boy
        var result = inst.start();
        Debug.Log($"{path}.start() = {result}");

        // how are you boy?
        inst.getPlaybackState(out var state);
        Debug.Log($"{inst} PlaybackState{state}");
        if (result != FMOD.RESULT.OK)        
            Debug.LogError($"{path}.start() failed = {result}.");
        
    }



    IEnumerator PauseAfter(EventInstance ei, float f)
    {
        Debug.Log($"Waiting {f} seconds to pause");
        yield return new WaitForSeconds(f);
        
        ei.getPlaybackState(out var state);
        Debug.Log($"State = {state}");

        if (state == PLAYBACK_STATE.PLAYING)
        {
            Debug.Log($"Stopping event");
            ei.stop(STOP.ALLOWFADEOUT);
        }
        else Debug.LogError($"Playback state after pausing = {state}");

        yield return new WaitForSeconds(1);
        PlayError(ei);


    }


    private void PlayError(EventInstance killMe)
    {
        killMe.release();
        rtm.PlayOneShot(_Path("UI/TEST_Error"));

    }


    #region little elves

    string _Path(string t)
    {

        if (string.IsNullOrWhiteSpace(t)) return null;

        t = t.Trim();

        if (t.StartsWith("event:/"))
            return t;

        return "event:/" + t;

    }

    EventInstance _Create(EventReference er)
    {
        EventInstance ei = rtm.CreateInstance(er);
        allInstances.Add(ei);
        return ei;
    }
    EventInstance _Create(string t)
    {
        EventInstance ei = rtm.CreateInstance(t);
        allInstances.Add(ei);
        return ei;
    }

    EventReference _Get(string t)
    {
        return rtm.PathToEventReference(t);
    }

    #endregion



    private void OnDestroy()
    {
        // Clean up instances
        foreach (var ei in allInstances)
        {
            if (!ei.isValid()) continue;
            ei.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            ei.release();
        }
        allInstances.Clear();
    }

}
