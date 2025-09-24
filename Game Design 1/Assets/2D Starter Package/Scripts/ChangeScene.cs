using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

namespace DigitalWorlds.StarterPackage2D
{
    public class ChangeScene : MonoBehaviour
    {
        [Header("Collectable Requirement")]
        public bool useCollectableRequirement = true;
        public string requiredItemName = "Star";
        public int requiredItemCount = 5;
        [TextArea] public string failMessage = "";

        [Header("UI Message Output")]
        public TextMeshProUGUI messageText;
        public float messageDuration = 2f;

        private Coroutine messageRoutine;

        private void ShowMessage(string msg)
        {
            Debug.Log(msg);
            if (messageText == null) return;

            if (messageRoutine != null)
                StopCoroutine(messageRoutine);

            messageRoutine = StartCoroutine(MessageRoutine(msg));
        }

        private IEnumerator MessageRoutine(string msg)
        {
            messageText.text = msg;
            yield return new WaitForSeconds(messageDuration);
            messageText.text = "";
        }

        private bool CanChangeScene()
        {
            if (!useCollectableRequirement) return true;

            if (requiredItemCount > 0 && CollectableManager.Instance != null)
            {
                var c = CollectableManager.Instance.FindCollectable(requiredItemName);
                if (c == null || c.count < requiredItemCount)
                {
                    string msg = !string.IsNullOrEmpty(failMessage)
                        ? failMessage
                        : $"{requiredItemName} x{requiredItemCount} required to change scene.";
                    ShowMessage(msg);
                    return false;
                }
            }
            return true;
        }

        public void LoadSceneByName(string name)
        {
            if (!CanChangeScene()) return;
            SceneManager.LoadScene(name);
        }

        public void LoadSceneByIndex(int index)
        {
            if (!CanChangeScene()) return;
            SceneManager.LoadScene(index);
        }

        public void LoadNextScene()
        {
            if (!CanChangeScene()) return;

            int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextIndex);
            }
            else
            {
                Debug.LogWarning("No more scenes to load. Already at the last scene.");
            }
        }

        public void LoadCurrentScene()
        {
            if (!CanChangeScene()) return;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
