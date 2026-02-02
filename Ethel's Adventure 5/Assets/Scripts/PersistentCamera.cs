using UnityEngine;

public class PersistentCamera : MonoBehaviour
{
    public static PersistentCamera Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Deletes camera if in scene.
        }
    }
}