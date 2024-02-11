/*
 * Author: Matthew Minnett
 * Desc: Move projectile in given direction
 * Date Created: 2023/01/29
 * 
 * ------------------Changelog-------------------
 * 2023/03/05 - No longer destroyed, uses object pooling (secondary as well)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrimaryAction : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Speed at which the object will travel")]
    protected float speed = 20f;

    [SerializeField]
    [Tooltip("Time before object is destroyed")]
    private float lifeTime = 1f;

    protected Vector3 direction;

    #region Unity Functions
    private void OnEnable()
    {
        StartCoroutine(SelfDestruct()); // goes inactive after lifetime is up
    }

    private void OnDisable()
    {
        StopAllCoroutines(); 
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime); // move in given direction by the speed
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

    /// <summary>
    /// Destroy game object if it collides with a wall, an enemy, or an enemy shield
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
            gameObject.SetActive(false);
        else if (other.CompareTag("Enemy"))
            gameObject.SetActive(false);
        else if (other.CompareTag("EnemyShield"))
            gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets gameobject inactive when the lifetime is up
    /// </summary>
    /// <returns></returns>
    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }
}
