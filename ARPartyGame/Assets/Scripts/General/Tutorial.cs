using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{


    public Image origin;
    public Sprite[] tutorialImages;

    private  string[] tutorialTexts =  {
        "At first you will need to fill a nickname and room name\n To create a room, click on the button \"Create Room\"\n To join a room, click on the button \"Join Room\"\n ",
        "This is the Lobby, where you can see all the players in the room\n To start the game, click on the button \"Set Anchor Image\"\n ",
        "Now choose if the anchor is a photo from your gallery or a photo from your camera\n ",
        "Now take a photo of the anchor and make sure its a stable object\n ",
        "After Selecting the anchor, you need to chec if the anchor is good enough\n If the anchor is good enough, you will see an AR board that will confirm the anchor\n ",
        "Now make sure every player is ready and click on \"Start AR Game\"\n ",
        "After every player is pointing their camera to the anchor, the game will start in 3 seconds\n ",
        "The goal is to pop the baloons in the environment\n ",
        "Every stage has it's own objective",
        "In the final stage you will meet the boss\n "
    };

    public Text tutorialText;
    public static int index= 0;


    // Update is called once per frame
    void Update()
    {
        origin.sprite = tutorialImages[index];
        tutorialText.text = tutorialTexts[index];
    }

    public void Next()
    {
        if (index < tutorialImages.Length - 1)
        {
            index++;
            //origin.sprite = tutorialImages[index];
        }
        else
        {
            index = 0;
            //origin.sprite = tutorialImages[index];
        }
    }

    public void Previous()
    {
        if (index > 0)
        {
            index--;
            //origin.sprite = tutorialImages[index];
        }
        else
        {
            index = tutorialImages.Length - 1;
            //origin.sprite = tutorialImages[index];
        }
    }

    public void onExit()
    {
        index = 0;
    }
}
