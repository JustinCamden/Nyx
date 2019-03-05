using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Health : MonoBehaviour
{
    [SerializeField, Tooltip("The maximum health of this agent.")]
    private float internalMaxHealth = 100.0f;
    public float MaxHealth
    {
        get
        {
            return internalMaxHealth;
        }
    }

    [SerializeField, Tooltip("The current health of this agent.")]
    private float internalHealth = 100.0f;
    public float Health
    {
        get
        {
            return internalHealth;
        }
    }

    [SerializeField, Tooltip("The owning team agent of this health component")]
    private SCR_TeamAgent owningTeamAgent;

    private bool blocking = false;
    private Vector3 blockingDirection;

    private void Start()
    {
        if (!owningTeamAgent)
        {
            owningTeamAgent = GetComponent<SCR_TeamAgent>();
        }
        internalHealth = Mathf.Min(internalMaxHealth, internalHealth);
    }

    public SCR_TeamAgent.Team HealthTeam
    {
        get
        {
            if (owningTeamAgent)
            {
                return owningTeamAgent.AgentTeam;
            }
            else
            {
                return SCR_TeamAgent.Team.Neutral;
            }
        }
    }

    public delegate void OnDeathDelegate();
    public OnDeathDelegate onDeath;

    public delegate void OnHitDelegate();
    public OnHitDelegate onHit;

    // Whether this agent is currently dead
    private bool internalIsDead = false;
    public bool IsDead
    {
        get
        {
            return internalIsDead;
        }
    }

    public void TakeDamage(float damage, Vector3 damageOrigin, bool canBeBlocked)
    {
        if (!internalIsDead)
        {
            // If we are blocking this direction, do nothing
            if (canBeBlocked && blocking && Vector3.Dot(damageOrigin - transform.position, blockingDirection) > 0.0f)
            {
                return;
            }

            // Otherwise apply damage as normal
            internalHealth -= damage;
            if (internalHealth <= 0.0f)
            {
                internalHealth = 0.0f;
                internalIsDead = true;
                if (onDeath != null)
                {
                    onDeath();
                }
            }
            else if (onHit != null)
            {
                onHit();
            }
        }
        return;
    }

    public void HealDamage(float healAmount)
    {
        if (!internalIsDead)
        {
            internalHealth = Mathf.Min(internalMaxHealth, internalHealth + healAmount);
        }
        return;
    }

    public void Die()
    {
        if (!internalIsDead)
        {
            internalIsDead = true;
            internalHealth = 0.0f;
            onDeath();
        }
    }

    public void BlockDirection(Vector3 direction)
    {
        blockingDirection = direction;
        blocking = true;
    }

    public void StopBlockingDirection()
    {
        blocking = false;
    }
}
