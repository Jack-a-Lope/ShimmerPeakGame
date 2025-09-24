using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;
using DigitalWorlds.StarterPackage2D;

public class BackPackManager : MonoBehaviour
{
    [Header("Backpack")]
    public GameObject backpack; 
    private GameObject curBackpack;
    public LayerMask player;

    [Header("BackPack Throw")]
    public float power = 10f;
    public float maxDistance = 2.0f;
    public float minDistance = 0.5f;

    public float maxDrag = 5f;
    public LineRenderer lr;

    Vector3 dragStartPos;
    Vector3 draggingPos;

    bool isDragging = false;
    bool isValid = false;
    public InputAction drag;

    //This is the code that spawns the backpack onto the ground for the player to interact with on the player position
    public void OpenBackpack()
    {

        if (curBackpack)
        {
            RaycastHit2D hit = Physics2D.CircleCast(curBackpack.transform.position, 1f, Vector2.zero, 0f, player);

            if (hit.collider != null)
            {
                Debug.Log(gameObject.name + " detected the player: " + hit.collider.name);
                Destroy(curBackpack);
            }
        }
        else
        {
            curBackpack = Instantiate(backpack, transform.position, transform.rotation, null);
        } 
        
    }

    private void Update()
    {
        // This code is activated from startDrag and is deactivated from endDrag
        // It calculates the line renderer display and whether or not to show it based off of magnitude and direction
        if (isDragging)
        {
            draggingPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            draggingPos.z = 0;

            Vector3 relativePos = draggingPos - transform.position;

            if (relativePos.magnitude > maxDistance)
            {
                relativePos = relativePos.normalized * 2;
            }
            if (relativePos.y > 0)
            {
                relativePos.y = 0;
            }
            if (relativePos.magnitude < minDistance)
            {
                lr.positionCount = 1;
                isValid = false;
                return;
            }
            isValid = true;
            lr.positionCount = 2;
            lr.SetPosition(1, transform.position - relativePos);
        }
    }


    private void OnEnable()
    {
        drag.Enable();
        drag.performed += startDrag;
        drag.canceled += endDrag;
    }

    private void OnDisable()
    {
        drag.canceled -= endDrag;
        drag.performed += startDrag;
        drag.Disable();
    }

    // Used to start the drag interaction for throwing the backpack while the backpack is equiped
    // isDragging is used to track whether the player is holding down, the start postion is set to the current transform
    // of the player and the start point of the line render is set. Additionally, the player is set to 'Busy' meaning that
    // they can't take ordinary actions like moving
    private void startDrag(InputAction.CallbackContext context)
    {
        if (GetComponent<PlayerMovementAdvanced>() != null && GetComponent<PlayerMovementAdvanced>().isGrounded== true 
            && curBackpack == null && !GetComponent<PlayerMovement>().GetIsMoving())
        {
            isDragging = true;
            GetComponent<PlayerMovementAdvanced>().EnableMovement(false);
            dragStartPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            dragStartPos.z = 0f;
            lr.positionCount = 1;
            lr.SetPosition(0, transform.position);
            GetComponent<PlayerMovement>().SetBusy(true);
        }
    }

    // Used at the end of dragging when the player releases the key. Sets isDragging to false and resets the line renderer.
    // Also, if the drag is valid, as determined by being a far enough magnitude away from the start, then it imparts a clamped
    // force in the direction and magnitude of the line render. It will instantiate the curBackpack into the scene and send
    // it flying. Afterwards it resets isValid and sets busy back to false to reallow movement
    private void endDrag(InputAction.CallbackContext context)
    {
        isDragging = false;
        lr.positionCount = 0;
        Vector3 dragReleasePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        dragReleasePos.z = 0;
        if (isValid)
        {
            Vector3 relativePos = dragReleasePos - transform.position;
            if (relativePos.magnitude > maxDistance)
            {
                relativePos = relativePos.normalized * 2;
            }
            if (relativePos.y > 0)
            {
                relativePos.y = 0;
            }
            Vector3 force = - relativePos + Vector3.zero;
            Vector3 clampedForce = Vector3.ClampMagnitude(force, maxDrag) * power;

            curBackpack = Instantiate(backpack, transform.position, transform.rotation, null);
            curBackpack.GetComponent<Rigidbody2D>().AddForce(clampedForce, ForceMode2D.Impulse);
        }
        isValid = false;
        GetComponent<PlayerMovement>().SetBusy(false);
        GetComponent<PlayerMovementAdvanced>().EnableMovement(true);

    }

    public bool GetBackpackOut()
    {
        return curBackpack;
    } 
}
