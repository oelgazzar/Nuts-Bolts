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
    public static event Action<Nut> OnNutInserted;
    public static event Action OnBoltCompleted;

    readonly Stack<Nut> _nuts = new();

    public Nut TopNut => _nuts.Count > 0 ? _nuts.Peek() : null;

    AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

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

    public void OnPointerClick(PointerEventData eventData)
    {
        OnBoltClicked?.Invoke(this);
    }

    public void BeginTransfer()
    {
        var nut = _nuts.Peek();
        var seq = DOTween.Sequence();
        seq.Append(nut.transform.DOMove(_abovePoint.position, .3f)
            .SetEase(Ease.InOutSine));
        seq.Join(nut.transform.DORotate(new Vector3(0, 180, 0), .2f)
            .SetEase(Ease.InOutSine));
        seq.OnStart(() =>
        {
            _audioSource.PlayOneShot(_nutSpinSfx);
        });
        AnimationQueue.Instance.Enqueue(seq);
    }

    public void CancelTransfer()
    {
        var nut = _nuts.Peek();
        var seq = DOTween.Sequence();
        var seq1 = DOTween.Sequence();
        var seq2 = DOTween.Sequence();
        seq1.Append(nut.transform.DOMove(_insertionPoints[_nuts.Count - 1].position, .3f)
            .SetEase(Ease.InOutSine));
        seq2.AppendInterval(.1f);
        seq2.Append(nut.transform.DORotate(new Vector3(0, 0, 0), .2f)
            .SetEase(Ease.InOutSine));
        seq.Append(seq1);
        seq.Join(seq2);
        seq.OnStart(() =>
        {
            _audioSource.PlayOneShot(_nutSpinSfx);
        });
        AnimationQueue.Instance.Enqueue(seq);
    }


    public bool IsEmpty()
    {
        return _nuts.Count == 0;
    }

    public bool CanReceiveNut(Nut transferredNut)
    {
        var topNutColor = _nuts.Count > 0 ? _nuts.Peek().Color : (Color?)null;
        return _nuts.Count < _maxNuts && (topNutColor == null || topNutColor == transferredNut.Color);
    }

    public void CompleteTransfer()
    {
        _nuts.Pop();
    }

    public void ReceiveNut(Nut transferredNut)
    {
        _nuts.Push(transferredNut);
        var seq = DOTween.Sequence();
        seq.Append(transferredNut.transform.DOMove(_abovePoint.position, .3f)
            .SetEase(Ease.InOutSine));
        seq.AppendCallback(() =>
        {
            _audioSource.PlayOneShot(_nutSpinSfx);
        });
        var insertionIndex = _nuts.Count - 1;
        seq.Append(transferredNut.transform.DOMove(_insertionPoints[insertionIndex].position, .3f)
            .SetEase(Ease.InOutSine));
        seq.Join(transferredNut.transform.DORotate(new Vector3(0, 0, 0), .3f).SetEase(Ease.InOutSine));
        seq.onComplete = () =>
        {
            OnNutInserted?.Invoke(transferredNut);
            CheckComplete();
        };
        AnimationQueue.Instance.Enqueue(seq);
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
}
