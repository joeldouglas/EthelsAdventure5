using UnityEngine;

using FMODUnity;
using FMOD.Studio;
using static AudioGrandad;

public class EmitterHook : MonoBehaviour
{

    public StudioEventEmitter emitter;

    public bool travelling;
    public Transform travelSource;

    public enum TravelDirection { X, Y }
    public TravelDirection direction;

    private Vector3 original;

    private void Awake()
    {
        
        if (travelling) original = transform.position;

        if (emitter == null) emitter = GetComponent<StudioEventEmitter>();

        if (emitter != null)
            allEmitters.Add(emitter);

        emitter.Play();

    }


    private void Update()
    {
        if (travelling && travelSource != null)
            transform.position = direction switch
            {
                TravelDirection.X => new Vector3(travelSource.position.x, original.y, original.z),
                TravelDirection.Y => new Vector3(original.x, travelSource.position.y, original.z),
            };
    }


    private void OnDestroy()
    {
        allEmitters.Remove(emitter);
    }

}
