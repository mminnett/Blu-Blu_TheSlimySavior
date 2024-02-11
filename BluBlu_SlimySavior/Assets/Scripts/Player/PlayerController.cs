/*
 * Author: Matthew Minnett
 * Desc: Gets keyboard input, sends direction data to playerbody gameobject.
 *       Check for mouse input to fire projectiles.
 * Date: 2023/01/29
 */

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Game object with PlayerBody script attached")]
    private PlayerBody playerBody;

    [SerializeField]
    [Tooltip("Camera that will follow the player")]
    private Camera playerCamera;

    private static float MAX_CAMERA_MOVE = 0f; // camera will move between these two values
    private static float MIN_CAMERA_MOVE = -20f;

    /// <summary>
    /// Call functions to move player and check for input
    /// </summary>
    private void Update()
    {
        UpdateRotation();
        UpdatePosition();
        UpdateCamera();
        CheckForAction();
    }

    #region Movement and other Actions
    /// <summary>
    /// Update the rotation of player based on mouse position
    /// </summary>
    void UpdateRotation()
    {
        Plane plane = new Plane(Vector3.up, playerBody.transform.position);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float distanceToPlane;

        plane.Raycast(ray, out distanceToPlane);

        Vector3 mouseWorldPos = ray.GetPoint(distanceToPlane);

        Vector3 aimDir = mouseWorldPos - playerBody.transform.position;
        aimDir.y = 0.0f;

        playerBody.UpdateAimDirection(aimDir); // update the direction the playerbody is facing
    }

    /// <summary>
    /// Update position of player based on vertical input
    /// </summary>
    void UpdatePosition()
    {
        float yDirection = Input.GetAxis("Vertical");
        playerBody.ForwardDirection = yDirection;
    }

    /// <summary>
    /// Camera moves to follow player, staying in range of MAX and MIN values
    /// </summary>
    void UpdateCamera()
    {
        Vector3 cameraPos = playerCamera.transform.position; // get current camera position
        // if the current player x position is between the MAX and MIN camera x positions
        if (playerBody.transform.position.x < MAX_CAMERA_MOVE && playerBody.transform.position.x > MIN_CAMERA_MOVE)
        {
            Quaternion cameraRot = playerCamera.transform.rotation;
            // Set camera x and z position to player x and z
            playerCamera.transform.SetPositionAndRotation(new Vector3(playerBody.transform.position.x, cameraPos.y, playerBody.transform.position.z), cameraRot);
        }
        // else if the player x position is greater than MAX x pos
        else if(playerBody.transform.position.x >= MAX_CAMERA_MOVE)
        {
            Quaternion cameraRot = playerCamera.transform.rotation;
            // Set the camera x to MAX value, set z to players z pos
            playerCamera.transform.SetPositionAndRotation(new Vector3(MAX_CAMERA_MOVE, cameraPos.y, playerBody.transform.position.z), cameraRot);
        }
        // else if the player x position is less than MIN x pos
        else if(playerBody.transform.position.x <= MIN_CAMERA_MOVE)
        {
            Quaternion cameraRot = playerCamera.transform.rotation;
            // Set the camera x to MIN value, set z to players z pos
            playerCamera.transform.SetPositionAndRotation(new Vector3(MIN_CAMERA_MOVE, cameraPos.y, playerBody.transform.position.z), cameraRot);
        }
    }

    /// <summary>
    /// Check to see if primary or secondary inputs have been pressed
    /// </summary>
    void CheckForAction()
    {
        bool checkForPrimaryFire = Input.GetButtonDown("Fire1");
        bool checkForSecondaryFire = Input.GetButtonUp("Fire2");

        if (playerBody)
        {
            if (checkForPrimaryFire)
                playerBody.FirePrimary(); // fire primary shot (straight shot)
            else if (checkForSecondaryFire)
                playerBody.FireSecondary(); // fire secondary shot (up shot)
        }
        else
        {
            Debug.Log("There is no PlayerBody attached to the player controller!");
        }
    }
    #endregion
}
