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

    private GameState currentGameState;
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

    public void StartGame(GameDifficulty difficulty)
    {
        if (currentGameState != GameState.MainMenu && currentGameState != GameState.GameOver)
            return;

        currentDifficulty = difficulty;
        currentScore = 0;
        currentCombo = 0;
        lastMatchTime = 0f;
        isProcessingMatch = false;
        firstSelectedCard = null;

        activeCards = cardSpawner.SpawnCards(difficulty);
        SetGameState(GameState.Playing);
        GameEvents.InvokeGameStarted(difficulty);
        GameEvents.InvokeScoreChanged(currentScore);
        
        DebugLog($"Game started with difficulty: {difficulty}");
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            StartGame(GameDifficulty.Easy);
        }
        if (Input.GetKey(KeyCode.B))
        {
            StartGame(GameDifficulty.Medium);
        }
        if (Input.GetKey(KeyCode.C))
        {
            StartGame(GameDifficulty.Hard);
        }
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
    
    public int GetCurrentScore() => currentScore;
    public int GetCurrentCombo() => currentCombo;
    public GameDifficulty GetCurrentDifficulty() => currentDifficulty;
}
