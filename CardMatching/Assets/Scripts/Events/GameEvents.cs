using System;

public static class GameEvents
{
    public static event Action<GameState> OnGameStateChanged;
    public static event Action<GameDifficulty> OnGameStarted;
    
    
    public static event Action<int> OnScoreChanged;
    
    public static void InvokeGameStateChanged(GameState state)
    {
        OnGameStateChanged?.Invoke(state);
    }
    
    public static void InvokeGameStarted(GameDifficulty difficulty)
    {
        OnGameStarted?.Invoke(difficulty);
    }
    
    public static void InvokeScoreChanged(int newScore)
    {
        OnScoreChanged?.Invoke(newScore);
    }
}