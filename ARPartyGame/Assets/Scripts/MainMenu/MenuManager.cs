using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using NativeGalleryNamespace;
using Vuforia;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [Header(" — -Menus — -")]
    public GameObject mainMenu;
    public GameObject lobbyMenu;
    public GameObject testMenu;
    [Header(" — -Main Menu — -")]
    public Button createRoomBtn;
    public Button joinRoomBtn;
    [Header(" — -Lobby Menu — -")]
    public Text roomName;
    public Text playerList;
    public Button startGameBtn;

    public Button startARGameBtn;

    public Button setAnchorBtn;


    public GameObject textureDelivery;

    public GameObject anchorCanvas;

    public GameObject gameMode;

    private WebCamTexture webCamTexture;

    public GameObject photoCanvas;

    public GameObject mainCamera;

    public GameObject testARCamera;

    public GameObject photoBtn;

    public RawImage photo;

    private ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    private Texture2D texture;

    private GameObject quad = null;

    private bool firstConnect = true;

    GameObject testBoard = null;

    GameObject testTarget = null;

    int testCounter = 0;


    private void Start()
    {
        createRoomBtn.interactable = false; // disable create room button
        joinRoomBtn.interactable = false; // disable join room button
        Debug.LogWarning("MenuManager Start Address: " + PhotonNetwork.NetworkingClient.GameServerAddress);

        customProperties["isReady"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

        if (ARGameManager.instance != null)
        {
            Destroy(ARGameManager.instance.gameObject);
        }
        if (GameManager.instance != null)
        {
            Destroy(GameManager.instance.gameObject);
        }


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
        if (firstConnect)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                setAnchorBtn.interactable = true; // enable set anchor button
                gameMode.SetActive(true);
            }
            else
            {
                setAnchorBtn.interactable = false; // disable set anchor button
                gameMode.SetActive(false);
            }
            startGameBtn.interactable = false; // disable start game button
            startARGameBtn.interactable = false; // disable start AR game button
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
    public void OnStartGameBtn(string sceneName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Starting " + sceneName);
            GetComponent<SideLoadImageTarget>().enabled = false;
            // PhotonNetwork.CurrentRoom.IsOpen = false;
            // PhotonNetwork.CurrentRoom.IsVisible = false;
            NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, sceneName);
        }
        else
        {
            Debug.LogWarning("Not master client");
        }
    }

    public void OnOpenAnchorCanvas()
    {
        Debug.LogWarning("onOpenAnchorCanvas()");
        anchorCanvas.SetActive(true);
    }

    public void OnCloseAnchorCanvas()
    {
        Debug.LogWarning("onCloseAnchorCanvas()");
        mainCamera.GetComponent<PhoneCamera>().enabled = false; // disable the camera
        photoBtn.SetActive(false);
        DisableAnchorCanvasView();
    }

    public void OnSetAnchorPhotoFromGallery(){
        ResetPlayersReady();
         
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
       {
           texture = GetComponent<AnchorManager>().SetAnchorPhotoFromGallery(path);

           checkImageTargetFunctionality();
       });

        if (permission == NativeGallery.Permission.Denied || permission == NativeGallery.Permission.ShouldAsk)
        {
            Debug.Log("Permission denied or should ask");
            texture = null;
        }
        else if (permission == NativeGallery.Permission.Granted)
        {
            Debug.Log("Permission granted");
        }
    }


    public void OnPhotoBtn()
    {

        Debug.LogWarning("OnPhotoBtn()");
        StartCoroutine(OnPhotoBtnClicked());
    }

    private IEnumerator OnPhotoBtnClicked(){
        ResetPlayersReady();

        yield return new WaitForEndOfFrame(); // wait for screen to be rendered

        WebCamTexture webCamTexture = mainCamera.GetComponent<PhoneCamera>().GetWebCamTexture(); // get the WebCamTexture

        texture = GetComponent<AnchorManager>().TakeSnapshot(webCamTexture);
        mainCamera.GetComponent<PhoneCamera>().enabled = false; // disable the camera

        photoBtn.SetActive(false);

        yield return new WaitForSeconds(1f);

        // checkImageTargetFunctionality();

        checkImageTargetFunctionality();
    }

    public void OnSetAnchorPhotoFromCamera()
    {
        Debug.LogWarning("OnSetAnchorPhotoFromCamera()");
        photoCanvas.SetActive(true);
        photoBtn.SetActive(true);
        // photo.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        mainCamera.GetComponent<PhoneCamera>().enabled = true;
    }

    private void DisableAnchorCanvasView()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photoCanvas.SetActive(false);

            anchorCanvas.SetActive(false);
        }
    }

    // Waits for all players to be ready
    private IEnumerator WaitForPlayersReady()
    {
        Debug.Log("Waiting for players to be ready");
        // yield return new WaitForSeconds(2);
        while (!ArePlayersReady())
        {
            Debug.Log("Not all players are ready, waiting...");
            yield return new WaitForSeconds(1);
        }
        startGameBtn.interactable = true;
        startARGameBtn.interactable = true;
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
            customProperties["isReady"] = false; // set player as not ready
            player.SetCustomProperties(customProperties);
        }
    }

    [PunRPC]
    public void OnSetAnchorPhoto(byte[] receivedByte)
    {

        if (quad != null) // if quad exists
        {
            Destroy(quad); // destroy quad
        }
        
        texture = GetComponent<AnchorManager>().SetAnchorPhoto(receivedByte); // set anchor photo

        // Assign texture to a temporary quad and destroy it after 5 seconds
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad); // create a quad object
        quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
        quad.transform.forward = Camera.main.transform.forward;
        quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

        Material material = quad.GetComponent<Renderer>().material;
        if (!material.shader.isSupported) // happens when Standard shader is not included in the build
            material.shader = Shader.Find("Legacy Shaders/Diffuse");

        material.mainTexture = texture;

        TexturesFunctions.SetTexture(texture);// set texture to TexturesFunctions

        Debug.Log("Texture set for player " + PhotonNetwork.LocalPlayer.ActorNumber);

        customProperties["isReady"] = true; // set player as ready
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);// update player properties

        Debug.Log("Player " + PhotonNetwork.LocalPlayer.ActorNumber + " is ready");
    }


    private void checkImageTargetFunctionality()
    {
        if (texture == null)
        {
            Debug.Log("texture is null");
            return;
        }
        else
        {
            Debug.Log("texture is not null");
        }
        if (texture.width == 0 || texture.height == 0)
        {
            Debug.Log("texture is empty");
            return;
        }

        testCounter++;

        GetComponent<SideLoadImageTarget>().SetTexture(texture, "TestImageTarget" + testCounter); // set texture to SideLoadImageTarget

        if (testTarget != null)
        {
            GetComponent<SideLoadImageTarget>().enabled = false;
            Destroy(GameObject.Find("TestImageTarget" + (testCounter - 1)));
            Debug.Log("TestImageTarget destroyed");
            GetComponent<SideLoadImageTarget>().enabled = true;
        }

        mainCamera.SetActive(false);

        testARCamera.SetActive(true);

        lobbyMenu.SetActive(false);

        testMenu.SetActive(true);


        testTarget = GameObject.Find("TestImageTarget" + testCounter);

        if (testTarget == null)
        {
            Debug.Log("TestImageTarget" + testCounter + " target is null");
            return;
        }
        else
        {
            Debug.Log("TestImageTarget" + testCounter + " target is not null");
        }

        GameObject testBoard = GameObject.Find("Test Board");
        if (testBoard != null)
        {
            // Debug.Log("Plane Yese set to texture");
            Debug.Log("Plane Scores Background is not null");
            // playersScores.GetComponent<Renderer>().material.mainTexture = TexturesFunctions.GetTexture();
        }
        else
        {
            Debug.Log("Plane Scores Background is null");
        }

        // playersScores.transform.localScale = new Vector3(-1f, 1f, 1f);

        // Place the plane as mTarget's child
        testBoard.transform.parent = testTarget.transform;

        testBoard.transform.position = new Vector3(0f, 0f, 0f);
        testBoard.transform.localScale = new Vector3(0.01f, 1f, 0.02f);
        testBoard.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        // cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        // // Place the cube as testTarget's child
        // cube.transform.parent = testTarget.transform;
        // cube.transform.localPosition = new Vector3(0, 0, 0);

        Debug.Log("TestBoard created inside TestImageTarget" + testCounter);
    }

    public void onGoodImageTargetBtnClick()
    {
        testMenu.SetActive(false);
        lobbyMenu.SetActive(true);
        mainCamera.SetActive(true);
        testARCamera.SetActive(false);

        startGameBtn.interactable = false; // disable start game button
        startARGameBtn.interactable = false; // disable start AR game button

        DisableAnchorCanvasView();

        photonView.RPC("OnSetAnchorPhoto", RpcTarget.AllBuffered, texture.EncodeToPNG());

        if (testBoard != null)
        {
            testBoard.SetActive(false);
        }

        StartCoroutine(WaitForPlayersReady());
    }

    public void onBadImageTargetBtnClick()
    {
        testMenu.SetActive(false);
        lobbyMenu.SetActive(true);
        mainCamera.SetActive(true);
        testARCamera.SetActive(false);

        if (testBoard != null)
        {
            testBoard.SetActive(false);
        }

        DisableAnchorCanvasView();
    }
<<<<<<< HEAD
    
    public void OnSetGameMode()
    {
        float val = gameMode.GetComponent<Slider>().value;
        photonView.RPC("SetGameMode", RpcTarget.AllBuffered, val);
    }

    [PunRPC]
    public void SetGameMode(float val)
    {
        switch (val)
        {
            case 0:
                GameObject.Find("CurrentGameMode").GetComponent<Text>().text = "Game Mode: Casual";
                break;
            case 1:
                GameObject.Find("CurrentGameMode").GetComponent<Text>().text = "Game Mode: Intermediate";
                break;
            case 2:
                GameObject.Find("CurrentGameMode").GetComponent<Text>().text = "Game Mode: Intense";
                break;
            default:
                GameObject.Find("CurrentGameMode").GetComponent<Text>().text = "Game Mode: Casual";
                break;
        }
        GameMode.SetGameMode((int)val);
    }
=======
>>>>>>> parent of 27d50e6 (Updated UI and added tutorial)

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

}
