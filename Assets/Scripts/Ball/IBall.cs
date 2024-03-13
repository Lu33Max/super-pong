using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allgemeines Interface, welches von jeder Ballvariante implementiert wird. Legt einige Standardfunktionen und -eigenschaften fest, die über die BallController gesteuert werden.
/// </summary>
public interface IBall
{
    /// <summary>
    /// Werte des Balls.
    /// </summary>
    public BallSO BallData { get; set; }
    /// <summary>
    /// Wird bei der Instanziierung aufgerufen.
    /// </summary>
    public void OnInstantiate(PlayerSO playerData = null);
    /// <summary>
    /// Bestimmt, wie sich der Ball bewegen soll.
    /// </summary>
    /// <param name="currentDirection">Die aktuelle Richtung, in die sich der Ball bisher bewegt hat.</param>
    /// <param name="hitCounter">Die Anzahl an Spielertreffern.</param>
    /// <returns>Absolute Bewegungsrichtung als Vector2 mit Geschwindigkeit bereits einbezogen.</returns>
    public Vector2 Move(Vector2 currentDirection, int hitCounter);
    /// <summary>
    /// Wird während der normalen Update-Methode aufgerufen.
    /// </summary>
    public void RegularUpdate();
    /// <summary>
    /// Wird aufgerufen, bevor das GameObject zerstört wird.
    /// </summary>
    public void Destroy();
    /// <summary>
    /// Bestimmt die weitere Bewegungsrichtung nach der Kollision mit einem Spieler.
    /// </summary>
    /// <param name="player">GameObject des getroffenen Spielers.</param>
    /// <param name="currentDirection">Die aktuelle Richtung, in die sich der Ball bisher bewegt hat.</param>
    /// <param name="hitCounter">Die Anzahl an Spielertreffern.</param>
    /// <returns>Absolute Bewegungsrichtung als Vector2 mit Geschwindigkeit bereits einbezogen.</returns>
    public Vector2 OnPlayerBounce(GameObject player, Vector2 currentDirection, int hitCounter);
    /// <summary>
    /// Wird aufgerufen, wenn der Ball eine Trefferzone betritt.
    /// </summary>
    public void OnZoneEnter(Collision2D collision);
}
