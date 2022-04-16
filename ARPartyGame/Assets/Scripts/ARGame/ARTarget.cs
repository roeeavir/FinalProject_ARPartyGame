using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARTarget : MonoBehaviour
{
    private int health = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool OnHit(int damage)
    {
        health -= damage;
        return health <= 0;
    }

    public int GetHealth()
    {
        return health;
    }
}
