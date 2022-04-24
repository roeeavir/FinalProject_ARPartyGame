using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardManager : MonoBehaviour
{

    private static List<PlayerData> playerDataList;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        if (playerDataList == null)
        {
            LoadScoreBoard();
        }
    }

    // Write all the players data to the score board
    public void WritePlayerData(Text highScoresText)
    {
        string highScoresTextString = "";

        LoadScoreBoard();

        if (playerDataList == null || playerDataList.Count == 0)
        {
            highScoresTextString = "No data to display";
        }
        else
        {
            SortPlayerDataList();
            for (int i = 0; i < playerDataList.Count; i++)
            {
                highScoresTextString += playerDataList[i].ToString() + "\n";
            }
        }
        if (highScoresText == null)
        {
            Debug.Log("No high scores text found");
            highScoresText = GameObject.Find("HighScoresText").GetComponent<Text>();
        }
        highScoresText.text = highScoresTextString;
    }

    public void AddPlayerData(string playerName, int score, string date)
    {
        if (playerDataList == null)
        {
            playerDataList = new List<PlayerData>();
        }

        PlayerData playerData = new PlayerData(playerName, score, date);
        playerDataList.Add(playerData);
        Debug.LogWarning("Player data added: " + playerData.ToString());

        if (playerDataList.Count > 10)
        {
            playerDataList.RemoveAt(10);
        }
    }

    private void SortPlayerDataList()
    {
        playerDataList.Sort((x, y) => y.score.CompareTo(x.score));
    }

    public void SaveScoreBoard()
    {
        string dataToSave = "";
        for (int i = 0; i < playerDataList.Count; i++)
        {
            dataToSave += playerDataList[i].playerName + ";" + playerDataList[i].score + ";" + playerDataList[i].date + "\n";
        }
        // string json = JsonUtility.ToJson(playerDataList);
        PlayerPrefs.SetString("ScoreBoard", dataToSave);
        PlayerPrefs.Save();
        Debug.LogWarning("ScoreBoard saved: " + dataToSave);
    }

    private void LoadScoreBoard()
    {
        string dataToLoad = PlayerPrefs.GetString("ScoreBoard");
        if (dataToLoad != null && dataToLoad != "")
        {
            playerDataList = new List<PlayerData>();
            string[] lines = dataToLoad.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(';');
                if (data.Length == 3)
                {
                    AddPlayerData(data[0], int.Parse(data[1]), data[2]);
                }
            }
            Debug.LogWarning("Data loaded from PlayerPrefs: " + playerDataList);
        } else {
            Debug.LogWarning("No data to load");
        }
    }

}
