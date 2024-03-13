using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kümmert sich um das Setup der Stage sowie die Wechsel zwischen den verschiedenen Game States
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip baseMusic;
    [SerializeField] private AudioClip lowHealthMusic;
    [SerializeField] private AudioClip criticalHealthMusic;
    [SerializeField] private AudioClip endScreenMusic;

    [Header("SFX")]
    [SerializeField] private AudioClip roundStartSFX;
    [SerializeField] private AudioClip lowHealthSFX;
    [SerializeField] private AudioClip gameOverSFX;

    [Header("Data")]
    [SerializeField] private List<PlayerSO> playerData;
    [SerializeField] private GameStateSO gameData;

    [Header("Player")]
    [SerializeField] private GameObject player2;
    [SerializeField] private GameObject aiPlayer;
    [SerializeField] private GameObject player2Inventory;
    [SerializeField] private GameObject aiPlayerInventory;

    [Header("Stage")]
    [SerializeField] private List<GameObject> stages;

    [Header("Round Start")]
    [SerializeField] private GameObject textReady;
    [SerializeField] private GameObject textGo;

    private AudioSource _audioSource;
    private int currentAudioIndex = -1;
    private int currentMap;

    // Die Einstellungen für das Spiel werden angewendet und SOs der Spieler bzw. des Spielfelds auf ihren Ausgangszustand zurückgesetzt
    private void Awake()
    {
        ApplyMatchSettings();
        ResetPlayerData();
        gameData.ResetGameState();
        Time.timeScale = 1.0f;
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.PlayOneShot(roundStartSFX);

        textReady.SetActive(true);

        StartCoroutine(HideReady());
        StartCoroutine(ShowGo());
        StartCoroutine(HideGo());
    }

    private void Update()
    {
        PlayMusic();
    }

    private IEnumerator HideReady()
    {
        yield return new WaitForSeconds(1.3f);

        textReady.SetActive(false);
    }

    private IEnumerator ShowGo()
    {
        yield return new WaitForSeconds(2.8f);

        textGo.SetActive(true);
    }

    private IEnumerator HideGo()
    {
        yield return new WaitForSeconds(3.5f);

        textGo.SetActive(false);
    }

    /// <summary>
    /// Wendet die im Menü getroffenen Einstellungen zu Spielerzahl, Lebenspunkten und ausgewählter Karte an.
    /// </summary>
    private void ApplyMatchSettings()
    {
        if(MatchData.IsMultiPlayer)
        {
            player2.SetActive(true);
            player2Inventory.SetActive(true);
        }
        else
        {
            aiPlayer.SetActive(true);
            aiPlayerInventory.SetActive(true);
        }

        foreach(PlayerSO player in playerData)
        {
            if(player.name == "Player")
            {
                player.MaxLife = MatchData.Player1Lifes;
            }
            else
            {
                player.MaxLife = MatchData.Player2Lifes;
            }
        }

        if(MatchData.SelectedMap != null)
        {
            stages[MatchData.SelectedMap ?? 0].SetActive(true);
            currentMap = MatchData.SelectedMap ?? 0;
        }
        else
        {
            int stage = Mathf.FloorToInt(Random.Range(0f, stages.Count - Mathf.Epsilon));
            stages[stage].SetActive(true);
            currentMap = stage;
        }
    }

    /// <summary>
    /// Die Daten jedes SpielerSOs werden zurüclgesetzt
    /// </summary>
    private void ResetPlayerData()
    {
        foreach (PlayerSO player in playerData)
        {
            player.ResetStats();
        }
    }

    /// <summary>
    /// Abhängig vom aktuellen State des Spielverlaufs ändert sich gespielte Musik
    /// </summary>
    private void PlayMusic()
    {
        if(gameData.GameState == GameStates.PauseState)
        {
            _audioSource.volume = 0.3f;
        }
        else
        {
            _audioSource.volume = 0.8f;
        }

        // Die Musik während dem Spiel wird mit sinkenden Leben der Spieler immer dramatischer.
        if(gameData.GameState == GameStates.PlayState)
        {
            if(currentAudioIndex == -1)
            {
                currentAudioIndex = 0;
                _audioSource.clip = baseMusic;
                _audioSource.Play();
            }

            if (currentAudioIndex == 0 && gameData.HealthState == HealthStates.LowHealth)
            {
                _audioSource.Stop();
                _audioSource.PlayOneShot(lowHealthSFX);
                _audioSource.clip = lowHealthMusic;
                _audioSource.Play();
                currentAudioIndex = 1;
            }
            else if (currentAudioIndex == 1 && gameData.HealthState == HealthStates.CriticalHealth)
            {
                _audioSource.Stop();
                _audioSource.PlayOneShot(lowHealthSFX);
                _audioSource.clip = criticalHealthMusic;
                _audioSource.Play();
                currentAudioIndex = 2;
            }
        }
        // Ist das Spieler beendet setzt die Musik des Game Over Bildschirm ein
        else if(gameData.GameState == GameStates.EndState)
        {
            if(currentAudioIndex != 4)
            {
                _audioSource.Stop();
                _audioSource.PlayOneShot(gameOverSFX);
                _audioSource.clip = endScreenMusic;
                _audioSource.Play();
                currentAudioIndex = 4;
            }
        }
    }

    #region GETTER and SETTER
    public GameStates GetGameState()
    {
        return gameData.GameState;
    }

    public void SetGameState(GameStates newState)
    {
        gameData.GameState = newState;
    }

    public HealthStates GetHealthState()
    {
        return gameData.HealthState;
    }

    public void SetHealthState(HealthStates newState)
    {
        gameData.HealthState = newState;
    }

    public int GetSelectedStage()
    {
        return currentMap;
    }
    #endregion
}
