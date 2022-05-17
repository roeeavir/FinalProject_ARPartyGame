using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Text health = null;

    private string maxHealth = " / 100";

    // Update the health text to the player's health
    public void UpdateHealth(float newHealth)
    {
        health.text = newHealth + maxHealth;
    }
}
