using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    private Transform[] spawnPoints;
    public GameObject[] ballons;

    private int numOfSpawnPoints = 3;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartSpawning());
    }

    IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(2.5f);

        if (spawnPoints != null)
        {
            for (int i = 0; i < numOfSpawnPoints; i++)
            {
                // ballons[i].GetComponent<BalloonScript>().groupId = 
                Instantiate(ballons[i], spawnPoints[i].position, Quaternion.identity);
            }
        }

        StartCoroutine(StartSpawning());
    }

    public void setSpawnPoints(Transform[] spawnPoints)
    {
        this.spawnPoints = spawnPoints;
        if (spawnPoints != null)
        {
            numOfSpawnPoints = spawnPoints.Length;
        } else {
            numOfSpawnPoints = 0;
        }
    }

}
