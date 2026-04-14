using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public partial class Bolt : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Transform _abovePoint;
    [SerializeField] Transform[] _insertionPoints;
    [SerializeField] Nut _nutPrefab;
    [SerializeField] NutColor[] _initialNutColors;
    [SerializeField] int _maxNuts = 4;
    [SerializeField] int _initialNuts = 0;
    [SerializeField] AudioClip _nutSpinSfx;
    [SerializeField] AudioClip _completionSFx;
    [SerializeField] ParticleSystem[] _completionEffects;

    public static event Action<Bolt> OnBoltClicked;
    public static event Action<Nut, int, bool> OnNutInserted;
    public static event Action OnBoltCompleted;

    readonly Stack<Nut> _nuts = new();

    public Nut TopNut => _nuts.Count > 0 ? _nuts.Peek() : null;

    private void Start()
    {
        if (_initialNuts > _maxNuts)
        {
            Debug.LogError("Initial nuts cannot be greater than max nuts.");
            _initialNuts = _maxNuts;
        }

        for (int i = 0; i < _initialNutColors.Length; i++)
        {
            AddNut(i, _initialNutColors[i]);
        }
    }

    private void AddNut(int index, NutColor nutColor)
    {
        var nut = Instantiate(_nutPrefab, _insertionPoints[index].position, Quaternion.identity);
        nut.SetColor(ColorMapper.GetColor(nutColor));
        _nuts.Push(nut);
    }

    public Nut Peek()
    {
        if (_nuts.Count == 0) return null;
        return _nuts.Peek();
    }

    public Nut Pop()
    {
        var nut = _nuts.Pop();
        NutAnimationController.Instance.PlayExitAnimation(nut, this);
        return nut;
    }

    public void Push(Nut transferredNut)
    {
        var nutsCount = _nuts.Count;
        var isNutMatch = nutsCount > 0 && _nuts.Peek().IsSameColorAs(transferredNut);

        _nuts.Push(transferredNut);
        NutAnimationController.Instance.PlayEnterAnimation(transferredNut, this)
            .OnComplete(() =>
            {
                Debug.Log($"nutsCount: {nutsCount}");
                OnNutInserted?.Invoke(transferredNut, nutsCount, isNutMatch);
                if (++nutsCount == _maxNuts)
                    CheckComplete();
            });
    }

    public bool IsEmpty()
    {
        return _nuts.Count == 0;
    }

    public bool IsComplete()
    {
        if (_nuts.Count < _maxNuts) return false;

        var topNut = _nuts.Peek();
        foreach (var nut in _nuts)
        {
            if (!nut.IsSameColorAs(topNut)) return false;
        }

        return true;
    }

    public bool CanReceiveNut(Nut transferredNut)
    {
        if (transferredNut == null) return false;

        var topNutColor = _nuts.Count > 0 ? _nuts.Peek().Color : (Color?)null;
        return _nuts.Count < _maxNuts && (topNutColor == null || topNutColor == transferredNut.Color);
    }

    private void CheckComplete()
    {
        if (!IsComplete())
            return;

        var color = _nuts.Peek().Color;
        foreach (var effect in _completionEffects)
        {
            var main = effect.main;
            main.startColor = color;
            effect.Play();
        }

        OnBoltCompleted?.Invoke();
    } 

    public Vector3 GetTopSlotPosition()
    {
        return _abovePoint.position;
    }

    public Vector3 GetInsertionPosition()
    {
        return _insertionPoints[_nuts.Count - 1].position;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnBoltClicked?.Invoke(this);
    }
}
