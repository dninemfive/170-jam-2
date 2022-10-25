using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Movement : MonoBehaviour
{
    private Collision Collision;
    [HideInInspector]
    public Rigidbody2D RigidBody;
    private AnimationScript Animation;

    [Space]
    [Header("Stats")]
    public float Speed = 10;
    public float JumpForce = 50;
    public float SlideSpeed = 5;
    public float WallJumpLerp = 10;
    public float DashSpeed = 20;

    [Space]
    [Header("Booleans")]
    public bool CanMove;
    public bool WallGrab;
    public bool WallJumped;
    public bool WallSlide;
    public bool IsDashing;

    [Space]

    private bool TouchingGround;
    private bool HasDashed;

    public Side Side = Side.None;

    [Space]
    [Header("Polish")]
    public ParticleSystem DashParticle;
    public ParticleSystem JumpParticle;
    public ParticleSystem WallJumpParticle;
    public ParticleSystem SlideParticle;

    [HideInInspector]
    public List<Control> Controls { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        Collision = GetComponent<Collision>();
        RigidBody = GetComponent<Rigidbody2D>();
        Animation = GetComponentInChildren<AnimationScript>();
        Controls = new()
        {
            new("Fire3", InputCheckType.GetButton, TryGrabWall),
            new("Fire3", InputCheckType.GetButtonUp, ReleaseWall),
            new("Jump", InputCheckType.GetButtonDown, Jump),
            new("Fire1", InputCheckType.GetButtonDown, TryDash)
        };
    }
    public void TryGrabWall()
    {
        if (!CanMove || !Collision.OnWall) return;
        Side = Collision.WallSide;
        WallGrab = true;
        WallSlide = false;
    }
    public void ReleaseWall()
    {
        WallGrab = false;
        WallSlide = false;
    }
    public void Jump()
    {
        Animation.SetTrigger("jump");
        if (Collision.OnGround) Jump(Vector2.up);
        if (Collision.OnWall && !Collision.OnGround) WallJump();
    }
    public void TryDash()
    {
        if (HasDashed) return;
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        if (xRaw != 0 || yRaw != 0) Dash(xRaw, yRaw);
    }
    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        
        Vector2 dir = new(x, y);

        Walk(dir);
        Animation.SetHorizontalMovement(x, y, RigidBody.velocity.y);

        foreach (Control control in Controls) control.TryHandle();
        if (!Collision.OnWall || !CanMove) ReleaseWall();
        if (Collision.OnGround && !IsDashing)
        {
            WallJumped = false;
            GetComponent<BetterJumping>().enabled = true;
        }        
        if (WallGrab && !IsDashing)
        {
            RigidBody.gravityScale = 0;
            if(x > .2f || x < -.2f)
            RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            RigidBody.velocity = new(RigidBody.velocity.x, y * (Speed * speedModifier));
        }
        else
        {
            RigidBody.gravityScale = 3;
        }

        if(Collision.OnWall && !Collision.OnGround)
        {
            if (x != 0 && !WallGrab) DoWallSlide();
        } 
        else
        {
            WallSlide = false;
        }      

        if(Collision.OnGround != TouchingGround)
        {
            if (!TouchingGround) TouchGround();
            TouchingGround = !TouchingGround;
        }

        WallParticle(y);

        if (WallGrab || WallSlide || !CanMove) return;
        if (x is not 0) Side = x.ToSide();
        Animation.Face(Side);
    }
    void TouchGround()
    {
        HasDashed = false;
        IsDashing = false;
        Side = Animation.SpriteRenderer.flipX.ToSide();
        JumpParticle.Play();
    }

    private void Dash(float x, float y)
    {
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

        HasDashed = true;

        Animation.SetTrigger("dash");

        RigidBody.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        RigidBody.velocity += dir.normalized * DashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        FindObjectOfType<GhostTrail>().ShowGhost();
        StartCoroutine(GroundDash());
        DOVirtual.Float(14, 0, .8f, (x) => RigidBody.drag = x);

        DashParticle.Play();
        RigidBody.gravityScale = 0;
        GetComponent<BetterJumping>().enabled = false;
        WallJumped = true;
        IsDashing = true;

        yield return new WaitForSeconds(.3f);

        DashParticle.Stop();
        RigidBody.gravityScale = 3;
        GetComponent<BetterJumping>().enabled = true;
        WallJumped = false;
        IsDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (Collision.OnGround)
            HasDashed = false;
    }
    private void WallJump()
    {
        Side = Collision.WallSide;
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = Collision.OnRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        WallJumped = true;
    }

    private void DoWallSlide()
    {
        WallSlide = true;
        Side = Collision.WallSide;
        if (!CanMove) return;
        bool pushingWall = (RigidBody.velocity.x > 0 && Collision.OnRightWall) || (RigidBody.velocity.x < 0 && Collision.OnLeftWall);
        float push = pushingWall ? 0 : RigidBody.velocity.x;

        RigidBody.velocity = new(push, -SlideSpeed);
    }

    private void Walk(Vector2 dir)
    {
        if (!CanMove || WallGrab) return;
        if (!WallJumped)
        {
            RigidBody.velocity = new(dir.x * Speed, RigidBody.velocity.y);
        }
        else
        {
            RigidBody.velocity = Vector2.Lerp(RigidBody.velocity, (new Vector2(dir.x * Speed, RigidBody.velocity.y)), WallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump(Vector2 scaledDirection, bool wall = false)
    {
        SlideParticle.transform.parent.localScale = new(ParticleSide, 1, 1);
        ParticleSystem particle = wall ? WallJumpParticle : JumpParticle;

        RigidBody.velocity = new(RigidBody.velocity.x, 0);
        RigidBody.velocity += scaledDirection * JumpForce;

        particle.Play();
    }
    IEnumerator DisableMovement(float time)
    {
        CanMove = false;
        yield return new WaitForSeconds(time);
        CanMove = true;
    }
    void WallParticle(float vertical)
    {
        if (WallSlide || (WallGrab && vertical < 0))
        {
            SlideParticle.transform.parent.localScale = new(ParticleSide, 1, 1);
        }
    }
    int ParticleSide => (int)Collision.WallSide;
}
