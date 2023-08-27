using Core;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class Navigation : MonoBehaviour
    {
        [Header("Loading (Async only)")]
        [SerializeField]
        private GameObject loadingScreen;
        [SerializeField]
        private GameObject loader;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private Animator flashAnimator;

        [SerializeField]
        private DialogueManager dialogue;

        [SerializeField]
        private GameObject skipDialogueButton;

        public static Navigation instance;

        [HideInInspector]
        public bool StageGenComplete;

        private bool coroutineActive;
        public bool loadingScreenActive { get; private set; }

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        private void Update()
        {
            if (loadingScreen != null && loadingScreen.activeSelf)
                loadingScreenActive = true;
            else
                loadingScreenActive = false;

            if (loadingScreenActive && loader != null && loader.activeSelf && StageGenComplete)
                loader.SetActive(false);

            if (loadingScreenActive && loader != null && !loader.activeSelf && !dialogue.dialogueActive)
                FadeIn();
        }

        public void MainMenu()
        {
            if (Timer.instance != null)
                Timer.instance.timerIsRunning = false;

            SceneManager.LoadScene(0);
            Time.timeScale = 1f;
        }

        public void Credits()
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }

        public void Settings()
        {
            SceneManager.LoadScene(2, LoadSceneMode.Single);
        }

        public void Game()
        {
            FadeOut(3);
        }

        public void GameOver()
        {
            FadeOut(4);
        }

        public void EndGame()
        {
            StartCoroutine(FadeOutCoroutine(5));
        }

        public void SkipIntro()
        {
            foreach (GameObject trigger in GameObject.FindGameObjectsWithTag("DialogueTrigger"))
                trigger.GetComponent<DialogueTrigger>().hasTriggered = true;

            if (gameObject.GetComponent<DialogueTrigger>() != null)
                gameObject.GetComponent<DialogueTrigger>().hasTriggered = true;

            DialogueManager.instance.SkipDialogue();

            skipDialogueButton.SetActive(false);
        }

        public void Quit()
        {
            Time.timeScale = 1f;
            if(!coroutineActive)
                StartCoroutine(QuitGame());
        }

        public void FadeIn()
        {
            if(!coroutineActive)
            {
                if (skipDialogueButton != null)
                    skipDialogueButton.SetActive(false);

                StartCoroutine(FadeInCoroutine());
            }
        }

        public void FadeOut(int scene)
        {
            if (!coroutineActive)
                StartCoroutine(FadeOutCoroutine(scene));
        }

        public IEnumerator FadeInCoroutine()
        {
            coroutineActive = true;
            if (loader != null)
                loader.SetActive(false);

            if (dialogue != null)
                dialogue.SkipDialogue();

            if (loadingScreen != null)
            {
                animator.SetTrigger("FadeIn");
                yield return new WaitForSeconds(1f);

                loadingScreen.SetActive(false);

                DialogueTrigger trigger = gameObject.GetComponent<DialogueTrigger>();
                if (trigger != null && trigger.hasTriggered == false)
                    trigger.TriggerDialogue();
            }
            coroutineActive = false;
        }

        public IEnumerator FadeOutCoroutine(int scene)
        {
            coroutineActive = true;
            if (loadingScreen != null)
            {
                loadingScreen.SetActive(true);
                animator.SetTrigger("FadeOut");

                if (flashAnimator != null)
                    flashAnimator.SetTrigger("Flash");

                yield return new WaitForSeconds(0.99f);
                SceneManager.LoadScene(scene);
            }
            else
                SceneManager.LoadScene(scene);
            coroutineActive = false;
        }

        public IEnumerator QuitGame()
        {
            coroutineActive = true;
            if (loadingScreen != null)
            {
                loadingScreen.SetActive(true);
                animator.SetTrigger("FadeOut");
                yield return new WaitForSeconds(0.99f);
                Application.Quit();
            }
            else
                Application.Quit();
            coroutineActive = false;
        }
    }
}