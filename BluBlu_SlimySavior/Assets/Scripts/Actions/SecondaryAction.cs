/*
 * Author: Matthew Minnett
 * Desc: Projectile spins and move upwards. Derives from primary action script.
 * Date Created: 2023/01/29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryAction : PrimaryAction
{
    private Vector3 rotateSpeed = new Vector3(0, 5.0f, 0);

    private void FixedUpdate()
    {
        Vector3 moveDirection = direction * speed * Time.fixedDeltaTime; // Move in given direction by the speed
        transform.Translate(moveDirection);
        transform.Rotate(rotateSpeed); // rotates the object by the rotate speed variable
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyShield") || other.CompareTag("EnemySpawner"))
            gameObject.SetActive(false); // destroy when it collides with an enemy shield
    }
}
