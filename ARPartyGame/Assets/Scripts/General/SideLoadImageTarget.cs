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
        Debug.LogWarning("Start SideLoadImageTarget");
        // Get the texture file
        textureFile = TexturesFunctions.GetTexture();
        // Use Vuforia Application to invoke the function after Vuforia Engine is initialized
        VuforiaApplication.Instance.OnVuforiaStarted += CreateImageTargetFromSideloadedTexture;
        // VuforiaApplication.Instance.OnVuforiaPaused += RepeatCreateImageTargetFromSideloadedTexture;

    }

    void OnEnable()
    {
        Debug.Log("OnEnable");
        mTarget = null;
        CreateImageTargetFromSideloadedTexture();
    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
        VuforiaApplication.Instance.OnVuforiaStarted -= CreateImageTargetFromSideloadedTexture;
        Debug.Log("CreateImageTargetFromSideloadedTexture is done");
        // VuforiaApplication.Instance.OnVuforiaPaused -= RepeatCreateImageTargetFromSideloadedTexture;
        // Debug.Log("RepeatCreateImageTargetFromSideloadedTexture is done");
    }


    void CreateImageTargetFromSideloadedTexture()
    {
        if (mTarget != null)
        {
            Debug.Log("Image Target already created");
            return;
        }
        Debug.Log("Yese of Sufferings");

        if (textureFile == null)
        {
            Debug.Log("Texture file is null");
            return;
        }

        CreateImageTargetFromTexture();

        if (targetName.Equals("DemoDynamicImageTarget"))
        {
            // Create plane at the origin
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.localScale = new Vector3(0.4f, 1f, 0.4f);
            // Add plane to the newly created game object
            // Instantiate(GameObject.CreatePrimitive(PrimitiveType.Plane), mTarget.transform);

            // Place the plane as mTarget's child
            plane.transform.parent = mTarget.transform;
        }
        else if (targetName.Equals("DynamicImageTarget"))
        {

            setTargetChildren();

            Debug.Log("Main AR game anchor has been created");
        }
        else
        {
            Debug.Log("Target name is not valid");
        }

        // VuforiaApplication.Instance.OnVuforiaStarted -= CreateImageTargetFromSideloadedTexture;
        // Debug.Log("CreateImageTargetFromSideloadedTexture is done");


        Debug.Log("Instant Image Target created " + mTarget.TargetName);

    }

    // void RepeatCreateImageTargetFromSideloadedTexture(bool b){
    //     if (b)
    //     {
    //         Debug.Log("Vuforia is paused");
    //     } else {
    //         mTarget = null;
    //         CreateImageTargetFromSideloadedTexture();
    //         Debug.Log("RepeatCreateImageTargetFromSideloadedTexture is done");
    //     }
    // }

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

        if (target == null)
        {
            Debug.Log("Image Target of name " + targetName + " is null");
        }
        else
        {
            Debug.Log("Image Target of name " + targetName + " is not null");
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

    public void setTargetChildren()
    {
        GameObject camera = GameObject.Find("ARCamera");
        if (camera != null)
        {
            camera.GetComponent<VuforiaBehaviour>().SetWorldCenter(WorldCenterMode.SPECIFIC_TARGET, mTarget.GetComponent<ImageTargetBehaviour>());
        }
        GameObject playersScores = GameObject.Find("Scores Background");
        if (playersScores != null)
        {
            // Debug.Log("Plane Yese set to texture");
            Debug.Log("Plane Scores Background is not null");
            // playersScores.GetComponent<Renderer>().material.mainTexture = TexturesFunctions.GetTexture();
        }
        else
        {
            Debug.Log("Plane Scores Background is null");
        }

        // playersScores.transform.localScale = new Vector3(-1f, 1f, 1f);

        // Place the plane as mTarget's child
        playersScores.transform.parent = mTarget.transform;

        playersScores.transform.position = new Vector3(0f, 0f, 0f);
        playersScores.transform.localScale = new Vector3(0.03f, 1f, 0.03f);
        playersScores.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

    }

}