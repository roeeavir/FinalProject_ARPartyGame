using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootScript : MonoBehaviour
{

    public GameObject arCamera;
    public GameObject smoke;

    private int score = 0;

    private int level;

    private string color = "";

    private void Start()
    {
        score = 0;
    }

    public void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(arCamera.transform.position, arCamera.transform.forward, out hit))
        {
            if (hit.transform.name.Contains("balloon"))
            {
                switch (level)
                {

                    case 3:
                        if (hit.transform.name.Contains(color))
                        {
                            AddScore(hit.transform.gameObject);

                            Destroy(hit.transform.gameObject);

                            Instantiate(smoke, hit.point, Quaternion.LookRotation(hit.normal));
                        } else {
                            SubstractScore(hit.transform.gameObject);

                            Destroy(hit.transform.gameObject);

                            Instantiate(smoke, hit.point, Quaternion.LookRotation(hit.normal));
                        }
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
    }
}
