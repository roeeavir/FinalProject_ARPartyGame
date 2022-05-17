using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


public class ARGameManager : MonoBehaviourPunCallbacks
{
    public static ARGameManager instance;

    [Header("Status")]
    public bool gameEnded = false;
    [Header("Game Manager Helpers")]
    public GameObject spawnManager = null;
    [Header("UI")]
    public GameObject playerUI = null;
    public GameObject bossUI;

    [Header("AR Camera")]
    public GameObject arCamera;


    // [Header("Texts")]
    private Text PlayersScores, PlayersTotalScores;


    private GameObject imageTarget;

    private bool hasBeenInitialized = false;

    private bool gameStarted = false;

    private Transform[] spawnPoints = null;

    private Text objectiveText;

    private SpawnScript spawnScript = null;

    private ShootScript shootScript = null;

    private ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    private bool waitPlayers = false;

    private const string PLAYERS_SCORES = " Players Scores";

    private bool restartTrack = true;

    private int levelScore = 0;

    private int totalScore = 0;

    private int gameLevel = 0;

    private int levelScoreGoal = 15;

    private string winnerInLevel = "";

    private string levelObjective = "";

    private const string lookAtAnchor = "All players need to point their camera at the anchor object in the room";

    private bool startNextLevel = true;

    private int gameMode = 0;

    private Timer timer;

    private bool allPlayersConnected = false;




    private void Awake()
    {
        if (instance == null) // Singleton
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    private void Start()
    {
        objectiveText = GameObject.Find("ObjectiveText").GetComponent<Text>();
        spawnScript = spawnManager.GetComponent<SpawnScript>();
        timer = GetComponent<Timer>();
        SetGameMode(); // Set the game mode

        SetCustomProperties(false, 0, 0);
        // Print every player buffered in Photon
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            Debug.LogWarning("Buffered Player: " + p.NickName + "\n");
        }
        photonView.RPC("ImInARGame", RpcTarget.AllBuffered); // Tell everyone that we are in the game
        Debug.LogWarning("Number of Players: " + PhotonNetwork.PlayerList.Length);
        DefaultObserverEventHandler.isTracking = false;
        arCamera.SetActive(true); // Activate AR camera (Tries to prevent a bug where the camera counts as the center of the AR world)

    }

    private void Update()
    {
        if (!allPlayersConnected)
        {
            allPlayersConnected = AreAllPlayersConnected(); // Check if all players are connected
            return;
        }
        if (imageTarget != null)
        {
            if (gameEnded) // If the game has ended, prevent the Update function from doing anything
            {
                return;
            }
            if (gameStarted)
            {
                if (shootScript != null)
                {
                    levelScore = shootScript.GetScore();
                    SetPlayersScores();
                    CheckGameStatus();
                }
                else
                {
                    Debug.LogWarning("shootScript is null");
                }

            }

            if (DefaultObserverEventHandler.isTracking)
            {

                if (startNextLevel && !gameStarted)
                {
                    if (!(bool)PhotonNetwork.LocalPlayer.CustomProperties["isReady"]) // If the player is not ready, update the player's properties to ready
                    {
                        SetCustomProperties(true, levelScore, totalScore); // Set the custom properties
                    }

                    if (ArePlayersReady())
                    {
                        timer.StartTimer(StartNextLevel); // Start the timer for the next level
                    }
                    else
                    {
                        if (!waitPlayers)
                        {
                            Debug.LogWarning("Not all players are ready");
                            StartCoroutine(WaitForPlayers());
                        }
                    }
                }

                if (restartTrack) // Restarts the image target tracking every 3 seconds
                {
                    RestartImageTargetState();
                    restartTrack = false;
                    StartCoroutine(WaitForTrack());
                }
            }
        }
        else
        {
            SetImageTarget(GameObject.Find("DynamicImageTarget")); // Sets the image target from the existing game object in the scene
            SetScoreTexts(); // Set the score texts
        }

    }
    // Initialize the first game level spawn points
    [PunRPC]
    void ImInARGame()
    {
        // Check if the player has been spawned already
        if (hasBeenInitialized)
        {
            return;
        }

        InitializeSpawnPoints(3);
        hasBeenInitialized = true;

    }
    // Initialize the spawn points by the number given
    void InitializeSpawnPoints(int size)
    {
        // Create 3 random spawn points
        spawnPoints = new Transform[size];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            string spawnPointName = "SpawnPoint" + PhotonNetwork.LocalPlayer.ActorNumber + "-" + i;

            spawnPoints[i] = SpawnPointsScript.CreateNewSpawnPoint();
            Debug.LogWarning(spawnPointName + ": " + spawnPoints[i].position);
        }
    }

    // Starts the game each level
    private void StartGame()
    {
        if (!spawnScript.enabled)
        {
            Debug.LogWarning("Game Started. Enabling SpawnScript");
            // Enable SpawnManager
            spawnScript.enabled = true;
        }

        spawnScript.SetNextTimeToSpawn(2.5f); // Set the time to spawn the next enemy
        spawnScript.SetGroupId(getDifficultyOfLevel()); // Set the spawn script's group id to the difficulty of the level
        spawnScript.setSpawnPoints(spawnPoints); // Set the spawn script's spawn points to the spawn points created in InitializeSpawnPoints
        objectiveText.text = levelObjective + "\nThe first to get to " + levelScoreGoal + " points wins!";

        if (shootScript == null)
        {
            shootScript = GameObject.Find("ShootManager").GetComponent<ShootScript>();
        }
        shootScript.SetLevel(gameLevel); // Set the level of the game to the current level for the shoot script


        if (playerUI != null)
        {
            playerUI.SetActive(true);
            Debug.LogWarning("PlayerUI enabled");
        }
        else
        {
            Debug.LogWarning("PlayerUI not found");
        }

        gameStarted = true;

        Debug.LogWarning("Game started (Level " + gameLevel + ")");
    }

    // Checks if all players are ready
    public bool ArePlayersReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!(bool)player.CustomProperties["isReady"]) // if any player is not ready
            {
                return false;
            }
        }
        return true;
    }

    // Notifies the players that the other players are not ready yet
    private IEnumerator WaitForPlayers()
    {
        waitPlayers = true;

        objectiveText.text += "\nWaiting for other players to be ready";

        Debug.LogWarning("\nWaiting for other players to be ready");

        yield return new WaitForSeconds(5);

        waitPlayers = false;
    }

    // Sets all the players scores to the scores board
    private void SetPlayersScores()
    {
        try
        {
            SetCustomProperties((bool)customProperties["isReady"], levelScore, totalScore); // Set the custom properties

            // Sets the normal scores (for the current level)
            PlayersScores.text = "";
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                PlayersScores.text += player.NickName + "'s Score: " + (int)player.CustomProperties["score"] + "\n";
            }

            // Sets the total scores
            PlayersTotalScores.text = "";
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                PlayersTotalScores.text += player.NickName + "'s Total Score: " + (int)player.CustomProperties["totalScore"] + "\n";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error: " + e.Message);
            gameEnded = true;
        }

    }
    // Checks if the player has reached the level's score objective and if so, end the level for all players
    private void CheckGameStatus()
    {
        if (levelScore >= levelScoreGoal)
        {
            Debug.LogWarning("A level winner has been decided!");
            photonView.RPC("FinishLevel", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName, levelScore);
        }
    }

    // End the level. Reset variables. Update objectives and scores.
    [PunRPC]
    private void FinishLevel(string winnerName, int lvlScore)
    {
        if (gameLevel == 4)
        {
            bossUI.SetActive(false);
        }
        FindObjectOfType<AudioManager>().Play("Win");
        gameStarted = false;
        startNextLevel = false;
        totalScore += levelScore;
        SetCustomProperties(false, levelScore, totalScore);
        SetPlayersScores();
        DefaultObserverEventHandler.isTracking = false; // Reset Vuforia's image tracking
        Debug.LogWarning("Game level " + gameLevel + " has been finished!");
        if (gameLevel != 0)
        {
            setLevelWinnerString(lvlScore); // Set the level winner string
        }
        spawnScript.setSpawnPoints(null);
        destroyAllEnemies(); // Destroy all enemies in the scene
        objectiveText.text = winnerName + winnerInLevel;
        playerUI.SetActive(false); // Disable the player UI (Shoot button and crosshair)
        gameEnded = gameLevel >= 4; // If the game is over (4 levels)
        levelScoreGoal += gameLevel * 2; // Increase the level score goal for the next level by a little
        StartCoroutine(WaitForNextLevel()); // Wait for the next level
    }

    // Starts the next level
    private void StartNextLevel()
    {
        gameLevel++;
        setLevelObjectiveString();
        if (gameLevel < 5) // If the game is not over
        {
            Debug.LogWarning("Starting next level (" + gameLevel + ")");
            if (gameLevel == 4)
            {
                bossUI.SetActive(true); // Activate the boss UI if the game reaches level 4 (last level)
            }
            StartGame();
        }
    }

    // Destroys all enemies in the scene
    private void destroyAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy"); // Find all objects with the enemy tag
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    // Sets the level winner string (the string that is displayed when the level is finished)
    private void setLevelWinnerString(int lvlScore)
    {
        string levelNumber = "";
        switch (gameLevel)
        {
            case 1:
                levelNumber = "1st";
                break;
            case 2:
                levelNumber = "2nd";
                break;
            case 3:
                levelNumber = "3rd";
                break;
            case 4:
                levelNumber = "Final";
                break;
            default:
                levelNumber = "Problem with level number senior";
                Debug.LogWarning("Bad game level: " + gameLevel + " in setLevelWinnerString");
                break;
        }
        winnerInLevel = " has won the " + levelNumber + " level with " + lvlScore + " points!";

    }

    // Sets the level objective string (the string that is displayed when the level has started)
    private void setLevelObjectiveString()
    {
        switch (gameLevel)
        {
            case 1:
                Debug.LogWarning("Level 1 Objective and Spawn Points");
                levelObjective = "Shoot the enemies regardless of color and earn the most points!";
                break;
            case 2:
                Debug.LogWarning("Level 2 Objective and Spawn Points");
                InitializeSpawnPoints(PhotonNetwork.PlayerList.Length + 1);
                levelObjective = "Shoot the enemies that match your color and earn the most points!";
                break;
            case 3:
                Debug.LogWarning("Level 3 Objective and Spawn Points");
                InitializeSpawnPoints(5);
                levelObjective = "Shoot the enemies that match your color and earn the most points! Your color will change for each enemy killed!";
                break;
            case 4:
                Debug.LogWarning("Level 4 Objective and Spawn Points");
                InitializeSpawnPoints(1);
                levelObjective = "Boss level!\n Shoot the boss and be the first to destroy it!";
                break;
            default:
                Debug.LogWarning("Bad game level: " + gameLevel + " in SetLevelObjectiveString");
                break;
        }
        levelScore = 0; // Reset the current level score
    }

    // Waits for the next level to start
    private IEnumerator WaitForNextLevel()
    {
        // Play a Sound fitting for the level winner and loser(s)
        if (winnerInLevel.Equals(PhotonNetwork.LocalPlayer.NickName))
        {
            FindObjectOfType<AudioManager>().Play("Win");
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("Lose");
        }

        Debug.LogWarning("Waiting for next level");
        yield return new WaitForSeconds(5);
        shootScript.ResetScore(); // Reset the player's score
        SetCustomProperties(false, 0, totalScore); // Reset the player's custom properties
        if (!gameEnded)
        {
            objectiveText.text = "Starting the next game level (" + (gameLevel + 1) + ")\n" + lookAtAnchor;
            startNextLevel = true;
        }
        else
        {
            StartCoroutine(WaitForGameEnd());
        }
    }

    // Sets the player's custom properties
    private void SetCustomProperties(bool b, int lScore, int tScore)
    {
        // customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["isReady"] = b;
        customProperties["score"] = levelScore;
        customProperties["totalScore"] = tScore;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    // Sets the image targets children to false as to prevent them from being stuck on the screen when they should be at the image target's position
    private IEnumerator WaitForTrack()
    {
        yield return new WaitForSeconds(3);
        for (int i = 0; i < imageTarget.transform.childCount; i++)
        {
            imageTarget.transform.GetChild(i).gameObject.SetActive(false); // Set all children to false
        }
        restartTrack = true; // Allow the image target to be tracked again
    }

    // Sets the image targets children active to the same variable as the tracking itself
    private void RestartImageTargetState()
    {
        for (int i = 0; i < imageTarget.transform.childCount; i++)
        {
            imageTarget.transform.GetChild(i).gameObject.SetActive(DefaultObserverEventHandler.isTracking);
        }
    }

    // Sets the image target to the one given
    private void SetImageTarget(GameObject newImageTarget)
    {
        // set imageTarget to from SideLoadImageTarget script
        Debug.LogWarning("Image Target yet to be set");
        imageTarget = newImageTarget;
        imageTarget.GetComponent<DefaultObserverEventHandler>().StatusFilter = DefaultObserverEventHandler.TrackingStatusFilter.Tracked; ;

        if (imageTarget != null)
            Debug.LogWarning("Image Target found");
        else
            Debug.LogWarning("Image Target not found");
    }

    // Returns the difficulty level of the level
    private int getDifficultyOfLevel()
    {
        switch (gameLevel)
        {
            case 1:
                return 0; // Normal
            case 2:
                return 3 + gameMode; // All enemies are fast and random
            case 3:
                return 4 + gameMode; // All enemies are faster and randomer
            case 4: // Boss
                levelScoreGoal = 100 * (gameMode + 1);
                return 100 * (gameMode + 1); // Boss
            default:
                Debug.LogWarning("Bad game level: " + gameLevel + " in getDifficultyOfLevel");
                return 0;
        }
    }

    // Sets the game mode to the given value
    private void SetGameMode()
    {
        gameMode = GameMode.gameMode;

        if (gameMode == 0)
            Debug.LogWarning("Game mode set to casual - " + gameMode);
        else if (gameMode == 1)
            Debug.LogWarning("Game mode set to intermediate - " + gameMode);
        else if (gameMode == 2)
            Debug.LogWarning("Game mode set to intense - " + gameMode);
        else
            Debug.LogWarning("Game mode set to bad value - " + gameMode);

        levelScoreGoal *= (gameMode + 1);
    }

    // Resets the anchor image tracking and the AR camera (fixes the anchor image tracking position)
    public void OnResetTargetObjectBtn()
    {
        Debug.LogWarning("Reset Target Object Button Pressed");
        DefaultObserverEventHandler.isTracking = false;
        SetCustomProperties(false, levelScore, totalScore); // Reset ready status
        gameObject.GetComponent<SideLoadImageTarget>().setTargetChildren();
        SetImageTarget(GameObject.Find("DynamicImageTarget")); // Sets the image target from the existing game object in the scene
        Debug.LogWarning("Reset Target Object Complete");
    }

    // Sets the score texts objects to the correct text variable
    private void SetScoreTexts()
    {
        PlayersScores = GameObject.Find("Players Level Scores").GetComponent<Text>();
        PlayersTotalScores = GameObject.Find("Players Total Scores").GetComponent<Text>();
        if (PlayersScores == null || PlayersTotalScores == null)
        {
            gameEnded = true;
            Debug.LogWarning("Players Scores or Total Scores not found");
        }
    }

    // Returns the player with the highest score (if there is a tie, it returns the first player)
    private Player GetWinner()
    {
        Player winner = null;
        int maxScore = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties["totalScore"] != null)
            {
                if ((int)p.CustomProperties["totalScore"] > maxScore)
                {
                    maxScore = (int)p.CustomProperties["totalScore"];
                    winner = p;
                }
            }
        }
        return winner;
    }

    // Wait for game to end
    private IEnumerator WaitForGameEnd()
    {
        Debug.LogWarning("Waiting for game end");
        Player winner = GetWinner(); // Returns the player that won the game
        objectiveText.text = "Game has ended!\n" + "The winner is player " + winner.NickName + " with " + (int)winner.CustomProperties["totalScore"] + " points!";

        if (winner.NickName == PhotonNetwork.LocalPlayer.NickName)
            FindObjectOfType<AudioManager>().Play("Win");
        else
            FindObjectOfType<AudioManager>().Play("Lose");

        AddPlayersDataToScoreboard(); // Add player data to scoreboard
        yield return new WaitForSeconds(5);

        Debug.LogWarning("Game ended");
        Destroy(NetworkManager.instance.gameObject); // Destroy network manager
        PhotonNetwork.LeaveRoom(); // Disconnect from room
        while (PhotonNetwork.InRoom)
        {
            yield return new WaitForSeconds(1);
        }
        SceneManager.LoadScene("Main", LoadSceneMode.Single); // Load main menu scene
        Destroy(gameObject); // Destroy game manager
    }


    // Add player(s) data to local scoreboard
    private void AddPlayersDataToScoreboard()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties["totalScore"] != null) // Checks if the player has a total score property
            {
                FindObjectOfType<LeaderBoardManager>().AddPlayerData(p.NickName, (int)p.CustomProperties["totalScore"], DateTime.Now.ToString("dd-MM-yyyy"));
            }
        }
        FindObjectOfType<LeaderBoardManager>().SaveScoreBoard();
    }

    // Check if all players are connected to the new scene
    private bool AreAllPlayersConnected()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.CustomProperties.ContainsKey("score")) // Checks if the player has a score property (if not, they are not connected yet)
            {
                return false;
            }
        }
        return true;
    }
}