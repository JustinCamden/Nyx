using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SCR_RobotCharacter))]
public class SCR_EnemyController_Standard : MonoBehaviour
{
    [SerializeField, Tooltip("The controlled robot character.")]
    private SCR_RobotCharacter controlledRobot;

    // Targets
    private SCR_PlayerController playerChar;
    private SCR_ControlTower controlTower;

    // AI variables
    private float moveDirection = 1.0f;
    [SerializeField, Tooltip("The attack range of the robot.")]
    private float attackRange = 1.0f;
    private bool victory = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!controlledRobot)
        {
            controlledRobot = GetComponent<SCR_RobotCharacter>();
        }
        playerChar = FindObjectOfType<SCR_PlayerController>();
        controlTower = FindObjectOfType<SCR_ControlTower>();
        if (controlTower.transform.position.x < transform.position.x)
        {
            moveDirection = -1.0f;
        }
    }

    private void FixedUpdate()
    {
        if (!victory)
        {
            RecalculateAI();
        }
    }

    void RecalculateAI()
    {
        // Attack the tower if nearby
        if (controlTower)
        {
            float distToTower = Vector3.Distance(transform.position, controlTower.transform.position);
            if (distToTower <= attackRange)
            {
                controlledRobot.moveInput = 0.0f;
                controlledRobot.Punch();

                return;
            }
        }

        // Attack the player if in the right direction and nearby
        if (playerChar)
        {
            Vector3 dirToPlayer = playerChar.transform.position - transform.position;
            if (Vector3.Dot(dirToPlayer, transform.forward) > 0.0f && dirToPlayer.magnitude <= attackRange)
            {
                controlledRobot.moveInput = 0.0f;
                controlledRobot.Punch();

                return;
            }
        }


        // Otherwise, continue moving
        controlledRobot.moveInput = moveDirection;
    }

    public void Victory()
    {
        victory = true;
        controlledRobot.OnVictory();
    }
}
