using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enthält verschiedene Werte der Spieler.
/// </summary>
[CreateAssetMenu(fileName = "Player", menuName = "Custom/Player")]
public class PlayerSO : ScriptableObject
{
    [field: SerializeField] public GameObject DefaultBall { get; set; }
    [field: SerializeField] public float Velocity { get; set; }
    [field: SerializeField] public int MaxLife { get; set; }
    [field: SerializeField] public int CurrentLife { get; set; }
    [field: SerializeField] public List<InventorySlot> Inventory { get; set; }
    [field: SerializeField] public GameObject SelectedBall { get; set; }


    /// <summary>
    /// Zurücksetzen der Spielerwerte zu Beginn des Spiels durch den GameManager
    /// </summary>
    public void ResetStats()
    {
        Inventory.Clear();
        Inventory.Add(new InventorySlot { Ball = DefaultBall, Usages = -1 });

        CurrentLife = MaxLife;
    }
}

/// <summary>
/// Custom-Klasse zum Speichern der Bälle im Spielerinventar mitsamt deren Nutzungen.
/// </summary>
[Serializable]
public class InventorySlot
{
    [field: SerializeField] public GameObject Ball { get; set; }
    [field: SerializeField] public int Usages;
}
