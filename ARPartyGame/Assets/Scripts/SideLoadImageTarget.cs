using UnityEngine;
using Vuforia;
using UnityEngine.Experimental.Rendering;

public class SideLoadImageTarget : MonoBehaviour
{
    private Texture2D textureFile;
    public float printedTargetSize = 0.32f;
    public string targetName = "DynamicImageTarget";

    private ImageTargetBehaviour mTarget = null;


    void Start()
    {
        // Get the texture file
        textureFile = TexturesFunctions.GetTexture();
        // Use Vuforia Application to invoke the function after Vuforia Engine is initialized
        VuforiaApplication.Instance.OnVuforiaStarted += CreateImageTargetFromSideloadedTexture;

    }

    void CreateImageTargetFromSideloadedTexture()
    {
        Debug.Log("Yese of Sufferings");

        if (textureFile == null)
        {
            Debug.Log("Texture file is null");
            return;
        }

        CreateImageTargetFromTexture();

        if (targetName.Equals("DynamicImageTarget"))
        {
            GameObject yese_plane = GameObject.Find("Yese");
            yese_plane.GetComponent<Renderer>().material.mainTexture = TexturesFunctions.GetTexture();
            Debug.Log("Plane Yese set to texture");

            // Create plane at the origin
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.localScale = new Vector3(0.4f, 1f, 0.4f);
            // Add plane to the newly created game object
            // Instantiate(GameObject.CreatePrimitive(PrimitiveType.Plane), mTarget.transform);

            // Place the plane as mTarget's child
            plane.transform.parent = mTarget.transform;
        }

        VuforiaApplication.Instance.OnVuforiaStarted -= CreateImageTargetFromSideloadedTexture;



        Debug.Log("Instant Image Target created " + mTarget.TargetName);

    }

    public ImageTargetBehaviour GetTestImageTarget(Texture2D texture, string tName)
    {
        textureFile = texture;
        targetName = tName;
        if (textureFile == null)
        {
            Debug.Log("Texture file is null");
            return null;
        }
        Debug.Log("Texture file is not null");
        // CreateImageTargetFromTexture();
        VuforiaApplication.Instance.OnVuforiaStarted += CreateImageTargetFromTexture;
        // ImageTargetBehaviour target = mTarget;
        // mTarget = null;
        // Debug.Log("Image Target created " + mTarget.TargetName);
        if (mTarget == null)
        {
            Debug.Log("Image Target is null");
            // return null;
        }
        // while (mTarget == null)
        // {
        // //     // CreateImageTargetFromSideloadedTexture();
        // //     // Debug.Log("Waiting for Image Target");
        // //     // yield return new WaitForSeconds(0.5f);
        // }

        // VuforiaBehaviour.Instance.enabled = true;

        return mTarget;
    }

    void CreateImageTargetFromTexture()
    {
        textureFile = TexturesFunctions.ChangeTextureFormat(textureFile, TextureFormat.RGB24);

        Debug.Log("Texture format changed");


        if (VuforiaBehaviour.Instance == null)
        {
            Debug.Log("VuforiaBehaviour is not null");
        }
        else
        {
            Debug.Log("VuforiaBehaviour is null");
        }

        // if (VuforiaBehaviour.Instance.ObserverFactory == null)
        // {
        //     Debug.Log("Observer Factory is null");
        // } else
        // {
        //     Debug.Log("Observer Factory is not null");
        // }

        ImageTargetBehaviour target = VuforiaBehaviour.Instance.ObserverFactory.CreateImageTarget(
                   textureFile,
                   printedTargetSize,
                   targetName);

        Debug.Log("Image Target created");

        // add the Default Observer Event Handler to the newly created game object
        target.gameObject.AddComponent<DefaultObserverEventHandler>();

        // add the ImageTargetBehaviour to the newly created game object
        target.gameObject.AddComponent<ImageTargetBehaviour>();

        // add mesh renderer to the newly created game object
        target.gameObject.AddComponent<MeshRenderer>();

        // add turnoffbehaviour to the newly created game object
        target.gameObject.AddComponent<TurnOffBehaviour>();

        // add image target mesh 177266 to the newly created game object
        // target.gameObject.AddComponent<ImageTargetMesh177266>();

        if (target == null)
        {
            Debug.Log("Image Target is null");
        }
        else
        {
            Debug.Log("Image Target is not null");
        }

        mTarget = target;
    }

    public ImageTargetBehaviour GetImageTarget()
    {
        return mTarget;
    }

    public void SetTexture(Texture2D texture, string name)
    {
        textureFile = texture;
        targetName = name;
    }

}