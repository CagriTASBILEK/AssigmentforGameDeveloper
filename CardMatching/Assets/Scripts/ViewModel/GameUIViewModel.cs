using System;
using System.Collections;
using Events;
using Managers;
using Model;
using ScriptableObjects;
using UnityEngine;

namespace ViewModel
{
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
        public int HighScore => model?.HighScore ?? 0;
        public int TotalScore => model?.TotalScore ?? 0;
    
        private string selectedDifficultyName;
        private GameDifficulty selectedDifficulty;
    
        public void SetSelectedDifficulty(string difficultyName)
        {
            selectedDifficultyName = difficultyName;
            var config = model?.GameConfig.GetGridConfig(difficultyName);
            if (config != null)
            {
                selectedDifficulty = config.difficulty;
            }
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
            return config.difficultyName; 
        }

        public void StartGame()
        {
            if (isInitialized && !string.IsNullOrEmpty(selectedDifficultyName))
            {
                gameManager.StartGame(selectedDifficultyName);
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
}
