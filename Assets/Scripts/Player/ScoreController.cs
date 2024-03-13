using System;
using UnityEngine;

/// <summary>
/// Stellt Funktionalitäten zum Anpassen der Leben eines Spielers.
/// </summary>
public class ScoreController : MonoBehaviour
{
    [SerializeField] private PlayerSO playerData;
    [SerializeField] private HealthBar healthBar;

    private GameManager gameManager;
    private BallController ballController;

    public event EventHandler<GameEndedEventArgs> OnGameEnded;

    public class GameEndedEventArgs
    {
        public string losingPlayerTag;
    }

    void Start()
    {
        healthBar.SetMaxHealth(playerData.MaxLife);

        // Wird ausgelöst, sobald der Ball in eine der Zonen eindringt
        ballController = FindObjectOfType<BallController>();
        ballController.OnScoreUpdate += UpdateScore;

        gameManager = FindObjectOfType<GameManager>();
    }

    /// <summary>
    /// Setzt den Lebenswert des Spielers entsprechend der übergebenen Punkte.
    /// </summary>
    /// <param name="e">playerTag - Tag des Spielers, der von der Änderung betroffen ist | score - Wert, um den die Leben geändert werden. (Abzüge werden durch negatives Vorzeichen gekennzeichnet)</param>
    private void UpdateScore(object sender, BallController.ScoreUpdateEventArgs e)
    {
        // Überorüft, ob die Änderung tatsächlich an diesem Spieler vorgenommen werden soll.
        if (gameObject.CompareTag(e.playerTag))
        {
            // Verhindert, dass der finale Schlag durch eine normale Berührung vorgenommen werden kann
            if(e.type != BallController.ScoreType.Touch || playerData.CurrentLife + e.score > 0)
            {
                playerData.CurrentLife += e.score;

                // Fällt das Lauf auf 0 ab, so wird der GameOver Bildschirm angezeigt und das Spielgeschehen pausiert
                if(playerData.CurrentLife <= 0)
                {
                    playerData.CurrentLife = 0;
                    OnGameEnded?.Invoke(this, new GameEndedEventArgs { losingPlayerTag = e.playerTag });
                    gameManager.SetGameState(GameStates.EndState);
                    Time.timeScale = 0f;
                }
            }
            // Würde das Leben durch eine Berührung unter 0 fallen, wird es auf 1 gesetzt
            else
            {
                playerData.CurrentLife = 1;
            }

            // Fällt das Leben des Spielers unter bestimmte Schwellenwert, wird der aktuelle HealthState geändert.
            if(playerData.CurrentLife <= playerData.MaxLife / 2 && gameManager.GetHealthState() == HealthStates.FullHealth)
            {
                gameManager.SetHealthState(HealthStates.LowHealth);
            }
            else if(playerData.CurrentLife <= playerData.MaxLife / 10 && gameManager.GetHealthState() == HealthStates.LowHealth)
            {
                gameManager.SetHealthState(HealthStates.CriticalHealth);
            }

            // Setzt die Länge der Lebensleiste
            healthBar.SetHealth(playerData.CurrentLife);
        }
    }
}
