using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Requires the character controller component
[RequireComponent(typeof(SCR_RobotCharacter))]

public class SCR_PlayerController : MonoBehaviour
{
    // Components
    [SerializeField, Tooltip("The robot character script used by the player.")]
    private SCR_RobotCharacter controlledRobot;
    [SerializeField, Tooltip("The player character input manager.")]
    private SCR_PlayerActions PlayerActions;

    private void Start()
    {
        // Validate components
        if (!controlledRobot)
        {
            controlledRobot = GetComponent<SCR_RobotCharacter>();
        }
        if (!PlayerActions)
        {
            PlayerActions = SCR_PlayerActions.Instance;
        }

        return;
    }

    private void Update()
    {
        // Update input on the robot
        controlledRobot.moveInput = PlayerActions.move.AxisValue;
        controlledRobot.jumpRequested = PlayerActions.jump.WasPressed;
    }
}
