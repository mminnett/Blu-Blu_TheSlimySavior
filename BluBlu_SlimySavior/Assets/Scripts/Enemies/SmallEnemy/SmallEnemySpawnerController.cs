/*
 * Author: Matthew Minnett
 * Desc: Keeps a list of all spawners in scene. Randomly selects a spawner and creates an enemy at location.
 * Date Created: 2023/03/05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallEnemySpawnerController : MonoBehaviour
{
    private static SmallEnemySpawnerController instance;
    public static SmallEnemySpawnerController Instance
    {
        get
        {
            // if we haven't assigned the spawner controller go find it
            if (instance == null)
            {
                instance = FindObjectOfType<SmallEnemySpawnerController>();
            }

            // if there isn't a spawner controller in the scene, print error
            //if (instance == null)
            //{
            //    Debug.LogError("No Spawner Controller Found");
            //}

            return instance;
        }
    }

    public List<SmallEnemySpawner> spawners = new List<SmallEnemySpawner>(); // list to store shields

    [SerializeField]
    [Tooltip("Small enemy body prefab")]
    private GameObject smallEnemy;

    [SerializeField]
    [Tooltip("Time between new enemy spawns")]
    private float spawnTime = 3f;

    [SerializeField]
    [Tooltip("Maximum amount of enemies allowed at once")]
    private int maxEnemies = 20;

    [HideInInspector]
    public EnemyTracker enemyTracker; // tracks all enemies in scene

    private bool canSpawn;

    private void Awake()
    {
        canSpawn = true;
    }

    private void Start()
    {
        SmallEnemySpawner[] tempArray = GetComponentsInChildren<SmallEnemySpawner>(); // create a temporary array at start

        if (tempArray != null)
        {
            // for each item in temp array
            foreach (SmallEnemySpawner s in tempArray)
            {
                s.controller = this; // set that Enemy Spawner's controller reference, so it can easily reference this script
                spawners.Add(s); // add that EnemySpawner to the list of spawners
            }
        }
        else
        {
            Debug.Log("No spawners in scene");
            canSpawn = false;
        }
    }

    private void Update()
    {
        if (canSpawn && enemyTracker.CheckRemainingEnemies() < maxEnemies) // if can spawn time is done and max enemies in scene has not been reached
        {
            StartCoroutine(SpawnEnemy()); // spawn new enemy
        }
    }

    /// <summary>
    /// Remove a spawner from the list
    /// </summary>
    /// <param name="spawner"></param>
    public void RemoveSpawner(SmallEnemySpawner spawner)
    {
        if (spawners.Contains(spawner)) // check if list contains that shield
        {
            spawners.Remove(spawner); // remove it if it does
        }
    }

    /// <summary>
    /// Check remaining spawners in list
    /// </summary>
    /// <returns></returns>
    public int CheckRemainingSpawners()
    {
        return spawners.Count;
    }

    /// <summary>
    /// Spawns small enemy at random spawner location
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEnemy()
    {
        canSpawn = false; // cannot spawn another for time period

        int spawn = Random.Range(0, spawners.Count); // selects random spawner

        //instantiate enemy
        GameObject enemyGO = GameObject.Instantiate(smallEnemy, spawners[spawn].transform.position, Quaternion.identity);

        enemyGO.transform.parent = enemyTracker.transform; // set parent to enemytracker
        enemyGO.SetActive(true);
        enemyTracker.AddEnemy(enemyGO); // add new enemy to enemy list

        yield return new WaitForSeconds(spawnTime); // wait until spawn time has been reached

        canSpawn = true; // can spawn a new enemy
    }
}
