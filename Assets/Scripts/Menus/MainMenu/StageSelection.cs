using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelection : MonoBehaviour
{
    [SerializeField] private List<Sprite> stageSprites;
    [SerializeField] private Image stageImage;

    private int currentIndex;

    private void OnEnable()
    {
        if(MatchData.SelectedMap != null)
        {
            stageImage.sprite = stageSprites[MatchData.SelectedMap + 1 ?? 0];
            currentIndex = MatchData.SelectedMap - 1 ?? 0;
        }
        else
        {
            stageImage.sprite = stageSprites[0];
            currentIndex = 0;
        }
    }

    public void MoveForward()
    {
        currentIndex++;

        if(currentIndex >= stageSprites.Count)
        {
            currentIndex = 0;
        }

        stageImage.sprite = stageSprites[currentIndex];
        MatchData.SelectedMap = currentIndex > 0 ? currentIndex - 1 : null;
    }

    public void MoveBackward()
    {
        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = stageSprites.Count -1;
        }

        stageImage.sprite = stageSprites[currentIndex];
        MatchData.SelectedMap = currentIndex > 0 ? currentIndex - 1 : null;
    }
}
