using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

    private Texture2D texture;

    GameObject quad = null;

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
        // PhotonNetwork.AutomaticallySyncScene = true; // Maybe Baby
        PhotonNetwork.LoadLevel(sceneName);
    }

    [PunRPC]
    public void SetAnchorPhoto(byte[] receivedByte)
    {
        if (quad != null)
        {
            Destroy(quad);
        }
        texture = new Texture2D(1, 1);
        texture.LoadImage(receivedByte);

        // Assign texture to a temporary quad and destroy it after 5 seconds
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
        quad.transform.forward = Camera.main.transform.forward;
        quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

        Material material = quad.GetComponent<Renderer>().material;
        if (!material.shader.isSupported) // happens when Standard shader is not included in the build
            material.shader = Shader.Find("Legacy Shaders/Diffuse");

        material.mainTexture = texture;

        TexturesFunctions.setTexture(texture);
    }
}