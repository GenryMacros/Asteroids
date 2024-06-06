using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class HoverIconButton : MonoBehaviour
{
    public Color activeColor;
    public Color inactiveColor;
    public AudioClip clip;
    
    public TMP_Text text;
    public Image icon;

    private AudioSource _audioSource;
    
    private void Start()
    {
        icon.enabled = false;
        text.color = inactiveColor;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = clip;
        _audioSource.Stop();
    }
    
    public void OnPointerEnter(BaseEventData data)
    {   
        _audioSource.Play();
        icon.enabled = true;
        text.color = activeColor;
    }
    
    public void OnPointerExit(BaseEventData data)
    {
        icon.enabled = false;
        text.color = inactiveColor;
    }

}
