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

    public ARPlayerController[] players;
    private List<int> pickedSpawnIndex;


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

    private bool wait = false;

    public Text PlayersScores;

    private const string PLAYERS_SCORES = "Players Scores:\n";

    private bool restartTrack = true;

    private int score = 0;

    private string gameType = "";
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
        pickedSpawnIndex = new List<int>();
        players = new ARPlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInARGame", RpcTarget.AllBuffered);
        debugText.text += "Number of Players: " + PhotonNetwork.PlayerList.Length + "\n";
        Debug.LogWarning("Number of Players: " + PhotonNetwork.PlayerList.Length);
        DefaultObserverEventHandler.isTracking = false;



    }

    private void Update()
    {
        if (imageTarget != null)
        {

            switch (gameType)
            {
                case "ballonGame":
                    if (shootScript != null)
                    {
                        score = shootScript.GetScore();
                        scoreText.text = score.ToString();
                        SetPlayersScores();
                        CheckGameStatus();
                    } else {
                        Debug.LogWarning("shootScript is null");
                    }
                    break;
                default:
                    break;
            }

            if (restartTrack)
            {
                for (int i = 0; i < imageTarget.transform.childCount; i++)
                {
                    imageTarget.transform.GetChild(i).gameObject.SetActive(DefaultObserverEventHandler.isTracking);
                    // if (!gameStarted)
                    //     imageTarget.transform.GetChild(i).gameObject.transform.LookAt(Camera.main.transform);
                }
                restartTrack = false;
                StartCoroutine(WaitForTrack());
            }
            if (gameEnded)
            {
                return;
            }


            if (DefaultObserverEventHandler.isTracking)
            {
                if (!gameStarted)
                {
                    if (!(bool)PhotonNetwork.LocalPlayer.CustomProperties["isReady"])
                    {
                        customProperties["isReady"] = true;
                        customProperties["score"] = score;
                        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
                    }

                    if (ArePlayersReady())
                    {
                        StartBalloonGame();
                    }
                    else
                    {
                        if (!wait)
                        {
                            Debug.LogWarning("Not all players are ready");
                            StartCoroutine(WaitForPlayers());
                        }
                    }
                }
            }


        }
        else
        {
            // set imageTarget to from SideLoadImageTarget script

            Debug.LogWarning("Image Target yet to be set");
            imageTarget = GameObject.Find("DynamicImageTarget");
            if (imageTarget != null)
            {
                Debug.LogWarning("Image Target found");
            }
            else
            {
                Debug.LogWarning("Image Target not found");
            }
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
        InitializePlayerReality();
        hasBeenInitialized = true;

    }

    void InitializePlayerReality()
    {
        // Create 3 random spawn points
        debugText.text += "SpawnPlayer1\n";
        spawnPoints = new Transform[3];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            string spawnPointName = "SpawnPoint" + PhotonNetwork.LocalPlayer.ActorNumber + "-" + i;
            GameObject newObj = new GameObject(spawnPointName);
            // Randomly set the position of the spawn point without the value of 0
            // Random number between -1 and 1 without a value of 0
            float val = Random.Range(0, 2);
            float x = val < 1 ? -i - 2 : i + 2;
            val = Random.Range(0, 2);
            float y = val < 1 ? -i - 2 : i + 2;
            val = Random.Range(0, 2);
            float z = val < 1 ? -i - 2 : i + 2;

            newObj.transform.position = new Vector3(x, y, z);
            pickedSpawnIndex.Add(i); // add the random spawn point to the list
            spawnPoints[i] = GameObject.Find(spawnPointName).transform;
            Debug.LogWarning(spawnPointName + ": " + spawnPoints[i].position);
            debugText.text += spawnPointName + ": " + spawnPoints[i].position + "\n";
        }
        // Spawn the player
        debugText.text += "SpawnPlayer2\n";

    }

    private void StartBalloonGame()
    {
        Debug.LogWarning("Game Started. Enabling SpawnScript");
        // Enable SpawnManager
        spawnScript.enabled = true;
        spawnScript.setSpawnPoints(spawnPoints);
        objectiveText.text = "Shoot the balloons and earn the most points!\nThe first to get to 100 points wins!";
        shootScript = GameObject.Find("ShootManager").GetComponent<ShootScript>();

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

        gameType = "ballonGame";


        gameStarted = true;
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
        wait = true;

        objectiveText.text += "\nWaiting for other players to be ready";

        yield return new WaitForSeconds(5);

        wait = false;
    }

    private void SetPlayersScores()
    {
        customProperties["score"] = score;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        PlayersScores.text = PLAYERS_SCORES;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            PlayersScores.text += player.NickName + "'s Score: " + (int)player.CustomProperties["score"] + "\n";
        }
    }

    private void CheckGameStatus()
    {
        if (score >= 100)
        {
            Debug.LogWarning("A winner has been decided!");
            photonView.RPC("SetGameEnded", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        }
    }

    [PunRPC]
    private void SetGameEnded(string winnerName)
    {
        gameEnded = true;
        objectiveText.text = winnerName + " has won the game!";
        playerUI.SetActive(false);
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


}
