using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dient der Auswahl der Leben für beide Spieler im Hauptmenü.
/// </summary>
public class LifeSelection : MonoBehaviour
{
    [SerializeField] private bool isPlayer1;
    [SerializeField] private GameObject[] buttons;

    private void OnEnable()
    {
        RecolorButtons();
    }

    /// <summary>
    /// Weist den Wert des Buttons den maximalen Leben des Spielers zu.
    /// </summary>
    /// <param name="newLifes"></param>
    public void ChangeSelection(int newLifes)
    {
        if (isPlayer1)
        {
            MatchData.Player1Lifes = newLifes;
        }
        else
        {
            MatchData.Player2Lifes = newLifes;
        }

        RecolorButtons();
    }

    /// <summary>
    /// Setzt den Wert der Lebenspunkte auf den Standardwert, festgelegt innerhlab der MacthData, zurück.
    /// </summary>
    public void ResetToDefault()
    {
        MatchData.Player1Lifes = MatchData.Player1LifesDefault;
        MatchData.Player2Lifes = MatchData.Player2LifesDefault;

        RecolorButtons();
    }

    /// <summary>
    /// Sorgt dafür, dass der zuletzt geklickte Button farblich hervorgehoben wird, um die Auswahl des Spielers zu visualisieren.
    /// </summary>
    private void RecolorButtons()
    {
        foreach (var button in buttons)
        {
            if (button.GetComponentInChildren<TextMeshProUGUI>().text == (isPlayer1 ? MatchData.Player1Lifes.ToString() : MatchData.Player2Lifes.ToString()))
            {
                button.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                button.GetComponent<Image>().color = Color.white;
            }
        }
    }
}
