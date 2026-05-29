using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player1 : MonoBehaviour
{
    public float racketSpeed;

    private Rigidbody2D rb;
    private Vector2 racketDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float directionY = 0f;

        if (Keyboard.current.wKey.isPressed)
        {
            directionY = 1f;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            directionY = -1f;
        }

        racketDirection = new Vector2(0, directionY).normalized;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = racketDirection * racketSpeed;
    }
}