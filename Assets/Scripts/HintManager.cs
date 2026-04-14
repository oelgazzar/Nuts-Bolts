using DG.Tweening;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(CanvasGroup))]
public class HintManager : MonoBehaviour
{
    [SerializeField] TMP_Text _hintText;
    [SerializeField] Vector2 _displayPosition;
    [SerializeField] Vector2 _hidePosition;

    CanvasGroup _canvasGroup;
    RectTransform _rectTransform;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup.alpha = 0;
        _rectTransform.anchoredPosition = _hidePosition;
    }

    public void DisplayHint(string hint)
    {
        _hintText.text = hint;

        var seq = DOTween.Sequence();
        seq.Append(_canvasGroup.DOFade(1, .5f));
        seq.Join(_rectTransform.DOMoveY(_displayPosition.y, .5f));
        seq.AppendInterval(1);
        seq.Append(_canvasGroup.DOFade(0, .5f));
        seq.onComplete = () =>
        {
            _rectTransform.anchoredPosition = _hidePosition;
        };
        seq.Play();
    }

}
