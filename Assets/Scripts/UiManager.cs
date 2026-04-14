using System;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] GameObject _levelCompleteWindow;
    [SerializeField] HintManager _hintManager;

    private void HandleLevelCompleted()
    {
        _levelCompleteWindow.SetActive(true);
    }
    private void HandleHintDisplay(string hint)
    {
        _hintManager.DisplayHint(hint);
    }

    private void OnEnable()
    {
        NutTransferController.OnLevelCompleted += HandleLevelCompleted;
        NutTransferController.OnHintToDisplay += HandleHintDisplay;
    }


    private void OnDisable()
    {
        NutTransferController.OnLevelCompleted -= HandleLevelCompleted;
        NutTransferController.OnHintToDisplay -= HandleHintDisplay;
    }

}
