/*
 * Author: Matthew Minnett
 * Desc: Keeps a list of all enemies in scene
 * Date Created: 2023/03/05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    private static EnemyTracker instance;
    public static EnemyTracker Instance
    {
        get
        {
            // if we haven't assigned the enemy tracker go find it
            if (instance == null)
            {
                instance = FindObjectOfType<EnemyTracker>();
            }

            // if there isn't an enemy tracker in the scene, print error
            //if (instance == null)
            //{
            //    Debug.LogError("No Enemy Tracker Found");
            //}

            return instance;
        }
    }

    [HideInInspector]
    public List<GameObject> enemies = new List<GameObject>(); // list to store enemies

    private void Start()
    {
        GameObject[] tempArray = GameObject.FindGameObjectsWithTag("Enemy"); // create a temporary array at start

        // for each item in temp array
        foreach (GameObject s in tempArray)
        {
            enemies.Add(s); // add that enemy to the list of enemies
        }

        if (SmallEnemySpawnerController.Instance)
        {
            GetComponentInChildren<SmallEnemySpawnerController>().enemyTracker = this;
        }
    }

    /// <summary>
    /// Add enemy to list
    /// </summary>
    /// <param name="enemy"></param>
    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy); // add new enemy
    }

    /// <summary>
    /// Remove enemy from list
    /// </summary>
    /// <param name="enemy"></param>
    public void RemoveEnemy(GameObject enemy)
    {
        if (enemies.Contains(enemy)) // check if list contains that enemy
        {
            enemies.Remove(enemy); // remove it if it does
        }
    }

    /// <summary>
    /// Return amount of enemies in scene
    /// </summary>
    /// <returns></returns>
    public int CheckRemainingEnemies()
    {
        return enemies.Count;
    }
}
