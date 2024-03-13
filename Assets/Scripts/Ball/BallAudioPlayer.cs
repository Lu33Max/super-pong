using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Zuständig für die Soundeffekte des Balls. Erhält vordefinierte AudioClips und spielt diese in entsprechenden Situationen.
/// </summary>
public class BallAudioPlayer : MonoBehaviour
{
    /// <summary>
    /// SFX beim Berühren von Wänden oder Spielern.
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
