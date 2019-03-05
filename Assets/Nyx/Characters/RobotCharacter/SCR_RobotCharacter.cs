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
    private float accelerationProgress = 0.0f;

    // Input variables
    public float moveInput = 0.0f;
    private bool jumpQueued = false;
    private bool jumpStarting = false;
    [SerializeField, Tooltip("The limit to how far we can go left and right")]
    float mapLimits = 10.0f;
    private bool victory;


    // Damage triggers
    [SerializeField, Tooltip("The damage trigger for the normal punch.")]
    private SCR_DamageTrigger punchDamageTrigger;
    [SerializeField, Tooltip("The damage trigger for jumping on enemies.")]
    private SCR_DamageTrigger jumpDamageTrigger;
    [SerializeField, Tooltip("The damage trigger for rocket punching.")]
    private SCR_DamageTrigger rocketPunchDamageTrigger;
    [SerializeField, Tooltip("Mesh for visualizing the rocket punch.")]
    private MeshRenderer rocketPunchMesh;

    // Animation variables
    // Types of actions the robot can perform
    private enum RobotAction
    {
        Idle,
        Punching,
        RocketPunching,
        HitReacting,
    }
    // The current action state of the character
    private RobotAction actionState = RobotAction.Idle;
    // Types of movement stances the robot can take
    private enum RobotStance
    {
        Idle,
        Walking,
        InAir,
        Blocking
    }
    // The current movement stance of the character
    private RobotStance stanceState;
    [SerializeField, Tooltip("The animator for the robot character.")]
    private Animator ownedAnimator;
    [SerializeField, Tooltip("The animation event holder for the robot character.")]
    private SCR_RobotAnimEvents ownedAnimEvents;

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
        if (!ownedAnimator)
        {
            ownedAnimator = GetComponentInChildren<Animator>();
        }
        if (!ownedAnimEvents)
        {
            ownedAnimEvents = GetComponentInChildren<SCR_RobotAnimEvents>();
            if (ownedAnimEvents && ownedAnimator)
            {
                if (!ownedAnimEvents.owningRobot)
                {
                    ownedAnimEvents.owningRobot = this;
                }
                if (!ownedAnimEvents.robotAnimator)
                {
                    ownedAnimEvents.robotAnimator = ownedAnimator;
                }
            }
        }

        rocketPunchMesh.enabled = false;

        // Register delegates
        if (ownedHealth)
        {
            ownedHealth.onHit += OnHit;
            ownedHealth.onDeath += OnDeath;
        }

        // Set initial facing direction depending on the current rotation of the robot
        facingDirection.x = Mathf.Sign(Vector3.Dot(transform.forward, Vector3.right));

        // Slightly bias facing direction so that rotation faces towards the camera
        // and we can see his beautiful face
        facingDirection.z = -0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ownedHealth.IsDead)
        {
            UpdateCharacterController();
        }
    }

    void UpdateCharacterController()
    {
        // Update the direction of movement
        if (moveInput != 0.0f)
        {
            if ((moveInput < 0.0f && transform.position.x <= -mapLimits)
                ||(moveInput > 0.0f && transform.position.x >= mapLimits))
            {
                moveInput = 0.0f;
            }
        }

        if (moveInput != 0.0f && CanMove())
        {
            // Update movement state
            if (stanceState != RobotStance.InAir)
            {
                SetStanceState(RobotStance.Walking);
            }

            // Accelerate towards max acceleration
            if (accelerationProgress < 1.0f)
            {
                accelerationProgress = Mathf.Min(accelerationProgress + (Time.deltaTime * (1.0f / moveAccelerationPeriod)), 1.0f);
            }
            moveDirection.x = moveSpeed * moveInput;

            // Update facing direction
            facingDirection.x = Mathf.Sign(Vector3.Dot(moveDirection, Vector3.right));
        }

        // Otherwise decelerate
        else if (accelerationProgress > 0.0f)
        {
            accelerationProgress = Mathf.Max(accelerationProgress - (Time.deltaTime * (1.0f / moveDecelerationPeriod)), 0.0f);
            moveDirection.x = moveAccelerationCurve.Evaluate(accelerationProgress) * moveSpeed * moveInput;
            if (stanceState == RobotStance.Walking)
            {
                SetStanceState(RobotStance.Idle);
            }
        }

        // If we are grounded
        if (ownedCharacterController.isGrounded)
        {

            if (jumpStarting)
            {
                jumpStarting = false;
            }
            else
            {
                if (!jumpQueued && stanceState == RobotStance.InAir)
                {
                    SetStanceState(RobotStance.Idle);
                }
                if (moveDirection.y > 0.0f)
                {
                    Debug.Log(moveDirection.y);
                }

                jumpDamageTrigger.DeactivateDamageTrigger();
                moveDirection.y = -gravity;
            }
        }

        // Else, gradually apply gravity
        else
        {
            // Multiply gravity by falling for a better feeling arc
            moveDirection.y -= gravity * gravityFallingMultiplier * Time.deltaTime;
            if (moveDirection.y < 0.0f)
            {
                jumpDamageTrigger.ActivateDamageTrigger();
            }
        }

        // Apply movement
        Vector3 moveUpdate = moveDirection * Time.deltaTime;
        moveUpdate.z = transform.position.z * -1.0f;
        ownedCharacterController.Move(moveUpdate);

        // Rotate towards the last facing direction
        if (transform.forward != facingDirection)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(facingDirection),
                Time.deltaTime * rotationSpeed);
        }

        return;
    }

    private void SetActionState(RobotAction newAction)
    {
        // Reset old state
        if (newAction != actionState)
        {
            switch (actionState)
            {
                case RobotAction.HitReacting:
                    {
                        ownedAnimator.SetBool("HitReact", false);
                        break;
                    }
                case RobotAction.Punching:
                    {
                        ownedAnimator.SetBool("Punching", false);
                        break;
                    }
                case RobotAction.RocketPunching:
                    {
                        ownedAnimator.SetBool("RocketPunching", false);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            // Setup new state
            actionState = newAction;
            switch(actionState)
            {
                case RobotAction.HitReacting:
                    {
                        ownedAnimator.SetBool("HitReact", true);
                        break;
                    }
                case RobotAction.Punching:
                    {
                        ownedAnimator.SetBool("Punching", true);
                        break;
                    }
                case RobotAction.RocketPunching:
                    {
                        ownedAnimator.SetBool("RocketPunching", true);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        return;
    }

    private void SetStanceState(RobotStance newStance)
    {
        // Reset old state
        if (newStance != stanceState)
        {
            switch (stanceState)
            {
                case RobotStance.Blocking:
                    {
                        ownedAnimator.SetBool("Blocking", false);
                        break;
                    }
                case RobotStance.InAir:
                    {
                        ownedAnimator.SetBool("InAir", false);
                        break;
                    }
                case RobotStance.Walking:
                    {
                        ownedAnimator.SetBool("Walking", false);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            // Setup new state
            stanceState = newStance;
            switch (stanceState)
            {
                case RobotStance.Blocking:
                    {
                        ownedAnimator.SetBool("Blocking", true);
                        break;
                    }
                case RobotStance.InAir:
                    {
                        ownedAnimator.SetBool("InAir", true);
                        break;
                    }
                case RobotStance.Walking:
                    {
                        ownedAnimator.SetBool("Walking", true);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        return;
    }

    private bool CanEnterAction()
    {
        return (!ownedHealth.IsDead 
            && (stanceState == RobotStance.Walking || stanceState == RobotStance.Idle)
             && actionState == RobotAction.Idle
             && !victory);
    }

    private bool CanMove()
    {
        return (!ownedHealth.IsDead
            && stanceState != RobotStance.Blocking
            && actionState != RobotAction.RocketPunching
            && !victory);
    }

    private bool CanJump()
    {
        return actionState == RobotAction.Idle && ownedCharacterController.isGrounded;
    }

    public void OnPunchStart()
    {
        punchDamageTrigger.ActivateDamageTrigger();
        return;
    }

    public void OnPunchEnd()
    {
        if (actionState == RobotAction.Punching)
        {
            SetActionState(RobotAction.Idle);
        }
        punchDamageTrigger.DeactivateDamageTrigger();
        return;
    }

    public void OnRocketPunchStart()
    {
        rocketPunchDamageTrigger.ActivateDamageTrigger();
        rocketPunchMesh.enabled = true;
        return;
    }

    public void OnRocketPunchEnd()
    {
        if (actionState == RobotAction.RocketPunching)
        {
            SetActionState(RobotAction.Idle);
        }
        rocketPunchDamageTrigger.DeactivateDamageTrigger();
        rocketPunchMesh.enabled = false;
        return;
    }

    public void OnHitReactEnd()
    {
        if (actionState == RobotAction.HitReacting)
        {
            SetActionState(RobotAction.Idle);
        }

        return;
    }

    public void OnJumpStart()
    {
        if (jumpQueued)
        {
            moveDirection.y = jumpForce;
            jumpQueued = false;
            jumpStarting = true;
        }
    }

    public void Punch()
    {
        if (CanEnterAction())
        {
            SetActionState(RobotAction.Punching);
        }
    }

    public void RocketPunch()
    {
        if (CanEnterAction())
        {
            SetActionState(RobotAction.RocketPunching);
        }
    }

    public void Block()
    {
        if (CanEnterAction())
        {
            SetStanceState(RobotStance.Blocking);
            ownedHealth.BlockDirection(facingDirection);
        }

        return;
    }

    public void EndBlock()
    {
        if (stanceState == RobotStance.Blocking)
        {
            SetStanceState(RobotStance.Idle);
            ownedHealth.StopBlockingDirection();
        }

        return;
    }

    public void Jump()
    {
        // Begin jump if not already queued
        if (!jumpQueued && CanMove() && CanJump())
        {
            SetStanceState(RobotStance.InAir);
            jumpQueued = true;
        }
    }

    public void OnHit()
    {
        if (CanEnterAction())
        {
            SetActionState(RobotAction.HitReacting);
        }
    }

    public void OnDeath()
    {
        ownedAnimator.SetTrigger("Dead");
        ownedCharacterController.detectCollisions = false;
        ownedCharacterController.enabled = false;
        punchDamageTrigger.DeactivateDamageTrigger();
        jumpDamageTrigger.DeactivateDamageTrigger();
        rocketPunchDamageTrigger.DeactivateDamageTrigger();
        rocketPunchMesh.enabled = false;
        Destroy(gameObject, 1.0f);
    }

    public void OnVictory()
    {
        victory = true;
        ownedAnimator.SetBool("Victory", true);
    }

    // Used because character controller grounded is terrible
    private bool IsGrounded()
    {
        int layerMask = 1 << 9;
        layerMask = ~layerMask;
        RaycastHit hit;
        return Physics.Raycast(transform.position, Vector3.down, out hit, ownedCharacterController.height);
    }
}
