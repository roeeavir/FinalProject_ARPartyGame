using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is used to store the player's data
[System.Serializable] // This makes it so that the class can be seen in the inspector
public class PlayerData
{
    public string playerName;
    public int score;
    public string date; // The date the game was played

    // Constructor
    public PlayerData(string playerName, int score, string date)
    {
        this.playerName = playerName;
        this.score = score;
        this.date = date;
    }

    // toString method
    public override string ToString()
    {
        return string.Format("Player {0} with {1} points at {2}", playerName, score, date);
    }
}
