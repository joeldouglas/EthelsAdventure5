using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Call to change scene on interaction
    public void LoadSceneByIndex(int sceneIndex)
    {
        Debug.Log("Loading Scene: " + sceneIndex);
        SceneManager.LoadScene(sceneIndex);
    }

}