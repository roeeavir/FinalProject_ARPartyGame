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

    private bool changeSpawnPoint = false;

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
                if (changeSpawnPoint)
                {
                    spawnPoints[i] = SpawnPointsScript.CreateNewSpawnPoint();
                    changeSpawnPoint = false;
                }
                else
                {
                    changeSpawnPoint = true;
                }
            }

        }

        if (!bossSpawned)
        {
            StartCoroutine(StartSpawning());
        }

    }

    // private IEnumerator MixSpawnPoints(){
    //     yield return new WaitForSeconds(nextTimeToSpawn * 2);

    //     float x = 0, y = 0, z = 0;
    //     if (spawnPoints != null)
    //     {
    //         for (int i = 0; i < numOfSpawnPoints; i++)
    //         {
    //             float val = Random.Range(0, 2);
    //             x = val < 1 ? -spawnPoints[i].position.x : spawnPoints[i].position.x;
    //             val = Random.Range(0, 2);
    //             y = val < 1 ? -spawnPoints[i].position.y : spawnPoints[i].position.y;
    //             val = Random.Range(0, 2);
    //             z = val < 1 ? -spawnPoints[i].position.z : spawnPoints[i].position.z;

    //             spawnPoints[i].position = new Vector3(x, y, z);
    //         }
    //     }

    //     StartCoroutine(MixSpawnPoints());
    // }

    // public Transform CreateNewSpawnPoint()
    // {
    //     float x = 0, y = 0, z = 0;

    //     float val = Random.Range(0, 2);
    //     x = val < 1 ? Random.Range(-2, -6) : Random.Range(2, 6);
    //     val = Random.Range(0, 2);
    //     y = val < 1 ? Random.Range(-2, -6) : Random.Range(2, 6);
    //     val = Random.Range(0, 2);
    //     z = val < 1 ? Random.Range(-2, -6) : Random.Range(2, 6);

    //     Transform newSpawnPoint = new GameObject().transform;
    //     newSpawnPoint.position = new Vector3(x, y, z);
    //     newSpawnPoint.parent = transform;
    //     return newSpawnPoint;
    // }

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