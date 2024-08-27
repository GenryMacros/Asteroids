using System.Collections.Generic;
using UnityEngine;

public class UICanvasControllerInput : MonoBehaviour
{

    [Header("Output")]
    public PlayerController inputs;
    
    public void VirtualMoveInput(Vector2 virtualMoveDirection)
    {
        if (UiManager.instance.isGamePaused() || UiManager.instance.endGameMenu.activeSelf)
        {
            return;
        }
        //virtualMoveDirection.Normalize();
        if (virtualMoveDirection == Vector2.zero)
        {
            inputs.MoveJoysticStop();
        }
        else
        {
            inputs.MoveJoystic(virtualMoveDirection);   
        }
    }
    
    public void VirtualShootInput(bool virtualShootState)
    {
        if ( UiManager.instance.isGamePaused() || UiManager.instance.endGameMenu.activeSelf)
        {
            return;
        }
        inputs.Fire();
    }

    public void VirtualPauseInput(bool virtualQuickState)
    {
        if (virtualQuickState && !UiManager.instance.endGameMenu.activeSelf)
        { 
            UiManager.instance.SwitchPause();
        }
    }
}