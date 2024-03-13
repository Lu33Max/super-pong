using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Spawner : MonoBehaviour, IBall
{
    [SerializeField] private BallSO _ballData;
    [SerializeField] private GameObject obstaclePrefab;

    private Vector2 lastSpawnPosition;

    public BallSO BallData { get => _ballData; set => _ballData = value; }

    public void Destroy()
    {
    }

    public void OnInstantiate(PlayerSO playerData = null)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _ballData.BallSprite;

        // Setzt den ersten Punkt zur Berechnung der Hindernis-Sawnpunkte auf den Ort der Instanziierung
        lastSpawnPosition = transform.position;
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

    /// <summary>
    /// L‰sst ein neues Obstacle erscheinen, sobald der Ball weit genug von dem alten entfernt ist
    /// </summary>
    public void RegularUpdate()
    {
        // Zus‰tzliche Bedingung der x-Koordinate verhindert, dass ein Hindernis im Bewegungsbereich der Spieler erscheint
        if(Vector2.Distance(transform.position, lastSpawnPosition) > 4f && Mathf.Abs(transform.position.x) < 5)
        {
            lastSpawnPosition = transform.position;
            StartCoroutine(SpawnObstacle());
        }
    }

    public Vector2 Move(Vector2 currentVelocity, int hitCounter)
    {
        // Bewegt sich normal in die aktuelle Richtung mit gleichbleibender Geschwindigkeitt,
        return currentVelocity.normalized * (_ballData.InitialSpeed + (_ballData.SpeedIncrease * hitCounter));
    }

    /// <summary>
    /// L‰sst das Hindernis an der neuen Position erscheinen, sobald der Ball weit genug weg ist, damit er nicht in das HIndernis eindringt
    /// </summary>
    private IEnumerator SpawnObstacle()
    {
        yield return new WaitUntil(() => Vector2.Distance(transform.position, lastSpawnPosition) > 1f);

        Instantiate(obstaclePrefab, lastSpawnPosition, transform.rotation);
    }
}
