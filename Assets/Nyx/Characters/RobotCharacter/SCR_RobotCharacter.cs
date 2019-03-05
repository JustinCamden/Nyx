using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SCR_TeamAgent))]
[RequireComponent(typeof(SCR_Health))]

public class SCR_RobotCharacter : MonoBehaviour
{
    // Components
    [SerializeField, Tooltip("The character controller of the robot.")]
    private CharacterController ownedCharacterController;
    [SerializeField, Tooltip("The team agent of the robot.")]
    private SCR_TeamAgent ownedTeamAgent;
    [SerializeField, Tooltip("The health component of the robot.")]
    private SCR_Health ownedHealth;

    // Movement variables
    [SerializeField, Tooltip("The movement speed of the robot.")]
    private float moveSpeed = 5.0f;
    [SerializeField, Tooltip("The acceleration curve of the robot.")]
    private AnimationCurve moveAccelerationCurve;
    [SerializeField, Tooltip("How long it takes to reach the maximum acceleration.")]
    private float moveAccelerationPeriod = 0.25f;
    [SerializeField, Tooltip("How long it takes to reach the maximum acceleration.")]
    private float moveDecelerationPeriod = 0.1f;
    [SerializeField, Tooltip("The rotation speed of the robot.")]
    private float rotationSpeed = 5.0f;
    [SerializeField, Tooltip("The force of each jump.")]
    private float jumpForce = 8.0f;
    [SerializeField, Tooltip("The force of gravity.")]
    private float gravity = 9.8f;
    [SerializeField, Tooltip("Number to multiply gravity by when falling")]
    private float gravityFallingMultiplier = 2.0f;

    // Movement caches
    private Vector3 moveDirection;
    private Vector3 facingDirection;
    private float accelerationProgress;

    // Input variables
    public float moveInput;
    public bool jumpQueued;

    // Start is called before the first frame update
    void Start()
    {
        // Validate references
        if (!ownedCharacterController)
        {
            ownedCharacterController = GetComponent<CharacterController>();
        }
        if (!ownedTeamAgent)
        {
            ownedTeamAgent = GetComponent<SCR_TeamAgent>();
        }
        if (!ownedHealth)
        {
            ownedHealth = GetComponent<SCR_Health>();
        }

        // Set initial facing direction depending on the current rotation of the robot
        facingDirection.x = Mathf.Sign(Vector3.Dot(transform.forward, Vector3.right));
    }

    // Update is called once per frame
    void Update()
    {
        // Update the direction of movement
        // If the move input is not zero
        if (moveInput != 0.0f)
        {
            // Accelerate towards max acceleration
            if (accelerationProgress < 1.0f)
            {
                accelerationProgress = Mathf.Min(accelerationProgress + (Time.deltaTime * (1.0f / moveAccelerationPeriod)), 1.0f);
            }
            moveDirection.x = moveSpeed * moveInput;

            // Update facing direction
            facingDirection.x = Mathf.Sign(Vector3.Dot(moveDirection, Vector3.right));

            // Slightly bias facing direction so that rotation faces towards the camera
            // and we can see his beautiful face
            facingDirection.z = -0.01f;
        }

        // Otherwise decelerate
        else if (accelerationProgress > 0.0f)
        {
            accelerationProgress = Mathf.Max(accelerationProgress - (Time.deltaTime * (1.0f / moveDecelerationPeriod)), 0.0f);
            moveDirection.x = moveAccelerationCurve.Evaluate(accelerationProgress) * moveSpeed * moveInput;
        }

        // If we are grounded
        if (ownedCharacterController.isGrounded)
        {
            // Begin jump if queued
            if (jumpQueued)
            {
                moveDirection.y = jumpForce;
            }

            // Otherwise, keep the character on the ground
            else
            {
                moveDirection.y = -gravity;
            }
        }

        // Else, gradually apply gravity
        else
        {
            // Multiply gravity by falling for a better feeling arc
            moveDirection.y -= gravity * gravityFallingMultiplier * Time.deltaTime; 
        }

        // Apply movement
        ownedCharacterController.Move(moveDirection * Time.deltaTime);

        // Rotate towards the last facing direction
        if (transform.forward != facingDirection)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(facingDirection),
                Time.deltaTime * rotationSpeed);
        }
    }
}
