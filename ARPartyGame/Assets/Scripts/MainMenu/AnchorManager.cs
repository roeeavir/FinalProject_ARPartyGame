using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnchorManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

        texture.SetPixels(webCamTexture.GetPixels()); // copy the pixels from the WebCamTexture to the new texture
        texture = TexturesFunctions.ResizeTexture(texture, TexturesFunctions.GetWidth(), TexturesFunctions.GetHeight()); // resize the texture
        texture = TexturesFunctions.RotateTexture(texture, 90); // rotate the texture 90 degrees
        texture.Apply(); // apply the changes to the texture

        return texture;

    }

    [PunRPC]
    public Texture2D SetAnchorPhoto(byte[] receivedByte)
    {
        Texture2D texture = new Texture2D(1, 1); // create a new texture
        texture.LoadImage(receivedByte); // load the texture from the byte array

        return texture;
    }

}
