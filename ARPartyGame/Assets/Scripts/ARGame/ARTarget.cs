using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARTarget : MonoBehaviour
{
    private int health = 100;

    public bool OnHit(int damage)
    {
        FindObjectOfType<AudioManager>().Play("FireHit");
        health -= damage;
        return health <= 0;
    }

    public int GetHealth()
    {
        return health;
    }
}
