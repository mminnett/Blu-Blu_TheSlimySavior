/*
 * Author: Matthew Minnett
 * Desc: Manages main menu buttons
 * Date Created: 2023/03/05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Continue game button")]
    Button continueBTN;

    private void OnEnable()
    {
        continueBTN.interactable = GameManager.Instance.SaveGameExists; // if save game exists, make button clickable
    }

    /// <summary>
    /// Call GameManager StartNewGame function
    /// </summary>
    public void NewGame()
    {
        GameManager.Instance.StartNewGame();
    }

    /// <summary>
    /// Exit standalone application/editor application
    /// </summary>
    public void ExitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    /// <summary>
    /// Call GameManager ContinuesGame function
    /// </summary>
    public void ContinueGame()
    {
        GameManager.Instance.ContinueGame();
    }

    /// <summary>
    /// Make option menu visible
    /// </summary>
    public void OptionsButton()
    {
        GameManager.Instance.OptionsMenu.Canvas.SetActive(true);
    }
}
