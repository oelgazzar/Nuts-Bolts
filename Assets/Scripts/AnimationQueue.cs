using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationQueue : MonoBehaviour
{
    public static AnimationQueue Instance { get; private set; }
    Queue<Sequence> _queue = new();

    bool _isRunning;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public void Enqueue(Sequence sequence)
    {
        _queue.Enqueue(sequence);
        if (!_isRunning)
        {
            StartCoroutine(Run());
        }
    }

    IEnumerator Run()
    {
        _isRunning = true;
        while (_queue.Count > 0)
        {
            var nextSequence = _queue.Dequeue();
            nextSequence.Play();
            yield return nextSequence.WaitForCompletion();
        }
        _isRunning = false;
    }
}
