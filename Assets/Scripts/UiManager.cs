using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    
    public TMP_Text scoreText;
    public Button newGameButton;
    public List<Image> lifeSprites;
    public int scoreRockWorth = 10;
    public int scoreAlienWorth = 20;
    
    private int score = 0;
    private int nextLifeIndex;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        scoreText.text = score.ToString();
        nextLifeIndex = lifeSprites.Count - 1;
        newGameButton.gameObject.SetActive(false);
    }

    public void IncreaseScore(int increment)
    {
        score += increment;
        scoreText.text = score.ToString();
    }

    public void TakeLife()
    {
        if (nextLifeIndex < lifeSprites.Count && nextLifeIndex >= 0)
        {
            lifeSprites[nextLifeIndex].enabled = false;
            nextLifeIndex -= 1;
            if (nextLifeIndex < 0)
            {
                SetupDeathScreen();
            }
        }
    }

    public void RestartGame()
    {
        Reset();
    }
    
    public void SetupDeathScreen()
    {
        newGameButton.gameObject.SetActive(true);
    }
    
    public void Reset()
    {
        score = 0;
        foreach (var sprite in lifeSprites)
        {
            sprite.enabled = true;
        }

        score = 0;
        scoreText.text = score.ToString();
        nextLifeIndex = lifeSprites.Count - 1;
        newGameButton.gameObject.SetActive(false);
        
        PlayerController.instance.Reset();
        ObstaclesSpawner.instance.Reset();
    }
}
