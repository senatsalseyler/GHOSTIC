using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public MovementJoystick movementJoystick;
    public float playerSpeed;
    private Rigidbody2D rb;
    private float levelWidth = 3.5f;
    [SerializeField]
    private float _rotationSpeed;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        DontExitTheScreen();
        SetPlayerVelocity();
        RotateInDirectionOfInput();
    }

    private void SetPlayerVelocity()
    {
        if (movementJoystick.joystickVec.y != 0)
        {
            rb.velocity = new Vector2(movementJoystick.joystickVec.x * playerSpeed,
                movementJoystick.joystickVec.y * playerSpeed);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void RotateInDirectionOfInput()
    {
        if(movementJoystick.joystickVec != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, movementJoystick.joystickVec);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            rb.MoveRotation(rotation);
        }
    }

    private void DontExitTheScreen()
    {
        if (transform.position.x >= levelWidth)
        {
            transform.position = new Vector3(levelWidth, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= -levelWidth)
        {
            transform.position = new Vector3(-levelWidth, transform.position.y, transform.position.z);
        }
    }
}