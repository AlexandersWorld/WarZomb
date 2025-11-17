using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] HealthBar targetHealthBar;
    [SerializeField] Transform gameOverScreen;

    [SerializeField] Button playAgainButton;
    [SerializeField] Button quitButton;

    [SerializeField] Transform tutorialTransform;
    [SerializeField] TextMeshProUGUI textTutorial;
    [SerializeField] TextMeshProUGUI textTutorialKeys;

    private int tutorialStep = 0; // 0 = intro, 1 = keys, 2 = start game

    private void Start()
    {
        InitialSetup();

        playAgainButton.onClick.AddListener(PlayAgain);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void InitialSetup()
    {
        Time.timeScale = 0f;

        gameOverScreen.gameObject.SetActive(false);

        tutorialTransform.gameObject.SetActive(true);
        textTutorial.gameObject.SetActive(true);
        textTutorialKeys.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleTutorialSteps();

        if (targetHealthBar.IsEmpty())
        {
            gameOverScreen.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void HandleTutorialSteps()
    {
        if (!tutorialTransform.gameObject.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            tutorialStep++;

            if (tutorialStep == 1)
            {
                textTutorial.gameObject.SetActive(false);
                textTutorialKeys.gameObject.SetActive(true);
            }
            else if (tutorialStep == 2)
            {
                tutorialTransform.gameObject.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }

    private void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
