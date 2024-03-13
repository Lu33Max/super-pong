using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour, IBall
{
    [SerializeField] private BallSO _ballData;

    public BallSO BallData { get => _ballData; set => _ballData = value; }

    public void Destroy()
    {
    }

    public void OnInstantiate(PlayerSO playerData = null)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _ballData.BallSprite;
    }

    public Vector2 OnPlayerBounce(GameObject player, Vector2 currentDirection, int hitCounter)
    {
        // Bestimmt die neue y-Richtung auf Basis der Distanz zum Mitteplpunkt des Spielers.
        //Trifft der Ball mittig, bewegt er sich auf einer geraden Linie. Je weiter auﬂen deer Treffer, umso steiler der Winkel.
        Vector2 ballPos = transform.parent.transform.position;
        Vector2 playerPos = player.transform.position;

        float xDirection, yDirection;

        xDirection = -currentDirection.x;
        yDirection = (ballPos.y - playerPos.y) / player.GetComponent<Collider2D>().bounds.size.y;

        return new Vector2(xDirection, yDirection) * (_ballData.InitialSpeed + (_ballData.SpeedIncrease * hitCounter));
    }

    public void OnZoneEnter(Collision2D collision)
    {
    }

    public void RegularUpdate()
    {
    }

    public Vector2 Move(Vector2 currentVelocity, int hitCounter)
    {
        // Bewegt den Ball auf einer geraden Linie und hoher Geschwindigkeit.
        return new Vector2(currentVelocity.x, 0).normalized * _ballData.InitialSpeed;
    }
}
