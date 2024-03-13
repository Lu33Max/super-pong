using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "GameState", menuName = "Custom/Game")]
public class GameStateSO : ScriptableObject
{
    [field: SerializeField] public GameStates GameState;
    [field: SerializeField] public HealthStates HealthState;

    public void ResetGameState()
    {
        GameState = GameStates.StartState;
        HealthState = HealthStates.FullHealth;
    }
}

public enum GameStates { PlayState = 0, PauseState = 1, EndState = 2, StartState = 4 }
public enum HealthStates { FullHealth = 0, LowHealth = 1, CriticalHealth = 2 }
