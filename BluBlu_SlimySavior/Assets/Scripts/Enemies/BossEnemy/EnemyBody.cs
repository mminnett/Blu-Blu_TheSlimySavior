/*
 * Author: Matthew Minnett
 * Desc: Moves enemy between two walls. Fires projectiles.
 * Date Created: 2023/01/29
 * 
 * ------------------Changelog-------------------
 * 2023/03/05 - Added audio
 *            - Added shield controller
 *            - Added lives percent
 *            - Added isDead tracker
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBody : MonoBehaviour
{
    static EnemyBody instance;
    public static EnemyBody Instance { get { return instance; } }

    [SerializeField]
    [Tooltip("Amount of hits before enemy is defeated")]
    private int lives = 20;

    [SerializeField]
    [Tooltip("Speed at which the enemy moves")]
    private float moveSpeed = 5f;

    [SerializeField]
    [Tooltip("Amount of time between enemy shots")]
    private float spitTime = 1f;

    [SerializeField]
    [Tooltip("Game object with enemy spit script attached")]
    private GameObject straightSpit;

    [SerializeField]
    [Tooltip("Location of enemy mouth, from which the spit object will originate from")]
    private Transform mouth;

    [SerializeField]
    [Tooltip("The Parent of the shields with the EnemyShieldController script attached")]
    private EnemyShieldController shieldController;

    [SerializeField]
    AudioSource source;

    [SerializeField]
    [Tooltip("Sounds the enemy will make while shields are up")]
    AudioClip[] idleSoundClips; 

    [SerializeField]
    [Tooltip("Sounds the enemy will make when damaged")]
    AudioClip[] damageClips;

    [SerializeField]
    [Tooltip("Sound the enemy makes when defeated")]
    AudioClip deathClip;

    [SerializeField]
    [Tooltip("Sound enemy makes when shields are all down")]
    AudioClip spitClip;

    [HideInInspector]
    public EnemyTracker enemyTracker; // enemy tracker tracks all enemies in scene. When this enemy is defeated, it is removed

    private Rigidbody rb;

    private float direction;

    private int shieldCount; // total shields
    private int currentShieldCount; // current shields still active
    public int CurrentShieldCount { get { return currentShieldCount; } }

    private bool canSpit;
    private bool nextPhase;
    private bool isDead;

    private int lastIdlePlayed; // tracks which idle audio clip was played last, so it isn't played again

    private int maxLives; // tracks enemies starting lives

    public float LivesPercent { get { return lives / (float)maxLives; } } // used for health bar

    #region Unity Functions
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        enemyTracker = GetComponentInParent<EnemyTracker>(); // find enemy tracker that should be parent

        rb = GetComponent<Rigidbody>();
        direction = -1; // affects the direction the enemy moves in
        canSpit = false; // set to false so it doesn't spit right away
        nextPhase = false; // starts in the first phase
        isDead = false;
        shieldCount = shieldController.CheckRemainingShields(); // track starting amount of shields

        maxLives = lives; // track starting lives

        StartCoroutine(SpitTime()); // start the spit time coroutine
        StartCoroutine(IdleSounds()); // play idle sounds
    }

    private void FixedUpdate()
    {
        if (!isDead) // move and shoot if enemy is alive
        {
            Vector3 moveDir = Vector3.zero;
            currentShieldCount = shieldController.CheckRemainingShields(); // get new count for shields

            moveDir.x = direction * moveSpeed * Time.fixedDeltaTime; // calculate direction to move in
            rb.MovePosition(transform.position + moveDir); // move position of rigidbody attached to gameobject

            // if the new shield count is less than or equal to half the amount of total shields
            // and the next phase has not started already
            if (currentShieldCount <= shieldCount * 0.5 && !nextPhase)
            {
                spitTime = spitTime * 0.75f; // make the spit time lower, so that it spits more frequently
                nextPhase = true; // set next phase to true so that spit time is lowered only once
            }

            if (canSpit) // if the spit timer is done counting (see coroutine)
            {
                canSpit = false; // set canSpit back to false

                Vector3 direction = mouth.position - transform.position; // get direction for spit to travel
                direction.y = 0f;
                direction.Normalize();

                //instantiate spit
                GameObject spitGO = GameObject.Instantiate(straightSpit, mouth.position, Quaternion.identity);

                spitGO.SetActive(true);

                EnemySpit straightScript = spitGO.GetComponent<EnemySpit>(); // get enemySpit script component
                straightScript.Spit(direction); // call the spit function

                StartCoroutine(SpitTime()); // start coroutine once again

                if (currentShieldCount == 0 && !source.isPlaying) // if there are no shields and the audio source is not currently playing
                {
                    float randPitch = Random.Range(0.7f, 1f);

                    source.clip = spitClip;
                    source.pitch = randPitch;
                    source.Play(); // play spit sound
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDead) // doesn't take damage if dead (prevents death sound from playing twice)
        {
            if (other.CompareTag("Wall")) // if enemy collides with a wall
                direction = -direction; // reverse direction
            else if (other.CompareTag("Ball")) // if player primary collides
            {
                if (shieldController.CheckRemainingShields() > 0) // check if there are still shields in scene
                {
                    Debug.Log("Shields are still up!"); // damage can't be taken
                }
                else // else, if there are no shields
                {
                    moveSpeed++; // enemy moves faster
                    lives--; // takes damage

                    int randShot = Random.Range(0, damageClips.Length);
                    float randPitch = Random.Range(0.7f, 1f);

                    source.clip = damageClips[randShot];
                    source.pitch = randPitch;
                    source.Play(); // play damage sound at random pitch

                    //Debug.Log("Enemy damaged! Lives remaining: " + lives);
                    if (lives <= 0)
                    {
                        enemyTracker.RemoveEnemy(gameObject);

                        source.Stop();
                        source.pitch = 1f; // set pitch back to default
                        source.PlayOneShot(deathClip); // play death sound

                        isDead = true; // set enemy to dead

                        Destroy(gameObject, deathClip.length); // destroy if lives reaches 0
                    }
                }
            }
        }
    }
    #endregion

    /// <summary>
    /// Waits a set amount of time before launching spit again
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
            float randTime = Random.Range(10f, 20f); // get random time from 10 seconds to 20 seconds
            yield return new WaitForSeconds(randTime); // wait random amount of seconds

            if (!isDead && currentShieldCount != 0) // if alive and shields are greater than 0
            {
                int randShot = lastIdlePlayed;
                while (randShot == lastIdlePlayed) // ensure that randShot is not the same as last idle played
                {
                    randShot = Random.Range(0, idleSoundClips.Length); // by looping until it is a different value
                }
                lastIdlePlayed = randShot; // update last idle played
                source.pitch = 1f; // pitch to default
                source.clip = idleSoundClips[randShot];
                source.Play(); // play idle sound
            }
        }
    }
}
