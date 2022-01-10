using UnityEngine;
using Vuforia;

public class SideLoadImageTarget : MonoBehaviour
{
    private Texture2D textureFile;
    public float printedTargetSize = 1f;
    public string targetName = "DynamicImageTarget";


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
        var mTarget = VuforiaBehaviour.Instance.ObserverFactory.CreateImageTarget(
            textureFile,
            printedTargetSize,
            targetName);

        // add the Default Observer Event Handler to the newly created game object
        mTarget.gameObject.AddComponent<DefaultObserverEventHandler>();

        // add the ImageTargetBehaviour to the newly created game object
        // mTarget.gameObject.AddComponent<ImageTargetBehaviour>();

        // Create plane at the origin
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        // Add plane to the newly created game object
        Instantiate(GameObject.CreatePrimitive(PrimitiveType.Plane), mTarget.transform);

        Debug.Log("Instant Image Target created " + mTarget.TargetName);
    }
}