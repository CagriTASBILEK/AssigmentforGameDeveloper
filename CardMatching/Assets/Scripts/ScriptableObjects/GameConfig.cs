using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Memory Game/Game Config")]
public class GameConfig : ScriptableObject
{
    [System.Serializable]
    public class GridConfig
    {
        public GameDifficulty difficulty;
        public int rows;
        public int columns;
        public float cardSpacing = 1.2f;
        
        public bool IsValid()
        {
            int totalCards = rows * columns;
            return totalCards % 2 == 0; 
        }
    }

    [Header("Grid Settings")]
    public GridConfig[] gridConfigs;
    public float cardFlipDuration = 0.3f;
    public float matchCheckDelay = 1f;
    public int baseScore = 100;
    public float comboMultiplier = 1.5f;
    public float maxComboTime = 5f;
    public GridConfig GetGridConfig(GameDifficulty difficulty)
    {
        for (int i = 0; i < gridConfigs.Length; i++)
        {
            if (gridConfigs[i].difficulty == difficulty)
            {
                if (!gridConfigs[i].IsValid())
                {
                    Debug.LogError($"GridConfig for difficulty {difficulty} is invalid! Rows: {gridConfigs[i].rows}, Columns: {gridConfigs[i].columns}");
                    return null;
                }
                return gridConfigs[i];
            }
        }
        return gridConfigs[0]; 
    }
}