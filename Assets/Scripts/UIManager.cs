using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI timerText;
    
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    
    [SerializeField] private GameManager gameManager;
    
    private void Start()
    {
        InitializeUI();
    }
    
    private void InitializeUI()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
        
        SetupButtonListeners();
        
        if (startButton != null)
            startButton.gameObject.SetActive(true);
        if (restartButton != null)
            restartButton.gameObject.SetActive(false);
    }
    
    private void SetupButtonListeners()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartButtonClicked);
    }
    
    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    public void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth}/{maxHealth}";
        }
    }
    
    public void UpdateTimerUI(float timeRemaining)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
    
    public void ShowWinScreen(bool show)
    {
        if (restartButton != null)
            restartButton.gameObject.SetActive(show);
    }
    
    public void ShowLoseScreen(bool show, string reason)
    {
        if (restartButton != null)
            restartButton.gameObject.SetActive(show);
    }
    
    public void ShowGameUI(bool show)
    {
        if (startButton != null)
            startButton.gameObject.SetActive(!show);
    }
    
    private void OnStartButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
        
        if (startButton != null)
            startButton.gameObject.SetActive(false);
        if (restartButton != null)
            restartButton.gameObject.SetActive(false);
    }
    
    private void OnRestartButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
        
        if (restartButton != null)
            restartButton.gameObject.SetActive(false);
    }
}
