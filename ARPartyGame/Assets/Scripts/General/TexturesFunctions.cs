using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TexturesFunctions : MonoBehaviourPunCallbacks
{
    private static Texture2D texture = null;

    private static int width = 512;

    private static int height = 512;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this); // Don't destroy this object when loading a new scene
    }

    // 
    [PunRPC]
    public void SetAnchorPhoto(byte[] receivedByte)
    {
        texture = new Texture2D(1, 1); // Create a new texture
        texture.LoadImage(receivedByte); // Load the texture from the byte array

        // Assign texture to a temporary quad and destroy it after 5 seconds
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad); // Create a quad
        quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f; // Place the quad 2.5 meters in front of the camera
        quad.transform.forward = Camera.main.transform.forward; // Make the quad face the camera
        quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f); // Scale the quad based on the height and width of the texture

        Material material = quad.GetComponent<Renderer>().material;
        if (!material.shader.isSupported) // happens when Standard shader is not included in the build
            material.shader = Shader.Find("Legacy Shaders/Diffuse");

        material.mainTexture = texture;
    }

    public static Texture2D GetTexture()
    {
        return texture;
    }

    public static void SetTexture(Texture2D texture)
    {
        TexturesFunctions.texture = texture;
    }

    // Changes the texture to a new format
    public static Texture2D ChangeTextureFormat(Texture2D texture, TextureFormat newFormat)
    {
        Texture2D newTexture = new Texture2D(width, height, newFormat, false);
        newTexture.SetPixels(texture.GetPixels());
        newTexture.Apply();
        return newTexture;
    }

    public static int GetWidth()
    {
        return width;
    }

    public static int GetHeight()
    {
        return height;
    }

    // Rotates the texture by the given degrees
    public static Texture2D RotateTexture(Texture2D originalTexture, int degrees)
    {
        Texture2D rotatedTexture = new Texture2D(originalTexture.height, originalTexture.width);
        // Iterate through the pixels of the original texture and rotates them individually
        for (int i = 0; i < originalTexture.width; i++)
        {
            for (int j = 0; j < originalTexture.height; j++)
            {
                rotatedTexture.SetPixel(j, originalTexture.width - i - 1, originalTexture.GetPixel(i, j)); // Rotate the pixel
            }
        }

        rotatedTexture.Apply(); // Apply all SetPixel calls

        return rotatedTexture;

    }

    // Resize the texture to the specified width and height.
    public static Texture2D ResizeTexture(Texture2D texture2D, int targetX, int targetY)
    {
        Texture2D result = new Texture2D(targetX, targetY, texture2D.format, true); // Create a new texture that will be the resized version of the texture2D
        Color[] rpixels = result.GetPixels(0); // Get the pixels from the original texture
        float incX = (1.0f / (float)targetX); // amount to increment x when stepping up rows
        float incY = (1.0f / (float)targetY); // amount to increment y when stepping up pixels
        for (int px = 0; px < rpixels.Length; px++) // Loop through all pixels
        {
            rpixels[px] = texture2D.GetPixelBilinear(incX * ((float)px % targetX), incY * ((float)Mathf.Floor(px / targetX))); // Set each pixel to be the result of the bilinear filtering
        }

        result.SetPixels(rpixels, 0); // Load the pixels into the texture
        result.Apply(); // Apply the new texture
        return result;
    }

}
