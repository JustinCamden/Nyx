using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_TeamAgent : MonoBehaviour
{
    public enum Team
    {
        Neutral,
        Player,
        Enemy
    }

    [SerializeField, Tooltip("The team of the agent.")]
    private Team internalTeam = Team.Neutral;
    public Team AgentTeam
    {
        get
        {
            return internalTeam;
        }
    }
}
