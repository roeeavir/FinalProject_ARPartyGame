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
    public GameObject tutorialMenu;

    public GameObject scoreBoardMenu;
    [Header(" — -Main Menu — -")]
    public Button createRoomBtn;
    public Button joinRoomBtn;
    [Header(" — -Lobby Menu — -")]
    public Text roomName, playerList;
    public Button startGameBtn, startARGameBtn;
    public Button setAnchorBtn;

    public GameObject textureDelivery;

    public GameObject anchorCanvas;

    public GameObject gameMode;

    private WebCamTexture webCamTexture;

    public GameObject photoCanvas;

    public GameObject mainCamera;

    public GameObject testARCamera;

    public GameObject photoBtn, takePhotoBtn, fromGalleryBtn;

    public RawImage photo;

    public Text lobbyObjectiveText;

    private Timer timer;

    private string nextScene = "";

    private ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    private Texture2D texture;

    private GameObject quad = null;

    private bool firstConnect = true;

    GameObject testBoard = null;

    GameObject testTarget = null;

    int testCounter = 0;

    [Header(" — -Tutorial Menu — -")]
    public Button tutorialBtn;

    public Button exitTutorialBtn;

    [Header(" — -Score Board Menu — -")]

    public Text scoreBoardText;




    private void Start()
    {
        createRoomBtn.interactable = false; // Disable create room button
        joinRoomBtn.interactable = false; // Disable join room button
        timer = GetComponent<Timer>();
        timer.SetTime(3);
        Debug.LogWarning("MenuManager Start Address: " + PhotonNetwork.NetworkingClient.GameServerAddress);

        customProperties["isReady"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

        if (ARGameManager.instance != null) // If the ARGameManager is not null, then the ARGameManager is being used
        {
            Destroy(ARGameManager.instance.gameObject);
        }
        if (GameManager.instance != null) // If the GameManager is not null, then the GameManager is being used
        {
            Destroy(GameManager.instance.gameObject);
        }

        try
        {
            PhotonNetwork.LeaveRoom(); // Leave the room if the player is already in one
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("MenuManager Start Exception: " + e.Message);
        }

    }
    public override void OnConnectedToMaster() // When the first client (the room creator) connects to the server
    {
        createRoomBtn.interactable = true; // Enable create room button
        joinRoomBtn.interactable = true; // Enable join room button
    }

    void SetMenu(GameObject menu)
    {
        mainMenu.SetActive(false); // Disable main menu
        lobbyMenu.SetActive(false); // Disable lobby menu
        menu.SetActive(true); // Enable selected menu
    }

    // Called when the player clicks the create room button
    public void OnCreateRoomBtn(Text roomNameInput)
    {
        Debug.LogWarning("Creating Room" + roomNameInput.text);
        // Check name
        if (!CheckValidPlayerName() || !CheckValidRoomName(roomNameInput.text)) // If the player name or room name is invalid
        {
            Debug.LogWarning("Invalid name");
            return;
        }

        NetworkManager.instance.CreateRoom(roomNameInput.text); // Create room
        roomName.text = "Room Name: " + roomNameInput.text;
    }

    // Called when the player clicks the join room button
    public void OnJoinRoomBtn(Text roomNameInput)
    {
        Debug.LogWarning("Joining room: " + roomNameInput.text);
        if (!CheckValidPlayerName() || !CheckValidRoomName(roomNameInput.text)) // If the player name or room name is invalid
        {
            Debug.LogWarning("Invalid name");
            return;
        }

        NetworkManager.instance.JoinRoom(roomNameInput.text);
        roomName.text = "Room Name: " + roomNameInput.text;
    }

    // Called when the text in the player name input field changes
    public void OnPlayerNameUpdate(Text playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }
    public override void OnJoinedRoom()
    {
        SetMenu(lobbyMenu); // Set the lobby menu as the active menu
        ResetPlayersReady(); // Reset the players ready status
        startGameBtn.interactable = false; // Disable start game button
        startARGameBtn.interactable = false; // Disable start AR game button
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }
    // Overrided method from Photon.Pun.MonoBehaviourPunCallbacks. Called when the player leaves the room
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
        foreach (Player player in PhotonNetwork.PlayerList) // For each player in the player list
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
                    playerList.text += " Ready!\n"; // Ready
                }
                else
                {
                    playerList.text += " X\n"; // Not Ready
                }
            }
        }
        if (firstConnect) // If this is the first time the player has connected to the room
        {
            if (PhotonNetwork.IsMasterClient)
            {
                setAnchorBtn.interactable = true; // Enable set anchor button
                gameMode.SetActive(true);
                lobbyObjectiveText.text = "Set the anchor image";
            }
            else
            {
                setAnchorBtn.interactable = false; // Disable set anchor button
                gameMode.SetActive(false);
                lobbyObjectiveText.text = "Waiting for host to set anchor image and start the game...";
            }
            startGameBtn.interactable = false; // Disable start game button
            startARGameBtn.interactable = false; // Disable start AR game button
            firstConnect = false;
        }
    }
    // Called when the player clicks the leave room button
    public void OnLeaveLobbyBtn()
    {
        PhotonNetwork.LeaveRoom();
        if (quad != null)
        {
            Destroy(quad); // Destroy the quad game object
        }
        SetMenu(mainMenu); // Set the main menu as the active menu
        // Reset all variables
        firstConnect = true;
        customProperties["isReady"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties); // Reset the player ready status
        roomName.text = "Room Name: ";
        playerList.text = "PlayerList size: ";
        startGameBtn.interactable = false;
        startARGameBtn.interactable = false;
        setAnchorBtn.interactable = false;
        gameMode.SetActive(false);
    }
    // Called when the player clicks the start game button
    public void OnStartGameBtn(string sceneName)
    {
        nextScene = sceneName;
        NetworkManager.instance.photonView.RPC("StartGameCountDown", RpcTarget.All); // Start the game countdown for all the players
    }

    // Starts the game countdown for all the players and calls the start game function for the master client
    [PunRPC]
    private void StartGameCountDown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timer.StartTimer(StartGame);
        }
        else
        {
            timer.StartTimer(null);
        }
    }

    // Called when the game countdown is finished. Starts the game
    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Starting " + nextScene);
            GetComponent<SideLoadImageTarget>().enabled = false;
            NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, nextScene); // Change the scene for all the players
        }
        else
        {
            Debug.LogWarning("Not master client");
        }
    }

    // Called when the player clicks the "Set Anchor" button
    public void OnOpenAnchorCanvas()
    {
        Debug.LogWarning("onOpenAnchorCanvas()");
        anchorCanvas.SetActive(true);
        takePhotoBtn.SetActive(true);
        fromGalleryBtn.SetActive(true);
    }

    // Called when the player finishes setting the anchor, or stops it midway
    public void OnCloseAnchorCanvas()
    {
        Debug.LogWarning("onCloseAnchorCanvas()");
        mainCamera.GetComponent<PhoneCamera>().enabled = false; // Disable the camera
        photoBtn.SetActive(false);
        DisableAnchorCanvasView(); // Disable the anchor canvas view
    }

    // Called when the player clicks the "From Gallery" button
    public void OnSetAnchorPhotoFromGallery()
    {
        ResetPlayersReady(); // Reset the players ready status

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
       {
           texture = GetComponent<AnchorManager>().SetAnchorPhotoFromGallery(path);

           checkImageTargetFunctionality(); // Check if the image target functionality is valid
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

    // Called when the player clicks the "Take Photo" button
    public void OnPhotoBtn()
    {
        Debug.LogWarning("OnPhotoBtn()");
        StartCoroutine(OnPhotoBtnClicked());
    }

    // Takes a screenshot of part of the screen and saves it to a variable
    private IEnumerator OnPhotoBtnClicked()
    {
        ResetPlayersReady();

        yield return new WaitForEndOfFrame(); // Wait for screen to be rendered

        WebCamTexture webCamTexture = mainCamera.GetComponent<PhoneCamera>().GetWebCamTexture(); // Get the WebCamTexture

        texture = GetComponent<AnchorManager>().TakeSnapshot(webCamTexture); // Take a screenshot of the screen
        mainCamera.GetComponent<PhoneCamera>().enabled = false; // Disable the camera

        photoBtn.SetActive(false);

        yield return new WaitForSeconds(1f);

        checkImageTargetFunctionality(); // Check if the image target functionality is valid
    }

    // Called when the player clicks the "Take Photo" button in the set anchor menu
    public void OnSetAnchorPhotoFromCamera()
    {
        Debug.LogWarning("OnSetAnchorPhotoFromCamera()");
        photoCanvas.SetActive(true);
        photoBtn.SetActive(true);
        takePhotoBtn.SetActive(false);
        fromGalleryBtn.SetActive(false);
        mainCamera.GetComponent<PhoneCamera>().enabled = true; // Enable the camera
    }
    // Called when the player clicks the "Cancel" button in the set anchor menu
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
        lobbyObjectiveText.text = "Waiting for target image to be sent to all players...";
        while (!ArePlayersReady())
        {
            Debug.Log("Not all players are ready, waiting...");
            yield return new WaitForSeconds(1); // Wait for 1 second
        }
        yield return new WaitForSeconds(1); // Wait for 1 second again, to make sure everyone is ready and synced
        startGameBtn.interactable = true;
        startARGameBtn.interactable = true;
        lobbyObjectiveText.text = "All players are ready, start the game!";
    }

    // Check if all players are ready
    public bool ArePlayersReady()
    {
        photonView.RPC("UpdateLobbyUI", RpcTarget.All); // Update the lobby UI for all the players
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!(bool)player.CustomProperties["isReady"]) // If any player is not ready
            {
                return false; // Someone is not ready
            }
        }
        return true; // All players are ready
    }

    // Set all players customProperty "isReady" to false
    public void ResetPlayersReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            customProperties["isReady"] = false; // Set player as not ready
            player.SetCustomProperties(customProperties);
        }
        photonView.RPC("UpdateLobbyUI", RpcTarget.All); // Update the lobby UI
    }

    [PunRPC]
    public void OnSetAnchorPhoto(byte[] receivedByte)
    {

        if (quad != null) // If quad exists
        {
            Destroy(quad); // Destroy quad
        }

        texture = GetComponent<AnchorManager>().SetAnchorPhoto(receivedByte); // Set anchor photo from byte array to a texture

        // Assign texture to a temporary quad and destroy it after 5 seconds
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad); // Create a quad object
        quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
        quad.transform.forward = Camera.main.transform.forward;
        quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

        Material material = quad.GetComponent<Renderer>().material;
        if (!material.shader.isSupported) // Happens when Standard shader is not included in the build
            material.shader = Shader.Find("Legacy Shaders/Diffuse");

        material.mainTexture = texture;

        TexturesFunctions.SetTexture(texture);// Set texture to TexturesFunctions

        Debug.Log("Texture set for player " + PhotonNetwork.LocalPlayer.ActorNumber);

        customProperties["isReady"] = true; // Set player as ready
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);// Update player properties

        Debug.Log("Player " + PhotonNetwork.LocalPlayer.ActorNumber + " is ready");
    }

    // Function to check if the image target functionality is valid by setting the texture to an image target and letting the player check if it is working
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

        testCounter++; // Increment test counter

        GetComponent<SideLoadImageTarget>().SetTexture(texture, "TestImageTarget" + testCounter); // Set texture to SideLoadImageTarget

        if (testTarget != null) // If testTarget exists
        {
            GetComponent<SideLoadImageTarget>().enabled = false;
            Destroy(GameObject.Find("TestImageTarget" + (testCounter - 1))); // Destroy previous test target
            Debug.Log("TestImageTarget destroyed");
            GetComponent<SideLoadImageTarget>().enabled = true;
        }

        mainCamera.SetActive(false);

        testARCamera.SetActive(true);

        lobbyMenu.SetActive(false);

        testMenu.SetActive(true);


        testTarget = GameObject.Find("TestImageTarget" + testCounter); // Find test target

        if (testTarget == null)
        {
            Debug.Log("TestImageTarget" + testCounter + " target is null"); // Test target is null
            return;
        }
        else
        {
            Debug.Log("TestImageTarget" + testCounter + " target is not null");
        }

        GameObject testBoard = GameObject.Find("Test Board");
        if (testBoard != null)
        {
            Debug.Log("Plane Scores Background is not null");
        }
        else
        {
            Debug.Log("Plane Scores Background is null");
        }

        // Place the plane as mTarget's child
        testBoard.transform.parent = testTarget.transform; // Set testBoard as testTarget's child

        testBoard.transform.position = new Vector3(0f, 0f, 0f); // Set testBoard position to the center of the test target
        testBoard.transform.localScale = new Vector3(0.01f, 1f, 0.02f); // Set testBoard scale
        testBoard.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Set testBoard rotation

        Debug.Log("TestBoard created inside TestImageTarget" + testCounter);
    }

    // Called when the player clicks the "Keep Image" button in the test environment
    public void onGoodImageTargetBtnClick()
    {
        testMenu.SetActive(false);
        lobbyMenu.SetActive(true);
        mainCamera.SetActive(true);
        testARCamera.SetActive(false);

        startGameBtn.interactable = false; // Disable start game button
        startARGameBtn.interactable = false; // Disable start AR game button

        DisableAnchorCanvasView();

        photonView.RPC("OnSetAnchorPhoto", RpcTarget.AllBuffered, texture.EncodeToPNG()); // Send anchor photo texture to all players

        if (testBoard != null)
        {
            testBoard.SetActive(false);
        }

        StartCoroutine(WaitForPlayersReady()); // Wait for players to be ready
    }

    // Called when the player clicks the "Reject Image" button in the test environment
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

        DisableAnchorCanvasView(); // Disable anchor canvas view
    }

    // Called when the player changes the the game mode silder bar's value
    public void OnSetGameMode()
    {
        float val = gameMode.GetComponent<Slider>().value; // Get the value of the slider
        photonView.RPC("SetGameMode", RpcTarget.AllBuffered, val); // Send game mode value to all players
    }

    // Set the game mode value
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

    // Called when the player clicks the "Tutorial" button in the main menu
    public void onTutorialClick()
    {
        tutorialMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    // Called when the player clicks the "Back" button in the tutorial menu
    public void onTutorialBackClick()
    {
        tutorialMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    // Called when the player clicks the "Score Board" button in the main menu
    public void onScoreBoardClick()
    {
        FindObjectOfType<LeaderBoardManager>().WritePlayerData(scoreBoardText.GetComponent<Text>()); // Write player data to the score board
        scoreBoardMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    // Called when the player clicks the "Back" button in the score board menu
    public void onScoreBoardBackClick()
    {
        scoreBoardMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    // Check if the room name is valid
    private bool CheckValidRoomName(string roomName)
    {
        if (roomName.Length > 8 || roomName.Length < 1)
        {
            Debug.Log("Room name is too long");
            StartCoroutine(NetworkManager.instance.ShowErrorMessage("Invalid Room Name!\nRoom name must be 1 to 8 characters long!"));
            return false;
        }
        else
        {
            return true;
        }
    }

    // Check if the player name is valid
    private bool CheckValidPlayerName()
    {
        if (PhotonNetwork.NickName.Length > 8 || PhotonNetwork.NickName.Length < 2)
        {
            Debug.Log("Player name is too long");
            StartCoroutine(NetworkManager.instance.ShowErrorMessage("Invalid Player Name!\nPlayer name must be 2 to 8 characters long!"));
            return false;
        }
        else
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                // Check if player name is already taken
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) // Prevents duplicate player names
                {
                    if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.NickName)
                    {
                        Debug.Log("Player name is already taken");
                        StartCoroutine(NetworkManager.instance.ShowErrorMessage("Invalid Player Name!\nPlayer name is already taken!"));
                        return false;
                    }
                }
            }
            return true;
        }
    }

    // Special thanks to:
    // Hagai and Tom pelephone for this QA
    // Extra special thanks to Elironi Pepperoni
    // Mega thanks to Noya D Adosh
}
