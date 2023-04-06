using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 50f;

    float verticalInput;
    private bool isMoving = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        // Check if the car is moving
        if (rb.velocity.magnitude > 0.1f)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        // Allow the car to turn only when it's moving
        if (isMoving)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            transform.Rotate(0f, horizontalInput * turnSpeed * Time.deltaTime, 0f);
        }
    }

    void FixedUpdate()
    {
        // Move the car forward
        Vector3 forward = -transform.forward * verticalInput * moveSpeed * Time.fixedDeltaTime;
        rb.velocity = forward * moveSpeed;
    }
}
