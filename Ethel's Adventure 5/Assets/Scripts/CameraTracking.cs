using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public float smoothness = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    void LateUpdate()
    {
        // Moves camera to player with smoothing
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothness);
    }
}