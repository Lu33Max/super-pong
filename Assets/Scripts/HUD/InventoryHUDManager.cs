using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryHUDManager : MonoBehaviour
{
    private UIDocument uiInventory;
    
    // Daten des zugeordneten Spielers
    [SerializeField] private PlayerSO playerData;
    [SerializeField] private GameObject player;

    // Templates für aktiven und inaktiven Zustand
    [SerializeField] private VisualTreeAsset basePanelTemplate;
    [SerializeField] private VisualTreeAsset selectedPanelTemplate;

    private readonly List<UIInventorySlot> slots = new();

    private void Start()
    {
        // Hinzufügen der Funktionen zu den Events
        var ballSwitch = player.GetComponent<BallSwitch>();

        ballSwitch.OnBallRemoved += RemovePanel;
        ballSwitch.OnBallUpdated += UpdatePanel;
        ballSwitch.OnBallSwitched += SwitchSelection;

        var itemCollector = player.GetComponent<ItemCollector>();

        itemCollector.OnBallUpdated += UpdatePanel;
        itemCollector.OnBallAdded += AddPanel;

        uiInventory = GetComponent<UIDocument>();

        // Zu Beginn der Runde wird über das gesamte Inventar des Spielers geloopt. Der erste Ball wird als aktiv festgelegt und alle weiten als inaktiv.
        for (int i = 0; i < playerData.Inventory.Count; i++)
        {
            if (i != 0)
            {
                UIInventorySlot newSlot = new(playerData.Inventory[i], basePanelTemplate, selectedPanelTemplate, false);
                slots.Add(newSlot);
                uiInventory.rootVisualElement.Q("BallColumn").Add(newSlot.basePanel);
            }
            else
            {
                UIInventorySlot newSlot = new(playerData.Inventory[i], basePanelTemplate, selectedPanelTemplate, true);
                slots.Add(newSlot);
                uiInventory.rootVisualElement.Q("BallColumn").Add(newSlot.selectedPanel);
            }
        }
    }

    /// <summary>
    /// Aktualisiert die Anzahl der angezeigten Nutzungen eines Balls.
    /// </summary>
    /// <param name="e">Index übermittelt die Stelle der Liste, die aktualisiert werden muss.</param>
    private void UpdatePanel(object sender, BallSwitch.BallUpdatesEventArgs e)
    {
        // Aktualisiert zunächst die beiden Panels des Elements
        slots[e.index].basePanel.Q<Label>().text = playerData.Inventory[e.index].Usages.ToString() + "x";
        slots[e.index].selectedPanel.Q<Label>().text = playerData.Inventory[e.index].Usages.ToString() + "x";

        // Aktualisiert nun den angezeigten Text, abhängig davon, welchen Status das angezeigte Panel hat (aktiv/inaktiv)
        uiInventory.rootVisualElement.Q("BallColumn")[e.index].Q<Label>().text = slots[e.index].selected ? slots[e.index].selectedPanel.Q<Label>().text : slots[e.index].basePanel.Q<Label>().text;
    }

    /// <summary>
    /// Entfernt ein Panel, wenn der zugehörige Ball aus dem Inventar entfernt wurde.
    /// </summary>
    /// <param name="e">Index übermittelt die Stelle, die entfernt werden muss.</param>
    private void RemovePanel(object sender, BallSwitch.BallUpdatesEventArgs e)
    {
        uiInventory.rootVisualElement.Q("BallColumn").RemoveAt(e.index);
        slots.RemoveAt(e.index);
    }

    /// <summary>
    /// Fügt ein neues Panel hinzu, sobald ein neuer Ball dem Inventar hinzugefügt wurde.
    /// </summary>
    /// <param name="e">Inventareintrag des neuen Balls.</param>
    private void AddPanel(object sender, ItemCollector.BallAddEventArgs e)
    {
        UIInventorySlot newSlot = new(e.ballToAdd, basePanelTemplate, selectedPanelTemplate, false);

        uiInventory.rootVisualElement.Q("BallColumn").Add(newSlot.basePanel);
        slots.Add(newSlot);
    }

    /// <summary>
    /// Wechselt die Anzeige des aktuell ausgewählten Balls.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">Übbermittelt den zuletzt ausgewählten sowie den neu ausgewählten Index.</param>
    private void SwitchSelection(object sender, BallSwitch.BallSwitchEventArgs e)
    {
        // Insofern der letzte Index existiert, der zugehörige Ball also nicht gelöscht wurde, wird das Panel an dieser Stelle entfernt und mit einem inaktiven ausgetauscht.
        if(e.lastIndex != null)
        {
            uiInventory.rootVisualElement.Q("BallColumn").RemoveAt(e.lastIndex ?? 0);
            uiInventory.rootVisualElement.Q("BallColumn").Insert(e.lastIndex ?? 0, slots[e.lastIndex ?? 0].basePanel);
            slots[e.lastIndex ?? 0].selected = false;
        }

        // Wechselt das Design des neu ausgewählten Panels von inaktiv zu aktiv.
        uiInventory.rootVisualElement.Q("BallColumn").RemoveAt(e.currentIndex);
        uiInventory.rootVisualElement.Q("BallColumn").Insert(e.currentIndex, slots[e.currentIndex].selectedPanel);
        slots[e.currentIndex].selected = true;
    }
}
