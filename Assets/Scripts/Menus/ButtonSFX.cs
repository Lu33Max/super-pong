using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    [SerializeField] private AudioClip hoverSFX;
    [SerializeField] private AudioClip pressedSFX;
    [SerializeField] private AudioClip gameStartSFX;
    [SerializeField] private AudioSource audioSource;
    
    public void OnHover()
    {
        audioSource.PlayOneShot(hoverSFX);
    }

    public void OnClick()
    {
        audioSource.PlayOneShot(pressedSFX);
    }

    public void OnGameStart()
    {
        audioSource.PlayOneShot(gameStartSFX);
    }
}
