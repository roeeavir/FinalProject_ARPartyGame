using UnityEngine;
using Vuforia;
using UnityEngine.Experimental.Rendering;

public class SideLoadImageTarget : MonoBehaviour
{
    private Texture2D textureFile;
    public float printedTargetSize = 0.32f;
    public string targetName = "DynamicImageTarget";

    public GameObject targetBoard;

    private ImageTargetBehaviour mTarget = null;


    void Start()
    {
        Debug.LogWarning("Start SideLoadImageTarget");
        // Get the texture file
        textureFile = TexturesFunctions.GetTexture();
        // Use Vuforia Application to invoke the function after Vuforia Engine is initialized
        VuforiaApplication.Instance.OnVuforiaStarted += CreateImageTargetFromSideloadedTexture;
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

        if (targetName.Equals("DemoDynamicImageTarget")) // For Bonus Game
        {
            // Create plane at the origin
            GameObject plane = Instantiate(targetBoard, mTarget.transform) as GameObject;
            plane.transform.localScale = new Vector3(0.4f, 0.8f, 0.4f);
            // Add plane to the newly created game object

            // Place the plane as mTarget's child
            plane.transform.parent = mTarget.transform;
        }
        else if (targetName.Equals("DynamicImageTarget")) // For Main Game
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

    void CreateImageTargetFromTexture()
    {
        textureFile = TexturesFunctions.ChangeTextureFormat(textureFile, TextureFormat.RGB24);

        Debug.Log("Texture format changed");


        if (VuforiaBehaviour.Instance == null)
        {
            Debug.LogWarning("VuforiaBehaviour is not null");
        }
        else
        {
            Debug.LogWarning("VuforiaBehaviour is null");
        }


        ImageTargetBehaviour target = VuforiaBehaviour.Instance.ObserverFactory.CreateImageTarget(
                   textureFile,
                   printedTargetSize,
                   targetName);

        Debug.LogWarning("Image Target created");

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
            camera.SetActive(false); // Reset Camera and world center by the anchor
            camera.GetComponent<VuforiaBehaviour>().SetWorldCenter(WorldCenterMode.DEVICE, mTarget.GetComponent<ImageTargetBehaviour>());
            camera.SetActive(true);
        } else {
            Debug.LogWarning("Camera is null");
        }

        GameObject tmpPlayersScores = GameObject.FindGameObjectWithTag("board"); 
        // if (tmpPlayersScores != null)
        // {
        //     Destroy(tmpPlayersScores);
        //     tmpPlayersScores = null;
        //     Debug.LogWarning("Players scores board has been destroyed");
        // }

        if (tmpPlayersScores == null){
            GameObject playersScores = Instantiate(targetBoard, mTarget.transform) as GameObject;
            if (playersScores != null)
            {
                Debug.LogWarning("Plane Scores Background is not null");
            }
            else
            {
                Debug.LogWarning("Plane Scores Background is null");
            }

            // Place the plane as mTarget's child
            playersScores.transform.parent = mTarget.transform;

            playersScores.transform.position = new Vector3(0f, 0f, 0f);
            playersScores.transform.localScale = new Vector3(0.03f, 1f, 0.03f);
            playersScores.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

}