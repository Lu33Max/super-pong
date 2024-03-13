using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Stellt Funktionen f�r die Kn�pfe des Hauptmen�s zur Verf�gung.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    /// <summary>
    /// Index der Spiel-Szene in den Build-Einstellungen.
    /// </summary>
    [SerializeField] private int startScene;

    /// <summary>
    /// L�dt die Spiel-Szene.
    /// </summary>
    public void StartGameScene()
    {
        SceneManager.LoadScene(startScene);
    }

    /// <summary>
    /// Setzt den Spielmodus auf Einzel- oder Mehrspieler.
    /// </summary>
    /// <param name="isMultiPlayer"></param>
    public void SetGameMode(bool isMultiPlayer)
    {
        MatchData.IsMultiPlayer = isMultiPlayer;
    }

    /// <summary>
    /// Closes the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
