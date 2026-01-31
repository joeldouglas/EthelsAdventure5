using UnityEngine;

public class SpacebarTransition : MonoBehaviour
{
    [SerializeField] private int sceneIndexToLoad;

    void Update()
    {
        // Check for Spacebar press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (SceneTransitions.Instance != null)
            {
                Debug.Log("Space pressed! Triggering transition via Manager.");
                SceneTransitions.Instance.TransitionTo(sceneIndexToLoad);
            }
            else
            {
                Debug.LogError("SceneTransitions script not found in scene!");
            }
        }
    }
}