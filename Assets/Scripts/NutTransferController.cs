using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NutTransferController : MonoBehaviour
{
    [SerializeField] Bolt[] _bolts;
    [SerializeField] AudioClip _puzzleCompleteSfx;
    [SerializeField] GameObject _levelCompletedPanel;

    public static event Action OnLevelCompleted;
    public static event Action<string> OnHintToDisplay;

    Bolt _from;
    Bolt _to;

    private void HandleBoltClicked(Bolt bolt)
    {
        if (_from == null)
        {
            if (bolt.IsEmpty())
            {
                OnHintToDisplay?.Invoke("Selected bolt has no nuts to transfer.");
                return;
            } else
            {
                _from = bolt;
                bolt.BeginTransfer();
            }
        }
        else if (_to == null)
        {
            if (bolt == _from)
            {
                OnHintToDisplay?.Invoke("Cannot transfer to the same bolt.");
                bolt.CancelTransfer();
                _from = null;
                return;
            }
            _to = bolt;
            TransferNut();
        }
    }

    private void TransferNut()
    {
        
        var transferredNut = _from.TopNut;
        if (_to.CanReceiveNut(transferredNut))
        {
            _from.CompleteTransfer();
            _to.ReceiveNut(transferredNut);
        } else
        {
            _from.CancelTransfer();
        }

        _from = null;
        _to = null;
    }

    void CheckLevelComplete(Nut _)
    {
        foreach (var bolt in _bolts)
        {
            if (!bolt.IsComplete() && !bolt.IsEmpty())
            {
                return;
            }
        }

        OnLevelCompleted?.Invoke();
    }

    private void OnEnable()
    {
        Bolt.OnBoltClicked += HandleBoltClicked;
        Bolt.OnNutInserted += CheckLevelComplete;
    }

    private void OnDisable()
    {
        Bolt.OnBoltClicked -= HandleBoltClicked;
        Bolt.OnNutInserted -= CheckLevelComplete;
    }
}