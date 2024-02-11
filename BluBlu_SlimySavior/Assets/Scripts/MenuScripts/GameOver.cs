/*
 * Author: Matthew Minnett
 * Desc: Used for Play Again and Return to Main Menu buttons on game over screen
 * Date Created: 2023/03/05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public void PlayAgain()
    {
        GameManager.Instance.PlayerRespawn();
    }

    public void ReturnToMainMenu()
    {
        if (GameManager.Instance.CurrentLevelIndex != 0) // if the current index is not first level
        {
            GameManager.Instance.LoadGame(); // update load game data
        }
        else
        {
            GameManager.Instance.SaveGameExists = false; // save game exists is false (if it wasn't already)
        }
        GameManager.Instance.ReturnToMainMenu();
        SoundManager.Instance.Mixer.SetFloat("Enemies", 0f); // ensure that enemy audio is raised again
    }
}
