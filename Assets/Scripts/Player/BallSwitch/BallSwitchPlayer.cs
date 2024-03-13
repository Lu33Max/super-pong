using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Bietet Funktionalität für das Wechseln des aktiven Balls der Spieler.
/// </summary>
public class BallSwitchPlayer : BallSwitch
{
    [SerializeField] private PlayerSO playerData;
    [SerializeField] private bool isPlayer1;

    private PlayerInputActions inputActions;
    private int inventoryIndex = 0;
    
    private BallController ballController;

    public override event EventHandler<BallUpdatesEventArgs> OnBallRemoved;
    public override event EventHandler<BallUpdatesEventArgs> OnBallUpdated;
    public override event EventHandler<BallSwitchEventArgs> OnBallSwitched;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        ballController = FindObjectOfType<BallController>();
        ballController.OnPlayerContact += BallSwitched;

        playerData.SelectedBall = playerData.Inventory[0].Ball;
    }

    // Fügt die Methoden zum Wechseln des Balls den entsprechenden Tasten hinzu
    private void OnEnable()
    {
        if (isPlayer1)
        {
            inputActions.Player.SwitchBall.Enable();
            inputActions.Player.SwitchBall.performed += ChangeSelectedBall;
        }
        else
        {
            inputActions.Player2.SwitchBall.Enable();
            inputActions.Player2.SwitchBall.performed += ChangeSelectedBall;
        }
    }

    private void OnDisable()
    {
        if(isPlayer1)
        {
            inputActions.Player.SwitchBall.Disable();
            inputActions.Player.SwitchBall.performed -= ChangeSelectedBall;
        }
        else
        {
            inputActions.Player2.SwitchBall.Disable();
            inputActions.Player2.SwitchBall.performed -= ChangeSelectedBall;
        }
    }

    /// <summary>
    /// Wechselt den ausgewählten Ball des Computergegners.
    /// </summary>
    private void ChangeSelectedBall(InputAction.CallbackContext context)
    {
        // Geht abhängig von dem Tastendruck einen Index weiter oder zurück und setzt den ausgewählten Ball
        float inputValue = context.ReadValue<float>();
        int lastIndex = inventoryIndex;
        
        if(inputValue < 0 && inventoryIndex > 0) 
        {
            inventoryIndex--;

            OnBallSwitched?.Invoke(this, new BallSwitchEventArgs { currentIndex = inventoryIndex, lastIndex = lastIndex });
            playerData.SelectedBall = playerData.Inventory[inventoryIndex].Ball;
        }
        else if(inputValue > 0 && inventoryIndex < playerData.Inventory.Count - 1)
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
