using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

/// <summary>
/// Eine abstrakte Basisklasse, die von den Controllern der Spieler sowie dem Computergegner implementiert wird.
/// </summary>
/// Ermöglicht den Zugriff auf das PlayerBounceEvent unabhängig vom Typ des Controllers.
public abstract class BasePlayerController : MonoBehaviour 
{
    public abstract event EventHandler<PlayerBounceEventArgs> OnPlayerBounce;
}

public class PlayerBounceEventArgs
{
    public Collision2D collision;
    public GameObject player;
    public PlayerSO playerData;
}
