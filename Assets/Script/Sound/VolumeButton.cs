using UnityEngine;
using UnityEngine.UI;

public class VolumeButton : MonoBehaviour
{
    public enum ButtonType
    {
        Music,
        Effects
    }

    public ButtonType buttonType;
    public Sprite onSprite;
    public Sprite offSprite;

    private Image buttonImage;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        UpdateVisual();
    }

    public void ToggleVolume()
    {
        if (VolumeManager.Instance != null)
        {
            if (buttonType == ButtonType.Music)
            {
                VolumeManager.Instance.ToggleMusic();
            }
            else if (buttonType == ButtonType.Effects)
            {
                //VolumeManager.Instance.ToggleEffects();
            }
            UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
        if (VolumeManager.Instance != null)
        {
            if (buttonType == ButtonType.Music)
            {
                buttonImage.sprite = VolumeManager.Instance.musicOn ? onSprite : offSprite;
            }
            else if (buttonType == ButtonType.Effects)
            {
                //buttonImage.sprite = VolumeManager.Instance.effectsOn ? onSprite : offSprite;
            }
        }
    }
}