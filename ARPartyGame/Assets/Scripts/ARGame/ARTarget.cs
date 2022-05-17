using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A class to handle an enemy being hit and not killed
public class ARTarget : MonoBehaviour
{
    private int maxHealth = 100 * (GameMode.gameMode + 1); // Sets the max health of the enemy based on the game mode

    private Image healthBar;
    private int health;

    private void Start()
    {
        health = maxHealth;
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Image>();
    }

    // Subtract health from the target and check if it is dead while also updating the health bar
    public bool OnHit(int damage)
    {
        FindObjectOfType<AudioManager>().Play("FireHit");
        health -= damage;
        healthBar.GetComponent<Image>().fillAmount = (float) health / maxHealth;
        return health <= 0;
    }

    public int GetHealth()
    {
        return health;
    }
}
