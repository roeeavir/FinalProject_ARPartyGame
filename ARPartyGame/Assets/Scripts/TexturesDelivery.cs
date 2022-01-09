using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TexturesDelivery : MonoBehaviourPunCallbacks
{
    private static Texture2D texture;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }


    [PunRPC]
    public void SetAnchorPhoto(byte[] receivedByte)
    {
        texture = new Texture2D(1, 1);
        texture.LoadImage(receivedByte);

        // Assign texture to a temporary quad and destroy it after 5 seconds
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
        quad.transform.forward = Camera.main.transform.forward;
        quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

        Material material = quad.GetComponent<Renderer>().material;
        if (!material.shader.isSupported) // happens when Standard shader is not included in the build
            material.shader = Shader.Find("Legacy Shaders/Diffuse");

        material.mainTexture = texture;
    }

    public static Texture2D getTexture()
    {
        return texture;
    }

    public static void setTexture(Texture2D texture)
    {
        TexturesDelivery.texture = texture;
    }
}
