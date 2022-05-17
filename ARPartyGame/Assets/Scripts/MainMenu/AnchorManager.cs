using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// A class to manage the set of the anchor in the game
public class AnchorManager : MonoBehaviour
{
    // Sets the anchor image texture from the gallery
    public Texture2D SetAnchorPhotoFromGallery(string path)
    {

        Debug.Log("Image path: " + path);
        if (path != null)
        {
            // Create Texture from selected image
            Texture2D texture = NativeGallery.LoadImageAtPath(path, 512, false);
            if (texture == null)
            {
                Debug.Log("Couldn't load texture from " + path);
                return null;
            }

            texture = TexturesFunctions.ResizeTexture(texture, TexturesFunctions.GetWidth(), TexturesFunctions.GetHeight());
            texture.Apply();

            return texture;
        }

        return null;
    }


    // Sets the anchor image texture from the camera
    public Texture2D TakeSnapshot(WebCamTexture webCamTexture)
    {
        Texture2D texture = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false); // create a new texture

        texture.SetPixels(webCamTexture.GetPixels()); // Copy the pixels from the WebCamTexture to the new texture
        texture = TexturesFunctions.ResizeTexture(texture, TexturesFunctions.GetWidth(), TexturesFunctions.GetHeight()); // resize the texture
        texture = TexturesFunctions.RotateTexture(texture, 90); // Rotate the texture 90 degrees
        texture.Apply(); // Apply the changes to the texture

        return texture;

    }

    // Set anchor photo from byte array
    [PunRPC]
    public Texture2D SetAnchorPhoto(byte[] receivedByte)
    {
        Texture2D texture = new Texture2D(1, 1); // Create a new texture
        texture.LoadImage(receivedByte); // Load the texture from the byte array

        return texture;
    }

}
