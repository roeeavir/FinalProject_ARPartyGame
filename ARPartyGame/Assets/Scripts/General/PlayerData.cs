using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int score;
    public string date;

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
