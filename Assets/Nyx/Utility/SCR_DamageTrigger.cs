using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SCR_DamageTrigger : MonoBehaviour
{
    [SerializeField, Tooltip("The team agent of this character")]
    private SCR_TeamAgent owningTeamAgent;

    [Tooltip("The amount of damage to apply on hit.")]
    public float damagePerHit = 35.0f;

    public delegate void OnHitDelegate(SCR_Health otherHealth);
    public OnHitDelegate onHit;

    [SerializeField, Tooltip("The collider for the damage trigger.")]
    private BoxCollider damageCollider;

    [SerializeField, Tooltip("Whether the trigger should automatically deactivate upon hitting an enemy.")]
    private bool deactivateOnHit = false;

    [SerializeField, Tooltip("Whether this damage trigger can be blocked")]
    private bool canBeBlocked = true;

    // Whether the trigger is currently active
    private bool internalActive = false;
    public bool Active
    {
        get
        {
            return internalActive;
        }
    }

    public SCR_TeamAgent.Team DamageTriggerTeam
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

    private void Start()
    {
        if (!damageCollider)
        {
            damageCollider = GetComponent<BoxCollider>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (internalActive)
        {
            CheckImpactObject(other);
        }

        return;
    }

    public void ActivateDamageTrigger()
    {
        if (!internalActive)
        {
            internalActive = true;
            Collider[] overlappingColliders = Physics.OverlapBox(damageCollider.transform.position, damageCollider.size / 2.0f);
            foreach (Collider other in overlappingColliders)
            {
                if (!internalActive)
                {
                    break;
                }
                CheckImpactObject(other);
            }
        }


        return;
    }

    public void DeactivateDamageTrigger()
    {
        if (internalActive)
        {
            internalActive = false;
        }
        return;
    }

    private void CheckImpactObject(Collider other)
    {
        if (damagePerHit > 0.0f)
        {
            SCR_Health otherHealth = other.gameObject.GetComponent<SCR_Health>();
            if (otherHealth)
            {
                if (DamageTriggerTeam != otherHealth.HealthTeam)
                {
                    otherHealth.TakeDamage(damagePerHit, transform.position, canBeBlocked);
                    if (onHit != null)
                    {
                        onHit(otherHealth);
                    }
                    if (deactivateOnHit)
                    {
                        DeactivateDamageTrigger();
                    }
                }
            }
        }

        return;
    }
}
