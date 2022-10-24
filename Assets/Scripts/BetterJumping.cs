using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJumping : MonoBehaviour
{
    private Rigidbody2D RigidBody;
    public float FallMultiplier = 2.5f;
    public float LowJumpMultiplier = 2f;

    void Start()
    {
        Debug.Log(Physics2D.gravity);
        RigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(RigidBody.velocity.y < 0)
        {
            RigidBody.velocity += Vector2.up * Physics2D.gravity.y * (FallMultiplier - 1) * Time.deltaTime;
        }
        else if(RigidBody.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            RigidBody.velocity += Vector2.up * Physics2D.gravity.y * (LowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
