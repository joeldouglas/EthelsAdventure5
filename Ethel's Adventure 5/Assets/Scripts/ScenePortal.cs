using UnityEngine;
using UnityEngine.SceneManagement; // Required to change scenes!


public class DoorwayTrigger : MonoBehaviour
{
    [SerializeField] private int sceneIndexToLoad; // Set this in the Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that hit the door is the Player
        if (other.CompareTag("Player"))
        {
            // This "turns the key" in your SceneTransitions script
            SceneTransitions.Instance.TransitionTo(sceneIndexToLoad);
        }
    }
}