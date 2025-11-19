using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType itemType = ItemType.Health;
    
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobAmount = 0.3f;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private bool hasAnimation = true;
    
    [SerializeField] private int healAmount = 20;
    [SerializeField] private bool isCollected = false;
    [SerializeField] private float lifetime = 30f;
    
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float spawnTime;
    
    public enum ItemType
    {
        Health,
        Weapon
    }
    
    private void Start()
    {
        InitializeItem();
    }
    
    private void Update()
    {
        if (isCollected)
            return;
        
        UpdateAnimation();
        CheckLifetime();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        HandleTriggerInteraction(other);
    }
    
    private void InitializeItem()
    {
        startPosition = transform.position;
        spawnTime = Time.time;
        isCollected = false;
        
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
        
        SetItemProperties();
    }
    
    private void SetItemProperties()
    {
        if (itemType == ItemType.Health)
        {
            healAmount = 25;
        }
    }
    
    private void UpdateAnimation()
    {
        if (!hasAnimation)
            return;
        
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    
    public void UseItem(Player player)
    {
        if (player == null || isCollected)
            return;
        
        isCollected = true;
        ApplyItemEffect(player);
        
        if (gameManager != null)
        {
            gameManager.UnregisterItem();
        }
        
        DestroyItem();
    }
    
    private void ApplyItemEffect(Player player)
    {
        if (itemType == ItemType.Health)
        {
            player.Heal(healAmount);
        }
        else if (itemType == ItemType.Weapon)
        {
            player.EquipWeapon();
        }
    }
    
    private void HandleTriggerInteraction(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null && !isCollected)
        {
            UseItem(player);
        }
    }
    
    private void CheckLifetime()
    {
        if (lifetime > 0 && Time.time >= spawnTime + lifetime)
        {
            DestroyItem();
        }
    }
    
    private void DestroyItem()
    {
        if (gameManager != null && !isCollected)
        {
            gameManager.UnregisterItem();
        }
        
        Destroy(gameObject);
    }
}
