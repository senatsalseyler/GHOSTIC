using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public MovementJoystick movementJoystick;
    public float playerSpeed;
    private Rigidbody2D rb;
    private float levelWidth = 3.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        DontExitTheScreen();
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