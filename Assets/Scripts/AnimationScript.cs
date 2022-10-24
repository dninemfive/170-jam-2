using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{

    private Animator Animator;
    private Movement Movement;
    private Collision Collision;
    [HideInInspector]
    public SpriteRenderer SpriteRenderer;

    void Start()
    {
        Animator = GetComponent<Animator>();
        Collision = GetComponentInParent<Collision>();
        Movement = GetComponentInParent<Movement>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Animator.SetBool("onGround", Collision.OnGround);
        Animator.SetBool("onWall", Collision.OnWall);
        Animator.SetBool("onRightWall", Collision.OnRightWall);
        Animator.SetBool("wallGrab", Movement.WallGrab);
        Animator.SetBool("wallSlide", Movement.WallSlide);
        Animator.SetBool("canMove", Movement.CanMove);
        Animator.SetBool("isDashing", Movement.IsDashing);

    }

    public void SetHorizontalMovement(float x,float y, float yVel)
    {
        Animator.SetFloat("HorizontalAxis", x);
        Animator.SetFloat("VerticalAxis", y);
        Animator.SetFloat("VerticalVelocity", yVel);
    }

    public void SetTrigger(string trigger)
    {
        Animator.SetTrigger(trigger);
    }

    public void Flip(int side)
    {

        if (Movement.WallGrab || Movement.WallSlide)
        {
            if (side == -1 && SpriteRenderer.flipX)
                return;

            if (side == 1 && !SpriteRenderer.flipX)
            {
                return;
            }
        }

        bool state = side == 1;
        SpriteRenderer.flipX = state;
    }
}
