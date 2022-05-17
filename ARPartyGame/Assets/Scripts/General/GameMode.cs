using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    public static int gameMode = 0; // Game mode is saved between scenes. 0 = Casual, 1 = Intermediate, 2 = Insane
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this); // Make sure this object is not destroyed when loading a new scene
    }

    // Update is called once per frame
    public static void SetGameMode(int mode)
    {
        gameMode = mode; // Set the game mode
    }
}
