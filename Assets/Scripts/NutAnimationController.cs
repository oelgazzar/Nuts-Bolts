using DG.Tweening;
using System;
using UnityEngine;

public class NutAnimationController : MonoBehaviour
{
    [SerializeField] float _transferAnimationDuration = .2f;
    [SerializeField] float _nutSpinAnimationDuration = .5f;
    [SerializeField] float _enterExitAnimationDuration = .2f;
    public static NutAnimationController Instance { get; private set; }

    public static event Action<bool> OnNutSpinChanged;

    private void Awake()
    {
        Instance = this;
    }

    public Sequence PlayTransferAnimation(Nut transferredNut, Bolt targetBolt)
    {
        var seq = DOTween.Sequence();
        var startPosition = transferredNut.transform.position;
        var endPosition = targetBolt.GetTopSlotPosition();
        var x = (startPosition.x + endPosition.x) / 2;
        var z = (startPosition.z + endPosition.z) / 2;
        var y = startPosition.y + 2;
        var midPoint = new Vector3(x, y, z);
        //seq.Append(transferredNut.transform.DOMove(midPoint, _transferAnimationDuration/2)
        //    .SetEase(Ease.InOutSine));
        seq.Append(transferredNut.transform.DOMove(endPosition, _transferAnimationDuration/2)
            .SetEase(Ease.InOutSine));
        AnimationQueue.Instance.Enqueue(seq);
        return seq;
    }

    public Sequence PlayExitAnimation(Nut transferredNut, Bolt sourceBolt)
    {
        var interval = _enterExitAnimationDuration - _nutSpinAnimationDuration;
        var seq = DOTween.Sequence();
        seq.Append(transferredNut.transform.DOMove(sourceBolt.GetTopSlotPosition(), _enterExitAnimationDuration)
            .SetEase(Ease.InOutSine));
        seq.Join(transferredNut.transform.DORotate(new Vector3(0, 180, 0), _nutSpinAnimationDuration)
            .SetEase(Ease.InOutSine)
            .OnStart(() => OnNutSpinChanged?.Invoke(true))
            .OnComplete(() => OnNutSpinChanged?.Invoke(false))
            );
        AnimationQueue.Instance.Enqueue(seq);
        return seq;
    }

    public Sequence PlayEnterAnimation(Nut transferredNut, Bolt targetBolt)
    {
        var seq = DOTween.Sequence();
        var seq1 = DOTween.Sequence();
        var seq2 = DOTween.Sequence();
        seq1.Append(transferredNut.transform.DOMove(targetBolt.GetInsertionPosition(), _enterExitAnimationDuration)
            .SetEase(Ease.InOutSine));
        seq2.AppendInterval(_enterExitAnimationDuration - _nutSpinAnimationDuration);
        seq2.Append(transferredNut.transform.DORotate(new Vector3(0, 0, 0), _nutSpinAnimationDuration)
            .SetEase(Ease.InOutSine)
            .OnStart(() => OnNutSpinChanged?.Invoke(true))
            .OnComplete(() => OnNutSpinChanged?.Invoke(false))
            );
        seq.Append(seq1);
        seq.Join(seq2);
        AnimationQueue.Instance.Enqueue(seq);
        return seq;
    }
}
