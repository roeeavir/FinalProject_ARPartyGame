using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void TakeDamage(float amount)
    {
        // FindObjectOfType<SoundController>().Play("Hit");
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
