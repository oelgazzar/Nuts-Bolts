using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip _levelCompletedSfx;
    [SerializeField] AudioClip _boltCompleteSfx;
    [SerializeField] AudioClip _nutSpinSfx;
    
    AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    private void HandleLevelCompleted()
    {
        _audioSource.PlayOneShot(_levelCompletedSfx);
    }
    private void HandleBoltCompleted()
    {
        _audioSource.PlayOneShot(_boltCompleteSfx);
    }

    private void OnEnable()
    {
        NutTransferController.OnLevelCompleted += HandleLevelCompleted;
        Bolt.OnBoltCompleted += HandleBoltCompleted;
    }

    private void OnDisable()
    {
        NutTransferController.OnLevelCompleted -= HandleLevelCompleted;
        Bolt.OnBoltCompleted -= HandleBoltCompleted;
    }

}
