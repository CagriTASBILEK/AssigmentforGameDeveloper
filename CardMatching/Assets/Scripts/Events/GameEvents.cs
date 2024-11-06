using System;
using Cards;

namespace Events
{
    public static class GameEvents
    {
    
        public static event Action<GameState> OnGameStateChanged;
        public static event Action<string, GameDifficulty> OnGameStarted;
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;
        public static event Action OnGameOver;

    
        public static event Action<Card> OnCardSelected;
        public static event Action<Card, Card> OnCardsMatched;
        public static event Action<Card, Card> OnCardsMismatched;

    
        public static event Action<int> OnScoreChanged;
        public static event Action<int> OnComboChanged;
    
        public static void InvokeGameStateChanged(GameState state)
        {
            OnGameStateChanged?.Invoke(state);
        }

        public static void InvokeGameStarted(string difficultyName, GameDifficulty difficulty)
        {
            OnGameStarted?.Invoke(difficultyName, difficulty);
        }


        public static void InvokeGamePaused()
        {
            OnGamePaused?.Invoke();
        }

        public static void InvokeGameResumed()
        {
            OnGameResumed?.Invoke();
        }

        public static void InvokeGameOver()
        {
            OnGameOver?.Invoke();
        }

        public static void InvokeCardSelected(Card card)
        {
            OnCardSelected?.Invoke(card);
        }

        public static void InvokeCardsMatched(Card card1, Card card2)
        {
            OnCardsMatched?.Invoke(card1, card2);
        }

        public static void InvokeCardsMismatched(Card card1, Card card2)
        {
            OnCardsMismatched?.Invoke(card1, card2);
        }

        public static void InvokeScoreChanged(int newScore)
        {
            OnScoreChanged?.Invoke(newScore);
        }

        public static void InvokeComboChanged(int newCombo)
        {
            OnComboChanged?.Invoke(newCombo);
        }
    
    }
}