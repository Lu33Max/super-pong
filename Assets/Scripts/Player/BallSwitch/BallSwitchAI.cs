using System;
using UnityEngine;

/// <summary>
/// Bietet Funktionalit�t f�r das Wechseln des aktiven Balls des Computergegners.
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
            // Sobald die Wartezeit abgeschlossen ist, wird ein neuer Ballwechsel durchgef�hrt.
            if (Time.time > lastSwitchTime + waitTime)
            {
                ChangeSelectedBall(UnityEngine.Random.Range(-1, 1));

                lastSwitchTime = Time.time;
                waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
            }
        }
    }

    /// <summary>
    /// Wechselt den ausgew�hlten Ball des Computergegners.
    /// </summary>
    /// <param name="inputValue">Zuf�lliger Wert, der bestimmt, ob der Inventarindex erh�rt oder verringert wird.</param>
    private void ChangeSelectedBall(float inputValue)
    {
        // Geht abh�ngig des generierten Werts zuf�llig einen Index weiter oder zur�ck.
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
    /// Wird ausgef�hrt, nachdem der Ballwechsel an das Ball GameObject weitergeleitet wurde.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">playerTag - Tag des Spielers, dessen Inventar betroffen ist</param>
    private void BallSwitched(object sender, BallController.PlayerContactEventArgs e)
    {
        // Testet, ob die �nderung an diesem GameObject durchgef�hrt werden soll.
        if (gameObject.CompareTag(e.playerTag) && inventoryIndex != 0)
        {
            // Zieht eine Nutzung des Balls ab
            playerData.Inventory[inventoryIndex].Usages--;

            // Sobald die Nutzungen unter 0 fallen wird es aus dem Inventar und der Anzeige im HUD entfernt sowie der aktuell ausgew�hlte Index verringert, damit dieser nicht
            // au�erhalb der Listen-L�nge geht.
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
