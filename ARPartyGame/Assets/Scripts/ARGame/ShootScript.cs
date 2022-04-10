using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


public class ShootScript : MonoBehaviourPunCallbacks
{

    public GameObject arCamera;
    public GameObject smoke;

    public GameObject bulletPrefab;

    public GameObject shootBtn;

    public GameObject popupScore;

    private int score = 0;

    private int level;

    private string currentColor = "";

    private string[] colorsStr = { "blue", "green", "orange", "purple", "pink" };

    private Color[] colors = { /*Blue*/new Color(0.1f, 0.5f, 0.75f), /*Green*/new Color(0.2f, 0.8f, 0.4f), /*Orange*/new Color(1f, 0.58f, 0.1f),
     /*Purple*/new Color(0.65f, 0.3f, 0.97f), /*Pink*/new Color(0.75f, 0.15f, 0.8f) };

    private int index;

    private Text objectiveText;

    private void Start()
    {
        score = 0;
        objectiveText = GameObject.Find("ObjectiveText").GetComponent<Text>();
        currentColor = colorsStr[PhotonNetwork.LocalPlayer.ActorNumber - 1];
        index = PhotonNetwork.LocalPlayer.ActorNumber - 1; 
        objectiveText.color = colors[index]; // Sets the color of the player to the color of the player's ID
        Debug.LogWarning("Players Color : " + colors[index]);
    }

    public void Shoot()
    {
        RaycastHit hit;

        StartCoroutine(SpawnBullet());

        if (currentColor.Equals(""))
        {
            currentColor = colorsStr[PhotonNetwork.LocalPlayer.ActorNumber - 1];
            index = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            Debug.LogWarning(currentColor);
        }

        if (Physics.Raycast(arCamera.transform.position, arCamera.transform.forward, out hit))
        {
            if (hit.transform.name.ToLower().Contains("jelly"))
            {
                int popScore = 0;
                switch (level)
                {
                    case 1:
                    default:
                        AddScore(hit.transform.gameObject);
                        popScore = hit.transform.gameObject.GetComponent<EnemyScript>().GetScore();
                        StartCoroutine(DestroyEnemy(hit, popScore));
                        break;
                    case 2:
                        if (hit.transform.name.ToLower().Contains(currentColor.ToLower()))
                        {
                            AddScore(hit.transform.gameObject);
                            popScore = hit.transform.gameObject.GetComponent<EnemyScript>().GetScore();
                        }
                        else
                        {
                            Debug.LogWarning("Wrong color");
                            photonView.RPC("SendScoreToAnotherPlayer", RpcTarget.All, hit.transform.name.ToLower(), hit.transform.gameObject.GetComponent<EnemyScript>().GetScore());
                            SubstractScore(hit.transform.gameObject);
                            popScore = -hit.transform.gameObject.GetComponent<EnemyScript>().GetScore();
                        }
                        StartCoroutine(DestroyEnemy(hit, popScore));
                        break;
                    case 3:
                        if (hit.transform.name.ToLower().Contains(currentColor.ToLower()))
                        {
                            AddScore(hit.transform.gameObject);
                            popScore = hit.transform.gameObject.GetComponent<EnemyScript>().GetScore();
                            index++;
                            if (index > colorsStr.Length - 1)
                            {
                                index = 0;
                            }
                            currentColor = colorsStr[index];
                            objectiveText.color = colors[index];
                        }
                        else
                        {
                            Debug.LogWarning("Wrong color");
                            photonView.RPC("SendScoreToAnotherPlayer", RpcTarget.All, hit.transform.name.ToLower(), hit.transform.gameObject.GetComponent<EnemyScript>().GetScore());
                            SubstractScore(hit.transform.gameObject);
                            popScore = -hit.transform.gameObject.GetComponent<EnemyScript>().GetScore();
                        }
                        StartCoroutine(DestroyEnemy(hit, popScore));
                        break;
                }
            }
        }
        else
        {
            Debug.LogWarning("No target");
        }
    }

    public void AddScore(GameObject enemy)
    {
        score += enemy.GetComponent<EnemyScript>().GetScore();
    }

    public void SubstractScore(GameObject enemy)
    {
        score -= enemy.GetComponent<EnemyScript>().GetScore();
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

    [PunRPC]
    private void SendScoreToAnotherPlayer(string name, int score)
    {
        Debug.LogWarning("Sending to another player. name:" + name + " score: " + score + " my color " + currentColor);
        if (name.ToLower().Contains(currentColor.ToLower()))
        {
            this.score += score;
        }
    }

    private IEnumerator DestroyEnemy(RaycastHit hit, int popScore)
    {
        Destroy(hit.transform.gameObject);
        Instantiate(smoke, hit.point, Quaternion.LookRotation(hit.normal));
        GameObject popup = Instantiate(popupScore, hit.point, Quaternion.LookRotation(arCamera.transform.forward));
        popup.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        popup.transform.forward = popup.transform.forward * -1; // Fixes rotation
        popup.GetComponent<TextMesh>().text = popScore >= 0 ? "+" + popScore : popScore.ToString();
        yield return new WaitForSeconds(1f);
        Destroy(popup);
    }

    // Spawns a bullet from camera position
    private IEnumerator SpawnBullet()
    {
        shootBtn.SetActive(false);
        GameObject bullet = Instantiate(bulletPrefab, arCamera.transform.position, arCamera.transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(arCamera.transform.forward * 1500);
        yield return new WaitForSeconds(0.5f);
        shootBtn.SetActive(true);
        Destroy(bullet, 2.0f);
    }


}
