using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Kontrolliert die Bewegung des Balls und seine Interaktion mit der Umgebung.
/// </summary>
public class BallController : MonoBehaviour
{
    private IBall currentBall;
    private GameObject currentBallGO;
    private GameManager gameManager;

    private bool isResetting = true;

    [SerializeField] private GameObject defaultBall;
    [SerializeField] private float maxMoveTime = 15f;

    public event EventHandler<ScoreUpdateEventArgs> OnScoreUpdate;
    public event EventHandler OnBallSwitched;
    public event EventHandler<PlayerContactEventArgs> OnPlayerContact;
    public event EventHandler<ItemCollectionEventArgs> OnItemCollected;

    /// Klassen f�r Custom Events, die vom Ball ausel�st werden k�nnen
    #region Custom Event Classes
    public class ScoreUpdateEventArgs : EventArgs
    {
        public string playerTag;
        public int score;
        public ScoreType type;
    }

    public class ItemCollectionEventArgs : EventArgs
    {
        public InventorySlot item;
        public string playerTag;
    }

    public class PlayerContactEventArgs : EventArgs
    {
        public string playerTag;
    }
    #endregion

    /// <summary>
    /// Eine Enum f�r verschiedene Schadenstypen.
    /// </summary>
    /// Additive ist f�r das klassische Pong-Punktesystem und wird nicht verwendet.
    public enum ScoreType { Touch = 0, Goal = 1, Additive = 2, Heal = 3 }

    private Rigidbody2D rb;
    private Vector2 _direction;
    
    /// <summary>
    /// Anzahl, wie h�ufig der Ball an Spielern abgeprallt ist
    /// </summary>
    private int hitCounter;
    /// <summary>
    /// Tag des zuletzt ber�hrten Spielers
    /// </summary>
    private string lastPlayer;
    /// <summary>
    /// Letzter Zeitpunkt, an dem ein Spieler ber�hrt wurde
    /// </summary>
    private float lastHitTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Der Standardball wird instanziiert und die entsprechende Methode des Balls aufgerufen
        currentBallGO = Instantiate(defaultBall, transform);
        currentBall = currentBallGO.GetComponent<IBall>();
        currentBall.OnInstantiate();

        gameManager = FindObjectOfType<GameManager>();

        // Hinzuf�gen von Methoden zu Events
        BasePlayerController[] playerControllers = FindObjectsByType<BasePlayerController>(FindObjectsSortMode.None);

        foreach (var controller in playerControllers)
        {
            controller.OnPlayerBounce += PlayerBounce;
        }

        // Nach dem Setup wird die Runde mit kurzer Verz�gerung gestartet
        StartCoroutine(RoundStart(3.5f));
    }

    void Update()
    {
        // Wenn der Ball sich bewegt und  eine geweisse Zeit nicht mehr von einem Spieler ber�hrt wurde, so resettet er sich
        if(_direction != Vector2.zero && !isResetting && Time.time > lastHitTime + maxMoveTime)
        {
            ResetBall();
        }

        // Solange das Spiel noch l�uft wird die Update Methode des aktuellen Balls aufgerufen
        if (gameManager.GetGameState() == GameStates.PlayState && !isResetting)
        {
            currentBall.RegularUpdate();
        }
    }

    private void FixedUpdate()
    {
        // Solange das Spiel noch l�uft wird die Physik-Update Methode des aktuellen Balls aufgerufen
        if (gameManager.GetGameState() == GameStates.PlayState && !isResetting)
        {
            rb.velocity = currentBall.Move(rb.velocity, hitCounter);
        }
    }

    /// <summary>
    /// Zerst�rt den aktuellen Ball, ersetzt ihn mit einem neuen und instanziiert diesen als Kind-Element des �bergeordneten Ball-Objekts.
    /// </summary>
    private void SwitchBall(GameObject newBall, PlayerSO playerData)
    {
        currentBall.Destroy();
        Destroy(currentBallGO);

        currentBallGO = Instantiate(newBall, transform);

        currentBall = currentBallGO.GetComponent<IBall>();
        currentBall.OnInstantiate(playerData);
    }

    /// <summary>
    /// Vergleicht den aktuellen Ball mit dem vom Spieler ausgew�hlten. Stimmen diese nicht �berein, wird ein Wechsel ausgef�hrt.
    /// </summary>
    /// <param name="playerData">SO des getroffenen Spielers.</param>
    private void SwitchOnContact(PlayerSO playerData)
    {
        var newBall = playerData.SelectedBall;

        if (newBall.GetComponent<IBall>().BallData.BallName != currentBall.BallData.BallName)
        {
            SwitchBall(newBall, playerData);
            OnBallSwitched?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Wird zum Starten des Balls aufgerufen. W�hlt eine zuf�llige Startrichtung f�r den Ball auf Basis des letzten Spielers, der punkten konnte.
    /// </summary>
    /// <param name="scoringPlayer">Tag der Zone des getroffenen Spielers oder null beim Start der Runde.</param>
    private IEnumerator RoundStart(float waitTime, string scoringPlayer = null)
    {
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(waitTime);

        if (gameManager.GetGameState() == GameStates.PlayState || gameManager.GetGameState() == GameStates.StartState)
        {
            // Die y-Komponente der BEwegung wird zuf�llig bestimmt
            float xDir, yDir = UnityEngine.Random.Range(-0.5f, 0.5f); ;

            // Die x-Komponente wird so gewahlt, dass sich der Ball in Richtung des punktenden Spielers bewegt bzw. zuf�llig zum Start der Runde
            switch (scoringPlayer)
            {
                case "Player1Zone":
                    {
                        xDir = 1;
                        break;
                    }
                case "Player2Zone":
                    {
                        xDir = -1;
                        break;
                    }
                default:
                    {
                        xDir = UnityEngine.Random.Range(-1f, 1f) > 0 ? 1 : -1;
                        break;
                    }
            }

            _direction = new Vector2(xDir, yDir).normalized;

            // Bei niedrigem Leben startet der Ball schneller als normal
            if(gameManager.GetHealthState() == HealthStates.LowHealth)
            {
                hitCounter = 10;
            }
            else if(gameManager.GetHealthState() == HealthStates.CriticalHealth)
            {
                hitCounter = 16;
            }

            rb.velocity = _direction * (currentBall.BallData.InitialSpeed + currentBall.BallData.SpeedIncrease * hitCounter);

            // Handelt es sich um den ersten Durchlauf der Methode, wird der Gamestate entsprechend ge�ndert
            if(gameManager.GetGameState() == GameStates.StartState)
            {
                gameManager.SetGameState(GameStates.PlayState);
            }

            lastHitTime = Time.time;

            isResetting = false;
        }
    }

    /// <summary>
    /// Setzt Position, Werte und den aktiven Ball nach Erzielen eines Punkts zur�ck.
    /// </summary>
    /// <param name="scoringPlayer">Tag des punktenden Spielers zur Bestimmung der neuen Ballrichtung.</param>
    private void ResetBall(string scoringPlayer = null)
    {
        isResetting = true;

        rb.velocity = Vector2.zero;
        transform.position = Vector2.zero;
        hitCounter = 0;
        lastPlayer = null;

        SwitchBall(defaultBall, null);
        StartCoroutine(RoundStart(2f, scoringPlayer));
    }

    /// <summary>
    /// Wird bei Ber�hrung mit einem Spieler aufgerufen. 
    /// </summary>
    /// <param name="e">Tag, SO und GameObject de getroffenen Spielers.</param>
    private void PlayerBounce(object sender, PlayerBounceEventArgs e)
    {
        if(!isResetting)
        {
            // Die Anzahl der Treffer und der zuletzt ber�hrte Spieler werden angepasst.
            hitCounter++;
            lastPlayer = e.player.tag;
            lastHitTime = Time.time;

            // Dem Spieler wird Ber�hrungsschaden zugef�gt und auf einen eventuellen Ballwechsel getestet.
            DealTouchDamage(e.player.tag);
            SwitchOnContact(e.playerData);

            // Das PlayerContactEvent wird ausgel�st. Dem aktuell ausgew�hltem Ball des Spielers wird dadurch eine Nutzung abgezogen.
            OnPlayerContact?.Invoke(this, new PlayerContactEventArgs { playerTag = e.player.tag });

            // Der aktuell ausgew�hlte Ball bestimmt die weitere Bewegungsrichtung des Balls.
            rb.velocity = currentBall.OnPlayerBounce(e.player, _direction, hitCounter);
        }
    }

    /// <summary>
    /// F�r das klassische Pong-Punktesystem. Aktuell nicht genutzt.
    /// </summary>
    /// <param name="zoneTag">Tag des getroffenen Zonen-Objekts</param>
    //private void AddScore(string zoneTag)
    //{
    //    switch (zoneTag)
    //    {
    //        case "Player1Zone":
    //            {
    //                OnScoreUpdate?.Invoke(this, new ScoreUpdateEventArgs { playerTag = "Player2", score = 1, type = ScoreType.Additive });
    //                break;
    //            }
    //        case "Player2Zone":
    //            {
    //                OnScoreUpdate?.Invoke(this, new ScoreUpdateEventArgs { playerTag = "Player1", score = 1, type = ScoreType.Additive });
    //                break;
    //            }
    //    }
    //}

    /// <summary>
    /// Verwendet von den Heilb�llen zum Heilen des Spielers
    /// </summary>
    public void HealPlayer(int health)
    {
        OnScoreUpdate.Invoke(this, new ScoreUpdateEventArgs { playerTag = lastPlayer, score = health });
    }

    /// <summary>
    /// F�gt dem getroffenen Spieler Ber�hrungsschaden zu.
    /// </summary>
    /// <param name="playerTag"></param>
    private void DealTouchDamage(string playerTag)
    {
        switch (playerTag)
        {
            case "Player1":
                {
                    OnScoreUpdate?.Invoke(this, new ScoreUpdateEventArgs { playerTag = "Player1", score = -currentBall.BallData.TouchDamage, type = ScoreType.Touch});
                    break;
                }
            case "Player2":
                {
                    OnScoreUpdate?.Invoke(this, new ScoreUpdateEventArgs { playerTag = "Player2", score = -currentBall.BallData.TouchDamage, type = ScoreType.Touch });
                    break;
                }
        }
    }

    /// <summary>
    /// F�gt dem getroffenen Spieler Torschaden zu.
    /// </summary>
    /// <param name="zoneTag"></param>
    private void DealGoalDamage(string zoneTag)
    {
        switch (zoneTag)
        {
            case "Player1Zone":
                {
                    OnScoreUpdate?.Invoke(this, new ScoreUpdateEventArgs { playerTag = "Player1", score = -currentBall.BallData.GoalDamage, type = ScoreType.Goal });
                    break;
                }
            case "Player2Zone":
                {
                    OnScoreUpdate?.Invoke(this, new ScoreUpdateEventArgs { playerTag = "Player2", score = -currentBall.BallData.GoalDamage, type = ScoreType.Goal });
                    break;
                }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ist der Ball in eine der Torzonen gelangt wird er zur�ckgesetzt und der Spieler erh�lt Schaden.
        if (collision.gameObject.CompareTag("Player1Zone") || collision.gameObject.CompareTag("Player2Zone"))
        {
            ResetBall(collision.gameObject.tag);
            DealGoalDamage(collision.gameObject.tag);
        }

        // Ist der Ball mit einem Item kolliediert wird dieses dem zuletzt getroffenem Spieler ins Inventar �bergeben und von der Karte entfernt.
        if (collision.gameObject.CompareTag("Item") && lastPlayer != null)
        {
            var item = collision.gameObject.GetComponent<Item>();
            OnItemCollected?.Invoke(this, new ItemCollectionEventArgs { item = new InventorySlot { Ball = item.itemGO, Usages = item.usages }, playerTag = lastPlayer });

            Destroy(collision.gameObject);
        }
    }
}
