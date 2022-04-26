using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
