
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

using FMODUnity;
using FMOD.Studio;
using static AudioGrandad;



public class SceneTransitions : MonoBehaviour
{

    private bool firstTimeStartup = true;

    public static SceneTransitions Instance { get; private set; }

    private bool isTransitioning = false;
    private int targetIndex = -1;


    public GameObject anim_ScreenChange;


    #region Audio Variables

    private static List<EventInstance> outInstances = new();

    #endregion



    #region Initialisation

    private void Start()
    {

        if (firstTimeStartup) AudioGrandad.Init();



        if (Instance != null) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        AudioGrandad.sceneTransitions = Instance;



        if (firstTimeStartup) StartCoroutine(Initialise_FromScene());

    }

    #endregion




    // golden boy
    public void TransitionTo(int buildIndex)
    {

        if (isTransitioning) return;

        if (buildIndex == -1)
        { Debug.LogError($"Invalid BuildIndex of {buildIndex}!!"); return; }



        isTransitioning = true;
        targetIndex = buildIndex;



        FadeOutSound();

        StartCoroutine(Transition());
                
    }



    private IEnumerator Transition()
    {



        // Store current EventInstances
        outInstances.Clear();
        outInstances.AddRange(allInstances);

        // stop 'em
        for (int i = 0; i < outInstances.Count; i++)
        {

            var ei = outInstances[i];
            if (!ei.isValid()) continue;

            ei.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        }
        // also stop emitters, if there are any
        if (allEmitters.Count > 0)
            foreach (var em in allEmitters)
                em.Stop();

        // UI_ScreenChange instantiation
        GameObject UI = Instantiate(anim_ScreenChange);
        Anim_ScreenChange anim = UI.GetComponent<Anim_ScreenChange>();        


        // await stopped
        for (int i = 0; i < outInstances.Count; i++)
        {

            var ei = outInstances[i];
            if (!ei.isValid()) continue;

            // waaait
            float timeout = 5f;
            while (ei.isValid() && !IsStopped(ei) && timeout > 0)
            {
                timeout -= Time.unscaledDeltaTime;
                yield return null;
            }

            // be free
            Release(ei);

        }
        outInstances.Clear();



        // load 'em
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (targetIndex != -1)
            LoadBanks_bySceneIndex(targetIndex, currentIndex, out _);



        // go where you need to go, king
        isTransitioning = false;
        var sceneChange = SceneManager.LoadSceneAsync(targetIndex, LoadSceneMode.Single);
        while (!sceneChange.isDone)
            yield return null;
        
        // wait and step to it
        yield return null;
        SceneStartup(targetIndex);

        // anim - bring screen back up
        anim.ReOpenScene();

    }


    private IEnumerator Initialise_FromScene()
    {

        firstTimeStartup = false;

        targetIndex = SceneManager.GetActiveScene().buildIndex;
        if (targetIndex != -1)
        {
            LoadBanks_bySceneIndex(targetIndex, -1, out List<string> banks);

            foreach (var bank in banks)
                while (!isBankLoaded(bank))
                    yield return null;

            yield return null;
            SceneStartup(targetIndex);
        }

    }




    



}
