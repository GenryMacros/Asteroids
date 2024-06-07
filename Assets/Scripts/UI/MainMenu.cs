using UnityEngine;


public class MainMenu : MonoBehaviour
{
    public void Start()
    {
        if (UiManager.instance)
        {
            UiManager.instance._isGameStopped = false;   
        }
    }

    public void StartGame()
    {
        SceneController.instance.ToGame();
    }
    
    public void CloseGame()
    {
        Application.Quit();
    }
    
}
