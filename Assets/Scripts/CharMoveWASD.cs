using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementWASD : MonoBehaviour
{
    public float playerSpeed;
    private Rigidbody2D rb;
    private float levelWidth = 3.5f;
    [SerializeField]
    private float _rotationSpeed;
    private Vector2 inputVector;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GetInput();
    }

    void FixedUpdate()
    {
        DontExitTheScreen();
        SetPlayerVelocity();
        RotateInDirectionOfInput();
    }

    private void GetInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D keys
        float vertical = Input.GetAxisRaw("Vertical");     // W/S keys
        inputVector = new Vector2(horizontal, vertical).normalized;
    }

    private void SetPlayerVelocity()
    {
        if (inputVector.y != 0 || inputVector.x != 0)
        {
            rb.linearVelocity = new Vector2(inputVector.x * playerSpeed, inputVector.y * playerSpeed);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void RotateInDirectionOfInput()
    {
        if(inputVector != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, inputVector);
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