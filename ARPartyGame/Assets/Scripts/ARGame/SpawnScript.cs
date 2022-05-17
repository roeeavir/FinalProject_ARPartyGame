using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is used to spawn the enemies
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
    }

    // Spawn enemies
    private IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(nextTimeToSpawn);

        if (spawnPoints != null && !bossSpawned)
        {
            for (int i = 0; i < numOfSpawnPoints; i++)
            {
                if (groupId >= 100) // If the group id is equal or greater than 100, spawn boss
                {
                    boss.GetComponent<EnemyScript>().groupId = groupId;
                    Instantiate(boss, spawnPoints[i].position, Quaternion.identity); // Spawn the boss
                    bossSpawned = true;
                    break;
                }
                if (groupId != 0)
                {
                    enemies[i].GetComponent<EnemyScript>().groupId = groupId; // Set the group id of the enemy
                }
                Instantiate(enemies[i], spawnPoints[i].position, Quaternion.identity);// Spawn enemy
                spawnPoints[i] = SpawnPointsScript.CreateNewSpawnPoint();
            }

        }

        if (!bossSpawned)
        {
            StartCoroutine(StartSpawning()); // Start spawning again (recursive)
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

    // Sets the next time to spawn enemies variable
    public void SetNextTimeToSpawn(float time)
    {
        nextTimeToSpawn = time;
    }

    public void SetGroupId(int gid)
    {
        groupId = gid;
    }

}