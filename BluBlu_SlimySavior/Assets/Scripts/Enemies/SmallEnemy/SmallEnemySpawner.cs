/*
 * Author: Matthew Minnett
 * Desc: Tracks health of small enemy spawner.
 * Date Created: 2023/03/05
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallEnemySpawner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Hits before destroyed")]
    private int lives = 1;

    [HideInInspector]
    public SmallEnemySpawnerController controller; // tracks all spawners in scene

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball2")) // if the secondary projectile of player collides
        {
            lives--; // shield has taken damage!
            if (lives <= 0) // if shield health is equal to 0
            {
                controller.RemoveSpawner(this); // remove from list
                Destroy(gameObject);
            }
        }
    }
}
