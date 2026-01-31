using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 8f;
    private Rigidbody2D rb;
    private Vector2 moveValue; // Now stores BOTH X (left/right) and Y (up/down)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // IMPORTANT: Set gravity to 0 so your player doesn't slide down the screen
        rb.gravityScale = 0;
    }

    // WHAT CALLS THIS? 
    // The "Player Input" component on your GameObject calls this 
    // whenever you press a key defined in your Actions file.
    void OnMove(InputValue value)
    {
        // This grabs the (X, Y) from your keyboard/joystick automatically
        moveValue = value.Get<Vector2>();
    }

    void Update()
    {
        // FIX: We apply moveValue.x to the X, and moveValue.y to the Y.
        // DO NOT multiply the existing velocity by speed again in the Y, 
        // or you will accelerate to infinity!
        rb.linearVelocity = new Vector2(moveValue.x * speed, moveValue.y * speed);

        // Debug.Log needs a capital 'L'!
        Debug.Log("Current Velocity: " + rb.linearVelocity);
    }
}
