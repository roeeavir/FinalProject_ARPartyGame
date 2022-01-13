using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using NativeGalleryNamespace;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [Header(" — -Menus — -")]
    public GameObject mainMenu;
    public GameObject lobbyMenu;
    [Header(" — -Main Menu — -")]
    public Button createRoomBtn;
    public Button joinRoomBtn;
    [Header(" — -Lobby Menu — -")]
    public Text roomName;
    public Text playerList;
    public Button startGameBtn;

    public Button setAnchorBtn;

    public Text debugText;

    public GameObject textureDelivery;

    public GameObject anchorCanvas;

    WebCamTexture webCamTexture;

    public GameObject photoCanvas;

    public Camera mainCamera;

    public GameObject photoBtn;

    public RawImage photo;

    private ExitGames.Client.Photon.Hashtable customeProperties = new ExitGames.Client.Photon.Hashtable();

    private Texture2D texture;

    GameObject quad = null;

    private bool firstConnect = true;

    private void Start()
    {
        createRoomBtn.interactable = false; // disable create room button
        joinRoomBtn.interactable = false; // disable join room button
        Debug.LogWarning("MenuManager Start Address: " + PhotonNetwork.NetworkingClient.GameServerAddress);
        debugText.text = "MenuManager Start Address: " + PhotonNetwork.NetworkingClient.GameServerAddress;

        customeProperties["isReady"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customeProperties);
    }
    public override void OnConnectedToMaster()
    {
        createRoomBtn.interactable = true; // enable create room button
        joinRoomBtn.interactable = true; // enable join room button
    }
    void SetMenu(GameObject menu)
    {
        mainMenu.SetActive(false); // disable main menu
        lobbyMenu.SetActive(false); // disable lobby menu
        menu.SetActive(true); // enable selected menu
    }
    public void OnCreateRoomBtn(Text roomNameInput)
    {
        Debug.LogWarning("Creating Room" + roomNameInput.text);
        NetworkManager.instance.CreateRoom(roomNameInput.text); // create room
        roomName.text = "Room Name: " + roomNameInput.text;
    }
    public void OnJoinRoomBtn(Text roomNameInput)
    {
        Debug.LogWarning("Joining room: " + roomNameInput.text);
        NetworkManager.instance.JoinRoom(roomNameInput.text);
        roomName.text = roomNameInput.text;
    }
    public void OnPlayerNameUpdate(Text playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }
    public override void OnJoinedRoom()
    {
        SetMenu(lobbyMenu);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }
    // Prints the player list
    [PunRPC]
    public void UpdateLobbyUI()
    {
        playerList.text = "";
        playerList.text += "PlayerList size: " + PhotonNetwork.PlayerList.Length + "\n";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.LogWarning("UpdateLobbyUI() Iterating Players, Player name: " + player.NickName);
            if (player.IsMasterClient)
            {
                playerList.text += player.NickName + " (Host)";
            }
            else
            {
                playerList.text += player.NickName;
            }
            if (player.CustomProperties["isReady"] != null)
            {
                if ((bool)player.CustomProperties["isReady"])
                {
                    playerList.text += " R\n"; // Ready
                }
                else
                {
                    playerList.text += " NR\n"; // Not Ready
                }
            }
        }
        if (firstConnect) {
            if (PhotonNetwork.IsMasterClient)
            {
                setAnchorBtn.interactable = true; // enable set anchor button
            }
            else
            {
                setAnchorBtn.interactable = false; // disable set anchor button
            }
            startGameBtn.interactable = false; // disable start game button
            firstConnect = false;
        }
    }
    public void OnLeaveLobbyBtn()
    {
        PhotonNetwork.LeaveRoom();
        if (quad != null)
        {
            Destroy(quad);
        }
        SetMenu(mainMenu);
    }
    public void OnStartGameBtn()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }

    public void onOpenAnchorCanvas()
    {
        Debug.LogWarning("onOpenAnchorCanvas()");
        anchorCanvas.SetActive(true);
    }

    public void onCloseAnchorCanvas()
    {
        Debug.LogWarning("onCloseAnchorCanvas()");
        anchorCanvas.SetActive(false);
    }

    // Sets the anchor image texture from the gallery
    public void OnSetAnchorPhotoFromGallery()
    {
        ResetPlayersReady();
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
       {
           Debug.Log("Image path: " + path);
           if (path != null)
           {
               // Create Texture from selected image
               Texture2D texture = NativeGallery.LoadImageAtPath(path, 512, false);
               if (texture == null)
               {
                   Debug.Log("Couldn't load texture from " + path);
                   return;
               }

               texture = TexturesFunctions.ResizeTexture(texture, TexturesFunctions.getWidth(), TexturesFunctions.getHeight());
               texture.Apply();

               startGameBtn.interactable = false; // disable start game button

               photonView.RPC("SetAnchorPhoto", RpcTarget.AllBuffered, texture.EncodeToPNG());


           }
       });

        Debug.Log("Permission result: " + permission);

        if (permission == NativeGallery.Permission.Denied)
        {
            Debug.Log("Permission denied");
        }
        else if (permission == NativeGallery.Permission.ShouldAsk)
        {
            Debug.Log("Permission denied");
        }
        else if (permission == NativeGallery.Permission.Granted)
        {

            Debug.Log("Permission granted");
        }

        photoCanvas.SetActive(false);

        anchorCanvas.SetActive(false);

    }

    public void OnSetAnchorPhotoFromCamera()
    {
        Debug.LogWarning("OnSetAnchorPhotoFromCamera()");
        photoCanvas.SetActive(true);
        photoBtn.SetActive(true);
        // photo.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        mainCamera.GetComponent<PhoneCamera>().enabled = true;
    }


    public void OnPhotoBtn()
    {
        Debug.LogWarning("OnPhotoBtn()");
        StartCoroutine(TakeSnapshot());
    }

    //     private IEnumerator TakeScreenshot()
    //     {
    //         // photo.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;

    //         // photo.GetComponent<AspectRatioFitter>().aspectRatio = 0.55f;

    //         // set photoCanvas to fit whole screen
    //         // photoCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.height, Screen.width);
    //         // photo.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.None;
    //         // photo.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.height, photo.GetComponent<RectTransform>().sizeDelta.x);
    //         // set left of photo to 0
    //         // photoCanvas.transform.localScale = new Vector3(1f, 1f, 1f);

    //         // RectTransformFunctions.SetLeft(photo.GetComponent<RectTransform>(), 0f);
    //         // RectTransformFunctions.SetRight(photo.GetComponent<RectTransform>(), 0f);
    //         // RectTransformFunctions.SetBottom(photo.GetComponent<RectTransform>(), 0f);

    //         // fit photo to screen
    //         // photo.GetComponent<AspectRatioFitter>().aspectRatio = 0.55f;


    //         yield return new WaitForEndOfFrame();
    // // wait 2 seconds for the texture to be updated
    //         yield return new WaitForSeconds(2);

    //         Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    //         screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
    //         screenTexture = ResizeTexture(screenTexture, 512, 512);
    //         screenTexture.Apply();

    //         NetworkManager.instance.photonView.RPC("SetAnchorPhoto", RpcTarget.AllBuffered, screenTexture.EncodeToPNG());

    //         startGameBtn.interactable = true;

    //         photoBtn.SetActive(false);

    //         anchorCanvas.SetActive(false);

    //         mainCamera.GetComponent<PhoneCamera>().enabled = false;
    //     }


    // Sets the anchor image texture from the camera
    private IEnumerator TakeSnapshot()
    {
        ResetPlayersReady();

        yield return new WaitForEndOfFrame(); // wait for screen to be rendered

        WebCamTexture webCamTexture = mainCamera.GetComponent<PhoneCamera>().GetWebCamTexture(); // get the WebCamTexture

        Texture2D snap = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false); // create a new texture


        snap.SetPixels(webCamTexture.GetPixels()); // copy the pixels from the WebCamTexture to the new texture
        snap = TexturesFunctions.ResizeTexture(snap, TexturesFunctions.getWidth(), TexturesFunctions.getHeight()); // resize the texture
        snap = TexturesFunctions.RotateTexture(snap, 90); // rotate the texture 90 degrees
        snap.Apply(); // apply the changes to the texture

        startGameBtn.interactable = false; // disable start game button

        photonView.RPC("SetAnchorPhoto", RpcTarget.AllBuffered, snap.EncodeToPNG()); // send the texture to every player


        photoBtn.SetActive(false);

        photoCanvas.SetActive(false);

        anchorCanvas.SetActive(false);

        mainCamera.GetComponent<PhoneCamera>().enabled = false;

        Debug.Log("Waiting for players to be ready");
        yield return new WaitForSeconds(2);
        while (!ArePlayersReady())
        {
            Debug.Log("Not all players are ready, waiting 2 more seconds");
            yield return new WaitForSeconds(2);
        }
        startGameBtn.interactable = true;
    }

    // Check if all players are ready
    // Are all players ready?
    public bool ArePlayersReady()
    {
        photonView.RPC("UpdateLobbyUI", RpcTarget.All); // update the lobby UI
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!(bool)player.CustomProperties["isReady"]) // if any player is not ready
            {
                return false;
            }
        }
        return true;
    }

    // Set all players customProperty "isReady" to false
    public void ResetPlayersReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            customeProperties["isReady"] = false; // set player as not ready
            player.SetCustomProperties(customeProperties);
        }
    }



    [PunRPC]
    public void SetAnchorPhoto(byte[] receivedByte)
    {

        if (quad != null) // if quad exists
        {
            Destroy(quad); // destroy quad
        }
        texture = new Texture2D(1, 1); // create a new texture
        texture.LoadImage(receivedByte); // load the texture from the byte array

        // Assign texture to a temporary quad and destroy it after 5 seconds
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad); // create a quad object
        quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
        quad.transform.forward = Camera.main.transform.forward;
        quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

        Material material = quad.GetComponent<Renderer>().material;
        if (!material.shader.isSupported) // happens when Standard shader is not included in the build
            material.shader = Shader.Find("Legacy Shaders/Diffuse");

        material.mainTexture = texture;

        TexturesFunctions.setTexture(texture);// set texture to TexturesFunctions

        Debug.Log("Texture set for player " + PhotonNetwork.LocalPlayer.ActorNumber);

        customeProperties["isReady"] = true; // set player as ready
        PhotonNetwork.LocalPlayer.SetCustomProperties(customeProperties);// update player properties

        Debug.Log("Player " + PhotonNetwork.LocalPlayer.ActorNumber + " is ready");
    }

}