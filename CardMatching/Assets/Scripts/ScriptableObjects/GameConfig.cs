using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Memory Game/Game Config")]
public class GameConfig : ScriptableObject
{
    [System.Serializable]
    public class DifficultyConfig
    {
        public GameDifficulty difficulty;
        public Vector2Int gridSize;
        public float timeLimit;
    }

    public List<DifficultyConfig> difficultyConfigs;
    public float cardFlipDuration = 0.3f;
    public float matchCheckDelay = 1f;
    public int baseScore = 100;
    public float comboMultiplier = 1.5f;
}