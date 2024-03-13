using System;
using UnityEngine;

/// <summary>
/// Bietet Funktionalität für das Wechseln des aktiven Balls des Computergegners.
/// </summary>
public class BallSwitchAI : BallSwitch
{
    [SerializeField] private PlayerSO playerData;
    [SerializeField] private float minWaitTime, maxWaitTime;

    private int inventoryIndex = 0;
    private float waitTime = 4;
    private float lastSwitchTime;

    private BallController ballController;
    private GameManager gameManager;

    public override event EventHandler<BallUpdatesEventArgs> OnBallRemoved;
    public override event EventHandler<BallUpdatesEventArgs> OnBallUpdated;
    public override event EventHandler<BallSwitchEventArgs> OnBallSwitched;

    private void Awake()
    {
        ballController = FindObjectOfType<BallController>();
        ballController.OnPlayerContact += BallSwitched;

        gameManager = FindObjectOfType<GameManager>();

        lastSwitchTime = Time.time;
        playerData.SelectedBall = playerData.Inventory[0].Ball;
    }

    private void Update()
    {
        if(gameManager.GetGameState() == GameStates.PlayState)
        {
            // Sobald die Wartezeit abgeschlossen ist, wird ein neuer Ballwechsel durchgeführt.
            if (Time.time > lastSwitchTime + waitTime)
            {
                ChangeSelectedBall(UnityEngine.Random.Range(-1, 1));

                lastSwitchTime = Time.time;
                waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
            }
        }
    }

    /// <summary>
    /// Wechselt den ausgewählten Ball des Computergegners.
    /// </summary>
    /// <param name="inputValue">Zufälliger Wert, der bestimmt, ob der Inventarindex erhört oder verringert wird.</param>
    private void ChangeSelectedBall(float inputValue)
    {
        // Geht abhängig des generierten Werts zufällig einen Index weiter oder zurück.
        int lastIndex = inventoryIndex;

        if (inputValue < 0 && inventoryIndex > 0)
        {
            inventoryIndex--;

            OnBallSwitched?.Invoke(this, new BallSwitchEventArgs { currentIndex = inventoryIndex, lastIndex = lastIndex });
            playerData.SelectedBall = playerData.Inventory[inventoryIndex].Ball;
        }
        else if (inputValue >= 0 && inventoryIndex < playerData.Inventory.Count - 1)
        {
            inventoryIndex++;

            OnBallSwitched?.Invoke(this, new BallSwitchEventArgs { currentIndex = inventoryIndex, lastIndex = lastIndex });
            playerData.SelectedBall = playerData.Inventory[inventoryIndex].Ball;
        }
    }

    /// <summary>
    /// Wird ausgeführt, nachdem der Ballwechsel an das Ball GameObject weitergeleitet wurde.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">playerTag - Tag des Spielers, dessen Inventar betroffen ist</param>
    private void BallSwitched(object sender, BallController.PlayerContactEventArgs e)
    {
        // Testet, ob die Änderung an diesem GameObject durchgeführt werden soll.
        if (gameObject.CompareTag(e.playerTag) && inventoryIndex != 0)
        {
            // Zieht eine Nutzung des Balls ab
            playerData.Inventory[inventoryIndex].Usages--;

            // Sobald die Nutzungen unter 0 fallen wird es aus dem Inventar und der Anzeige im HUD entfernt sowie der aktuell ausgewählte Index verringert, damit dieser nicht
            // außerhalb der Listen-Länge geht.
            if (playerData.Inventory[inventoryIndex].Usages < 1)
            {
                playerData.Inventory.RemoveAt(inventoryIndex);

                OnBallRemoved?.Invoke(this, new BallUpdatesEventArgs { index = inventoryIndex });

                inventoryIndex--;
                playerData.SelectedBall = playerData.Inventory[inventoryIndex].Ball;

                OnBallSwitched?.Invoke(this, new BallSwitchEventArgs { currentIndex = inventoryIndex, lastIndex = null });
            }
            // Updatet das HUD Panel des aktiven Balls, um die noch verbliebende Anzahl an Nutzung widerzuspiegeln.
            else
            {
                OnBallUpdated?.Invoke(this, new BallUpdatesEventArgs { index = inventoryIndex });
            }
        }
    }
}
