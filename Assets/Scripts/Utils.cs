using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum Side { Left = -1, None = 0, Right = 1 }
public enum InputCheckType { GetButtonDown, GetButtonUp, GetButton }
public delegate void HandleInput();
public delegate bool CheckButtonStatus(string buttonName);
public class Control
{
    public string InputName { get; private set; }
    public InputCheckType InputCheckType { get; private set; }
    public HandleInput HandleInput { get; private set; }
    public Control(string inputName, InputCheckType status, HandleInput handler)
    {
        InputName = inputName;
        InputCheckType = status;
        HandleInput = handler;
    }
    public void TryHandle()
    {
        if (InputCheckType.ToMethod()(InputName)) HandleInput();
    }
}
public static class Utils
{
    public static Side Opposite(this Side side) => side switch
    {
        Side.Left => Side.Right,
        Side.Right => Side.Left,
        _ => Side.None
    };
    /// <summary>
    /// Returns the <see cref="Side"/> value corresponding to whether the sprite is flipped.
    /// </summary>
    /// <param name="flipX">A boolean value equivalent to <see cref="SpriteRenderer.flipX"/>.</param>
    /// <returns><see cref="Side.Left">Left</see> if <c>flipX</c> is <c>true</c> or <see cref="Side.Right">Right</see> otherwise.</returns>
    public static Side ToSide(this bool flipX) => flipX ? Side.Left : Side.Right;
    /// <summary>
    /// Returns the <see cref="Side"/> value corresponding to whether the x component of a vector is to the right or left of the player.
    /// </summary>
    /// <param name="xComponent">The x component of the vector.</param>
    /// <param name="current">The default <see cref="Side"/> value, in case the component is 0.</param>
    /// <returns><see cref="Side.Right">Right</see> if <c>xComponent > 0</c>, <see cref="Side.Left">Left</see> if <c>xComponent < 0</c>, or <c>current</c> otherwise.</returns>
    public static Side ToSide(this float xComponent, Side current = Side.None) => xComponent switch
    {
        > 0 => Side.Right,
        < 0 => Side.Left,
        _ => current
    };
    public static Side ToSide(this Vector2 vec) => vec.x.ToSide();
    public static CheckButtonStatus ToMethod(this InputCheckType status) => status switch
    {
        InputCheckType.GetButtonDown => Input.GetButtonDown,
        InputCheckType.GetButton => Input.GetButton,
        InputCheckType.GetButtonUp => Input.GetButtonUp,
        _ => delegate(string s)
        {
            Debug.LogError($"Can't check if {s} is being pressed because {status} does not correspond to a valid Input method.");
            return false;
        }
    };
}
