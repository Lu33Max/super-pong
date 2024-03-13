using System;
using UnityEngine;

/// <summary>
/// Stellt Funktionalit�ten zum Anpassen der Leben eines Spielers.
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

        // Wird ausgel�st, sobald der Ball in eine der Zonen eindringt
        ballController = FindObjectOfType<BallController>();
        ballController.OnScoreUpdate += UpdateScore;

        gameManager = FindObjectOfType<GameManager>();
    }

    /// <summary>
    /// Setzt den Lebenswert des Spielers entsprechend der �bergebenen Punkte.
    /// </summary>
    /// <param name="e">playerTag - Tag des Spielers, der von der �nderung betroffen ist | score - Wert, um den die Leben ge�ndert werden. (Abz�ge werden durch negatives Vorzeichen gekennzeichnet)</param>
    private void UpdateScore(object sender, BallController.ScoreUpdateEventArgs e)
    {
        // �beror�ft, ob die �nderung tats�chlich an diesem Spieler vorgenommen werden soll.
        if (gameObject.CompareTag(e.playerTag))
        {
            // Verhindert, dass der finale Schlag durch eine normale Ber�hrung vorgenommen werden kann
            if(e.type != BallController.ScoreType.Touch || playerData.CurrentLife + e.score > 0)
            {
                playerData.CurrentLife += e.score;

                // F�llt das Lauf auf 0 ab, so wird der GameOver Bildschirm angezeigt und das Spielgeschehen pausiert
                if(playerData.CurrentLife <= 0)
                {
                    playerData.CurrentLife = 0;
                    OnGameEnded?.Invoke(this, new GameEndedEventArgs { losingPlayerTag = e.playerTag });
                    gameManager.SetGameState(GameStates.EndState);
                    Time.timeScale = 0f;
                }
            }
            // W�rde das Leben durch eine Ber�hrung unter 0 fallen, wird es auf 1 gesetzt
            else
            {
                playerData.CurrentLife = 1;
            }

            // F�llt das Leben des Spielers unter bestimmte Schwellenwert, wird der aktuelle HealthState ge�ndert.
            if(playerData.CurrentLife <= playerData.MaxLife / 2 && gameManager.GetHealthState() == HealthStates.FullHealth)
            {
                gameManager.SetHealthState(HealthStates.LowHealth);
            }
            else if(playerData.CurrentLife <= playerData.MaxLife / 10 && gameManager.GetHealthState() == HealthStates.LowHealth)
            {
                gameManager.SetHealthState(HealthStates.CriticalHealth);
            }

            // Setzt die L�nge der Lebensleiste
            healthBar.SetHealth(playerData.CurrentLife);
        }
    }
}
