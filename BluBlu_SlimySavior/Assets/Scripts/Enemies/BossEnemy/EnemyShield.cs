/*
 * Author: Matthew Minnett
 * Desc: Destroys when hit with game object a set amount of times
 * Date Created: 2023/01/29
 * 
 * ------------------Changelog-------------------
 * 2023/03/05 - Added shield controller
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Amount of hits before object is destroyed")]
    private int lives = 5;

    [HideInInspector]
    public EnemyShieldController controller; // keeps a list of shields in scene

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball2")) // if the secondary projectile of player collides
        {
            lives--; // shield has taken damage!
            if (lives <= 0) // if shield health is equal to 0
            {
                controller.RemoveShield(this); // shield is destroyed! Removee from list
                Destroy(gameObject);
            }
        }
    }
}
