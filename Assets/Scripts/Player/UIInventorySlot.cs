using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// Beiinhaltet Informationen �ber ein eiziges UI-Element des Spielerinventars dar. Wird f�r jeden Ball im Inventar neu instanziiert.
/// </summary>
/// Grundidee entstammt folgendem Video. Alle UI Elemente wurde entsprechend der Anwendung in ihren Eigenschaften, Aussehen und Verhalten angepasst, sodass au�er der Grundidee kein weiterer
/// Code direkt �bernommen wurde.
/// Quelle: https://www.youtube.com/watch?v=GEHiGpD0NBs
public class UIInventorySlot
{
    /// <summary>
    /// Template f�r das normale Aussehen, wenn der Ball nicht ausgew�hlt wurde.
    /// </summary>
    public VisualElement basePanel;
    /// <summary>
    /// Template f�r das Aussehen, wenn der Ball gerade ausgew�hlt ist.
    /// </summary>
    public VisualElement selectedPanel;

    /// <summary>
    /// Gibt an, ob der zugeh�rige Ball gerade ausgew�hlt ist.
    /// </summary>
    public bool selected;

    public UIInventorySlot(InventorySlot slot, VisualTreeAsset baseTemplate, VisualTreeAsset selectedTemplate, bool selected)
    {
        TemplateContainer ballPanelContainer = baseTemplate.Instantiate();
        TemplateContainer selectedPanelContainer = selectedTemplate.Instantiate();

        this.selected = selected;

        basePanel = ballPanelContainer;
        selectedPanel = selectedPanelContainer;

        var ballData = slot.Ball.GetComponent<IBall>().BallData;

        // Setzt die Text-Elemente auf die Anzahl der Nutzungen des Balls sowie die Bild-Elemente auf den zugeh�rigen Sprite
        basePanel.Q<Label>().text = ballData.BallName == "Default" ? "\u221E" : slot.Usages.ToString() + "x";
        basePanel.Q<IMGUIContainer>().style.backgroundImage = new StyleBackground(ballData.BallSprite);

        selectedPanel.Q<Label>().text = ballData.BallName == "Default" ? "\u221E" : slot.Usages.ToString() + "x";
        selectedPanel.Q<IMGUIContainer>().style.backgroundImage = new StyleBackground(ballData.BallSprite);
    }
}
