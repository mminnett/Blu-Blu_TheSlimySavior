/*
 * Author: Matthew Minnett
 * Desc: Updates HUD information
 * Date Created: 2023/03/05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Player health slider")]
    Slider health;
    [SerializeField]
    [Tooltip("Text to display enemy/shield count")]
    TextMeshProUGUI enemies;

    [SerializeField]
    [Tooltip("Title of enemy/shield count display")]
    TextMeshProUGUI enemyTitle;
    [SerializeField]
    [Tooltip("Boss health slider")]
    Slider bossHealth;

    [SerializeField]
    [Tooltip("Name of final level/scene")]
    string bossLevelName;

    private void Awake()
    {
        bossHealth.gameObject.SetActive(false); // make boss health invisible
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentLevelName != GameManager.Instance.MainMenuName) // if not main menu
        {
            if (GameManager.Instance.CurrentLevelName != bossLevelName) // if not boss level
            {
                enemyTitle.text = "Enemies Remaining"; // change enemy title text
                enemies.text = EnemyTracker.Instance.enemies.Count.ToString(); // display enemy count
                bossHealth.gameObject.SetActive(false); // set boss health to inactive
            }
            else if (GameManager.Instance.CurrentLevelName == bossLevelName) // if boss level
            {
                enemyTitle.text = "Shields Remaining"; // change enemy title text
                enemies.text = EnemyBody.Instance.CurrentShieldCount.ToString(); // display shield count
                bossHealth.gameObject.SetActive(true); // set boss health to visible
                bossHealth.value = EnemyBody.Instance.LivesPercent; // change value to boss health percentage
            }
        }
        health.value = GameManager.Instance.PlayerBody.LivesPercent; // update health to player health percentage
    }
}
