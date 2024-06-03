using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;


public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    
    public TMP_Text scoreText;
    public GameObject endGameMenu;
    public GameObject pauseGameMenu;
    public List<Image> lifeSprites;
    public int scoreRockWorth = 50;
    public int scoreAlienWorth = 100;

    private bool _isGameStopped = false;
    private int _score = 0;
    private int _nextLifeIndex;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        scoreText.text = _score.ToString();
        _nextLifeIndex = lifeSprites.Count - 1;
        endGameMenu.gameObject.SetActive(false);
        pauseGameMenu.SetActive(false);
    }

    public void IncreaseScore(int increment)
    {
        _score += increment;
        scoreText.text = _score.ToString();
    }

    public void TakeLife()
    {
        if (_nextLifeIndex < lifeSprites.Count && _nextLifeIndex >= 0)
        {
            lifeSprites[_nextLifeIndex].enabled = false;
            _nextLifeIndex -= 1;
            if (_nextLifeIndex < 0)
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
        endGameMenu.gameObject.SetActive(true);
    }
    
    public void Reset()
    {
        _score = 0;
        foreach (var sprite in lifeSprites)
        {
            sprite.enabled = true;
        }

        scoreText.text = _score.ToString();
        _nextLifeIndex = lifeSprites.Count - 1;
        endGameMenu.gameObject.SetActive(false);
        
        PlayerController.instance.Reset();
        ObstaclesSpawner.instance.Reset();
    }
    
    public void TransferToMainMenu()
    {
        _isGameStopped = false;
        pauseGameMenu.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public bool isGamePaused()
    {
        return _isGameStopped;
    }

    public void SwitchPause()
    {
        if (!_isGameStopped)
        {
            _isGameStopped = true;
            pauseGameMenu.SetActive(true);
        }
        else
        {
            _isGameStopped = false;
            pauseGameMenu.SetActive(false);
        }
    }
}
