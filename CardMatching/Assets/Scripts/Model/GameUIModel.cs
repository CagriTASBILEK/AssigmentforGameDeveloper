public class GameUIModel
{
    private GameConfig gameConfig;
    private int currentScore;
    private int currentCombo;
    private GameState currentGameState;
    private GameDifficulty currentDifficulty;

    public GameConfig GameConfig => gameConfig;
    public int CurrentScore => currentScore;
    public int CurrentCombo => currentCombo;
    public GameState CurrentGameState => currentGameState;
    public GameDifficulty CurrentDifficulty => currentDifficulty;

    public bool Initialize()
    {
        if (GameManager.Instance == null || GameManager.Instance.Config == null)
        {
            return false;
        }

        gameConfig = GameManager.Instance.Config;
        currentDifficulty = GameDifficulty.Easy;
        currentGameState = GameState.MainMenu;
        SubscribeToEvents();
        return true;
    }

    private void SubscribeToEvents()
    {
        GameEvents.OnScoreChanged += UpdateScore;
        GameEvents.OnComboChanged += UpdateCombo;
        GameEvents.OnGameStateChanged += UpdateGameState;
    }

    private void UpdateScore(int score)
    {
        currentScore = score;
    }

    private void UpdateCombo(int combo)
    {
        currentCombo = combo;
    }

    private void UpdateGameState(GameState state)
    {
        currentGameState = state;
    }

    public void SetDifficulty(GameDifficulty difficulty)
    {
        currentDifficulty = difficulty;
    }

    public string GetDifficultyDescription(GameConfig.GridConfig config)
    {
        return $"{config.difficulty} ({config.rows}x{config.columns})";
    }
}