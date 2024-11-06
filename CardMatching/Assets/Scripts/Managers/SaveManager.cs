using UnityEngine;

namespace Managers
{
    public class SaveManager : MonoBehaviour
    {
        private const string HIGH_SCORE_KEY = "HighScore";
        private const string TOTAL_SCORE_KEY = "TotalScore";

        public static SaveManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public int GetHighScore()
        {
            return PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        }

        public int GetTotalScore()
        {
            return PlayerPrefs.GetInt(TOTAL_SCORE_KEY, 0);
        }

        public void UpdateScores(int gameScore)
        {
            int currentHighScore = GetHighScore();
            if (gameScore > currentHighScore)
            {
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, gameScore);
            }
        
            int currentTotalScore = GetTotalScore();
            PlayerPrefs.SetInt(TOTAL_SCORE_KEY, currentTotalScore + gameScore);
        
            PlayerPrefs.Save();
        }

        public void ResetScores()
        {
            PlayerPrefs.DeleteKey(HIGH_SCORE_KEY);
            PlayerPrefs.DeleteKey(TOTAL_SCORE_KEY);
            PlayerPrefs.Save();
        }
    }
}
