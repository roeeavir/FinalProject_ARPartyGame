using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    private Transform[] spawnPoints;
    public GameObject[] enemies;

    public GameObject boss;

    private int numOfSpawnPoints = 3;

    private float nextTimeToSpawn = 2.5f;

    private int groupId = 0;


    private bool bossSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartSpawning());
        // StartCoroutine(MixSpawnPoints());
    }

    private IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(nextTimeToSpawn);

        if (spawnPoints != null && !bossSpawned)
        {
            for (int i = 0; i < numOfSpawnPoints; i++)
            {
                if (groupId >= 100)
                {
                    boss.GetComponent<EnemyScript>().groupId = groupId;
                    Instantiate(boss, spawnPoints[i].position, Quaternion.identity);
                    // boss.transform.localScale = new Vector3(2f, 2f, 2f);
                    bossSpawned = true;
                    break;
                }
                if (groupId != 0)
                {
                    enemies[i].GetComponent<EnemyScript>().groupId = groupId;
                }
                Instantiate(enemies[i], spawnPoints[i].position, Quaternion.identity);
                spawnPoints[i] = SpawnPointsScript.CreateNewSpawnPoint();
            }

        }

        if (!bossSpawned)
        {
            StartCoroutine(StartSpawning());
        }

    }

    public void setSpawnPoints(Transform[] spawnPoints)
    {
        this.spawnPoints = spawnPoints;
        if (spawnPoints != null)
        {
            numOfSpawnPoints = spawnPoints.Length;
        }
        else
        {
            numOfSpawnPoints = 0;
        }
    }

    public void SetNextTimeToSpawn(float time)
    {
        nextTimeToSpawn = time;
    }

    public void SetGroupId(int gid)
    {
        groupId = gid;
    }

}