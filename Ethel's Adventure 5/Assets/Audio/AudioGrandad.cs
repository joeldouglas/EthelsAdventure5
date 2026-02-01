using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using db = UnityEngine.Debug;

using FMOD;
using FMODUnity;
using FMOD.Studio;
using rtm = FMODUnity.RuntimeManager;

using static SceneTransitions;

public static class AudioGrandad
{

    private static bool DB = true;


    #region Initialisation

    public static void Init()
    {

        // Load initial banks
        foreach (var bank in banks_Startup)
        {
            if (DB) db.Log($"Loading startup bank: {bank}");
            rtm.LoadBank(bank);
        }

    }

    #endregion



    /// <summary>
    /// 
    ///         SCENE INDEXES
    ///         
    ///         [0]     Tutorial
    ///         [1]     BackAlley
    ///         [2]     Beach
    ///         [3]     Catsino Exterior
    ///         [4]     Catsino Lobby
    ///         [5]     Forest
    ///         [6]     Game Over
    ///         [7]     Menu
    ///         [8]     Sabine Shop
    ///         [9]     Win Scene
    ///         [99]    FOR ME ONLY -- Battle scene
    /// 
    /// </summary>



    #region Bank Management



    public static string BankPath(string path)
    {
        return "bank:/" + path;        
    }

    // Hard-coded Banks list per Scene
    private static Dictionary<int, List<string>> banks_bySceneIndex =
        new Dictionary<int, List<string>>
        {
            { 0, new List<string>   { "0_Tutorial" } },
            { 1, new List<string>   {  }  },
            { 2, new List<string>   { "2_Beach" } },
            { 8, new List<string>   { "8_SabineShop" } },
            { 99, new List<string>  { "X_Battle" } }
        };



    // Getter (Scene BuildIndex)
    public static List<string> Get_SceneIndex_Banks(int i)
    {

        if (!banks_bySceneIndex.TryGetValue(i, out List<string> banks))
        { db.LogWarning($"No banks to retrieve at BuildIndex {i}"); return new(); }
        
        else 
        {
            if (DB) db.Log($"Successfully retrieved Bank paths for Scene {i}");
            return banks;
        }

    }



    // Load Banks of Next Scene, Unload Banks of Current Scene that are no longer needed
    public static void LoadBanks_bySceneIndex(int to, int from, out List<string> toBanks)
    {

        if (!banks_bySceneIndex.TryGetValue(to, out toBanks))
        {
            db.LogWarning($"Oopsie no audio banks loaded for BuildIndex {to}");
            toBanks = new();
        }

        else
            foreach (var path in toBanks)
            {
                if (!isBankLoaded(path))
                {
                    db.Log($"Loading new bank: {path}");
                    rtm.LoadBank(path, loadSamples: true);
                }
                else if (DB) db.Log($"Bank {path} is already loaded");
            }




        if (!banks_bySceneIndex.TryGetValue(from, out var fromBanks))
        {
            if (from != -1)
            { db.LogWarning($"No audio banks from current scene - BuildIndex {from}"); }
        }
        else
            foreach (var path in fromBanks)
            {
                if (path == "Master.strings" ||
                    path == "Core")
                { 
                    if (DB) db.Log($"Skipping {path}"); 
                    continue; 
                }
                else if (!toBanks.Contains(path))
                {
                    if (DB) db.Log($"Unloading {path}");
                    rtm.UnloadBank(path);
                }
            }

    }



    // Banks for Startup
    private static List<string> banks_Startup = new List<string>
    {
        "Core.strings", "Core"
    };



    public static bool isBankLoaded(string path)
    {

        var system = rtm.StudioSystem;

        var result = system.getBank(BankPath(path), out Bank bank);
        if (result != RESULT.OK)
        {
            db.LogWarning($"Bank {path} is not loaded");
            return false;

        }

        if (DB) db.Log($"Bank {path} is successfully loaded");
        return bank.isValid();

    }



    #endregion



    #region Event Instances

    public static List<EventInstance> allInstances = new();

    private static EventInstance currentMusicInstance;
    private static EventInstance currentAmbienceInstance;



    // hard-coded Events to initialise on scene startup
    /// <summary>
    /// 
    ///         SCENESTART_EVENT PATHS
    ///         
    ///         The Events to play instantly on Scene load
    /// 
    /// </summary>
    private static Dictionary<int, List<string>> sceneStart_EventPaths = 
        new Dictionary<int, List<string>>
        {
            { 0, new List<string> { "MUS/Tutorial" } },
            { 1, new List<string> {  } },
            { 2, new List<string> {  } },
            { 3, new List<string> {  } },
            { 4, new List<string> { "AMB/Casino"  } },
            { 5, new List<string> {  } },
            { 6, new List<string> {  } },
            { 7, new List<string> {  } },
            { 8, new List<string> { "MUS/SabineShop" } },
            { 9, new List<string> {  } },
            { 99, new List<string> { "MUS/Battle_1" } }
        };

    /* EventReferences dictionary might not actually be necessary
        public static Dictionary<int, List<EventReference>> sceneStart_EventRefs =
        new Dictionary<int, List<EventReference>>();    
    */




    public static EventInstance Create(string path)
    {
        
        if (DB) db.Log($"Creating {path}");

        var ei = rtm.CreateInstance(Path(path));
        Register(ei);
        return ei;

    }



    public static void OneShot(string path)
    {
        rtm.PlayOneShot(Path(path));
    }

    public static void OneShot_Param(string path, string parameter = "", float value = -99)
    {
        var ei = rtm.CreateInstance(Path(path));

        if (!string.IsNullOrEmpty(parameter))
            if (value != -99)
                ei.setParameterByName(parameter, value);

        ei.start();
        ei.release();
    }





    public static void Parameter(EventInstance ei, string parameter, float value)
    {
        ei.setParameterByName(parameter, value);
    }



    public static void Release(EventInstance ei)
    {
        if (ei.isValid())
        {
            if (DB) db.Log($"Releasing {EventPath(ei)}");

            ei.release();
            Unregister(ei);
        }
    }



    public static void Register(EventInstance ei) 
    { 
        if (DB) db.Log($"Added {EventPath(ei)} to allInstances");
        allInstances.Add(ei); 
    }

    public static void Unregister(EventInstance ei)
    {
        for (int i = allInstances.Count - 1; i >= 0; i--)
            if (allInstances[i].handle == ei.handle)
            {
                if (DB) db.Log($"Unregisted {EventPath(allInstances[i])}");
                allInstances.RemoveAt(i);                 
                return; 
            }
    }

    



    public static bool IsStopped(EventInstance ei)
    {

        if (!ei.isValid())
        {
            if (DB) db.Log($"{EventPath(ei)} has stopped");
            return true;
        }

        ei.getPlaybackState(out var state);
        if (DB) db.Log($"{EventPath(ei)} state = {state}");
        return state == PLAYBACK_STATE.STOPPED;

    }



    public static string EventPath(EventInstance ei)
    {
        if (!ei.isValid()) return "<invalid>";

        var r1 = ei.getDescription(out var desc);
        if (r1 != FMOD.RESULT.OK || !desc.isValid()) return "<no description>";

        var r2 = desc.getPath(out string path);
        if (r2 != FMOD.RESULT.OK) return "<unknown path>";

        return path;
    }



    #endregion



    #region Transition Handler

    public static SceneTransitions sceneTransitions;

    private static string sceneFadeOutPath = "SFX/SceneTransition";
    private static EventReference sceneFadeIn;    



    /* moved to SceneTransitions Transition routine
    public static void TransitionOut(int index)
    {

        var ai = new List<EventInstance>(allInstances);
        foreach (var ei in ai)
            if (ei.isValid())
                ei.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

    }
    */


    public static void FadeOutSound()
    {
        OneShot(sceneFadeOutPath);
        if (DB) db.Log($"Playing OneShot: sceneFadeOutPath");

        /*EventInstance ei = Create(sceneFadeOutPath);
        ei.start();
        if (DB) db.Log($"Started FadeOutSound as OneShot: {EventPath(ei)}");*/
    }


    public static void SceneStartup(int i)
    {
        List<EventInstance> instancesToStart = new();
        if (sceneStart_EventPaths.TryGetValue(i, out var refs))
            foreach (var er in refs)
            {
                EventInstance ei = Create(er);
                ei.start();
                if (DB) db.Log($"Started {ei.GetType().Name} to start scene {i}");
            }
        else db.LogWarning($"No Event References to start scene {i}");        
    }



    #endregion



    #region Event References

    public static EventReference Path(string path)
    {
        string newPath = "event:/" + path;  
        if (DB) db.Log($"Converted {path} to {newPath}");
        return rtm.PathToEventReference(newPath);
    }

    #endregion



    #region Spatialised Events

    public static List<StudioEventEmitter> allEmitters = new();

    #endregion



}
