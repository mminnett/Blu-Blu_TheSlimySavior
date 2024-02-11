/*
 * Author: Matthew Minnett
 * Desc: Move projectile in given direction
 * Date Created: 2023/01/29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpit : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Speed at which projectile moves")]
    protected float speed = 15f;

    [SerializeField]
    [Tooltip("Time before game object is destroyed")]
    private float lifeTime = 5f;

    protected Vector3 direction;

    #region UnityFunctions
    private void OnEnable()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime); // move in direction by the speed
    }
    #endregion

    /// <summary>
    /// Take in a direction variable that the projectile will travel
    /// </summary>
    /// <param name="dir"></param>
    public void Spit(Vector3 dir)
    {
        direction = dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
            Destroy(gameObject); // destroy on collision with wall
        else if (other.CompareTag("Player"))
            Destroy(gameObject); // destroy on collision with player
    }
}
