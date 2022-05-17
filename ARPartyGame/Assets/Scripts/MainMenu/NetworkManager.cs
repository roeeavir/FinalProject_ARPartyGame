using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

    public Text connectionText, badInputText;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // make sure this object is not destroyed when loading a new scene
        }
    }
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Connect to the server
    }
    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName); // Create a room
    }
    public void JoinRoom(string roomName)
    {
        Debug.LogWarning("Joining room: " + roomName);
        if (PhotonNetwork.PlayerList.Length <= 4) // Prevents room from being joined if it is full
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.LogWarning("Room is full");
        }
    }
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName); // Loads the scene
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (this.CanRecoverFromDisconnect(cause)) // If the connection is lost, try to reconnect
        {
            connectionText.enabled = true;
            Debug.LogWarning("Recovering from disconnect...");
            connectionText.text = "Recovering from disconnect...";
            this.Recover();
            Debug.LogWarning("Reconnecting to the server");
            PhotonNetwork.ConnectUsingSettings();
        }
        else // 
        {
            Debug.LogWarning("Disconnected from the server");
            connectionText.text = "Disconnected from the server";
            connectionText.enabled = true;
            StartCoroutine(DisableConnectionText());
        }
    }

    // Check if can recover from disconnect
    private bool CanRecoverFromDisconnect(DisconnectCause cause)
    {
        switch (cause)
        {
            // the list here may be non exhaustive and is subject to review
            case DisconnectCause.Exception:
            case DisconnectCause.ServerTimeout:
            case DisconnectCause.ClientTimeout:
            case DisconnectCause.DisconnectByServerLogic:
            case DisconnectCause.DisconnectByServerReasonUnknown:
                return true;
        }
        return false;
    }

    // Try to recover from disconnect
    private void Recover()
    {
        if (!PhotonNetwork.ReconnectAndRejoin())
        {
            Debug.LogError("ReconnectAndRejoin failed, trying Reconnect");
            if (!PhotonNetwork.Reconnect())
            {
                Debug.LogError("Reconnect failed, trying ConnectUsingSettings");
                if (!PhotonNetwork.ConnectUsingSettings())
                {
                    Debug.LogError("ConnectUsingSettings failed");
                    connectionText.text = "Connection failed";
                }
            }
            else
            {
                Debug.Log("Reconnected");
                connectionText.text = "Reconnected";
            }
        }
        else
        {
            Debug.Log("Reconnected");
            connectionText.text = "Reconnected";
        }
        StartCoroutine(DisableConnectionText());
    }

    // Disable the connection text after a few seconds
    private IEnumerator DisableConnectionText()
    {
        yield return new WaitForSeconds(3);
        connectionText.text = "";
        connectionText.enabled = false;
    }

    // When a player has finished joining a room, this is called
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        connectionText.enabled = true;
        connectionText.text = "Joined room";
        StartCoroutine(DisableConnectionText());
    }

    // When a player fails to join the room
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Join room failed");
        StartCoroutine(ShowErrorMessage("Room doesn't exist"));
        base.OnJoinRoomFailed(returnCode, message);
    }

    // Show error message
    public IEnumerator ShowErrorMessage(string message)
    {
        badInputText.text = message;
        yield return new WaitForSeconds(3);
        badInputText.text = "";
    }


}