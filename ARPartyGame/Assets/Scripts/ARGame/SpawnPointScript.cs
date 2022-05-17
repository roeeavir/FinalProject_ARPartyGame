using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsScript : MonoBehaviour
{
    private const int MAX_RANGE = 6;
    private const int MIN_RANGE = 2;
    // Create a new spawn point in range
    public static Transform CreateNewSpawnPoint()
    {
        float x = 0, y = 0, z = 0;

        float val = Random.Range(0, 2); // Randomly choose between 0 and 1
        x = val < 1 ? Random.Range(-MAX_RANGE, -MIN_RANGE) : Random.Range(MIN_RANGE, MAX_RANGE); // Randomly choose between negative and positive range
        val = Random.Range(0, 2); // Randomly choose between 0 and 1
        y = val < 1 ? Random.Range(-MAX_RANGE, -MIN_RANGE) : Random.Range(MIN_RANGE, MAX_RANGE); // Randomly choose between negative and positive range
        val = Random.Range(0, 2); // Randomly choose between 0 and 1
        z = val < 1 ? Random.Range(-MAX_RANGE, -MIN_RANGE) : Random.Range(MIN_RANGE, MAX_RANGE); // Randomly choose between negative and positive range

        Transform newSpawnPoint = new GameObject().transform;
        newSpawnPoint.position = new Vector3(x, y, z); // Set the position of the new spawn point
        return newSpawnPoint;
    }
}