using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Dient der Menüsteuerung im Game Over Menü nach Ende einer Runde.
/// </summary>
public class EndScreenManager : MonoBehaviour
{
    /// <summary>
    /// Index der Szene des Hauptmenüs in den Build-Einstellungen.
    /// </summary>
    [SerializeField] private int menuScene;
    [SerializeField] private GameObject gameOverScreen;

    private void Start()
    {
        // Fügt die Anzeige des Menüs dem OnGameEnded-Event hinzu
        ScoreController[] scoreControllers = FindObjectsByType<ScoreController>(FindObjectsSortMode.None);

        foreach (ScoreController scoreController in scoreControllers)
        {
            scoreController.OnGameEnded += ShowGameOverMenu;
        }
    }

    /// <summary>
    /// Das Game Over Menü wird angezeigt und der Text entsprechend des gewinnenden Spielers angepasst
    /// </summary>
    /// <param name="e">Enthält den Tag des zuletzt geschadeten Spielers, der verloren hat.</param>
    private void ShowGameOverMenu(object sender, ScoreController.GameEndedEventArgs e)
    {
        gameOverScreen.SetActive(true);
        gameOverScreen.GetComponentInChildren<TextMeshProUGUI>().text = e.losingPlayerTag == "Player1" ? "Player2 wins!" : "Player1 wins!";
    }

    /// <summary>
    /// Neuladen der Szene für eine erneute Runde mit gleichen Einstellungen.
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Laden der Hauptmenü-Szene.
    /// </summary>
    public void QuitToMenu()
    {
        SceneManager.LoadScene(menuScene);
    }
}
