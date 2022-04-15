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

    private float distance = 2f;

    private Text objectiveText;

    private Text colorText;

    private string tempObjective = "";

    private void Start()
    {
        score = 0;
        objectiveText = GameObject.Find("ObjectiveText").GetComponent<Text>();
        colorText = GameObject.Find("ColorText").GetComponent<Text>();
        currentColor = colorsStr[PhotonNetwork.LocalPlayer.ActorNumber - 1];
        index = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        colorText.color = colors[index]; // Sets the color of the player to the color of the player's ID
        Debug.LogWarning("Players Color : " + colors[index]);
        colorText.text = "YOUR COLOR IS:\n" + currentColor; // Sets the color of the player to the color of the player's ID
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
                        DestroyEnemy(hit, popScore);
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
                        DestroyEnemy(hit, popScore);
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
                            colorText.color = colors[index];
                            colorText.text = "YOUR COLOR IS:\n" + currentColor;
                        }
                        else
                        {
                            Debug.LogWarning("Wrong color");
                            photonView.RPC("SendScoreToAnotherPlayer", RpcTarget.All, hit.transform.name.ToLower(), hit.transform.gameObject.GetComponent<EnemyScript>().GetScore());
                            SubstractScore(hit.transform.gameObject);
                            popScore = -hit.transform.gameObject.GetComponent<EnemyScript>().GetScore();
                        }
                        DestroyEnemy(hit, popScore);
                        break;
                    case 4: // Boss
                        HandleBossHit(hit);
                        break;
                }
            }
        }
        else
        {
            Debug.LogWarning("No target");
        }
    }

    // Adds score
    public void AddScore(GameObject enemy)
    {
        score += enemy.GetComponent<EnemyScript>().GetScore();
    }

    // Substracts the score of the enemy from the player's score
    public void SubstractScore(GameObject enemy)
    {
        score -= enemy.GetComponent<EnemyScript>().GetScore();
        if (score < 0)
        {
            score = 0;
        }
    }

    // Resets the score to 0
    public void ResetScore()
    {
        score = 0;
    }

    // Returns the current score
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

    // Destroys the enemy and shows a pop up score
    private void DestroyEnemy(RaycastHit hit, int enemyScore)
    {
        Destroy(hit.transform.gameObject);
        Instantiate(smoke, hit.point, Quaternion.LookRotation(hit.normal));
        ShowPopupScore(enemyScore);
    }

    // Spawns a bullet from camera position
    private IEnumerator SpawnBullet()
    {
        shootBtn.SetActive(false);
        GameObject bullet = Instantiate(bulletPrefab, arCamera.transform.position, arCamera.transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(arCamera.transform.forward * 2000);
        yield return new WaitForSeconds(0.5f);
        shootBtn.SetActive(true);
        Destroy(bullet, 2.0f);
    }

    // Handles the boss hit
    private void HandleBossHit(RaycastHit hit)
    {

        hit.transform.gameObject.GetComponent<EnemyScript>().SetFreeze(true);

        if (hit.transform.gameObject.GetComponent<ARTarget>().OnHit())
        { // Is boss dead
            StartCoroutine(handleBossDeadAnimation(hit));

            return;
        }

        StartCoroutine(handleBossDamagedAnimation(hit));

    }

    // Shows a popup score when the enemy is hit
    private void ShowPopupScore(int score)
    {
        GameObject popup = Instantiate(popupScore, arCamera.transform.position + arCamera.transform.forward * distance, arCamera.transform.rotation * Quaternion.Euler(0, 0, 90));
        popup.GetComponent<TextMesh>().text = score >= 0 ? "+" + score : score.ToString();
        popup.GetComponent<TextMesh>().color = colors[index];
        Destroy(popup, 1.0f);
    }

    // Handles the boss dead animation
    private IEnumerator handleBossDeadAnimation(RaycastHit hit)
    {
        hit.transform.gameObject.GetComponent<BoxCollider>().enabled = false;
        hit.transform.gameObject.GetComponent<Animation_Test>().DeathAni();

        yield return new WaitForSeconds(2f);


        Debug.LogWarning("Boss Dead");
        hit.transform.gameObject.GetComponent<EnemyScript>().SetScore(hit.transform.gameObject.GetComponent<EnemyScript>().GetScore() * 2);
        ShowPopupScore(hit.transform.gameObject.GetComponent<EnemyScript>().GetScore());
        AddScore(hit.transform.gameObject);
        DestroyEnemy(hit, hit.transform.gameObject.GetComponent<EnemyScript>().GetScore());
        hit.transform.gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    // Handles the boss damaged animation
    private IEnumerator handleBossDamagedAnimation(RaycastHit hit)
    {
        hit.transform.gameObject.GetComponent<BoxCollider>().enabled = false;
        hit.transform.gameObject.GetComponent<Animation_Test>().DamageAni();

        yield return new WaitForSeconds(1f);

        hit.transform.gameObject.GetComponent<Animation_Test>().IdleAni();
        hit.transform.gameObject.GetComponent<EnemyScript>().SetFreeze(false);

        ShowPopupScore(hit.transform.gameObject.GetComponent<EnemyScript>().GetScore());
        AddScore(hit.transform.gameObject);
        hit.transform.gameObject.transform.position = SpawnPointsScript.CreateNewSpawnPoint().position;
        hit.transform.gameObject.GetComponent<EnemyScript>().AppenedBossSpeedMultiplier(); // Speeds boss up

        hit.transform.gameObject.GetComponent<BoxCollider>().enabled = true;

        if (tempObjective.Equals(""))
        {
            tempObjective = objectiveText.text;
        }

        objectiveText.text = tempObjective + "\nBoss HP: " + hit.transform.gameObject.GetComponent<ARTarget>().GetHealth();
    }

}
