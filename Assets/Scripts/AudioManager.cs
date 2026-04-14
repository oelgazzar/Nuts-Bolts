using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip _levelCompletedSfx;
    [SerializeField] AudioClip _boltCompleteSfx;
    [SerializeField] AudioClip _nutSpinSfx;
    
    AudioSource _audioSource;
    AudioClip _lastPlayedClip;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    private void HandleLevelCompleted()
    {
        PlayClip(_levelCompletedSfx);
    }
    private void HandleBoltCompleted()
    {
        PlayClip(_boltCompleteSfx);
    }

    void PlayClip(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
        _lastPlayedClip = clip;

    }

    private void HandleNutSpin(bool value)
    {
        if (value)
        {
            _audioSource.PlayOneShot(_nutSpinSfx);
        } else if (_lastPlayedClip == _nutSpinSfx)
        {
            _audioSource.Stop();
        }
    }

    private void OnEnable()
    {
        NutTransferController.OnLevelCompleted += HandleLevelCompleted;
        Bolt.OnBoltCompleted += HandleBoltCompleted;
        NutAnimationController.OnNutSpinChanged += HandleNutSpin;
    }

    private void OnDisable()
    {
        NutTransferController.OnLevelCompleted -= HandleLevelCompleted;
        Bolt.OnBoltCompleted -= HandleBoltCompleted;
        NutAnimationController.OnNutSpinChanged -= HandleNutSpin;
    }

}
