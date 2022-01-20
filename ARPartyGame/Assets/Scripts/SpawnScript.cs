using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    private Transform[] spawnPoints;
    public GameObject[] ballons;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartSpawning());
    }

    IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(3);
        
        if (spawnPoints != null)
        {
            for (int i = 0; i < 3; i++)
            {
                Instantiate(ballons[i], spawnPoints[i].position, Quaternion.identity);
            }
        }

        StartCoroutine(StartSpawning());
    }

    public void setSpawnPoints(Transform[] spawnPoints)
    {
        this.spawnPoints = spawnPoints;
    }
}
