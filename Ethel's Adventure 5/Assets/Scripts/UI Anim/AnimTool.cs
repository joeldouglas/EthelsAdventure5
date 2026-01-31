using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using static AnimTool.Anim;

public class AnimTool : MonoBehaviour
{

    [Header("Animation Control")]
    public bool autoplay;
    private bool stopped;

    public enum Anim
    {
        None,

        Swelling,
        Turning,
        SpinningX,
        SpinningY,
        

        // One-Shots
        HoverUp,
        SwellUp

    }
    public Anim anim;
    private HashSet<Anim> oneShots = new HashSet<Anim>
    {
        None, HoverUp
    };

    [Header("Animation Variables")]
    public float speed;
    public float delay;
    public float amount;

    private Vector3 origPos;
    private Quaternion origRot;
    private Vector3 origScale;



    #region testing
    private bool testing = false;
    private Anim testChange = None;
    #endregion


    
    void Start()
    {

        if (testing) { testChange = anim; StartCoroutine(Test_Changes()); }

        origPos = gameObject.transform.position;
        origRot = transform.rotation;
        origScale = transform.localScale;

        if (autoplay)
        {

            if (oneShots.Contains(anim))
            {
                autoplay = false;
                stopped = true;
                return;
            }

            Play();
        }
        else stopped = true;
        
    }



    #region Playback

    public void Play()
    {

        stopped = false;

        switch (anim)
        {
            case Anim.None:
                return;

            case Anim.Swelling: Swelling(); break;

            case Anim.Turning: Turning(); break;

            case Anim.SpinningX: SpinningX(); break;

            case Anim.SpinningY: SpinningY(); break;

        }

    }

    private void Stop()
    {
        stopped = true;

        LeanTween.cancelAll(gameObject);

        transform.position = origPos;
        transform.rotation = origRot;
        transform.localScale = origScale;
    }

    #endregion



    #region Effects

    void Swelling()
    {

        if (stopped) return;

        Vector3 full = new Vector3(origScale.x + amount, origScale.y + amount, origScale.z + amount);

        LeanTween.scale(gameObject, full, speed)
            .setDelay(delay)
            .setOnComplete(() =>
            {
                LeanTween.scale(gameObject, origScale, speed)
                .setDelay(delay)
                .setOnComplete(() =>
                {
                    Swelling();
                });
            });

    }



    void Turning()
    {

        if (stopped) return;

        LeanTween.rotateZ(gameObject, amount, speed)
            .setDelay(delay)
            .setOnComplete(() =>
            {
                LeanTween.rotateZ(gameObject, -amount, speed * 2)
                    .setDelay(delay)
                    .setOnComplete(() =>
                    {
                        Turning();
                    });
            });

    }



    void SpinningX()
    {

        if (stopped) return;

        LeanTween.rotateX(gameObject, 360, speed)
            .setEaseInOutQuint()
            .setDelay(delay)
            .setOnComplete(() =>
            {
                SpinningX();
            });

    }


    void SpinningY()
    {

        if (stopped) return;

        LeanTween.rotateX(gameObject, 360, speed)
            .setEaseInOutQuint()
            .setDelay(delay)
            .setOnComplete(() =>
            {
                SpinningY();
            });

    }



    #region OneShots

    public void HoverUp_In()
    {

        LeanTween.moveLocalY(gameObject, amount, speed)
            .setEaseOutQuad();            

    }
    public void HoverUp_Out()
    {

        LeanTween.moveLocalY(gameObject, -amount, speed)
            .setEaseOutQuad();

    }



    public void SwellUp_In()
    {

        Vector3 full = new Vector3(origScale.x + amount, origScale.y + amount, origScale.z + amount);

        LeanTween.scale(gameObject, full, speed)
            .setEaseOutQuad();

    }
    public void SwellUp_Out()
    {

        LeanTween.scale(gameObject, origScale, speed)
            .setEaseOutQuad();

    }


    #endregion


    #endregion





    #region Testing checker

    IEnumerator Test_Changes()
    {
        if (anim != testChange)
        {
            Stop();
            testChange = anim;
            Play();
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Test_Changes());

    }

    #endregion


}
