using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARTarget : MonoBehaviour
{
    private const int MAX_HEALTH = 100;

    private Image healthBar;
    private int health = MAX_HEALTH;

    private void Start()
    {
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Image>();
    }

    // void Awake()
    // {
    //     healthBar.SetActive(true);
    // }

    public bool OnHit(int damage)
    {
        FindObjectOfType<AudioManager>().Play("FireHit");
        health -= damage;
        healthBar.GetComponent<Image>().fillAmount = health / MAX_HEALTH;
        return health <= 0;
    }

    public int GetHealth()
    {
        return health;
    }
}
