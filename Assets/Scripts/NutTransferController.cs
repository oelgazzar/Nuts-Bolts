using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NutTransferController : MonoBehaviour
{
    [SerializeField] Bolt[] _bolts;
    [SerializeField] AudioClip _puzzleCompleteSfx;
    [SerializeField] GameObject _levelCompletedPanel;

    public static event Action OnLevelCompleted;
    public static event Action<string> OnHintToDisplay;
    public static event Action<Nut> NutTransfered;

    readonly Stack<TransferGroupCommand> _transferHistory = new();

    Bolt _sourceBolt;
    Bolt _targetBolt;
    Nut _transferredNut;

    private void HandleBoltClicked(Bolt bolt)
    {
        if (_sourceBolt == null)
        {
            // Skip if bolt is empty
            if (!bolt.IsEmpty())
            {
                _sourceBolt = bolt;
                _transferredNut = bolt.Pop();
            }
        }

        else
        {
            if (bolt == _sourceBolt)
            {
                // Return to same bolt and reset
                bolt.Push(_transferredNut);
                _sourceBolt = null;
                _transferredNut = null;
                return;
            }

            // Otherwise, try transfer
            _targetBolt = bolt;
            TryTransferNut();
        }
    }

    private void TryTransferNut()
    {
        TransferGroupCommand transferGroup = new();

        if (_targetBolt.CanReceiveNut(_transferredNut))
        {
            var transferCommand = new TransferCommand(_sourceBolt, _targetBolt, _transferredNut);
            transferGroup.AddTransfer(transferCommand);
            transferCommand.Execute();

            // Group Transfer
            while (!_sourceBolt.IsEmpty() &&
                _targetBolt.CanReceiveNut(_sourceBolt.Peek()))
            {
                _transferredNut = _sourceBolt.Pop();
                transferCommand = new TransferCommand(_sourceBolt, _targetBolt, _transferredNut);
                transferGroup.AddTransfer(transferCommand);
                transferCommand.Execute();
            }
        }
        else
        {
            _sourceBolt.Push(_transferredNut);
        }

        _transferHistory.Push(transferGroup);

        _sourceBolt = null;
        _targetBolt = null;
        _transferredNut = null;
    }

    public void Undo()
    {
        if (_transferHistory.Count == 0) return;

        Debug.Log("undoing..");


        var lastTransferGroup = _transferHistory.Pop();
        lastTransferGroup.Undo();
    }

    void CheckWin()
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
        Bolt.OnBoltCompleted += CheckWin;
    }

    private void OnDisable()
    {
        Bolt.OnBoltClicked -= HandleBoltClicked;
        Bolt.OnBoltCompleted -= CheckWin;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 20), "Undo")) {
            Undo();
        }
    }
}

public class TransferCommand : ICommand
{
    readonly Bolt _sourceBolt;
    readonly Bolt _targetBolt;
    readonly Nut _transferredNut;

    public TransferCommand(Bolt sourceBolt, Bolt targetBolt, Nut transferredNut)
    {
        _sourceBolt = sourceBolt;
        _targetBolt = targetBolt;
        _transferredNut = transferredNut;
    }

    public void Execute()
    {
        NutAnimationController.Instance.PlayTransferAnimation(_transferredNut, _targetBolt);
        _targetBolt.Push(_transferredNut);
    }

    public void Undo()
    {
        _targetBolt.Pop();
        NutAnimationController.Instance.PlayTransferAnimation(_transferredNut, _sourceBolt);
        _sourceBolt.Push(_transferredNut);
    }
}

public class TransferGroupCommand : ICommand
{
    readonly List<TransferCommand> _commands;

    public TransferGroupCommand()
    {
        _commands = new List<TransferCommand>();
    }

    public void AddTransfer(TransferCommand command)
    {
        _commands.Add(command);
    }

    public void Execute()
    {
        foreach (var command in _commands)
        {
            command.Execute();
        }
    }

    public void Undo()
    {
        for (var i = _commands.Count - 1; i >= 0; i--)
        {
            _commands[i].Undo();
        }
    }
}


public interface ICommand
{
    void Execute();
    void Undo();
}