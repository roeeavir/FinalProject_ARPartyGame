using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCam;
    private Texture defaultBackground;


    public RawImage background;

    public AspectRatioFitter fit;

    private void Start()
    {
        defaultBackground = background.texture; // Get the default background texture
        WebCamDevice[] devices = WebCamTexture.devices; // Get the list of available devices

        if (devices.Length == 0)
        {
            Debug.Log("No camera detected");
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name, TexturesFunctions.GetWidth(), TexturesFunctions.GetHeight()); // Create a new WebCamTexture
            }
        }

        if (backCam == null)
        {
            Debug.Log("Unable to find back camera");
            return;
        }

        Debug.Log("BackCam ratio: " + ((float)backCam.width / (float)backCam.height));

        backCam.Play(); // Play the WebCamTexture
        background.texture = backCam; // Set the background to the WebCamTexture

        camAvailable = true;

    }

    private void Update()
    {
        if (!camAvailable)
            return;

        float ratio = (float)backCam.width / (float)backCam.height; // Get the ratio of the WebCamTexture
        fit.aspectRatio = ratio;

        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f; // Check if the WebCamTexture is mirrored vertically
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -backCam.videoRotationAngle; // Get the rotation of the WebCamTexture
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }

    private void OnDisable()
    {
        background.texture = defaultBackground; // Set the background to the default texture
        backCam.Stop(); // Stop the WebCamTexture
    }

    private void OnEnable()
    {
        Start();
    }

    public WebCamTexture GetWebCamTexture()
    {
        return backCam;
    }


}
