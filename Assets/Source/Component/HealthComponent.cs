using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100;
    public float currentHealth;
    public float healthRegenRate = 1.0f;

    [Header("Mana")]
    public float maxMana = 100;
    public float currentMana;
    public float manaRegenRate = 1.0f;

    [Header("Movement")]
    public float moveSpeed;

    [Header("Ability")]
    public float attack = 10.0f;
    public float defense = 5.0f;


    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    public void TakeDamage(float damage)
    {
        float effectiveDamage = damage - defense;
        currentHealth -= effectiveDamage;
        Debug.Log("Remaining health: " + currentHealth.ToString("F1"));
         // 몬스터가 맞았다고 알림
    SendMessage("OnHit", SendMessageOptions.DontRequireReceiver);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
    Debug.Log("YOUDIE");
    SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
    Destroy(gameObject, 2f);
    }

    public void RegenerateResources()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += healthRegenRate * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }

        if (currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana);
        }
    }

    public bool UseMana(float amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            Debug.Log("Remaining mana: " + currentMana.ToString("F1"));
            return true;
        }
        Debug.Log("Not enough mana!");
        return false;
    }

    public bool UseHealth(float amount)
    {
        if (currentHealth >= amount)
        {
            currentHealth -= amount;
            Debug.Log("Remaining health: " + currentHealth.ToString("F1"));
            return true;
        }
        Debug.Log("Not enough health!");
        return false;
    }

    void Update()
    {
        RegenerateResources();
    }
}
