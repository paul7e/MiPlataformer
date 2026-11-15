using platform.Controller;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
namespace platform.UI
{
    public class PauseUI : MonoBehaviour
    {
        [SerializeField] float timeScale = 0f;

        [SerializeField] Button resumeButton;

        [SerializeField] Button quitButton;

        [SerializeField] UnityEvent onMenuOpened; // Event to notify when the menu is opened for music controller
        [SerializeField] UnityEvent onMenuClosed;

        private void OnEnable()
        {
            Time.timeScale = timeScale;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            resumeButton.onClick.AddListener(ResumeScene);
            quitButton.onClick.AddListener(QuitGame);

            onMenuOpened?.Invoke();

        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            resumeButton.onClick.RemoveListener(ResumeScene);
            quitButton.onClick.RemoveListener(QuitGame);

            onMenuClosed?.Invoke();
        }


        void ResumeScene()
        {
            gameObject.SetActive(false);
        }


        void QuitGame()
        {
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;

            #else
                Application.Quit();

            #endif
        }
    }
}
