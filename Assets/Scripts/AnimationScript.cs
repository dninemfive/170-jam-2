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

    public void Face(Side side)
    {
        if (Movement.WallGrab || Movement.WallSlide) return;
        if (side is not Side.Left and not Side.Right) Debug.LogError($"Argument 'side' of AnimationScript.Face must be either {Side.Left} or {Side.Right}, not {side}.");
        SpriteRenderer.flipX = side == Side.Left;
    }
}
