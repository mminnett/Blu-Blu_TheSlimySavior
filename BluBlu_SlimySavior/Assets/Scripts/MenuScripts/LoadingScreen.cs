/*
 * Author: Matthew Minnett
 * Desc: Updates loading screen slider
 * Date Created: 2023/03/05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Loading progress slider")]
    Slider slider;

    private void OnEnable()
    {
        slider.value = 0;
    }
    
    /// <summary>
    /// Update value of slider to given load progress
    /// </summary>
    /// <param name="progress"></param>
    public void UpdateLoadBar(float progress)
    {
        float prog = Mathf.Clamp01((float)progress / .9f);
        slider.value = prog;
    }
}
