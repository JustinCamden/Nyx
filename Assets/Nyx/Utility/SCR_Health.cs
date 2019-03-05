using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Health : MonoBehaviour
{
    [SerializeField, Tooltip("The maximum health of this agent.")]
    private float maxHealth = 100.0f;

    [SerializeField, Tooltip("The current health of this agent.")]
    private float internalHealth = 100.0f;
    public float Health
    {
        get
        {
            return internalHealth;
        }
    }

    delegate void OnDeathDelegate();
    OnDeathDelegate OnDeath;

    // Whether this agent is currently dead
    private bool internalIsDead = false;
    public bool IsDead
    {
        get
        {
            return internalIsDead;
        }
    }

    public void TakeDamage(float damage)
    {
        if (!internalIsDead)
        {
            internalHealth -= damage;
            if (internalHealth <= 0.0f)
            {
                internalHealth = 0.0f;
                internalIsDead = true;
                OnDeath();
            }
        }
        return;
    }

    public void HealDamage(float healAmount)
    {
        if (!internalIsDead)
        {
            internalHealth = Mathf.Min(maxHealth, internalHealth + healAmount);
        }
        return;
    }

    public void Die()
    {
        if (!internalIsDead)
        {
            internalIsDead = true;
            internalHealth = 0.0f;
            OnDeath();
        }
    }
}
