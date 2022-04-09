using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    private Transform[] spawnPoints;
    public GameObject[] ballons;

    private int numOfSpawnPoints = 3;

    private float nextTimeToSpawn = 2.5f;

    private int groupId = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartSpawning());
        StartCoroutine(MixSpawnPoints());
    }

    private IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(nextTimeToSpawn);

        if (spawnPoints != null)
        {
            for (int i = 0; i < numOfSpawnPoints; i++)
            {
                if (groupId != 0)
                {
                    ballons[i].GetComponent<BalloonScript>().groupId = groupId;
                }
                Instantiate(ballons[i], spawnPoints[i].position, Quaternion.identity);
            }
        }

        StartCoroutine(StartSpawning());
    }

    private IEnumerator MixSpawnPoints(){
        yield return new WaitForSeconds(nextTimeToSpawn * 4);

        float x = 0, y = 0, z = 0;
        if (spawnPoints != null)
        {
            for (int i = 0; i < numOfSpawnPoints; i++)
            {
                float val = Random.Range(0, 2);
                x = val < 1 ? -spawnPoints[i].position.x : spawnPoints[i].position.x;
                val = Random.Range(0, 2);
                y = val < 1 ? -spawnPoints[i].position.y : spawnPoints[i].position.y;
                val = Random.Range(0, 2);
                z = val < 1 ? -spawnPoints[i].position.z : spawnPoints[i].position.z;

                spawnPoints[i].position = new Vector3(x, y, z);
            }
        }

        StartCoroutine(MixSpawnPoints());
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
