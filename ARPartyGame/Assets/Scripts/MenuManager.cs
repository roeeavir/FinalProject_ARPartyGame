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

    public GameObject photo;

    private void Start()
    {
        createRoomBtn.interactable = false;
        joinRoomBtn.interactable = false;
        Debug.LogWarning("MenuManager Start Address: " + PhotonNetwork.NetworkingClient.GameServerAddress);
        debugText.text = "MenuManager Start Address: " + PhotonNetwork.NetworkingClient.GameServerAddress;
    }
    public override void OnConnectedToMaster()
    {
        createRoomBtn.interactable = true;
        joinRoomBtn.interactable = true;
    }
    void SetMenu(GameObject menu)
    {
        mainMenu.SetActive(false);
        lobbyMenu.SetActive(false);
        menu.SetActive(true);
    }
    public void OnCreateRoomBtn(Text roomNameInput)
    {
        Debug.LogWarning("Creating Room" + roomNameInput.text);
        NetworkManager.instance.CreateRoom(roomNameInput.text);
        roomName.text = roomNameInput.text;
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
                playerList.text += player.NickName + " (Host) \n";
            }
            else
            {
                playerList.text += player.NickName + "\n";
            }
        }
        if (PhotonNetwork.IsMasterClient)
        {
            startGameBtn.interactable = false;
            setAnchorBtn.interactable = true;
        }
        else
        {
            startGameBtn.interactable = false;
            setAnchorBtn.interactable = false;
        }
    }
    public void OnLeaveLobbyBtn()
    {
        PhotonNetwork.LeaveRoom();
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

    public void OnSetAnchorPhotoFromGallery()
    {
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

               texture = ResizeTexture(texture, 512, 512);
               texture.Apply();

               //    // Assign texture to a temporary quad and destroy it after 5 seconds
               //    GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
               //    quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
               //    quad.transform.forward = Camera.main.transform.forward;
               //    quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

               //    Material material = quad.GetComponent<Renderer>().material;
               //    if (!material.shader.isSupported) // happens when Standard shader is not included in the build
               //        material.shader = Shader.Find("Legacy Shaders/Diffuse");

               //    material.mainTexture = texture;

               // get TexturesDelivery Component
               //    TexturesDelivery delivery = textureDelivery.GetComponent<TexturesDelivery>();
               //    if (delivery == null)
               //    {
               //        Debug.Log("TexturesDelivery not found");
               //        return;
               //    } else
               //    {
               //        Debug.Log("TexturesDelivery found");
               //        delivery.photonView.RPC("SetAnchorPhoto", RpcTarget.AllBuffered, texture.EncodeToPNG());
               //    }
               NetworkManager.instance.photonView.RPC("SetAnchorPhoto", RpcTarget.AllBuffered, texture.EncodeToPNG());

               startGameBtn.interactable = true;

               //    Destroy(quad, 5f);

               //    // If a procedural texture is not destroyed manually, 
               //    // it will only be freed after a scene change
               //    Destroy(texture, 5f);
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

        anchorCanvas.SetActive(false);
    }

    public void OnSetAnchorPhotoFromCamera()
    {
        Debug.LogWarning("OnSetAnchorPhotoFromCamera()");
        photoCanvas.SetActive(true);
        photoBtn.SetActive(true);
        photo.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        mainCamera.GetComponent<PhoneCamera>().enabled = true;
    }


    public void OnPhotoBtn()
    {
        Debug.LogWarning("OnPhotoBtn()");
        StartCoroutine(TakeScreenshot());
    }

    private IEnumerator TakeScreenshot()
    {
        photo.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;

        yield return new WaitForEndOfFrame();

        Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTexture = ResizeTexture(screenTexture, 512, 512);
        screenTexture.Apply();

        NetworkManager.instance.photonView.RPC("SetAnchorPhoto", RpcTarget.AllBuffered, screenTexture.EncodeToPNG());

        startGameBtn.interactable = true;

        photoBtn.SetActive(false);

        anchorCanvas.SetActive(false);

        mainCamera.GetComponent<PhoneCamera>().enabled = false;
    }

    Texture2D ResizeTexture(Texture2D texture2D, int targetX, int targetY)
    {
        Texture2D result = new Texture2D(targetX, targetY, texture2D.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetX);
        float incY = (1.0f / (float)targetY);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = texture2D.GetPixelBilinear(incX * ((float)px % targetX), incY * ((float)Mathf.Floor(px / targetX)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }
}