using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;
    bool isMoving = false;
    bool busy = false;

    [Header("Jumping")]
    public float jumpPower = 65f;
    public int maxJumps = 2;
    private int jumpsLeft;

    [Header("Dashing")]
    public float dashPower = 50f;
    public int maxDashes = 1;
    public float dashTime = 0.5f;
    public float dashCooldown = 0.2f;
    private int dashesLeft;
    private bool isDashing = false;
    private bool canDash = true;

    [Header("Ground Check")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 15f;
    public float fallSpeedMultiplier = 2f;

    Vector2 faceDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        faceDirection = Vector2.right;
    }

    // Update is called once per frame
    void Update()
    {
        if (busy) { return; }
        


        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        
        if (rb.linearVelocity.magnitude > 0 )
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        GroundCheck();
    }

    /*public void Move(InputAction.CallbackContext context)
    {

        horizontalMovement = context.ReadValue<Vector2>().x;
        
        if (horizontalMovement < 0)
        {
            faceDirection = Vector2.left;
        }
        else if (horizontalMovement > 0)
        {
            faceDirection = Vector2.right;
        }
    }*/

    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
    }

    /*public void Jump(InputAction.CallbackContext context)
    {
        // Jumps left is restored by GroundCheck called in Update loop
        // Checks to see if the player can jump because they are grounded or still have a double jump left
        if (jumpsLeft > 0 && !busy)
        {
            // Removes Double Jump if the backpack is out in the scene
            if (!GetComponent<BackPackManager>().GetBackpackOut())
            {
                jumpsLeft = 1;
            }
            // Hold to do a big jump
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsLeft--;
            }

            // Tap to do a little jump
            else if (context.canceled)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocityY * 0.5f);
                jumpsLeft--;
            }
        }          
    }*/

    /*public void Dash(InputAction.CallbackContext context)
    {
        Debug.Log("Q");

        if (GetComponent<BackPackManager>().GetBackpackOut() && dashesLeft > 0 && !busy && canDash)
        {
            Debug.Log("Dash");

            dashesLeft--;
            isDashing = true;
            canDash = false;
            float gravity = baseGravity;
            //baseGravity = 0f;
            //rb.linearVelocityX = rb.linearVelocity.x + dashPower; rb.linearVelocityY = 0;
            //isDashing = false;
            //baseGravity = gravity;
            //canDash = true;
        }
    }*/

    private bool GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpsLeft = maxJumps;
            dashesLeft = maxDashes;
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(groundCheckPos.position, groundCheckSize);
    }

    public void SetBusy(bool val)
    {
        busy = val;
    }

    public bool GetBusy()
    {
        return busy;
    }

    public bool GetGrounded()
    {
        return GroundCheck();
    }

    public bool GetIsMoving()
    {
        return isMoving;
    }

    public Vector2 GetDirection()
    {
        return faceDirection;
    }
}
