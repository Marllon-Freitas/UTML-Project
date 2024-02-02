using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    private float distanceToHearSoundEffect;

    [SerializeField]
    private AudioSource[] soundEffects;

    [SerializeField]
    private AudioSource[] backgroundMusic;

    public bool playBackgroundMusic;
    private int backgroundMusicIndex;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Update()
    {
        if (!playBackgroundMusic)
            StopAllBackgroundMusic();
        else
        {
            if (!backgroundMusic[backgroundMusicIndex].isPlaying)
                PlayBackgroundMusic(backgroundMusicIndex);
        }
    }

    public void PlaySoundEffect(int _soundEffectIndex, Transform _soundSource)
    {
        // if (soundEffects[_soundEffectIndex].isPlaying)
        //     return;

        if (
            _soundSource != null
            && Vector2.Distance(
                PlayerManager.instance.player.transform.position,
                _soundSource.position
            ) > distanceToHearSoundEffect
        )
            return;

        if (_soundEffectIndex < soundEffects.Length)
        {
            soundEffects[_soundEffectIndex].pitch = Random.Range(0.85f, 1.1f);
            soundEffects[_soundEffectIndex].Play();
        }
    }

    public void StopSoundEffect(int _soundEffectIndex) => soundEffects[_soundEffectIndex].Stop();

    public void PlayBackgroundMusic(int _backgroundMusicIndex)
    {
        backgroundMusicIndex = _backgroundMusicIndex;
        StopAllBackgroundMusic();

        if (_backgroundMusicIndex < backgroundMusic.Length)
            backgroundMusic[backgroundMusicIndex].Play();
    }

    public void StopAllBackgroundMusic()
    {
        for (int i = 0; i < backgroundMusic.Length; i++)
        {
            backgroundMusic[i].Stop();
        }
    }
}
