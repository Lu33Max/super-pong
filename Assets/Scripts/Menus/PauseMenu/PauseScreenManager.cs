using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

/// <summary>
/// Dient der Steuerung des Pausemen�s sowie der Pausier-Funktion allgemein.
/// </summary>
public class PauseScreenManager : MonoBehaviour
{
    /// <summary>
    /// Das Pausenmen� als UI-Element.
    /// </summary>
    [SerializeField] private GameObject pauseMenu;
    /// <summary>
    /// Der Index der Hauptmen�-Szene in den Build-Einstellungen.
    /// </summary>
    [SerializeField] private int menuSceneIndex = 0;

    private GameManager gameManager;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        inputActions = new PlayerInputActions();
    }

    // Die Funktion zum Pausieren wird der Pause-Action angef�gt bzw. wieder entfernt bei inaktiv werden des GOs.
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
    // Der Code f�r diese Region stammt urspr�nglich aus folgendem Video und wurde hinsichtlich des globalen GameState-Managements dieses Projekts angepasst
    // Quelle: https://www.youtube.com/watch?v=JivuXdrIHK0

    /// <summary>
    /// Der Pause-Bildschirm wird ge�ffnet bzw. bei bereits ge�ffnetem Zustand wieder geschlossen.
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
    /// Wird das Spiel pausiert, so wird das Men� angezeigt, die Zeit angehalten und der GameState entsprechend gesetzt.
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
    /// L�dt die Szene des Hauptmen�s.
    /// </summary>
    public void QuitToMenu()
    {
        SceneManager.LoadScene(menuSceneIndex);
    }
}
