using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is used to handle a player being hit by a another player
public class Target : MonoBehaviour
{
    public float health = 100f;

    private PlayerUI playerUI = null;

    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        playerUI = GameObject.FindObjectOfType<PlayerUI>();
    }

    // Subtract health from the target and check if it is dead
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (playerUI != null)
        {
            playerUI.UpdateHealth(health);
        }
        Debug.LogWarning("Health of " + gameObject.name + " is " + health);
        if (health <= 0f)
            Die();
    }

    private void Die()
    {
        Debug.LogWarning(gameObject.name + " has died");
        isDead = true;
    }

    public bool IsAlive()
    {
        return !isDead;
    }
}
