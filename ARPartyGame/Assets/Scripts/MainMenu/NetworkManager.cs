using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

    public Text connectionText;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }
    public void JoinRoom(string roomName)
    {
        if (PhotonNetwork.PlayerList.Length <= 4)
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
        // PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel(sceneName);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (this.CanRecoverFromDisconnect(cause))
        {
            connectionText.enabled = true;
            Debug.LogWarning("Recovering from disconnect...");
            connectionText.text = "Recovering from disconnect...";
            this.Recover();
        }
    }

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
            } else
            {
                Debug.Log("Reconnected");
                connectionText.text = "Reconnected";
            }
        } else {
            Debug.Log("Reconnected");
            connectionText.text = "Reconnected";
        }
        StartCoroutine(DisableConnectionText());
    }

    private IEnumerator DisableConnectionText()
    {
        yield return new WaitForSeconds(3);
        connectionText.text = "";
        connectionText.enabled = false;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        connectionText.enabled = true;
        connectionText.text = "Joined room";
        StartCoroutine(DisableConnectionText());
    }


}