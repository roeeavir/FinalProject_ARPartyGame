using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using Vuforia;
using UnityEngine.UI;
public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Status")]
    public bool gameEnded = false;
    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerController[] players;
    private int playersInGame;
    private List<int> pickedSpawnIndex;
    [Header("Reference")]
    public GameObject imageTarget;
    //instance
    public static GameManager instance;

    public Text listOfPlayers, debugText, health;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        pickedSpawnIndex = new List<int>();
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
        debugText.text += "Number of Players: " + PhotonNetwork.PlayerList.Length + "\n";
        Debug.Log("Number of Players: " + PhotonNetwork.PlayerList.Length);
        DefaultObserverEventHandler.isTracking = false;
    }
    private void Update()
    {
        // Debug.LogWarning("is tracking " + DefaultObserverEventHandler.isTracking);

        foreach (GameObject gameObj in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (gameObj.name == "Player(Clone)" && imageTarget != null)
            {
                gameObj.transform.SetParent(imageTarget.transform);
            }
        }
        for (int i = 1; i < imageTarget.transform.childCount; i++)
        {
            imageTarget.transform.GetChild(i).gameObject.SetActive(DefaultObserverEventHandler.isTracking);
            if (!health.enabled)
            {
                health.enabled = DefaultObserverEventHandler.isTracking;
            }
        }
    }
    [PunRPC]
    void ImInGame()
    {
        debugText.text += "ImInGame\n";
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();

            foreach (PlayerController player in players)
            {
                if (player != null)
                {
                    listOfPlayers.text += player.photonView.Owner.NickName + "\n"; // add player to the list
                }
                else
                {
                    listOfPlayers.text += "null\n";
                }
            }
        }
    }
    void SpawnPlayer()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        int rand = Random.Range(0, spawnPoints.Length); // random spawn point
        while (pickedSpawnIndex.Contains(rand)) // check if the random spawn point is already picked
        {
            rand = Random.Range(0, spawnPoints.Length); // random spawn point
        }
        pickedSpawnIndex.Add(rand); // add the random spawn point to the list
        debugText.text += spawnPoints[rand].position + "\n";
        debugText.text += Quaternion.identity + "\n";
        GameObject playerObject = null;
        playerObject = (GameObject)PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[rand].position, Quaternion.identity); // spawn player
        //intialize the player
        PlayerController playerScript = playerObject.GetComponent<PlayerController>();
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        players[PhotonNetwork.LocalPlayer.ActorNumber - 1] = playerScript;

        debugText.text += "SpawnPlayer\n";
    }
    public PlayerController GetPlayer(int playerID)
    {
        Debug.LogWarning("GetPlayer with id: " + playerID);
        return players.First(x => x.id == playerID);
    }
    public PlayerController GetPlayer(GameObject playerObj)
    {
        Debug.LogWarning("GetPlayer with object: " + playerObj.GetComponent<PlayerController>().id);
        return players.First(x => x.gameObject == playerObj);
    }
}