using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Requires the character controller component
[RequireComponent(typeof(SCR_RobotCharacter))]

public class SCR_PlayerController : MonoBehaviour
{
    // Components
    [SerializeField, Tooltip("The robot character script used by the player.")]
    private SCR_RobotCharacter controlledRobot;
    [SerializeField, Tooltip("The player character input manager.")]
    private SCR_PlayerActions PlayerActions;
    [SerializeField, Tooltip("Text used to display health.")]
    private Text healthText;
    [SerializeField, Tooltip("Health used by the controlled robot")]
    private SCR_Health controlledHealth;
    [SerializeField, Tooltip("Health Regen Rate per second")]
    private float healthRegenRate = 1.0f;
    [SerializeField, Tooltip("Game manager reference for calling the death function.")]
    private SCR_GameManager gameManager;

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
        if (!controlledHealth)
        {
            controlledHealth = GetComponent<SCR_Health>();
        }
        if (controlledHealth)
        {
            controlledHealth.onHit += OnHit;
            controlledHealth.onDeath += OnDeath;
        }


        return;
    }

    private void Update()
    {
        // Update input on the robot
        controlledRobot.moveInput = PlayerActions.move.AxisValue;
        if (PlayerActions.jump.WasPressed)
        {
            controlledRobot.Jump();
        }
        if (PlayerActions.punch.WasPressed)
        {
            controlledRobot.Punch();
        }
        if (PlayerActions.rocketPunch.WasPressed)
        {
            controlledRobot.RocketPunch();
        }
        if (PlayerActions.block.WasPressed)
        {
            controlledRobot.Block();
        }
        else if (PlayerActions.block.WasReleased)
        {
            controlledRobot.EndBlock();
        }
        if (!controlledHealth.IsDead)
        {
            controlledHealth.HealDamage(healthRegenRate * Time.deltaTime);
            int healthPercent = (int)(100.0f * (controlledHealth.Health / controlledHealth.MaxHealth));
            string newHealthText = "Health: ";
            newHealthText += healthPercent.ToString();
            healthText.text = newHealthText;
        }
        else
        {
            healthText.text = "Dead";
        }
    }

    public void OnHit()
    {

    }

    public void OnDeath()
    {
        if (gameManager)
        {
            gameManager.PlayerLose();
        }
    }
}
