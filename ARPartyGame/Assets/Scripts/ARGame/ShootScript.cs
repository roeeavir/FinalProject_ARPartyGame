using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class ShootScript : MonoBehaviourPunCallbacks
{

    public GameObject arCamera;
    public GameObject smoke;

    public GameObject bulletPrefab;

    public GameObject shootBtn;

    public GameObject popupScore;

    private int score = 0;

    private int level;

    private string color = "";

    private string[] colors = { "blue", "green", "orange", "purple", "pink" };

    private void Start()
    {
        score = 0;
    }

    public void Shoot()
    {
        RaycastHit hit;

        StartCoroutine(SpawnBullet());

        if (color.Equals(""))
        {
            color = colors[PhotonNetwork.LocalPlayer.ActorNumber - 1];
            Debug.LogWarning(color);
        }

        if (Physics.Raycast(arCamera.transform.position, arCamera.transform.forward, out hit))
        {
            if (hit.transform.name.ToLower().Contains("jelly"))
            {
                int popScore = 0;
                switch (level)
                {
                    case 2:
                        if (hit.transform.name.ToLower().Contains(color.ToLower()))
                        {
                            AddScore(hit.transform.gameObject);
                            popScore = hit.transform.gameObject.GetComponent<EnemyScript>().GetScore();
                        }
                        else
                        {
                            Debug.LogWarning("Wrong color");
                            photonView.RPC("sendScoreToAnotherPlayer", RpcTarget.All, hit.transform.name.ToLower(), hit.transform.gameObject.GetComponent<EnemyScript>().GetScore());
                        }
                        StartCoroutine(DestroyEnemy(hit, popScore));
                        break;
                    case 3:
                        if (hit.transform.name.ToLower().Contains(color.ToLower()))
                        {
                            AddScore(hit.transform.gameObject);
                            popScore = hit.transform.gameObject.GetComponent<EnemyScript>().GetScore();
                        }
                        else
                        {
                            Debug.LogWarning("Wrong color");
                            photonView.RPC("sendScoreToAnotherPlayer", RpcTarget.All, hit.transform.name.ToLower(), hit.transform.gameObject.GetComponent<EnemyScript>().GetScore());
                            SubstractScore(hit.transform.gameObject);
                            popScore = -hit.transform.gameObject.GetComponent<EnemyScript>().GetScore();
                        }
                        StartCoroutine(DestroyEnemy(hit, popScore));
                        break;
                    case 1:
                    default:
                        AddScore(hit.transform.gameObject);
                        popScore = hit.transform.gameObject.GetComponent<EnemyScript>().GetScore();
                        StartCoroutine(DestroyEnemy(hit, popScore));
                        break;

                }
            }
        } else {
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
        Debug.LogWarning("Sending to another player. name:" + name + " score: " + score + " my color " + color);
        if (name.ToLower().Contains(color.ToLower()))
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
