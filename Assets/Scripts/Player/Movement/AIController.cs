using System;
using UnityEngine;

/// <summary>
/// Bietet die Funktionalität zum Bewegen des Computer-Pongs.
/// </summary>
public class AIController : BasePlayerController
{
    [SerializeField] private float precision = 0.5f;
    [SerializeField] private PlayerSO playerData;

    private Rigidbody2D rb;
    private Transform ball;

    private Vector2 moveInput;
    private float? lastBallHit = null;

    private GameManager gameManager;

    public override event EventHandler<PlayerBounceEventArgs> OnPlayerBounce;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ball = GameObject.Find("Ball").transform;

        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (gameManager.GetGameState() == GameStates.PlayState)
        {
            // Versucht seine y-Position stets auf die des Balls abzustimmen. Die Genauigkeit bestimmt eine gewisse Puffer-Distanz, da durch die Ungenauigkeiten der Simulation
            // nie der exakte Wert erreicht werden kann.
            if (transform.position.y < (ball.position.y - precision))
            {
                moveInput = new Vector2(0f, 1f);
            }
            else if (transform.position.y > (ball.position.y + precision))
            {
                moveInput = new Vector2(0f, -1f);
            }
            else
            {
                moveInput = new Vector2(0f, 0f);
            }
        }
    }

    private void FixedUpdate()
    {
        // Bewegt den Computergegner entsprechend der aktuelen Richtung und Geschwindigkeit.
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
