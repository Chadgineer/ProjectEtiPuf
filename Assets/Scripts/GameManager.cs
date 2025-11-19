using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool isGameActive = false;
    
    [SerializeField] private int playerScore = 0;
    [SerializeField] private int scoreMultiplier = 1;
    [SerializeField] private int targetScore = 100;
    
    [SerializeField] private float roundDuration = 60f;
    [SerializeField] private float timeRemaining = 60f;
    [SerializeField] private bool timerActive = false;
    [SerializeField] private float bonusTimePerKill = 5f;
    
    [SerializeField] private int maxEnemies = 5;
    [SerializeField] private float enemySpawnInterval = 10f;
    
    [SerializeField] private float itemSpawnInterval = 2f;
    
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Player player;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private float spawnRadius = 10f;
    
    private Coroutine itemSpawnCoroutine;
    private Coroutine enemySpawnCoroutine;
    private int currentEnemyCount = 0;
    
    public bool IsGameActive => isGameActive;
    
    private void Start()
    {
        InitializeGame();
    }
    
    private void Update()
    {
        if (isGameActive)
        {
            UpdateTimer();
            CheckWinCondition();
            CheckLoseCondition();
        }
    }
    
    private void InitializeGame()
    {
        isGameActive = false;
        playerScore = 0;
        timeRemaining = roundDuration;
        timerActive = false;
        
        if (uiManager == null)
            uiManager = FindFirstObjectByType<UIManager>();
        if (player == null)
            player = FindFirstObjectByType<Player>();
        
        if (uiManager != null)
        {
            uiManager.UpdateScoreUI(playerScore);
            uiManager.UpdateTimerUI(timeRemaining);
        }
    }
    
    public void StartGame()
    {
        if (isGameActive)
            return;
        
        isGameActive = true;
        timerActive = true;
        timeRemaining = roundDuration;
        playerScore = 0;
        scoreMultiplier = 1;
        
        if (player != null)
        {
            player.ResetPlayer();
        }
        
        ClearAllEntities();
        currentEnemyCount = 0;
        SpawnEnemy(player != null ? player.transform.position : Vector3.zero);
        
        if (itemSpawnCoroutine != null)
        {
            StopCoroutine(itemSpawnCoroutine);
        }
        itemSpawnCoroutine = StartCoroutine(SpawnItemsContinuously());
        
        if (enemySpawnCoroutine != null)
        {
            StopCoroutine(enemySpawnCoroutine);
        }
        enemySpawnCoroutine = StartCoroutine(SpawnEnemiesContinuously());
        
        if (uiManager != null)
        {
            uiManager.UpdateScoreUI(playerScore);
            uiManager.UpdateTimerUI(timeRemaining);
            uiManager.ShowGameUI(true);
        }
    }
    
    public void RestartGame()
    {
        isGameActive = false;
        timerActive = false;
        
        if (itemSpawnCoroutine != null)
        {
            StopCoroutine(itemSpawnCoroutine);
            itemSpawnCoroutine = null;
        }
        
        if (enemySpawnCoroutine != null)
        {
            StopCoroutine(enemySpawnCoroutine);
            enemySpawnCoroutine = null;
        }
        
        ClearAllEntities();
        currentEnemyCount = 0;
        
        if (uiManager != null)
        {
            uiManager.ShowGameUI(false);
        }
        
        InitializeGame();
        StartGame();
    }
    
    public void AddScore(int points)
    {
        int actualPoints = points * scoreMultiplier;
        playerScore += actualPoints;
        
        if (uiManager != null)
        {
            uiManager.UpdateScoreUI(playerScore);
        }
    }
    
    
    private void UpdateTimer()
    {
        if (timerActive && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            
            if (timeRemaining < 0)
            {
                timeRemaining = 0;
                timerActive = false;
            }
            
            if (uiManager != null)
            {
                uiManager.UpdateTimerUI(timeRemaining);
            }
        }
    }
    
    public void AddTime(float bonusSeconds)
    {
        timeRemaining += bonusSeconds;
        if (uiManager != null)
        {
            uiManager.UpdateTimerUI(timeRemaining);
        }
    }
    
    private void CheckWinCondition()
    {
        if (!isGameActive)
            return;
        
        if (playerScore >= targetScore)
        {
            isGameActive = false;
            timerActive = false;
            
            if (itemSpawnCoroutine != null)
            {
                StopCoroutine(itemSpawnCoroutine);
                itemSpawnCoroutine = null;
            }
            
            if (enemySpawnCoroutine != null)
            {
                StopCoroutine(enemySpawnCoroutine);
                enemySpawnCoroutine = null;
            }
            
            if (uiManager != null)
            {
                uiManager.ShowWinScreen(true);
            }
        }
    }
    
    private void CheckLoseCondition()
    {
        if (!isGameActive)
            return;
        
        if (timeRemaining <= 0)
        {
            LoseGame("Time's up!");
        }
        else if (player != null && player.GetHealth() <= 0)
        {
            LoseGame("Player defeated!");
        }
    }
    
    public void LoseGame(string reason)
    {
        isGameActive = false;
        timerActive = false;
        
        if (itemSpawnCoroutine != null)
        {
            StopCoroutine(itemSpawnCoroutine);
            itemSpawnCoroutine = null;
        }
        
        if (enemySpawnCoroutine != null)
        {
            StopCoroutine(enemySpawnCoroutine);
            enemySpawnCoroutine = null;
        }
        
        if (uiManager != null)
        {
            uiManager.ShowLoseScreen(true, reason);
        }
    }
    
    private IEnumerator SpawnEnemiesContinuously()
    {
        yield return new WaitForSeconds(enemySpawnInterval);
        
        while (isGameActive)
        {
            if (currentEnemyCount < maxEnemies && player != null)
            {
                Vector3 playerPos = player.transform.position;
                SpawnEnemy(playerPos);
            }
            
            yield return new WaitForSeconds(enemySpawnInterval);
        }
    }
    
    private IEnumerator SpawnItemsContinuously()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(itemSpawnInterval);
            
            if (isGameActive && player != null)
            {
                Vector3 playerPos = player.transform.position;
                SpawnItem(playerPos);
            }
        }
    }
    
    private void SpawnEnemy(Vector3 centerPosition)
    {
        if (enemyPrefab == null || currentEnemyCount >= maxEnemies)
            return;
        
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = centerPosition + new Vector3(randomCircle.x, 1f, randomCircle.y);
        
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        currentEnemyCount++;
    }
    
    private void SpawnItem(Vector3 centerPosition)
    {
        if (itemPrefab == null)
            return;
        
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = centerPosition + new Vector3(randomCircle.x, 1f, randomCircle.y);
        
        Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
    }
    
    public void UnregisterEnemy()
    {
        currentEnemyCount = Mathf.Max(0, currentEnemyCount - 1);
        AddScore(10);
        AddTime(bonusTimePerKill);
    }
    
    public void UnregisterItem()
    {
        AddScore(5);
    }
    
    private void ClearAllEntities()
    {
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in allEnemies)
        {
            Destroy(enemy.gameObject);
        }
        
        Item[] allItems = FindObjectsByType<Item>(FindObjectsSortMode.None);
        foreach (Item item in allItems)
        {
            Destroy(item.gameObject);
        }
        
        currentEnemyCount = 0;
    }
}
