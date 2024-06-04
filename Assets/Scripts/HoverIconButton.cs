using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverIconButton : MonoBehaviour
{
    public Color activeColor;
    public Color inactiveColor;
    
    public TMP_Text text;
    public Image icon;

    private void Start()
    {
        icon.enabled = false;
        text.color = inactiveColor;
    }
    
    public void OnPointerEnter(BaseEventData data)
    {   
        icon.enabled = true;
        text.color = activeColor;
    }
    
    public void OnPointerExit(BaseEventData data)
    {
        icon.enabled = false;
        text.color = inactiveColor;
    }

}
