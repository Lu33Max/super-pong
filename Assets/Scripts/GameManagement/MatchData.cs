using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchData : MonoBehaviour
{
    public static int Player1Lifes { get; set; } = 50;
    public static int Player2Lifes { get; set; } = 50;

    public static int Player1LifesDefault { get; } = 50;
    public static int Player2LifesDefault { get; } = 50;

    public static bool IsMultiPlayer { get; set; } = false;

    public static int? SelectedMap { get; set; } = null;
}
