using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using NUnit.Framework;


public class QuestPrize : MonoBehaviour
{

    public List<Mask> maskLibrary;

    [SerializeField] private GameObject prizePanel;
    [SerializeField] private TextMeshProUGUI prizeText;
    [SerializeField] private Image prizeImage;


    private Mask currentPendingMask;


    private void Start()
    {
        int pl = PlayerController.Instance.currentLevel;
        int min = pl;
        int max = pl + 2;
        max = Mathf.Clamp(max, min, maskLibrary.Count - 1);

        int randomTier = Random.Range(min, max);

        StartCoroutine(ShowPrizeSequence(randomTier));
    }



    private IEnumerator ShowPrizeSequence(int tierIndex)
    {
        yield return new WaitForSeconds(0.5f);

        Mask archetype = maskLibrary[Random.Range(0, maskLibrary.Count)];
        Mask runtimeMask = Instantiate(archetype);
        Mask.RarityData selected = archetype.rarityTiers[tierIndex];

        runtimeMask.finalCuteness = selected.cutenessValue;
        runtimeMask.finalFear = selected.fearValue;
        runtimeMask.finalColor = selected.tierColor;
        runtimeMask.runtimeDisplayName = $"{selected.rarityLabel} {archetype.maskTypeName} Mask";

        string hexColor = ColorUtility.ToHtmlStringRGB(runtimeMask.finalColor);
        if (prizeText != null)
            prizeText.text = $"<color=#{hexColor}>{runtimeMask.runtimeDisplayName}</color>\n" +
                             $"Cuteness: +{runtimeMask.finalCuteness} | Fear: +{runtimeMask.finalFear}";

        if (prizeImage != null) prizeImage.sprite = runtimeMask.maskIcon;
        currentPendingMask = runtimeMask;
        if (prizePanel != null) prizePanel.SetActive(true);

        TeamManager.Instance.SetTrayVisibility(true);
        foreach (var slot in TeamManager.Instance.slotUIs)
        {
            if (slot != null) slot.SetButtonState(true);
        }
    }

    public void TrashCurrentPrize()
    {
        Destroy(gameObject);
    }

    public void SelectCatForPrize(int index)
    {
        if (currentPendingMask == null) return;
        TeamManager.Instance.EquipMaskToCat(index, currentPendingMask);
    }


}
