/*
 * Author: Matthew Minnett
 * Desc: Loads levels, manages game state
 * Date Created: 2023/03/05
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Variables
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            // if we haven't assigned the game manager go find it
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }

            // if there isn't a game manager in the scene, print error
            if (instance == null)
            {
                Debug.LogError("No Game Manager Found");
            }

            return instance;
        }
    }

    // Player Variables
    [SerializeField]
    [Tooltip("Player Game Object")]
    private GameObject playerGO;
    public GameObject PlayerGO { get { return playerGO; } }

    private PlayerBody playerBody;
    public PlayerBody PlayerBody { get { return playerBody; } }

    private PlayerController playerController;

    // Level Variables
    [SerializeField]
    [Tooltip("Names of each level/scene")]
    private string[] levelNames;
    private int currentLevelIndex = 0;
    public int CurrentLevelIndex { get { return currentLevelIndex; } }

    private string currentLevelName;
    public string CurrentLevelName { get { return currentLevelName; } }
    bool isLoading = false;

    Vector3 startPosition;

    // Menu variables
    [SerializeField]
    public PauseMenu pauseMenu;
    [SerializeField]
    GameObject victoryScreen;
    [SerializeField]
    LoadingScreen loadingScreen;
    [SerializeField]
    GameOver gameOver;
    [SerializeField]
    OptionsMenu optionsMenu;
    public OptionsMenu OptionsMenu { get { return optionsMenu; } }

    [SerializeField]
    string mainMenuName;
    public string MainMenuName { get { return mainMenuName; } }

    // Save Variables
    private bool saveGameExists = false;
    public bool SaveGameExists { get { return saveGameExists; } set { saveGameExists = value; } }
    #endregion

    #region Unity Functions
    private void Awake()
    {
        // Set this instance to be the game manager
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        LoadGame(); // see if there is a game to load
        OptionsMenu.Instance.LoadOptions(); // load saved options (if any)

        playerBody = playerGO.GetComponentInChildren<PlayerBody>(); // find player body script
        playerController = playerGO.GetComponent<PlayerController>(); // find player controller script

        startPosition = playerBody.transform.position; // track start pos of player

        // start at main menu
        ReturnToMainMenu();
    }
    #endregion

    #region Level Functions
    /// <summary>
    /// Loads level, given the name.
    /// </summary>
    /// <param name="levelName"></param>
    /// <returns></returns>
    private IEnumerator LoadLevel(string levelName)
    {
        isLoading = true;
        pauseMenu.CanPause = false; // cannot pause when loading

        if(OptionsMenu.Instance)
            OptionsMenu.Instance.Ready = false; // don't update audio in loading screen

        loadingScreen.gameObject.SetActive(true); // make loading screen visible

        playerGO.SetActive(false); // player inactive

        // check to see if the current level has been set (loaded)
        if (!string.IsNullOrEmpty(currentLevelName))
        {
            // Unload the current level
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentLevelName);

            yield return SoundManager.Instance.UnloadLevel(); // volume lowers to -80

            while (!asyncUnload.isDone)
            {
                yield return null; // suspend, come back and check again
            }
        }

        // laod scene in background in a different thread
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        // wait for load to complete
        while (!asyncLoad.isDone)
        {
            yield return null; // suspend, come back and check again
        }

        for(int i = 0; i < 100; i++) // for loop 'fakes' loading screen
        {
            loadingScreen.UpdateLoadBar(i * 0.01f);
            yield return new WaitForSeconds(.01f); // waits .01 second before updating loadbar again
        }

        // set the active scene so that objects are created in this scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));

        // level has been loaded
        currentLevelName = levelName;

        SoundManager.LevelLoadComplete(); // lerp volume back to overall volume

        if (currentLevelName != mainMenuName) // if the now loaded level is not the main menu
        {
            playerGO.SetActive(true); // set player active
            playerBody.transform.position = startPosition; // update position
            pauseMenu.CanPause = true; // can pause again
        }
        // reset object pools
        PlayerPool.Instance.GameReset();
        PlayerAltPool.Instance.GameReset();

        if (OptionsMenu.Instance)
            OptionsMenu.Instance.Ready = true; // options menu updates

        isLoading = false;
        loadingScreen.gameObject.SetActive(false); // make loading screen invisible
    }

    /// <summary>
    /// Save data or show victory screen when level is complete
    /// </summary>
    public void LevelComplete()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levelNames.Length && currentLevelIndex != 0) // if current level index does not exceed amount of level names, and not the first level
        {
            SaveGame(); // save current level
            StartCoroutine(LoadLevel(levelNames[currentLevelIndex])); // load next level
        }
        else
        {
            // canpause to false when game complete
            pauseMenu.CanPause = false;

            victoryScreen.SetActive(true); // make victory screen visible
            playerBody.CanMove = false; // player cannot move
            PlayerPrefs.DeleteKey("currentLevel"); // delete currentlevel save
        }
    }

    #region Start/Restart/Continue
    /// <summary>
    /// Deletes current level save data, starts game from first level
    /// </summary>
    public void StartNewGame()
    {
        PlayerPrefs.DeleteKey("currentLevel");
        currentLevelIndex = 0;
        StartCoroutine(LoadLevel(levelNames[currentLevelIndex]));
    }

    /// <summary>
    /// Loads current level again
    /// </summary>
    public void RestartLevel()
    {
        StartCoroutine(LoadLevel(currentLevelName));
    }

    /// <summary>
    /// Continues game from saved index
    /// </summary>
    public void ContinueGame()
    {
        // current level name will be set when the game is loaded
        StartCoroutine(LoadLevel(levelNames[currentLevelIndex]));
    }
    #endregion

    #region Save/Load
    /// <summary>
    /// Saves game, if the current index is not the first level
    /// </summary>
    public void SaveGame()
    {
        if (currentLevelIndex != 0)
        {
            // save the current level the player is on
            PlayerPrefs.SetInt("currentLevel", currentLevelIndex);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Detects if there is save data for current level, updates data accordingly
    /// </summary>
    public void LoadGame()
    {
        if(PlayerPrefs.HasKey("currentLevel"))
        {
            saveGameExists = true; // a save game exists!

            int temp = PlayerPrefs.GetInt("currentLevel");
            if(temp > levelNames.Length) // if the saved level is greater than amount of levels
            {
                temp = 0; // set to first level
                saveGameExists = false; // save game does not exist
            }

            currentLevelIndex = temp; // set the current level index to the loaded data
        }
    }
    #endregion

    /// <summary>
    /// Loads main menu scene
    /// </summary>
    public void ReturnToMainMenu()
    {
        pauseMenu.Pause(false); // close pause menu if open
        pauseMenu.CanPause = false; // cannot pause
        gameOver.gameObject.SetActive(false); // gameover screen inactive

        // reset player data
        playerBody.Reset();
        PlayerPool.Instance.GameReset();
        PlayerAltPool.Instance.GameReset();

        StartCoroutine(LoadLevel(mainMenuName)); // load main menu
    }
    #endregion

    #region Player Death/Respawn
    /// <summary>
    /// Displays gameover screen on player death
    /// </summary>
    public void PlayerDeath()
    {
        pauseMenu.CanPause = false; // cannot pause
        gameOver.gameObject.SetActive(true); // gameover screen visible
        SoundManager.Instance.GameOverAudio(); // play gameover audio

        SoundManager.Instance.Mixer.SetFloat("Enemies", -80f); // make enemies silent
    }

    /// <summary>
    /// Resets data for player
    /// </summary>
    public void PlayerRespawn()
    {
        gameOver.gameObject.SetActive(false); // gameover screen invisible

        // reset player data
        playerBody.Reset();

        SoundManager.Instance.Mixer.SetFloat("Enemies", 0f); // enemies are noisy again

        // reset object pools
        PlayerPool.Instance.GameReset();
        PlayerAltPool.Instance.GameReset();

        RestartLevel(); // call restart level function to restart current level
    }
    #endregion
}
