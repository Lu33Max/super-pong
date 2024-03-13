using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dient zum Einstellen der Lebensanzeige.
/// </summary>
public class HealthBar : MonoBehaviour
{
    public Slider slider;

    [SerializeField] private Image fillImage;
    [SerializeField] private Color baseColor;
    [SerializeField] private Color lowColor;
    [SerializeField] private Color criticalColor;

    /// <summary>
    /// Setzt den Maximalwert der Lebensleiste auf die maximalen Spielerleben.
    /// </summary>
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    /// <summary>
    /// Setzt den aktuellen Wert der Lebensleiste auf den aktuellen Wert des Spielerlebens.
    /// </summary>
    public void SetHealth(int health)
    {
        slider.value = health;

        // Ändert die Farbe der Lebensleiste entsprechend der aktuellen Lebenspunkte.
        if (health <= slider.maxValue / 10)
        {
            SetColor(HealthStates.CriticalHealth);
        }
        else if (health <= slider.maxValue / 2)
        {
            SetColor(HealthStates.LowHealth);
        }
        else
        {
            SetColor(HealthStates.FullHealth);
        }
    }

    /// <summary>
    /// Passt die Farbe der Lebensleiste den noch übrigen Lebenspunkten an.
    /// </summary>
    /// <param name="state">Der aktuelle Status der Spielerleben</param>
    private void SetColor(HealthStates state)
    {
        switch(state)
        {
            case HealthStates.FullHealth:
                fillImage.color = baseColor;
                break;
            case HealthStates.LowHealth:
                fillImage.color = lowColor;
                break;
            case HealthStates.CriticalHealth:
                fillImage.color = criticalColor;
                break;
        }
    }
}
