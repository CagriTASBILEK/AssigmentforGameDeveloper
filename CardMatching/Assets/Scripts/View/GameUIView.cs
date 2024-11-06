using System.Collections;
using System.Collections.Generic;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewModel;

namespace View
{
    public class GameUIView : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject gamePanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject gameFinishPanel;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private TMP_Dropdown difficultyDropdown;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI totalScoreText;

        [Header("Buttons")]
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;

        private GameUIViewModel viewModel;

        private void Awake()
        {
            viewModel = gameObject.AddComponent<GameUIViewModel>();
            viewModel.OnInitialized += OnViewModelInitialized;
        }
    
        private void OnViewModelInitialized()
        {
            InitializeUI();
            BindEvents();
            InitializePanels();
            UpdateScoreTexts();
        }
    
        private void InitializePanels()
        {
            mainMenuPanel.SetActive(false);
            gamePanel.SetActive(false);
            pausePanel.SetActive(false);
            gameFinishPanel.SetActive(false);
        
            ShowMainMenu();
        }
    
        private void ShowMainMenu()
        {
            UpdateGameState(GameState.MainMenu);
        }
        private void InitializeUI()
        {
        
            difficultyDropdown.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>();
        
            foreach (var config in viewModel.DifficultyConfigs)
            {
                string optionText = viewModel.GetDifficultyText(config);
                options.Add(new TMP_Dropdown.OptionData(optionText));
            }
        
            difficultyDropdown.AddOptions(options);
            difficultyDropdown.value = 0;
        
            if (viewModel.DifficultyConfigs.Length > 0)
            {
                viewModel.SetSelectedDifficulty(viewModel.DifficultyConfigs[0].difficultyName);
            }

        
            startGameButton.onClick.AddListener(viewModel.StartGame);
            pauseButton.onClick.AddListener(viewModel.PauseGame);
            resumeButton.onClick.AddListener(viewModel.ResumeGame);
            mainMenuButton.onClick.AddListener(viewModel.ReturnToMainMenu);
        
            difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
        }

        private void BindEvents()
        {
            viewModel.OnScoreUpdated += UpdateScore;
            viewModel.OnComboUpdated += UpdateCombo;
            viewModel.OnGameStateChanged += UpdateGameState;
            GameEvents.OnGameOver += ShowGameOver;
            GameEvents.OnGameOver += UpdateScoreTexts; 
        }
    
        private void UpdateScoreTexts()
        {
            highScoreText.text = $"High Score: {viewModel.HighScore}";
            totalScoreText.text = $"Total Point: {viewModel.TotalScore}";
        }

        private void OnDifficultyChanged(int index)
        {
            if (index >= 0 && index < viewModel.DifficultyConfigs.Length)
            {
                var selectedConfig = viewModel.DifficultyConfigs[index];
                viewModel.SetSelectedDifficulty(selectedConfig.difficultyName);
            }
        }

        private void UpdateScore(int score)
        {
            scoreText.text = $"Point: {score}";
            finalScoreText.text = $"Final Point: {score}";
        }

        private void UpdateCombo(int combo)
        {
            comboText.text = $"Combo: x{combo}";
        }

        private void ShowGameOver()
        {
            StartCoroutine(GameOverSequence());
        }

        private IEnumerator GameOverSequence()
        {
            gameFinishPanel.SetActive(true);
            yield return new WaitForSeconds(5f);
            gameFinishPanel.SetActive(false);
            viewModel.ReturnToMainMenu();
        }

        private void UpdateGameState(GameState state)
        {
            mainMenuPanel.SetActive(state == GameState.MainMenu);
            gamePanel.SetActive(state == GameState.Playing);
            pausePanel.SetActive(state == GameState.Paused);
        
            if (state == GameState.MainMenu)
            {
                gameFinishPanel.SetActive(false);
                UpdateScoreTexts(); 
            }
        }

        private void OnDestroy()
        {
            if (viewModel != null)
            {
                viewModel.OnInitialized -= OnViewModelInitialized;
                viewModel.OnScoreUpdated -= UpdateScore;
                viewModel.OnComboUpdated -= UpdateCombo;
                viewModel.OnGameStateChanged -= UpdateGameState;
                GameEvents.OnGameOver -= ShowGameOver;
                GameEvents.OnGameOver -= UpdateScoreTexts;
            }
        }
    }
}
