using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : PlayerSystem
{
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    #region Events

    private void OnEnable()
    {
        player.ID.playerEvents.OnPlayerPlaySound += PlaySound;
    }

    private void PlaySound(AudioClip sound)
    {
        if (sound != null)
        {
            audioSource.Stop();
            audioSource.clip = sound;
            audioSource.Play();
        }
    }

    private void OnDisable()
    {
        player.ID.playerEvents.OnPlayerPlaySound -= PlaySound;
    }

    #endregion
}
