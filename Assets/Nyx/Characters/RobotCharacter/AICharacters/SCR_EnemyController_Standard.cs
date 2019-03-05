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

    // AI variables
    private float moveDirection = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (!controlledRobot)
        {
            controlledRobot = GetComponent<SCR_RobotCharacter>();
        }
        playerChar = FindObjectOfType<SCR_PlayerController>();
        if (playerChar.transform.position.x < transform.position.x)
        {
            moveDirection = -1.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
      //  controlledRobot.moveInput = moveDirection;
    }

    private void FixedUpdate()
    {
        RecalculateAI();
    }

    void RecalculateAI()
    {
        if (playerChar.transform.position.x < transform.position.x)
        {
            moveDirection = -1.0f;
        }
        else
        {
            moveDirection = 1.0f;
        }
    }
}
