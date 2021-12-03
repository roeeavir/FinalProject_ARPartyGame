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

    public Text listOfPlayers, debugText;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        pickedSpawnIndex = new List<int>();
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
        debugText.text += "Started\nPlayers: " + PhotonNetwork.PlayerList.Length + "\n";
        DefaultObserverEventHandler.isTracking = false;

        foreach (PlayerController player in players)
        {
            if (player != null)
            {
                listOfPlayers.text += player.photonView.Owner.NickName + "\n"; // add player to the list
            }
        }
    }
    private void Update()
    {
        Debug.Log("is tracking " + DefaultObserverEventHandler.isTracking);

        foreach (GameObject gameObj in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (gameObj.name == "Player(Clone)")
            {
                gameObj.transform.SetParent(imageTarget.transform);
            }
        }
        for (int i = 1; i < imageTarget.transform.childCount; i++)
        {
            imageTarget.transform.GetChild(i).gameObject.SetActive(DefaultObserverEventHandler.isTracking);
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
        }
    }
    void SpawnPlayer()
    {

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
        players[PhotonNetwork.LocalPlayer.ActorNumber - 1] = playerScript;
        debugText.text += "SpawnPlayer1\n";
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        debugText.text += "SpawnPlayer2\n";
    }
    public PlayerController GetPlayer(int playerID)
    {
        return players.First(x => x.id == playerID);
    }
    public PlayerController GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }
}