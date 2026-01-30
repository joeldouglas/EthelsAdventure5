using UnityEngine;
using UnityEngine.SceneManagement; // Required to change scenes!

public class ScenePortal : MonoBehaviour
{
    [Header("Settings")]
    public string sceneToLoad; // Type the name of the scene exactly as it appears in your project

    // This runs when something enters the "Trigger" area
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.tag);

        // Check if the object that touched us has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Load the scene we typed in the Inspector
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}