using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private bool isGrounded = true;
    
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private int maxStamina = 100;
    [SerializeField] private int currentStamina = 100;
    [SerializeField] private float staminaRegenRate = 10f;
    
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float lastAttackTime = 0f;
    [SerializeField] private bool canAttack = true;
    
    [SerializeField] private bool hasWeapon = false;
    
    [SerializeField] private bool isSprinting = false;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private Vector3 moveDirection = Vector3.zero;
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private bool isDead = false;
    
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Transform attackPoint;
    
    private void Start()
    {
        InitializePlayer();
    }
    
    private void Update()
    {
        if (isDead || gameManager == null || !gameManager.IsGameActive)
            return;
        
        HandleInput();
        RegenerateStamina();
        CheckGrounded();
    }
    
    private void FixedUpdate()
    {
        if (isDead)
            return;
        
        MovePlayer();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        HandleTriggerInteraction(other);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        HandleCollisionInteraction(collision);
    }
    
    private void InitializePlayer()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
        
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
        if (uiManager == null)
            uiManager = FindFirstObjectByType<UIManager>();
        
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        isDead = false;
        canAttack = true;
        
        if (attackPoint == null)
        {
            GameObject attackPointObj = new GameObject("AttackPoint");
            attackPointObj.transform.SetParent(transform);
            attackPointObj.transform.localPosition = Vector3.forward * 1f;
            attackPoint = attackPointObj.transform;
        }
        
        if (uiManager != null)
        {
            uiManager.UpdateHealthUI(currentHealth, maxHealth);
        }
    }
    
    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        isMoving = moveDirection.magnitude > 0.1f;
        
        isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && isMoving;
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
        
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
        {
            Attack();
        }
    }
    
    private void MovePlayer()
    {
        if (!isMoving)
        {
            currentSpeed = 0f;
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
            return;
        }
        
        float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;
        currentSpeed = targetSpeed;
        
        if (isSprinting)
        {
            ConsumeStamina(Time.fixedDeltaTime * 20f);
        }
        
        Vector3 moveVector = moveDirection * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + moveVector);
        
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
    
    private void Jump()
    {
        if (isGrounded && currentStamina >= 20)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            ConsumeStamina(20);
            isGrounded = false;
        }
    }
    
    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
    
    public void Attack()
    {
        if (!canAttack || Time.time < lastAttackTime + attackCooldown)
            return;
        
        if (currentStamina < 10)
            return;
        
        lastAttackTime = Time.time;
        ConsumeStamina(10);
        
        Collider[] hitColliders = Physics.OverlapSphere(attackPoint.position, attackRange);
        
        foreach (Collider col in hitColliders)
        {
            Enemy targetEnemy = col.GetComponent<Enemy>();
            if (targetEnemy != null)
            {
                int damage = CalculateDamage();
                targetEnemy.TakeDamage(damage);
            }
        }
    }
    
    private int CalculateDamage()
    {
        int baseDamage = attackDamage;
        
        if (hasWeapon)
        {
            baseDamage = (int)(baseDamage * 1.5f);
        }
        
        int finalDamage = baseDamage + Random.Range(-5, 6);
        return Mathf.Max(1, finalDamage);
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead)
            return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        if (uiManager != null)
        {
            uiManager.UpdateHealthUI(currentHealth, maxHealth);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        
        if (uiManager != null)
        {
            uiManager.UpdateHealthUI(currentHealth, maxHealth);
        }
    }
    
    public int GetHealth()
    {
        return currentHealth;
    }
    
    public void CollectItem(Item item)
    {
        if (item == null)
            return;
        
        item.UseItem(this);
        
        if (gameManager != null)
        {
            gameManager.UnregisterItem();
        }
    }
    
    private void HandleTriggerInteraction(Collider other)
    {
        Item item = other.GetComponent<Item>();
        if (item != null)
        {
            CollectItem(item);
        }
        
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            TakeDamage(enemy.GetDamage());
        }
    }
    
    private void HandleCollisionInteraction(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            Vector3 pushDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(pushDirection * 5f, ForceMode.Impulse);
        }
    }
    
    private void ConsumeStamina(float amount)
    {
        currentStamina -= (int)amount;
        currentStamina = Mathf.Max(0, currentStamina);
        
        if (currentStamina <= 0)
        {
            isSprinting = false;
        }
    }
    
    private void RegenerateStamina()
    {
        if (!isSprinting && currentStamina < maxStamina)
        {
            currentStamina += (int)(staminaRegenRate * Time.deltaTime);
            currentStamina = Mathf.Min(maxStamina, currentStamina);
        }
    }
    
    public void EquipWeapon()
    {
        hasWeapon = true;
        attackDamage += 10;
    }
    
    private void Die()
    {
        if (isDead)
            return;
        
        isDead = true;
        
        if (gameManager != null)
        {
            gameManager.LoseGame("Player defeated!");
        }
    }
    
    public void ResetPlayer()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        isDead = false;
        canAttack = true;
        hasWeapon = false;
        
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        if (uiManager != null)
        {
            uiManager.UpdateHealthUI(currentHealth, maxHealth);
        }
    }
}
