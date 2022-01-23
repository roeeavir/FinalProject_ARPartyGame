using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootScript : MonoBehaviour
{

    public GameObject arCamera;
    public GameObject smoke;

    public static int score = 0;


    public void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(arCamera.transform.position, arCamera.transform.forward, out hit))
        {
            if (hit.transform.name == "balloon1(Clone)" || hit.transform.name == "balloon2(Clone)"
                || hit.transform.name == "balloon3(Clone)")
            {
                AddScore(hit.transform.gameObject);

                Destroy(hit.transform.gameObject);

                Instantiate(smoke, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }

    public void AddScore(GameObject balloon)
    {
        score += balloon.GetComponent<BalloonScript>().GetScore();
    }

    public void ResetScore()
    {
        score = 0;
    }

    public int GetScore()
    {
        return score;
    }
}
