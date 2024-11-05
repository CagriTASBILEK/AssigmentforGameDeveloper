using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private CardSpawner cardSpawner;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = false;

    [SerializeField] private GameState currentGameState;
    private string currentDifficultyName;
    private GameDifficulty currentDifficulty;
    private Card[] activeCards;
    private Card firstSelectedCard;
    private int currentScore;
    private int currentCombo;
    private float lastMatchTime;
    private bool isProcessingMatch;

    public GameConfig Config => gameConfig;
    public GameState CurrentGameState => currentGameState;
    
    private CommandInvoker commandInvoker;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Initialize()
    {
        LoadGameConfig();
        commandInvoker = new CommandInvoker();
        SubscribeToEvents();
        SetGameState(GameState.MainMenu);
    }
    private void LoadGameConfig()
    {
        gameConfig = Resources.Load<GameConfig>("Configs/GameConfig");
        if (gameConfig == null)
        {
            Debug.LogError("GameConfig not found in Resources/Configs folder!");
            return;
        }
        cardSpawner.Initialize(gameConfig);
    }
    
    private void SubscribeToEvents()
    {
        GameEvents.OnCardSelected += HandleCardSelection;
    }

    private void UnsubscribeFromEvents()
    {
        GameEvents.OnCardSelected -= HandleCardSelection;
    }

    public void StartGame(string difficultyName)
    {
        CleanupGame();
        
        currentDifficultyName = difficultyName;
        var gridConfig = gameConfig.GetGridConfig(difficultyName);
        if (gridConfig == null) return;

        currentDifficulty = gridConfig.difficulty;
        activeCards = cardSpawner.SpawnCards(gridConfig);
        SetGameState(GameState.Playing);
        GameEvents.InvokeGameStarted(difficultyName, currentDifficulty);
        
        DebugLog($"Game started with difficulty: {difficultyName}");
    }
    
    private void HandleCardSelection(Card selectedCard)
    {
        if (currentGameState != GameState.Playing || isProcessingMatch)
            return;

        var flipCommand = new FlipCardCommand(selectedCard, true);
        commandInvoker.ExecuteCommand(flipCommand);

        if (firstSelectedCard == null)
        {
            firstSelectedCard = selectedCard;
            DebugLog("First card selected");
        }
        else
        {
            if (firstSelectedCard != selectedCard)
            {
                isProcessingMatch = true;
                StartCoroutine(CheckMatchCoroutine(firstSelectedCard, selectedCard));
            }
        }
    }

    private IEnumerator CheckMatchCoroutine(Card card1, Card card2)
    {
        yield return new WaitForSeconds(gameConfig.matchCheckDelay);

        bool isMatch = card1.CardId == card2.CardId;

        if (isMatch)
        {
            HandleMatch(card1, card2);
        }
        else
        {
            HandleMismatch(card1, card2);
        }

        firstSelectedCard = null;
        isProcessingMatch = false;
        
        CheckGameCompletion();
    }

    private void HandleMatch(Card card1, Card card2)
    {
        var matchCommand = new MatchCardsCommand(card1, card2);
        commandInvoker.ExecuteCommand(matchCommand);
    
        UpdateScore(true);
        GameEvents.InvokeCardsMatched(card1, card2);
        
        DebugLog("Cards matched!");
    }

    private void HandleMismatch(Card card1, Card card2)
    {
        var flipCommand1 = new FlipCardCommand(card1, false);
        var flipCommand2 = new FlipCardCommand(card2, false);
    
        commandInvoker.ExecuteCommand(flipCommand1);
        commandInvoker.ExecuteCommand(flipCommand2);
    
        UpdateScore(false);
        GameEvents.InvokeCardsMismatched(card1, card2);
        
        DebugLog("Cards mismatched!");
    }

    private void UpdateScore(bool isMatch)
    {
        if (isMatch)
        {
            float timeSinceLastMatch = Time.time - lastMatchTime;
            
            if (timeSinceLastMatch <= gameConfig.maxComboTime)
            {
                currentCombo++;
            }
            else
            {
                currentCombo = 1;
            }

            int scoreToAdd = (int)(gameConfig.baseScore * Mathf.Pow(gameConfig.comboMultiplier, currentCombo - 1));
            currentScore += scoreToAdd;
            lastMatchTime = Time.time;

            GameEvents.InvokeComboChanged(currentCombo);
        }
        else
        {
            currentCombo = 0;
        }

        GameEvents.InvokeScoreChanged(currentScore);
    }

    private void CheckGameCompletion()
    {
        bool allMatched = true;
        
        for (int i = 0; i < activeCards.Length; i++)
        {
            if (!activeCards[i].IsMatched)
            {
                allMatched = false;
                break;
            }
        }

        if (allMatched)
        {
            HandleGameOver();
        }
    }

    private void HandleGameOver()
    {
        SetGameState(GameState.GameOver);
        GameEvents.InvokeGameOver();
        
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
        }
        
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.UpdateScores(currentScore);
        }
        
        DebugLog($"Game Over! Final Score: {currentScore}");
    }

    public void PauseGame()
    {
        if (currentGameState == GameState.Playing)
        {
            SetGameState(GameState.Paused);
            GameEvents.InvokeGamePaused();
        }
    }

    public void ResumeGame()
    {
        if (currentGameState == GameState.Paused)
        {
            SetGameState(GameState.Playing);
            GameEvents.InvokeGameResumed();
        }
    }
    public void CleanupGame()
    {
        
        if (cardSpawner != null)
        {
            cardSpawner.ReturnAllCards();
            activeCards = null;
        }

        
        currentScore = 0;
        currentCombo = 0;
        lastMatchTime = 0f;
        isProcessingMatch = false;
        firstSelectedCard = null;

        
        GameEvents.InvokeScoreChanged(currentScore);
        GameEvents.InvokeComboChanged(currentCombo);
        SetGameState(GameState.MainMenu);
    }

    

    private void SetGameState(GameState newState)
    {
        currentGameState = newState;
        GameEvents.InvokeGameStateChanged(newState);
    }

    private void DebugLog(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[GameManager] {message}");
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    public string GetCurrentDifficultyName() => currentDifficultyName;
    public GameDifficulty GetCurrentDifficulty() => currentDifficulty;
    public int GetCurrentScore() => currentScore;
    public int GetCurrentCombo() => currentCombo;
    
    [Header("Debug Buttons")]
    [SerializeField] private bool resetScores;

    private void OnValidate()
    {
        if (resetScores)
        {
            resetScores = false;
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.ResetScores();
                Debug.Log("All scores have been reset!");
            }
        }
    }
}
