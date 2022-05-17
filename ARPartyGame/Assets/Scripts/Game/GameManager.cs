using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Demo game is used to demonstrate our initial setup and how to use Photon Network and Vuforia in a game
// This class is used to manage the demo game.
public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Status")]
    public bool gameEnded = false;
    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerController[] players;
    private List<int> pickedSpawnIndex;
    // [Header("Reference")]
    private GameObject imageTarget;

    //instance
    public static GameManager instance;

    private bool hasBeenSpawned = false;


    public Text health;
    private void Awake()
    {
        if (instance == null) // Singleton
        {
            instance = this;
        }
    }
    private void Start()
    {
        // Print every player buffered in Photon
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            Debug.LogWarning("Buffered Player: " + p.NickName + "\n");
        }
        pickedSpawnIndex = new List<int>();
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered); // Tell everyone that we are in the game
        Debug.LogWarning("Number of Players: " + PhotonNetwork.PlayerList.Length);
        DefaultObserverEventHandler.isTracking = false;

    }
    private void Update()
    {
        if (imageTarget != null)
        {
            foreach (GameObject gameObj in GameObject.FindObjectsOfType(typeof(GameObject)))
            {
                if (gameObj.name == "Player(Clone)" && imageTarget != null)
                {
                    Debug.LogWarning("Player(Clone) found");
                    gameObj.transform.SetParent(imageTarget.transform);
                }
            }
            for (int i = 1; i < imageTarget.transform.childCount; i++)
            {
                imageTarget.transform.GetChild(i).gameObject.SetActive(DefaultObserverEventHandler.isTracking); // Set all children to tracking state
                if (!health.enabled)
                {
                    health.enabled = DefaultObserverEventHandler.isTracking; // Set health text to tracking state
                }
            }
        }
        else
        {
            // set imageTarget to from SideLoadImageTarget script
            imageTarget = GameObject.Find("DemoDynamicImageTarget");
        }

    }
    // This method is called when a player is added to the game
    [PunRPC]
    void ImInGame()
    {
        // Check if the player has been spawned already
        if (hasBeenSpawned)
        {
            return;
        }

        SpawnPlayer();
        hasBeenSpawned = true;

    }
    // This method spawns a player
    void SpawnPlayer()
    {
        int rand = Random.Range(0, spawnPoints.Length); // Random spawn point
        while (pickedSpawnIndex.Contains(rand)) // Check if the random spawn point is already picked
        {
            rand = Random.Range(0, spawnPoints.Length); // Random spawn point
        }
        pickedSpawnIndex.Add(rand); // Add the random spawn point to the list
        GameObject playerObject = null;
        playerObject = (GameObject)PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[rand].position, Quaternion.identity); // spawn player
        // Intialize the player
        PlayerController playerScript = playerObject.GetComponent<PlayerController>();
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        Debug.LogWarning(playerScript);
        players[PhotonNetwork.LocalPlayer.ActorNumber - 1] = playerScript;
        if (players[PhotonNetwork.LocalPlayer.ActorNumber - 1] == null)
        {
            Debug.LogWarning("Player is null");
        }
        else
        {
            Debug.LogWarning("Player is not null");
        }

    }
    public PlayerController GetPlayer(int playerID)
    {
        Debug.LogWarning("GetPlayer with id: " + playerID);
        return players.First(x => x.id == playerID); // return the player with the given id
    }
    public PlayerController GetPlayer(GameObject playerObj)
    {
        Debug.LogWarning("GetPlayer with object: " + playerObj.GetComponent<PlayerController>().id);
        return players.First(x => x.gameObject == playerObj); // return the player with the given id
    }

    // Called when a player has clicked on the "Return to Main Menu" button
    public void OnReturnToMainMenu(){
        Destroy(NetworkManager.instance.gameObject); // Destroy network manager
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Main", LoadSceneMode.Single); // Load main menu scene
        Destroy(gameObject); // Destroy game manager
    }

}