using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MessageUIManager : MonoBehaviour
{
    public static MessageUIManager Instance { get; private set; }

    [Header("UI Reference")]
    public Canvas messageCanvas;              // Screen Space - Overlay Canvas
    public TextMeshProUGUI messageText;       // 중앙 메시지 TMP
    public float defaultDuration = 2f;        // 기본 표시 시간

    [Header("Predefined Messages")]
    public List<string> predefinedMessages = new List<string>();

    private Coroutine routine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (messageText != null)
            messageText.text = "";
    }

    /// <summary>
    /// 인덱스로 미리 등록된 메시지 출력
    /// </summary>
    public void ShowPredefined(int index)
    {
        if (index < 0 || index >= predefinedMessages.Count) return;
        Show(predefinedMessages[index], defaultDuration);
    }

    /// <summary>
    /// 직접 문자열 출력
    /// </summary>
    public void Show(string msg, float duration = -1f)
    {
        if (messageText == null) return;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ShowRoutine(msg, duration < 0 ? defaultDuration : duration));
    }

    private IEnumerator ShowRoutine(string msg, float duration)
    {
        messageText.text = msg;
        messageCanvas.enabled = true;

        yield return new WaitForSeconds(duration);

        messageText.text = "";
        messageCanvas.enabled = false;
    }
}
