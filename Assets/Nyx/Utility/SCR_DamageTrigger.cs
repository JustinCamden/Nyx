using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_DamageTrigger : MonoBehaviour
{
    [SerializeField, Tooltip("The team agent of this character")]
    private SCR_TeamAgent owningTeamAgent;

    [Tooltip("The amount of damage to apply on hit.")]
    public float damagePerHit = 35.0f;

    public delegate void OnHitDelegate(SCR_Health HitHeath);
    public OnHitDelegate OnHit;

    private void Awake()
    {
        if (!owningTeamAgent)
        {
            owningTeamAgent = GetComponent<SCR_TeamAgent>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (damagePerHit > 0.0f)
        {
            SCR_Health otherHealth = other.gameObject.GetComponent<SCR_Health>();
            if (otherHealth)
            {
                if (owningTeamAgent)
                {
                    SCR_TeamAgent otherTeamAgent = other.gameObject.GetComponent<SCR_TeamAgent>();
                    if (!otherTeamAgent || otherTeamAgent.AgentTeam != owningTeamAgent.AgentTeam)
                    {
                        otherHealth.TakeDamage(damagePerHit);
                        OnHit(otherHealth);
                    }
                }
                else
                {
                    otherHealth.TakeDamage(damagePerHit);
                    OnHit(otherHealth);
                }
            }
        }
    }
}
