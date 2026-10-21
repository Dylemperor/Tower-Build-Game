using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private Rigidbody2D rb;

    // Only contacts on surfaces facing mostly upward count as ground.
    private const float MinGroundNormalAngle = 45f;
    private const float MaxGroundNormalAngle = 135f;

    private ContactFilter2D groundFilter;
    private float horizontalInput;

    private bool IsGrounded => rb.IsTouching(groundFilter);

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        groundFilter.SetNormalAngle(MinGroundNormalAngle, MaxGroundNormalAngle);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    // Called by the PlayerInput component's "Move" event.
    public void Move(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;
    }

    // Called by the PlayerInput component's "Jump" event.
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
