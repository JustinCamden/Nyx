using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Requires the character controller component
[RequireComponent(typeof(CharacterController))]

public class SCR_PlayerCharacter_Controller : MonoBehaviour
{
    // Components
    private CharacterController ownedCharacterController;

    // Public variables
    [SerializeField, Range(0.0f, 10.0f), Tooltip("How fast the character can move.")]
    private float movementSpeed = 6.0f;
    [SerializeField, Tooltip("The force of each jump.")]
    private float jumpForce = 8.0f;
    [SerializeField, Tooltip("The force of gravity.")]
    private float gravity = 9.8f;

    // Private variables
    // The direction of movement
    private Vector3 movementDirection;

    // Start is called before the first frame update
    void Start()
    {
        // Set component references
        ownedCharacterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if grounded
        if (ownedCharacterController.isGrounded)
        {
            // Calculate movement direction from axis
            movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            movementDirection = movementDirection.normalized * movementSpeed;

           // if (Input.GetButton("Jump"))
        }
    }
}
