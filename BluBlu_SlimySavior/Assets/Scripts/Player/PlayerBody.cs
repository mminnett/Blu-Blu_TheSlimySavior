/*
 * Author: Matthew Minnett
 * Desc: Moves player gameobject based on input. Calls functions to fire projectiles
 * Date: 2023/01/29
 * 
 * ------------------Changelog-------------------
 * 2023/03/05 - Added audio
 *            - Now uses object pools for primary and secondary attack
 *            - Particles follow player when moving
 *            - Resets on death
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public class PlayerBody : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Particles that play when player moves")]
    public ParticleSystem dust;
    private bool dustPlaying; // tracks if particles are playing or not

    [SerializeField]
    [Tooltip("Speed at which the player moves")]
    private float moveSpeed = 15f;

    [SerializeField]
    [Tooltip("Amount of hits player can take before being destroyed")]
    private int lives = 3;

    [SerializeField]
    [Tooltip("Game object with the PrimaryAction script attached")]
    private GameObject straightSpit;

    [SerializeField]
    [Tooltip("Game object with the SecondaryAction script attached")]
    private GameObject upSpit;

    [SerializeField]
    [Tooltip("Location that the projectiles will spawn")]
    public Transform mouth;

    [SerializeField]
    [Tooltip("Audio source for player")]
    AudioSource mouthSource;
    [SerializeField]
    [Tooltip("Sounds played when attacking")]
    AudioClip[] spitSoundClips;
    [SerializeField]
    [Tooltip("Sound played when damaged")]
    AudioClip damageSoundClip;
    [SerializeField]
    [Tooltip("Sound played on death")]
    AudioClip deathSoundClip;
    [SerializeField]
    [Tooltip("Audio source for player trail (movement sound)")]
    AudioSource trailSource;
    [SerializeField]
    [Tooltip("Audio clips played when player moves")]
    AudioClip[] moveSoundClips;

    private bool canMove = true; // used when player can no longer move
    public bool CanMove { get { return canMove; } set { canMove = value; } }

    private float forwardDirection;

    public float ForwardDirection
    {
        set { forwardDirection = value; }
        get { return forwardDirection; }
    }

    private int maxLives; // track starting lives
    public int MaxLives { get { return maxLives; } }

    public float LivesPercent { get { return lives / (float)maxLives; } } // player health percentage used for health bar on HUD

    private Rigidbody rb;

    #region UnityFunctions
    private void Awake()
    {
        dustPlaying = false; // particles are not playing
        rb = GetComponent<Rigidbody>();
        Debug.Log("Lives: " + lives);

        maxLives = lives; // set the max lives
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Vector3 moveDir;

            moveDir = transform.forward * forwardDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position + moveDir); // move position of object in calculated direction
            if (moveDir != Vector3.zero)
            {
                if (!trailSource.isPlaying) // if the trail audio is not playing
                {
                    int randShot = Random.Range(0, moveSoundClips.Length); // get random sound
                    float randPitch = Random.Range(0.7f, 1f); // get random pitch

                    trailSource.clip = moveSoundClips[randShot];
                    trailSource.pitch = randPitch;
                    trailSource.Play(); // play random move sound at random pitch
                }

                if (!dustPlaying) // if particles are not playing
                {
                    dustPlaying = true;
                    StartCoroutine(CreateDust()); // play particles
                }
            }
            else if (moveDir == Vector3.zero) // if not moving
            {
                StopDust(); // stop particles
                dustPlaying = false;

                trailSource.Stop(); // stop movement audio
            }
        }
    }
    #endregion

    #region Actions

    /// <summary>
    /// Fires the straight spit attack on left click
    /// </summary>
    public void FirePrimary()
    {
        if (canMove)
        {
            if (straightSpit)
            {
                Vector3 direction = mouth.position - transform.position; // moves away from mouth position
                direction.y = 0f;
                direction.Normalize();

                GameObject spitGO = PlayerPool.Instance.GetPooledObject(); // get pooled object from primary pool
                spitGO.transform.position = mouth.position;
                spitGO.transform.rotation = Quaternion.identity;

                spitGO.SetActive(true);

                PrimaryAction straightScript = spitGO.GetComponent<PrimaryAction>(); // get primary action script
                straightScript.Spit(direction); // call spit function

                SpitAudio(); // play audio for spitting
            }
            else
            {
                Debug.Log("There is no game object for primary action attached to the player body!");
            }
        }
    }

    /// <summary>
    /// Fires upward spit attack on right click
    /// </summary>
    public void FireSecondary()
    {
        if (canMove)
        {
            if (upSpit)
            {
                Vector3 direction = mouth.position - transform.position; // moves away from mouth position
                direction.y = 0.6f;
                direction.Normalize();

                GameObject spitGO = PlayerAltPool.Instance.GetPooledObject(); // get pooled object from secondary pool
                spitGO.transform.position = mouth.position;
                spitGO.transform.rotation = Quaternion.identity;

                spitGO.SetActive(true);

                SecondaryAction upScript = spitGO.GetComponent<SecondaryAction>(); // get secondary action script
                upScript.Spit(direction); // call the spit function

                SpitAudio(); // play spit sound
            }
            else
            {
                Debug.Log("There is no game object for secondary action attached to the player body!");
            }
        }
    }

    /// <summary>
    /// Update rotation of player body based on given aim direction
    /// </summary>
    /// <param name="aimDir"></param>
    public void UpdateAimDirection(Vector3 aimDir)
    {
        if (canMove) // if player is able to move
        {
            aimDir.Normalize();
            transform.rotation = Quaternion.LookRotation(aimDir);
        }
    }
    #endregion

    /// <summary>
    /// Play dust particles
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreateDust()
    {
        dust.Play();

        yield return new WaitForSeconds(dust.main.duration);
        dustPlaying = false;
    }

    /// <summary>
    /// Stop dust particles
    /// </summary>
    public void StopDust()
    {
        dust.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && canMove) // if collides with enemy/enemy spit
        {
            lives--; // take damage

            mouthSource.Stop();
            mouthSource.pitch = 1f; // set pitch back to default
            mouthSource.PlayOneShot(damageSoundClip); // play damage sound

            if (lives <= 0)
            {
                mouthSource.Stop();
                mouthSource.pitch = 1f; // set pitch back to default
                mouthSource.PlayOneShot(deathSoundClip); // play death sound

                canMove = false; // player cannot move

                GameManager.Instance.PlayerDeath(); // call game manager's player death function for game over
            }
        }
    }

    /// <summary>
    /// Play random spit audio clip at random pitch
    /// </summary>
    private void SpitAudio()
    {
        int randShot = Random.Range(0, spitSoundClips.Length);
        float randPitch = Random.Range(0.7f, 1f);

        mouthSource.clip = spitSoundClips[randShot];
        mouthSource.pitch = randPitch;
        mouthSource.Play();
    }

    /// <summary>
    /// Reset player data
    /// </summary>
    public void Reset()
    {
        lives = maxLives;
        dustPlaying = false;
        canMove = true;
    }
}
