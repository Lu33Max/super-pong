using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Bietet die Funktionalität zum Bewegen des Spieler-Pongs.
/// </summary>
public class PlayerController : BasePlayerController
{
    Rigidbody2D rb;

    [SerializeField] private bool isPlayer1;
    [SerializeField] private PlayerInputActions inputActions;
    [SerializeField] private PlayerSO playerData;
    
    private Vector2 moveInput;
    private InputAction move;
    private float? lastBallHit = null;

    private GameManager gameManager;

    public override event EventHandler<PlayerBounceEventArgs> OnPlayerBounce;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    // Weist die Bewgungsaktion den entsprechenden Tasten zu 
    private void OnEnable()
    {
        if (isPlayer1)
        {
            move = inputActions.Player.Move;
        }
        else
        {
            move = inputActions.Player2.Move;
        }

        move.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();    
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if(gameManager.GetGameState() == GameStates.PlayState)
        {
            // Liest die Eingabe des Spielers aus
            moveInput = move.ReadValue<Vector2>();
        }
    }

    private void FixedUpdate()
    {
        // Bewegt den Spieler-Pong entsprechend der Eingabe des Spielers
        rb.MovePosition(rb.position + Time.deltaTime * playerData.Velocity * moveInput);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Löst das PlayerBounceEvent aus, sobald er vom Ball getroffen wurde.
        // Um mehrfaches Auslösen bei der selben Kollision zu verhindern, kann dieses erst nach einer gewisseen Zeit erneut ausgelöst werden.
        if (collision.gameObject.CompareTag("Ball") && (lastBallHit == null || lastBallHit + 0.2 < Time.time))
        {
            lastBallHit = Time.time;
            OnPlayerBounce?.Invoke(this, new PlayerBounceEventArgs { collision = collision, player = gameObject, playerData = playerData });
        }
    }
}
