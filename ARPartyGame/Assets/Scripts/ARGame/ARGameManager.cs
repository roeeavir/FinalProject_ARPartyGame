using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ARGameManager : MonoBehaviourPunCallbacks
{
    public static ARGameManager instance;

    [Header("Status")]
    public bool gameEnded = false;
    [Header("Game Manager Helpers")]
    public GameObject spawnManager = null;
    [Header("UI")]
    public GameObject playerUI = null;

    // [Header("Texts")]
    private Text PlayersScores, PlayersTotalScores;


    private GameObject imageTarget;

    private bool hasBeenInitialized = false;

    private bool gameStarted = false;

    private Transform[] spawnPoints = null;

    private Text debugText, objectiveText;

    private SpawnScript spawnScript = null;

    private ShootScript shootScript = null;

    private ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    private bool waitPlayers = false;


    private const string PLAYERS_SCORES = " Players Scores";

    private bool restartTrack = true;

    private int levelScore = 0;

    private int totalScore = 0;

    private int gameLevel = 0;

    private int levelScoreGoal = 5;

    private string winnerInLevel = "";

    private string levelObjective = "";

    private const string lookAtAnchor = "All players need to point their camera at the anchor object in the room";


    private bool startNextLevel = true;

    private int gameMode = 0;

    private Timer timer;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    private void Start()
    {
        // Set debugText from the canvas
        debugText = GameObject.Find("DebugText").GetComponent<Text>();
        objectiveText = GameObject.Find("ObjectiveText").GetComponent<Text>();
        spawnScript = spawnManager.GetComponent<SpawnScript>();
        timer = GetComponent<Timer>();
        SetGameMode();

        customProperties["isReady"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        // Print every player buffered in Photon
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            debugText.text += p.NickName + "\n";
            Debug.LogWarning("Buffered Player: " + p.NickName + "\n");
        }
        // players = new ARPlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInARGame", RpcTarget.AllBuffered);
        Debug.LogWarning("Number of Players: " + PhotonNetwork.PlayerList.Length);
        DefaultObserverEventHandler.isTracking = false;

    }

    private void Update()
    {
        if (imageTarget != null)
        {
            if (gameEnded)
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
                    if (!(bool)PhotonNetwork.LocalPlayer.CustomProperties["isReady"])
                    {
                        SetCustomProperties(true, levelScore, totalScore);
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

                if (restartTrack)
                {
                    RestartImageTargetState();
                    restartTrack = false;
                    StartCoroutine(WaitForTrack());
                }
            }
        }
        else
        {
            SetImageTarget(GameObject.Find("DynamicImageTarget"));
            SetScoreTexts();
        }

    }
    [PunRPC]
    void ImInARGame()
    {
        // Check if the player has been spawned already
        if (hasBeenInitialized)
        {
            return;
        }

        debugText.text += "ImInARGame\n";
        InitializeSpawnPoints(3);
        hasBeenInitialized = true;

    }

    void InitializeSpawnPoints(int size)
    {
        // Create 3 random spawn points
        debugText.text += "SpawnPlayer1\n";
        spawnPoints = new Transform[size];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            string spawnPointName = "SpawnPoint" + PhotonNetwork.LocalPlayer.ActorNumber + "-" + i;

            spawnPoints[i] = SpawnPointsScript.CreateNewSpawnPoint();
            Debug.LogWarning(spawnPointName + ": " + spawnPoints[i].position);
            debugText.text += spawnPointName + ": " + spawnPoints[i].position + "\n";
        }
        // Spawn the player
        debugText.text += "SpawnPlayer2\n";

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

        spawnScript.SetNextTimeToSpawn(2.5f);
        spawnScript.SetGroupId(getDifficultyOfLevel());
        spawnScript.setSpawnPoints(spawnPoints);
        objectiveText.text = levelObjective + "\nThe first to get to " + levelScoreGoal + " points wins!";

        if (shootScript == null)
        {
            shootScript = GameObject.Find("ShootManager").GetComponent<ShootScript>();
        }
        shootScript.SetLevel(gameLevel);
        // shootScript.SetLevel(4);


        if (playerUI != null)
        {
            playerUI.SetActive(true);
            Debug.LogWarning("PlayerUI enabled");
        }
        else
        {
            Debug.LogWarning("PlayerUI not found");
        }

        // gameLevel = 1;


        gameStarted = true;

        Debug.LogWarning("Game started (Level " + gameLevel + ")");
    }

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

    private IEnumerator WaitForPlayers()
    {
        waitPlayers = true;

        objectiveText.text += "\nWaiting for other players to be ready";

        Debug.LogWarning("\nWaiting for other players to be ready");

        yield return new WaitForSeconds(5);

        waitPlayers = false;
    }

    private void SetPlayersScores()
    {
        try {
            SetCustomProperties((bool)customProperties["isReady"], levelScore, totalScore);

            // PlayersScores.text = "Level " + gameLevel + PLAYERS_SCORES + ":\n";
            PlayersScores.text = "";
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                PlayersScores.text += player.NickName + "'s Score: " + (int)player.CustomProperties["score"] + "\n";
            }

            // PlayersTotalScores.text = "\nTotal " + PLAYERS_SCORES + " This Far:\n";
            PlayersTotalScores.text = "";
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                PlayersTotalScores.text += player.NickName + "'s Total Score: " + (int)player.CustomProperties["totalScore"] + "\n";
            }
        } catch (System.Exception e)
        {
            Debug.LogWarning("Error: " + e.Message);
            gameEnded = true;
        }

    }

    private void CheckGameStatus()
    {
        if (levelScore >= levelScoreGoal)
        {
            Debug.LogWarning("A level winner has been decided!");
            photonView.RPC("FinishLevel", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName, levelScore);
        }
    }

    [PunRPC]
    private void FinishLevel(string winnerName, int lvlScore)
    {
        gameStarted = false;
        startNextLevel = false;
        totalScore += levelScore;
        SetCustomProperties(false, levelScore, totalScore);
        SetPlayersScores();
        DefaultObserverEventHandler.isTracking = false; // Reset Vuforia's image tracking
        Debug.LogWarning("Game level " + gameLevel + " has been finished!");
        if (gameLevel != 0)
            setLevelWinnerString(lvlScore);
        spawnScript.setSpawnPoints(null);
        destroyAllEnemies();
        objectiveText.text = winnerName + winnerInLevel;
        playerUI.SetActive(false);
        gameEnded = gameLevel >= 4;
        StartCoroutine(WaitForNextLevel());
    }

    private void StartNextLevel()
    {
        gameLevel++;
        setLevelObjectiveString();
        if (gameLevel < 5)
        {
            Debug.LogWarning("Starting next level (" + gameLevel + ")");
            StartGame();
        }
    }

    private void destroyAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    private void setLevelWinnerString(int lvlScore)
    {
        switch (gameLevel)
        {
            case 1:
                winnerInLevel = " has won the first level with " + lvlScore + " points!";
                break;
            case 2:
                winnerInLevel = " has won the second level with " + lvlScore + " points!";
                break;
            case 3:
                winnerInLevel = " has won the third level with " + lvlScore + " points!";
                break;
            case 4:
                winnerInLevel = " has won the final level with " + lvlScore + " points!";
                break;
            // case 5:
            //     winnerInLevel = " has won the game with " + lvlScore + " points!!!";
            // photonView.RPC("SetGameEnded", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
            // break;
            default:
                Debug.LogWarning("Bad game level: " + gameLevel + " in setLevelWinnerString");
                break;
        }

    }

    private void setLevelObjectiveString()
    {
        switch (gameLevel)
        {
            case 1:
                Debug.LogWarning("Level 1 Objective and Spawn Points");
                levelObjective = "Shoot the enemies and earn the most points!";
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
        levelScore = 0;


    }

    private IEnumerator WaitForNextLevel()
    {
        Debug.LogWarning("Waiting for next level");
        yield return new WaitForSeconds(5);
        shootScript.ResetScore();
        SetCustomProperties(false, 0, totalScore);
        if (!gameEnded)
        {
            objectiveText.text = "Starting the next game level (" + (gameLevel + 1) + ")\n" + lookAtAnchor;
            startNextLevel = true;
        } else {
            StartCoroutine(WaitForGameEnd());
        }
    }

    private void SetCustomProperties(bool b, int lScore, int tScore)
    {
        // customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["isReady"] = b;
        customProperties["score"] = levelScore;
        customProperties["totalScore"] = tScore;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    private IEnumerator WaitForTrack()
    {
        yield return new WaitForSeconds(3);
        for (int i = 0; i < imageTarget.transform.childCount; i++)
        {
            imageTarget.transform.GetChild(i).gameObject.SetActive(false);
        }
        restartTrack = true;
    }

    private void RestartImageTargetState()
    {
        for (int i = 0; i < imageTarget.transform.childCount; i++)
        {
            imageTarget.transform.GetChild(i).gameObject.SetActive(DefaultObserverEventHandler.isTracking);
            // if (!gameStarted)
            //     imageTarget.transform.GetChild(i).gameObject.transform.LookAt(Camera.main.transform);
        }
    }

    // [PunRPC]
    // private void SetGameEnded(string winnerName)
    // {
    //     gameEnded = true;
    //     Destroy(NetworkManager.instance.gameObject);
    //     StartCoroutine(WaitForGameEnd());
    // }


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
                return 100 * (gameMode + 1);
            default:
                Debug.LogWarning("Bad game level: " + gameLevel + " in getDifficultyOfLevel");
                return 0;
        }
    }

    private void SetGameMode()
    {
        gameMode = GameMode.gameMode;
        if (gameMode == 0)
        {
            Debug.LogWarning("Game mode set to casual - " + gameMode);
        }
        else if (gameMode == 1)
        {
            Debug.LogWarning("Game mode set to intermediate - " + gameMode);
        }
        else if (gameMode == 2)
        {
            Debug.LogWarning("Game mode set to intense - " + gameMode);
        }
        else
        {
            Debug.LogWarning("Game mode set to bad value - " + gameMode);
        }
        levelScoreGoal *= (gameMode + 1);
    }

    public void OnResetTargetObjectBtn()
    {
        Debug.LogWarning("Reset Target Object Button Pressed");
        DefaultObserverEventHandler.isTracking = false;
        gameObject.GetComponent<SideLoadImageTarget>().setTargetChildren();
        SetImageTarget(GameObject.Find("DynamicImageTarget"));
        Debug.LogWarning("Reset Target Object Complete");
    }

    private void SetScoreTexts()
    {
        PlayersScores = GameObject.Find("Players Level Scores").GetComponent<Text>();
        PlayersTotalScores = GameObject.Find("Players Total Scores").GetComponent<Text>();
        if (PlayersScores == null || PlayersTotalScores == null){
            gameEnded = true;
            Debug.LogWarning("Players Scores or Total Scores not found");
        }
    }

    private Player GetWinner(){
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

    private IEnumerator WaitForGameEnd()
    {
        Debug.LogWarning("Waiting for game end");
        Player winner = GetWinner();
        objectiveText.text = "Game has ended!\n" + "The winner is player " + winner.NickName + " with " + (int)winner.CustomProperties["totalScore"] + " points!";
        yield return new WaitForSeconds(5);
        Debug.LogWarning("Game ended");
        Destroy(NetworkManager.instance.gameObject);
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
        {
            yield return new WaitForSeconds(1);
        }
        // customProperties["score"] = 0;
        // PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
        Destroy(gameObject);
    }
}