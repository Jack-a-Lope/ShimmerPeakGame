using UnityEngine;
using UnityEngine.Events;
using TMPro;

using DigitalWorlds.StarterPackage2D;

public class Teleporter2D : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform destination;
    public UnityEvent onTeleported;

    [Header("Activation Settings")]
    public string requiredTag = "Player";
    public bool requireKeyPress = false;
    public KeyCode activationKey = KeyCode.E;

    [Header("Collectable Requirement")]
    public bool useCollectableRequirement = true;
    public string requiredItemName = "Star";
    public int requiredItemCount = 5;
    [TextArea] public string failMessage = "";

    [Header("UI Message Output")]
    public TextMeshProUGUI messageText;
    public float messageDuration = 2f;

    private bool playerInRange = false;
    private Transform player;
    private Coroutine messageRoutine;

    private void Update()
    {
        if (requireKeyPress && playerInRange && player != null)
        {
            if (Input.GetKeyDown(activationKey))
            {
                TryTeleport(player);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(requiredTag))
        {
            playerInRange = true;
            player = collision.transform;

            if (!requireKeyPress)
            {
                TryTeleport(player);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(requiredTag))
        {
            playerInRange = false;
            player = null;
        }
    }

    private void TryTeleport(Transform player)
    {
        if (destination == null)
        {
            Debug.LogWarning("Teleporter destination is not assigned");
            return;
        }

        if (!CanTeleport())
            return;

        player.position = destination.position;
        onTeleported.Invoke();
    }

    private bool CanTeleport()
    {
        if (!useCollectableRequirement) return true;

        if (requiredItemCount > 0 && CollectableManager.Instance != null)
        {
            var c = CollectableManager.Instance.FindCollectable(requiredItemName);
            if (c == null || c.count < requiredItemCount)
            {
                string msg = !string.IsNullOrEmpty(failMessage)
                    ? failMessage
                    : $"{requiredItemName} x{requiredItemCount} required to use this teleporter.";
                ShowMessage(msg);
                return false;
            }
        }
        return true;
    }

    private void ShowMessage(string msg)
    {
        Debug.Log(msg);
        if (messageText == null) return;

        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(MessageRoutine(msg));
    }

    private System.Collections.IEnumerator MessageRoutine(string msg)
    {
        messageText.text = msg;
        yield return new WaitForSeconds(messageDuration);
        messageText.text = "";
    }
}
