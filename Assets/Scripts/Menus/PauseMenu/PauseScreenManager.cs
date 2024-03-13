using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

/// <summary>
/// Dient der Steuerung des Pausemenüs sowie der Pausier-Funktion allgemein.
/// </summary>
public class PauseScreenManager : MonoBehaviour
{
    /// <summary>
    /// Das Pausenmenü als UI-Element.
    /// </summary>
    [SerializeField] private GameObject pauseMenu;
    /// <summary>
    /// Der Index der Hauptmenü-Szene in den Build-Einstellungen.
    /// </summary>
    [SerializeField] private int menuSceneIndex = 0;

    private GameManager gameManager;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        inputActions = new PlayerInputActions();
    }

    // Die Funktion zum Pausieren wird der Pause-Action angefügt bzw. wieder entfernt bei inaktiv werden des GOs.
    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.UI.Pause.performed += PauseTriggered;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        inputActions.UI.Pause.performed -= PauseTriggered;
    }


    #region Start/Stop Pause
    // Der Code für diese Region stammt ursprünglich aus folgendem Video und wurde hinsichtlich des globalen GameState-Managements dieses Projekts angepasst
    // Quelle: https://www.youtube.com/watch?v=JivuXdrIHK0

    /// <summary>
    /// Der Pause-Bildschirm wird geöffnet bzw. bei bereits geöffnetem Zustand wieder geschlossen.
    /// </summary>
    /// <param name="context"></param>
    private void PauseTriggered(InputAction.CallbackContext context)
    {
        if (gameManager.GetGameState() == GameStates.PlayState)
        {
            Pause();
        }
        else if (gameManager.GetGameState() == GameStates.PauseState)
        {
            Resume();
        }
    }

    /// <summary>
    /// Wird das Spiel pausiert, so wird das Menü angezeigt, die Zeit angehalten und der GameState entsprechend gesetzt.
    /// </summary>
    private void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        gameManager.SetGameState(GameStates.PauseState);
    }

    /// <summary>
    /// Wird das Spiel fortgesetzt erfolgen die obigen Operationen nur entgegengesetzt.
    /// </summary>
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        gameManager.SetGameState(GameStates.PlayState);
    }
    #endregion

    /// <summary>
    /// Lädt die Szene des Hauptmenüs.
    /// </summary>
    public void QuitToMenu()
    {
        SceneManager.LoadScene(menuSceneIndex);
    }
}
