using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ball", menuName = "Custom/Ball")]
public class BallSO : ScriptableObject
{
    /// <summary>
    /// Name des Balls.
    /// </summary>
    [field: SerializeField] public string BallName { get; private set; }
    /// <summary>
    /// Sprite des Balls.
    /// </summary>
    [field: SerializeField] public Sprite BallSprite { get; private set; }
    /// <summary>
    /// Die anfängliche Geschwindigkeit bei Instanziierung.
    /// </summary>
    [field: SerializeField] public float InitialSpeed { get; private set; }
    /// <summary>
    /// Der Geschwindigkeitszuwachs mit jedem Treffer.
    /// </summary>
    [field: SerializeField] public float SpeedIncrease { get; private set; }
    /// <summary>
    /// Der Schaden, der Spielern bei Berührung zugeefügt wird.
    /// </summary>
    [field: SerializeField] public int TouchDamage { get; private set; }
    /// <summary>
    /// Der Schaden, der Spielern bei einem Tor zugefügt wird.
    /// </summary>
    [field: SerializeField] public int GoalDamage { get; private set; }
}
