/*
 * Author: Matthew Minnett
 * Desc: Displays pause screen and pauses game
 * Date Created: 2023/03/05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Mixer snapshot when game is paused")]
    AudioMixerSnapshot pausedSnap;
    [SerializeField]
    [Tooltip("Mixer snapshot when game is unpaused")]
    AudioMixerSnapshot unpausedSnap;

    [SerializeField]
    [Tooltip("Canvas for pause menu")]
    Canvas canvas;

    private bool canPause = false;
    public bool CanPause { get { return canPause; } set { canPause = value; } }

    private bool isPaused;
    public bool IsPaused { get { return isPaused; } }

    private void Update()
    {
        // if cancel button hit, can pause is true, and options menu is not active
        if(Input.GetButtonDown("Cancel") && canPause && !OptionsMenu.Instance.Canvas.activeInHierarchy)
        {
            // pause the game
            Pause(!isPaused);
        }
    }

    /// <summary>
    /// Pause/unpause the game
    /// </summary>
    /// <param name="paused"></param>
    public void Pause(bool paused)
    {
        isPaused = paused;

        canvas.gameObject.SetActive(isPaused); // set pause menu to visible/not visible based on whether is paused or not

        if(isPaused)
        {
            Time.timeScale = 0; // set timescale to 0 when paused
            pausedSnap.TransitionTo(.1f); // transition to paused snapshot
            GameManager.Instance.PlayerBody.CanMove = false; // player cannot move
        }
        else
        {
            Time.timeScale = 1; // set timescale to 1 when unpaused
            unpausedSnap.TransitionTo(.1f); // transition to unpaused snapshot
            GameManager.Instance.PlayerBody.CanMove = true; // player can move
        }
    }

    /// <summary>
    /// Close pause menu (used for return to game button)
    /// </summary>
    public void ReturnToGame()
    {
        Pause(!isPaused);
    }
}
