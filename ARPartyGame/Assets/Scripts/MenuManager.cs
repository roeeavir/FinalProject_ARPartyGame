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
        webCamTexture = new WebCamTexture();
        GetComponent<Renderer>().material.mainTexture = webCamTexture; //Add Mesh Renderer to the GameObject to which this script is attached to
        webCamTexture.Play();
        StartCoroutine(TakePhoto());
    }

    IEnumerator TakePhoto()  // Start this Coroutine on some button click
    {

        // NOTE - you almost certainly have to do this here:

        yield return new WaitForEndOfFrame();

        // it's a rare case where the Unity doco is pretty clear,
        // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
        // be sure to scroll down to the SECOND long example on that doco page 

        Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();

        NetworkManager.instance.photonView.RPC("SetAnchorPhoto", RpcTarget.AllBuffered, photo.EncodeToPNG());

        startGameBtn.interactable = true;

        // //Encode to a PNG
        // byte[] bytes = photo.EncodeToPNG();
        // //Write out the PNG. Of course you have to substitute your_path for something sensible
        // File.WriteAllBytes(your_path + "photo.png", bytes);
    }

}