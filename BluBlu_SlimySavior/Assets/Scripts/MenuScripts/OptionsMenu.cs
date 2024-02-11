/*
 * Author: Matthew Minnett
 * Desc: Sliders control audio settings
 * Date Created: 2023/03/05
 */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public enum SliderType { Master, Music, SFX }

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Button to close options screen")]
    Button backBTN;

    [SerializeField]
    [Tooltip("Master audio slider")]
    Slider master;
    [SerializeField]
    [Tooltip("Music audio slider")]
    Slider music;
    [SerializeField]
    [Tooltip("Sound effects audio slider")]
    Slider sfx;

    private bool ready = false;
    public bool Ready {  set { ready = value; } }

    [SerializeField]
    [Tooltip("Pause menu canvas")]
    GameObject canvas;
    public GameObject Canvas { get { return canvas; } }

    private static OptionsMenu instance;
    public static OptionsMenu Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        canvas.SetActive(false); // set pause menu to inactive
    }

    /// <summary>
    /// Sets pause menu to inactive
    /// </summary>
    public void BackButton()
    {
        canvas.SetActive(false);
    }

    private void Update()
    {
        if (ready) // if game is ready for audio updates, update values of each mixer group to corresponding slider value
        {
            SoundManager.Instance.overallVolume = master.value;
            SoundManager.Instance.Mixer.SetFloat("Master", master.value);
            SoundManager.Instance.Mixer.SetFloat("Music", music.value);
            SoundManager.Instance.Mixer.SetFloat("Effects", sfx.value);
            SaveOptions();
        }
    }

    /// <summary>
    /// Called when the slider is moved. Corresponding value is updated to new value
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void UpdateSlider(SliderType type, float value)
    {
        switch(type)
        {
            case SliderType.Master:
                master.value = value;
                break;
            case SliderType.Music:
                music.value = value;
                break;
            case SliderType.SFX:
                sfx.value = value;
                break;
        }    
    }

    /// <summary>
    /// Save the new mixer values
    /// </summary>
    private void SaveOptions()
    {
        PlayerPrefs.SetFloat("master", master.value);
        PlayerPrefs.SetFloat("music", music.value);
        PlayerPrefs.SetFloat("sfx", sfx.value);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Load mixer values
    /// </summary>
    public void LoadOptions()
    {
        UpdateSlider(SliderType.Master, PlayerPrefs.GetFloat("master"));
        UpdateSlider(SliderType.Music, PlayerPrefs.GetFloat("music"));
        UpdateSlider(SliderType.SFX, PlayerPrefs.GetFloat("sfx"));

        ready = true;
    }

    /// <summary>
    /// Make options screen active
    /// </summary>
    public void OpenOptions()
    {
        canvas.SetActive(true);
    }
}
