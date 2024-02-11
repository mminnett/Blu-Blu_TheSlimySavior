/*
 * Author: Matthew Minnett
 * Desc: Bounces back and forth between two walls, shooting a projectile at the player and mocking them.
 * Date Created: 2023/03/05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnemyBody : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Amount of hits before enemy is defeated")]
    private int lives = 3;

    [SerializeField]
    [Tooltip("Speed at which the enemy moves")]
    private float moveSpeed = 5f;

    [SerializeField]
    [Tooltip("Amount of time between enemy shots")]
    private float spitTime = 1f;

    [SerializeField]
    [Tooltip("Game object with fly enemy spit script attached")]
    private GameObject trackSpit;

    [SerializeField]
    [Tooltip("Location of enemy mouth, from which the spit object will originate from")]
    private Transform mouth;

    [SerializeField]
    AudioSource source;

    [SerializeField]
    [Tooltip("Sounds the enemy will make at random times")]
    AudioClip[] idleSoundClips;

    [SerializeField]
    [Tooltip("Sound the enemy makes when damage is taken")]
    AudioClip damageClip;

    [SerializeField]
    [Tooltip("Sound that plays on death")]
    AudioClip deathClip;

    [HideInInspector]
    public EnemyTracker enemyTracker; // used to track enemies in scene

    private Rigidbody rb;

    private float direction;

    private bool canSpit;

    private bool isDead;

    private int lastIdlePlayed; // tracks last idle sound clip that was played

    private void Awake()
    {
        enemyTracker = GetComponentInParent<EnemyTracker>(); // find enemy tracker (should be parent)

        rb = GetComponent<Rigidbody>();
        direction = -1; // affects the direction the enemy moves in
        canSpit = false; // set to false so it doesn't spit right away
        isDead = false;

        StartCoroutine(SpitTime()); // start the spit time coroutine
        StartCoroutine(IdleSounds()); // start idle sounds coroutine
    }

    private void FixedUpdate()
    {
        if (!isDead && GameObject.FindWithTag("Player")) // if there is a player and enemy is alive
        {
            transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform); // front of enemy is pointed at player

            Vector3 moveDir = Vector3.zero;

            moveDir.x = direction * moveSpeed * Time.fixedDeltaTime; // calculate direction to move in
            rb.MovePosition(transform.position + moveDir); // move position of rigidbody attached to gameobject

            if (canSpit) // if the spit timer is done counting (see coroutine)
            {
                canSpit = false; // set canSpit back to false

                // Direction of spit is aimed toward the player, as the enemy is looking at the player
                Vector3 direction = mouth.position - transform.position; // get direction for spit to travel
                direction.Normalize();

                //instantiate spit
                GameObject spitGO = GameObject.Instantiate(trackSpit, mouth.position, Quaternion.identity);

                spitGO.SetActive(true);

                EnemySpit trackScript = spitGO.GetComponent<EnemySpit>(); // get enemySpit script component
                trackScript.Spit(direction); // call the spit function

                StartCoroutine(SpitTime()); // start coroutine once again
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDead) // doesn't take damage if dead (prevents from death sound playing twice)
        {
            if (other.CompareTag("Wall")) // if enemy collides with a wall
                direction = -direction; // reverse direction
            else if (other.CompareTag("Ball2")) // if the secondary projectile of player collides
            {
                lives--; // enemy has taken damage!

                source.Stop();
                source.PlayOneShot(damageClip); // play damage sound

                if (lives <= 0) // if enemy health is equal to 0
                {
                    enemyTracker.RemoveEnemy(gameObject);

                    source.Stop();
                    source.PlayOneShot(deathClip); // play death sound

                    isDead = true; // enemy is now dead

                    Destroy(gameObject, deathClip.length); // destroy after death clip is finished playing
                }
            }
        }
    }

    /// <summary>
    /// Counts down until enemy can spit again
    /// </summary>
    /// <returns></returns>
    IEnumerator SpitTime()
    {
        yield return new WaitForSeconds(spitTime); // counts to spit time

        canSpit = true; // when count is done, enemy can spit again
    }

    /// <summary>
    /// Plays a random idle sound at random time
    /// </summary>
    /// <returns></returns>
    private IEnumerator IdleSounds()
    {
        while (true)
        {
            float randTime = Random.Range(15f, 30f); // Get random time from 15 to 30 seconds
            yield return new WaitForSeconds(randTime); // wait random amount of time

            if (!isDead) // if alive
            {
                int randShot = lastIdlePlayed;
                float randPitch = Random.Range(0.7f, 1f);

                while(randShot == lastIdlePlayed)
                {
                    randShot = Random.Range(0, idleSoundClips.Length); // loop through until new sound is not same as last played
                }

                lastIdlePlayed = randShot;
                source.clip = idleSoundClips[randShot];
                source.pitch = randPitch;
                source.Play(); // play random sound at random pitch
            }
        }
    }
}
