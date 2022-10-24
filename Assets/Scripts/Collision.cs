using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{

    [Header("Layers")]
    public LayerMask GroundLayer;

    [Space]

    public bool OnGround;
    public bool OnWall;
    public bool OnRightWall;
    public bool OnLeftWall;
    public int WallSide;

    [Space]

    [Header("Collision")]

    public float CollisionRadius = 0.25f;
    public Vector2 BottomOffset, RightOffset, LeftOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {  
        OnGround = Physics2D.OverlapCircle((Vector2)transform.position + BottomOffset, CollisionRadius, GroundLayer);
        OnRightWall = Physics2D.OverlapCircle((Vector2)transform.position + RightOffset, CollisionRadius, GroundLayer);
        OnLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + LeftOffset, CollisionRadius, GroundLayer);
        OnWall = OnRightWall || OnLeftWall;
        WallSide = OnRightWall ? -1 : 1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position  + BottomOffset, CollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + RightOffset, CollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + LeftOffset, CollisionRadius);
    }
}
