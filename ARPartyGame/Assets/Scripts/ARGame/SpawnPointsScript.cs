using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsScript : MonoBehaviour
{
    
    public static Transform CreateNewSpawnPoint(){
        float x = 0, y = 0, z = 0;

        float val = Random.Range(0, 2);
        x = val < 1 ? Random.Range(-2, -6) : Random.Range(2, 6);
        val = Random.Range(0, 2);
        y = val < 1 ? Random.Range(-2, -6) : Random.Range(2, 6);
        val = Random.Range(0, 2);
        z = val < 1 ? Random.Range(-2, -6) : Random.Range(2, 6);

        Transform newSpawnPoint = new GameObject().transform;
        newSpawnPoint.position = new Vector3(x, y, z);
        // newSpawnPoint.parent = transform;
        return newSpawnPoint;
    }
}
