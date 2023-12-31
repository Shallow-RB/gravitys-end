using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeathDisplay : MonoBehaviour
{
    [SerializeField]
    private Image healthBarSprite;

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        healthBarSprite.fillAmount = currentHealth / maxHealth; 
    }
}
