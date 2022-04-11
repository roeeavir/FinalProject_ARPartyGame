using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class ShootScript : MonoBehaviourPunCallbacks
{

    public GameObject arCamera;
    public GameObject smoke;

    private int score = 0;

    private int level;

    private string color = "";

    private string[] colors = { "blue", "red", "yellow", "pink", "green" };

    private void Start()
    {
        score = 0;
    }

    public void Shoot()
    {
        RaycastHit hit;

        if (color.Equals(""))
        {
            color = colors[PhotonNetwork.LocalPlayer.ActorNumber - 1];
            Debug.LogWarning(color);
        }

        if (Physics.Raycast(arCamera.transform.position, arCamera.transform.forward, out hit))
        {
            if (hit.transform.name.Contains("balloon"))
            {
                switch (level)
                {
                    case 2:
                        if (hit.transform.name.Contains(color.ToLower()))
                        {
                            AddScore(hit.transform.gameObject);
                        }
                        else
                        {
                            Debug.LogWarning("Wrong color");
                            photonView.RPC("sendScoreToAnotherPlayer", RpcTarget.All, hit.transform.name, hit.transform.gameObject.GetComponent<BalloonScript>().GetScore());
                        }
                        Destroy(hit.transform.gameObject);
                        Instantiate(smoke, hit.point, Quaternion.LookRotation(hit.normal));
                        break;
                    case 3:
                        if (hit.transform.name.Contains(color.ToLower()))
                        {
                            AddScore(hit.transform.gameObject);
                        }
                        else
                        {
                            Debug.LogWarning("Wrong color");
                            photonView.RPC("sendScoreToAnotherPlayer", RpcTarget.All, hit.transform.name, hit.transform.gameObject.GetComponent<BalloonScript>().GetScore());
                            SubstractScore(hit.transform.gameObject);
                        }
                        Destroy(hit.transform.gameObject);
                        Instantiate(smoke, hit.point, Quaternion.LookRotation(hit.normal));
                        break;
                    case 1:
                    default:
                        AddScore(hit.transform.gameObject);
                        Destroy(hit.transform.gameObject);
                        Instantiate(smoke, hit.point, Quaternion.LookRotation(hit.normal));
                        break;

                }
            }
        }
    }

    public void AddScore(GameObject balloon)
    {
        score += balloon.GetComponent<BalloonScript>().GetScore();
    }

    public void SubstractScore(GameObject balloon)
    {
        score -= balloon.GetComponent<BalloonScript>().GetScore();
        if (score < 0)
        {
            score = 0;
        }
    }

    public void ResetScore()
    {
        score = 0;
    }

    public int GetScore()
    {
        return score;
    }

    public void SetScore(int s)
    {
        score = s;
    }

    public void SetLevel(int lvl)
    {
        level = lvl;
        Debug.LogWarning("Level: " + level);
    }

    // public void SetColors(string[] colors)
    // {
    //     this.colors = colors;
    // }

    [PunRPC]
    private void sendScoreToAnotherPlayer(string name, int score)
    {
        Debug.LogWarning("Sending to another player. name:" + name + " score: " + score + " my color " + color);
        if (name.Contains(color.ToLower()))
        {
            this.score += score;
        }
    }

}
