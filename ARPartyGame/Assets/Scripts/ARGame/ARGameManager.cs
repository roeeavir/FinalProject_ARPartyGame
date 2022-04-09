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
    [Header("Status")]
    public bool gameEnded = false;
    [Header("Players")]
    public static ARGameManager instance;

    // public ARPlayerController[] players;

    private GameObject imageTarget;

    private bool hasBeenInitialized = false;

    private bool gameStarted = false;

    private Transform[] spawnPoints = null;

    public GameObject spawnManager = null;

    public GameObject playerUI = null;

    private Text debugText, objectiveText, scoreText;

    private SpawnScript spawnScript = null;

    private ShootScript shootScript = null;

    private ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    private bool waitPlayers = false;

    public Text PlayersScores;

    private const string PLAYERS_SCORES = " Players Scores:";

    private bool restartTrack = true;

    private int levelScore = 0;

    private int totalScore = 0;

    private int gameLevel = 0;

    private int roundScoreGoal = 20;

    private string winnerInLevel = "";

    private string levelObjective = "";

    private const string lookAtAnchor = "All players need to point their camera at the anchor object in the room";

    private Color[] colors = { Color.blue, Color.red, Color.yellow, Color.magenta, Color.green };

    private bool startNextRound = true;



    private int colorID = 0;
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

        colorID = PhotonNetwork.LocalPlayer.ActorNumber - 1; // Sets the color of the player to the color of the player's ID
        objectiveText.color = colors[colorID];
        Debug.LogWarning("Players Color : " + colors[colorID]);

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
                    scoreText.text = levelScore.ToString();
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

                if (startNextRound && !gameStarted)
                {
                    if (!(bool)PhotonNetwork.LocalPlayer.CustomProperties["isReady"])
                    {
                        SetCustomProperties(true, levelScore, totalScore);
                    }

                    if (ArePlayersReady())
                    {
                        StartNextLevel();
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
            GameObject newObj = new GameObject(spawnPointName);
            // Randomly position the spawn points
            float val = Random.Range(0, 2);
            float x = val < 1 ? -i - 2 : i + 2;
            val = Random.Range(0, 2);
            float y = val < 1 ? -i - 2 : i + 2;
            val = Random.Range(0, 2);
            float z = val < 1 ? -i - 2 : i + 2;

            newObj.transform.position = new Vector3(x, y, z);
            spawnPoints[i] = GameObject.Find(spawnPointName).transform;
            Debug.LogWarning(spawnPointName + ": " + spawnPoints[i].position);
            debugText.text += spawnPointName + ": " + spawnPoints[i].position + "\n";
        }
        // Spawn the player
        debugText.text += "SpawnPlayer2\n";

    }

    // Starts the game each level
    private void StartBalloonGame()
    {
        if (!spawnScript.enabled)
        {
            Debug.LogWarning("Game Started. Enabling SpawnScript");
            // Enable SpawnManager
            spawnScript.enabled = true;
        }

        spawnScript.setSpawnPoints(spawnPoints);
        objectiveText.text = levelObjective + "\nThe first to get to " + roundScoreGoal + " points wins!";

        if (shootScript == null)
        {
            shootScript = GameObject.Find("ShootManager").GetComponent<ShootScript>();
            // shootScript.SetColors(colors);
        }
        shootScript.SetLevel(gameLevel);


        if (playerUI != null)
        {
            playerUI.SetActive(true);
            Debug.LogWarning("PlayerUI enabled");
            scoreText = playerUI.GetComponentInChildren<Text>();
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
        SetCustomProperties((bool)customProperties["isReady"], levelScore, totalScore);

        PlayersScores.text = "Round " + gameLevel + PLAYERS_SCORES;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            PlayersScores.text += player.NickName + "'s Score: " + (int)player.CustomProperties["score"] + "\n";
        }

        PlayersScores.text += "\nTotal " + PLAYERS_SCORES + " This Far\n";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            PlayersScores.text += player.NickName + "'s Total Score: " + (int)player.CustomProperties["totalScore"] + "\n";
        }
    }

    private void CheckGameStatus()
    {
        if (levelScore >= roundScoreGoal)
        {
            Debug.LogWarning("A round winner has been decided!");
            photonView.RPC("FinishLevel", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        }
    }

    [PunRPC]
    private void FinishLevel(string winnerName)
    {
        gameStarted = false;
        startNextRound = false;
        totalScore += levelScore;
        SetCustomProperties(false, levelScore, totalScore);
        // SetImageTarget(null);
        // Reset Vuforia's image tracking
        DefaultObserverEventHandler.isTracking = false;
        Debug.LogWarning("Game level " + gameLevel + " has been finished!");
        if (gameLevel != 0)
            setLevelWinnerString();
        spawnScript.setSpawnPoints(null);
        destroyAllBalloons();
        objectiveText.text = winnerName + winnerInLevel;
        playerUI.SetActive(false);
        StartCoroutine(WaitForNextRound());
    }

    private void StartNextLevel()
    {
        Debug.LogWarning("Starting next level (" + gameLevel + ")");
        gameLevel++;
        setLevelObjectiveString();
        StartBalloonGame();
    }

    private void destroyAllBalloons()
    {
        GameObject[] balloons = GameObject.FindGameObjectsWithTag("balloon");
        foreach (GameObject balloon in balloons)
        {
            Destroy(balloon);
        }
    }

    private void setLevelWinnerString()
    {
        switch (gameLevel)
        {
            case 1:
                winnerInLevel = " has won the first round with " + levelScore + " points!";
                break;
            case 2:
                winnerInLevel = " has won the second round with " + levelScore + " points!";
                break;
            case 3:
                winnerInLevel = " has won the third round with " + levelScore + " points!";
                break;
            case 4:
                winnerInLevel = " has won the fourth round with " + levelScore + " points!";
                break;
            case 5:
                winnerInLevel = " has won the fifth round with " + levelScore + " points!";
                break;
            case 6:
                winnerInLevel = " has won the game with " + levelScore + " points!!!";
                photonView.RPC("SetGameEnded", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
                break;
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
                levelObjective = "Shoot the balloons and earn the most points!";
                break;
            case 2:
                Debug.LogWarning("Level 2 Objective and Spawn Points");
                InitializeSpawnPoints(PhotonNetwork.PlayerList.Length + 1);
                levelObjective = "Shoot the balloons in your color and earn the most points!\n Hitting other players balloons will give them points in your stead!";
                break;
            case 3:
                Debug.LogWarning("Level 3 Objective and Spawn Points");
                InitializeSpawnPoints(PhotonNetwork.PlayerList.Length + 1);
                levelObjective = "Shoot the balloons in your color and earn the most points!\n Hitting other players balloons will give them points in your stead and will make you lose points!";
                break;
            case 4:
                Debug.LogWarning("Level 4 Objective and Spawn Points");
                InitializeSpawnPoints(3);
                levelObjective = "Mini boss round!\n Shoot the big balloon and be the first to pop it!";
                break;
            case 5:
                Debug.LogWarning("Level 5 Objective and Spawn Points");
                InitializeSpawnPoints(4);
                levelObjective = "Final Round!\n Shoot the mega ultra horsing balloon and be the first to pop it!";
                break;
            default:
                Debug.LogWarning("Bad game level: " + gameLevel + " in SetLevelObjectiveString");
                break;
        }
        levelScore = 0;


    }

    private IEnumerator WaitForNextRound()
    {
        yield return new WaitForSeconds(5);
        if (gameLevel <= 6)
        {
            objectiveText.text = "Starting the next game level (" + (gameLevel + 1) + ")\n" + lookAtAnchor;
            shootScript.ResetScore();
            SetCustomProperties(false, 0, totalScore);
            startNextRound = true;
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
            // if (!gameStarted)
            //     imageTarget.transform.GetChild(i).gameObject.transform.LookAt(Camera.main.transform);
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

    [PunRPC]
    private void SetGameEnded(string winnerName)
    {
        gameEnded = true;
        Destroy(NetworkManager.instance.gameObject);
        StartCoroutine(WaitForGameEnd());
    }



    private IEnumerator WaitForGameEnd()
    {
        Debug.LogWarning("Waiting for game end");
        yield return new WaitForSeconds(5);
        Debug.LogWarning("Game ended");
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
        {
            yield return new WaitForSeconds(1);
        }
        customProperties["score"] = 0;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
        Destroy(gameObject);
    }

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



}
