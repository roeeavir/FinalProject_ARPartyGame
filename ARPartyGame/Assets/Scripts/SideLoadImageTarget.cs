using UnityEngine;
using Vuforia;
using UnityEngine.Experimental.Rendering;

public class SideLoadImageTarget : MonoBehaviour
{
    private Texture2D textureFile;
    public float printedTargetSize = 0.32f;
    public string targetName = "DynamicImageTarget";

    private static ImageTargetBehaviour mTarget = null;


    void Start()
    {
        // Get the texture file
        textureFile = TexturesFunctions.getTexture();
        // Use Vuforia Application to invoke the function after Vuforia Engine is initialized
        VuforiaApplication.Instance.OnVuforiaStarted += CreateImageTargetFromSideloadedTexture;
    }

    void CreateImageTargetFromSideloadedTexture()
    {
        Debug.Log("Yese of sufferings");

        // set plane texture

        GameObject yese_plane = GameObject.Find("Yese");
        yese_plane.GetComponent<Renderer>().material.mainTexture = TexturesFunctions.getTexture();
        Debug.Log("Plane Yese set to texture");

        textureFile = TexturesFunctions.changeTextureFormat(textureFile, TextureFormat.RGB24);

        ImageTargetBehaviour mTarget = VuforiaBehaviour.Instance.ObserverFactory.CreateImageTarget(
            textureFile,
            printedTargetSize,
            targetName);

        // add the Default Observer Event Handler to the newly created game object
        mTarget.gameObject.AddComponent<DefaultObserverEventHandler>();

        // add the ImageTargetBehaviour to the newly created game object
        mTarget.gameObject.AddComponent<ImageTargetBehaviour>();

        // Create plane at the origin
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(0.4f, 1f, 0.4f);
        // Add plane to the newly created game object
        // Instantiate(GameObject.CreatePrimitive(PrimitiveType.Plane), mTarget.transform);

        // Place the plane as mTarget's child
        plane.transform.parent = mTarget.transform;

        Debug.Log("Instant Image Target created " + mTarget.TargetName);
    }

    public static ImageTargetBehaviour GetImageTarget()
    {
        return mTarget;
    }

}