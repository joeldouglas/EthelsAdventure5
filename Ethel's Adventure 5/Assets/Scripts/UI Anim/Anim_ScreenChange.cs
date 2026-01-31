using UnityEngine;
using System.Collections;

public class Anim_ScreenChange : MonoBehaviour
{

    public RectTransform leftPanel;
    public RectTransform rightPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        DontDestroyOnLoad(gameObject);

        leftPanel.offsetMin = new Vector2(-50, leftPanel.offsetMin.y);
        leftPanel.offsetMax = new Vector2(-1925, leftPanel.offsetMax.y);

        rightPanel.offsetMin = new Vector2(1925, rightPanel.offsetMin.y);
        rightPanel.offsetMax = new Vector2(-50, rightPanel.offsetMax.y);

        CloseScene();

    }

    public void CloseScene()
    {

        LeanTween.value(leftPanel.offsetMax.x, -900, 0.75f)
            .setEaseOutExpo()
            .setOnUpdate(value =>
            leftPanel.offsetMax = new Vector2(value, leftPanel.offsetMax.y));

        LeanTween.value(rightPanel.offsetMin.x, 900, 0.75f)
            .setEaseOutExpo()
            .setOnUpdate(value =>
            rightPanel.offsetMin = new Vector2(value, rightPanel.offsetMin.y));


    }

    /*IEnumerator TestPause()
    {
        yield return new WaitForSeconds(1);
        ReOpenScene();
    }*/

    public void ReOpenScene()
    {     
        LeanTween.value(leftPanel.offsetMax.x, -1925, 0.66f)
            .setEaseOutExpo()
            .setOnUpdate(value =>
            leftPanel.offsetMax = new Vector2(value, leftPanel.offsetMax.y));

        LeanTween.value(rightPanel.offsetMin.x, 1925, 0.66f)
            .setEaseOutExpo()
            .setOnUpdate(value =>
            rightPanel.offsetMin = new Vector2(value, rightPanel.offsetMin.y))
            .setOnComplete(() =>
            {
                Destroy(this.gameObject);
            });
    }

    
}
