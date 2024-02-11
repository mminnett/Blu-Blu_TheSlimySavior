/*
 * Author: Matthew Minnett
 * Desc: Tracks level completion status and level music
 * Date Created: 2023/03/05
 */

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;

    public static LevelManager Instance
    {
        get
        {
            if(!instance)
            {
                instance = FindObjectOfType<LevelManager>();
            }
            //if(!instance)
            //{
            //    Debug.LogError("Can't find level manager");
            //}

            return instance;
        }
    }

    [SerializeField]
    [Tooltip("Looping music track")]
    AudioClip levelMusic;
    public AudioClip LevelMusic {  get { return levelMusic; } }

    private void Awake()
    {
        if(!instance)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (SmallEnemySpawnerController.Instance && EnemyTracker.Instance) // if there are spawners and an enemy tracker
        {
            if (EnemyTracker.Instance.enemies.Count == 0 && SmallEnemySpawnerController.Instance.spawners.Count == 0)
            {
                GameManager.Instance.LevelComplete(); // if no enemies and no spawners, level complete
            }
        }
        else // if there are no spawners
        {
            if (EnemyTracker.Instance)
            {
                if (EnemyTracker.Instance.enemies.Count == 0) // if no enemies
                {
                    GameManager.Instance.LevelComplete(); // level complete
                }
            }
        }
    }
}
