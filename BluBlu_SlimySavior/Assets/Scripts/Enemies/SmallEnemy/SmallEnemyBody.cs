/*
 * Author: Matthew Minnett
 * Desc: Chases player and damages on impact.
 * Date Created: 2023/03/05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallEnemyBody : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Speed at which enemy moves")]
    protected float speed = 5f;

    [SerializeField]
    [Tooltip("Hits enemy takes before defeat")]
    protected int lives = 1;

    [SerializeField]
    AudioSource source;

    [SerializeField]
    [Tooltip("Sound played on death")]
    AudioClip damage;

    [SerializeField]
    [Tooltip("Idle sound effect")]
    AudioClip idle;

    [HideInInspector]
    public EnemyTracker enemyTracker; // tracks enemies in scene

    private bool isDead;

    private void Start()
    {
        enemyTracker = GetComponentInParent<EnemyTracker>();
        isDead = false;

        StartCoroutine(IdleSounds()); // start the idle sound coroutine
    }

    private void Update()
    {
        if (!isDead && GameObject.FindWithTag("Player")) // if alive and player exists
        {
            transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform); // face player

            transform.position += transform.forward * speed * Time.deltaTime; // chase player
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball")) // if player primary collides
        {
            if (!isDead) // if alive
            {
                lives--; // takes damage

                Debug.Log("Enemy damaged! Lives remaining: " + lives);
                if (lives <= 0)
                {
                    enemyTracker.RemoveEnemy(gameObject);

                    source.Stop();
                    source.PlayOneShot(damage); // play death sound

                    isDead = true; // enemy is dead

                    Destroy(gameObject, damage.length); // destroy after death clip is done
                }
            }
        }
    }

    /// <summary>
    /// Plays idle sound at random pitch, at random time between 5 and 10 seconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator IdleSounds()
    {
        while(true)
        {
            float randTime = Random.Range(5f, 10f);
            yield return new WaitForSeconds(randTime);

            if(!isDead)
            {
                float randPitch = Random.Range(0.7f, 1f);

                source.pitch = randPitch;
                source.PlayOneShot(idle);
            }
        }
    }
}
