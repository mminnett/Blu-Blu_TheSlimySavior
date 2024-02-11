/*
 * Author: Matthew Minnett
 * Desc: Changes volume of music depending on game state
 * Date Created: 2023/03/05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    #region Variables
    [SerializeField]
    AudioSource source;

    [SerializeField]
    AudioMixer mixer;
    public AudioMixer Mixer { get { return mixer; } }

    [SerializeField]
    [Tooltip("Audio that plays on game over")]
    AudioClip gameOver;

    public float overallVolume;

    static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }
    #endregion

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Get level music, fade in
    /// </summary>
    public static void LevelLoadComplete()
    {
        AudioClip music = LevelManager.Instance.LevelMusic; // find level music

        if(music) // if level music exists
        {
            instance.source.clip = music;
            instance.source.Play(); // play music
        }

        instance.AudioFadeLevelStart(); // fade audio in
    }

    /// <summary>
    /// Fade value of master volume from start input to end input, over inputted time
    /// </summary>
    /// <param name="startVol"></param>
    /// <param name="endVol"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator LerpVolume(float startVol, float endVol, float time)
    {
        float currentVol = startVol;
        float currentTime = 0f;

        while(currentTime < time)
        {
            currentTime += Time.deltaTime;
            currentTime = Mathf.Clamp(currentTime, 0f, time);

            currentVol = Mathf.Lerp(startVol, endVol, currentTime / time);

            mixer.SetFloat("Master", currentVol);

            yield return null;
        }
    }

    /// <summary>
    /// Fade audio in
    /// </summary>
    public void AudioFadeLevelStart()
    {
        StartCoroutine(LerpVolume(-80f, overallVolume, 1f));
    }

    /// <summary>
    /// Fade audio out
    /// </summary>
    /// <returns></returns>
    public IEnumerator UnloadLevel()
    {
        yield return LerpVolume(overallVolume, -80f, 1f);
    }

    /// <summary>
    /// Play the game over music instead of level music
    /// </summary>
    public void GameOverAudio()
    {
        instance.source.clip = gameOver;
        instance.source.Play();
    }
}
