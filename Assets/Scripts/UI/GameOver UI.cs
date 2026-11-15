using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace FirstPS.UI

{

    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] float timeScale = 0.2f;

        [SerializeField] float interactionDelay = 1f;

        [SerializeField] Button restartButton;

        [SerializeField] Button quitButton;


        private void OnEnable()
        {
            Time.timeScale = timeScale;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            StartCoroutine(setInteractionRutine());

            restartButton.onClick.AddListener(reloadScene);
            quitButton.onClick.AddListener(quitGame);
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            restartButton.onClick.RemoveListener(reloadScene);
            quitButton.onClick.RemoveListener(quitGame);
        }

        IEnumerator setInteractionRutine()
        {
            restartButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(interactionDelay);
            restartButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(true);

        }

        void reloadScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }

        void quitGame()
        {
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;

            #else
                Application.Quit();

            #endif
        }

    }
}