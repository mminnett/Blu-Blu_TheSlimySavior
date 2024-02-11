/*
 * Author: Matthew Minnett
 * Desc: Object pool for player primary attack
 * Date Created: 2023/03/05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerPool : MonoBehaviour
{
    //List to hold projectiles
    public List<GameObject> pooledObjects = new List<GameObject>();

    [SerializeField]
    private GameObject pooledObjectPrefab;  //prefab to create new spit projectiles from
    [SerializeField]
    private int pooledAmount = 20;      //default amount of projectiles to add to the pool


    private static PlayerPool instance;
    public static PlayerPool Instance { get { return instance; } }

    void Awake()
    {
        instance = this;    //set instance as the game object this is attached to
    }

    private void Start()
    {
        for (int i = 0; i < pooledAmount; i++)
        {
            CreatePooledObject();
        }
    }

    /// <summary>
    /// Create object and add to pool
    /// </summary>
    /// <returns></returns>
    GameObject CreatePooledObject()
    {
        //instantiate a projectile
        GameObject temp = Instantiate(pooledObjectPrefab);
        temp.SetActive(false);

        //set the projectiles parent
        temp.transform.SetParent(transform);

        //add the new projectile to my list
        pooledObjects.Add(temp);

        //return the projectile
        return temp;
    }

    /// <summary>
    /// Get inactive object from pool. Creates new if none available
    /// </summary>
    /// <returns></returns>
    public GameObject GetPooledObject()
    {
        //looping through list of projectiles
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            //finding the first inactive projectile in the list
            if (!pooledObjects[i].activeInHierarchy)
                return pooledObjects[i];    //return it
        }

        return CreatePooledObject();
    }

    //function runs when the game needs to be reset
    public void GameReset()
    {
        //looping through projectiles and disabling them
        foreach (GameObject o in pooledObjects)
        {
            o.SetActive(false);
        }
    }
}
