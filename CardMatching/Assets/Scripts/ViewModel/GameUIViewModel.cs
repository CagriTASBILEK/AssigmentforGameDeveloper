using System;
using System.Collections;
using UnityEngine;

public class GameUIViewModel : MonoBehaviour
{
    private GameUIModel model;
    private GameManager gameManager;
    private bool isInitialized;
    
    public event Action<int> OnScoreUpdated;
    public event Action<int> OnComboUpdated;
    public event Action<GameState> OnGameStateChanged;
    public event Action OnInitialized;
    
    public GameConfig.GridConfig[] DifficultyConfigs => model.GameConfig.gridConfigs;
    
    public GameDifficulty SelectedDifficulty
    {
        get => model?.CurrentDifficulty ?? GameDifficulty.Easy;
        set { if (model != null) model.SetDifficulty(value); }
    }
    private void Start()
    {
        StartCoroutine(InitializeWhenReady());
    }
    private IEnumerator InitializeWhenReady()
    {
       
        while (GameManager.Instance == null || GameManager.Instance.Config == null)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        model = new GameUIModel();
        if (model.Initialize())
        {
            gameManager = GameManager.Instance;
            SubscribeToEvents();
            isInitialized = true;
            OnInitialized?.Invoke();
        }
        else
        {
            Debug.LogError("Failed to initialize GameUIModel");
        }
    }
    
    

    private void SubscribeToEvents()
    {
        GameEvents.OnScoreChanged += score => OnScoreUpdated?.Invoke(score);
        GameEvents.OnComboChanged += combo => OnComboUpdated?.Invoke(combo);
        GameEvents.OnGameStateChanged += state => OnGameStateChanged?.Invoke(state);
    }

    public string GetDifficultyText(GameConfig.GridConfig config)
    {
        return model.GetDifficultyDescription(config);
    }

    public void StartGame()
    {
        if (isInitialized)
        {
            gameManager.StartGame(SelectedDifficulty);
        }
    }

    public void PauseGame()
    {
        if (isInitialized)
        {
            gameManager.PauseGame();
        }
    }
    public void ResumeGame()
    {
        if (isInitialized)
        {
            gameManager.ResumeGame();
        }
    }
    public void ReturnToMainMenu()
    {
        if (isInitialized && gameManager != null)
        {
            gameManager.CleanupGame();
            GameEvents.InvokeGameStateChanged(GameState.MainMenu);
        }
    }
    private void OnDestroy()
    {
        if (model != null)
        {
            GameEvents.OnScoreChanged -= score => OnScoreUpdated?.Invoke(score);
            GameEvents.OnComboChanged -= combo => OnComboUpdated?.Invoke(combo);
            GameEvents.OnGameStateChanged -= state => OnGameStateChanged?.Invoke(state);
        }
    }
}
