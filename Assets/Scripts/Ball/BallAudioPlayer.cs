using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Zust�ndig f�r die Soundeffekte des Balls. Erh�lt vordefinierte AudioClips und spielt diese in entsprechenden Situationen.
/// </summary>
public class BallAudioPlayer : MonoBehaviour
{
    /// <summary>
    /// SFX beim Ber�hren von W�nden oder Spielern.
    /// </summary>
    private AudioSource pongSFX;

    private void Start()
    {
        pongSFX = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        pongSFX.Play();
    }
}
