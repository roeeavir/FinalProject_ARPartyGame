using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;


public class ARGameManager : MonoBehaviourPunCallbacks
{
    [Header("Status")]
    public bool gameEnded = false;
    [Header("Players")]
    public static ARGameManager instance;

    public ARPlayerController[] players;
    private List<int> pickedSpawnIndex;

    public Text debugText;

    private GameObject imageTarget;

    private bool hasBeenInitialized = false;

    private bool gameStarted = false;

    private Transform[] spawnPoints = null;

    public GameObject spawnManager = null;

    public GameObject playerUI = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // else
        // {
        //     Destroy(gameObject);
        // }
    }
    // Start is called before the first frame update
    private void Start()
    {
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
        // Debug.LogWarning("is tracking " + DefaultObserverEventHandler.isTracking);
        if (imageTarget != null)
        {

            // foreach (GameObject gameObj in GameObject.FindObjectsOfType(typeof(GameObject)))
            // {
            //     if (gameObj.name == "Player(Clone)" && imageTarget != null)
            //     {
            //         gameObj.transform.SetParent(imageTarget.transform);
            //     }
            // }
            // for (int i = 1; i < imageTarget.transform.childCount; i++)
            // {
            if (DefaultObserverEventHandler.isTracking && !gameStarted)
            {
                if (!gameStarted)
                {
                    Debug.LogWarning("Game Started. Enabling SpawnScript");
                }
                if (playerUI != null)
                {
                    playerUI.SetActive(true);
                }
                Debug.LogWarning("PlayerUI enabled");
                // Enable SpawnManager
                spawnManager.GetComponent<SpawnScript>().enabled = true;
                spawnManager.GetComponent<SpawnScript>().setSpawnPoints(spawnPoints);

                gameStarted = true;
            }
            // imageTarget.transform.GetChild(i).gameObject.SetActive(DefaultObserverEventHandler.isTracking);
            // if (!health.enabled)
            // {
            //     health.enabled = DefaultObserverEventHandler.isTracking;
            // }
            // }
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
            GameObject newObj = new GameObject("SpawnPoint" + i);
            newObj.transform.position = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
            pickedSpawnIndex.Add(i); // add the random spawn point to the list
            spawnPoints[i] = GameObject.Find("SpawnPoint" + i).transform;
        }
        // Spawn the player
        debugText.text += "SpawnPlayer2\n";

    }
}
