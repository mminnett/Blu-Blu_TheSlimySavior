/*
 * Author: Matthew Minnett
 * Desc: Keeps a list of Enemy Shields in scene
 * Date Created: 2023/03/05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShieldController : MonoBehaviour
{
    public List<EnemyShield> shields = new List<EnemyShield>(); // list to store shields

    private void Start()
    {
        EnemyShield[] tempArray = GetComponentsInChildren<EnemyShield>(); // create a temporary array at start

        // for each item in temp array
        foreach (EnemyShield s in tempArray)
        {
            s.controller = this; // set that EnemyShield's controller reference, so it can easily reference this script
            shields.Add(s); // add that EnemyShield to the list of shields
        }
    }

    /// <summary>
    /// Remove a shield from list of shields
    /// </summary>
    /// <param name="shield"></param>
    public void RemoveShield(EnemyShield shield)
    {
        if (shields.Contains(shield)) // check if list contains that shield
        {
            shields.Remove(shield); // remove it if it does
        }
    }

    /// <summary>
    /// Return amount of shields in list
    /// </summary>
    /// <returns></returns>
    public int CheckRemainingShields()
    {
        return shields.Count;
    }    
}
