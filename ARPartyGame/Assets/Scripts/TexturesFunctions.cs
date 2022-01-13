using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TexturesFunctions : MonoBehaviourPunCallbacks
{
    private static Texture2D texture;

    private static int width = 512;

    private static int height = 512;


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
        TexturesFunctions.texture = texture;
    }

    public static Texture2D changeTextureFormat(Texture2D texture, TextureFormat newFormat)
    {
        Texture2D newTexture = new Texture2D(width, height, newFormat, false);
        newTexture.SetPixels(texture.GetPixels());
        newTexture.Apply();
        return newTexture;
    }

    public static int getWidth()
    {
        return width;
    }

    public static int getHeight()
    {
        return height;
    }

    public static Texture2D RotateTexture(Texture2D originalTexture, int degrees)
    {
        Texture2D rotatedTexture = new Texture2D(originalTexture.height, originalTexture.width);

        for (int i = 0; i < originalTexture.width; i++)
        {
            for (int j = 0; j < originalTexture.height; j++)
            {
                rotatedTexture.SetPixel(j, originalTexture.width - i - 1, originalTexture.GetPixel(i, j));
            }
        }

        rotatedTexture.Apply();

        return rotatedTexture;

    }

    public static Texture2D ResizeTexture(Texture2D texture2D, int targetX, int targetY)
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
