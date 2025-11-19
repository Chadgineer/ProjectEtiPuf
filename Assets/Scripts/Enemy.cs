using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int currentHealth = 50;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 10f;
    
    [SerializeField] private EnemyState currentState = EnemyState.Patrolling;
    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 patrolTarget;
    [SerializeField] private float stateChangeCooldown = 2f;
    [SerializeField] private float lastStateChange = 0f;
    
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float lastAttackTime = 0f;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private int attackPower = 15;
    
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] private float rotationSpeed = 5f;
    
    [SerializeField] private float aggroRange = 8f;
    [SerializeField] private float deaggroRange = 15f;
    [SerializeField] private bool isAggroed = false;
    [SerializeField] private float aggroDuration = 5f;
    [SerializeField] private float aggroTimer = 0f;
    
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform player;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float distanceToPlayer = float.MaxValue;
    
    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Attacking,
        Dead
    }
    
    private void Start()
    {
        InitializeEnemy();
    }
    
    private void Update()
    {
        if (currentState == EnemyState.Dead)
            return;
        
        UpdateAI();
        UpdateDistanceToPlayer();
        CheckAggroState();
    }
    
    private void FixedUpdate()
    {
        if (currentState == EnemyState.Dead)
            return;
        
        MoveEnemy();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        HandleTriggerInteraction(other);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        HandleCollisionInteraction(collision);
    }
    
    private void InitializeEnemy()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
        
        if (player == null)
        {
            Player playerObj = FindFirstObjectByType<Player>();
            if (playerObj != null)
                player = playerObj.transform;
        }
        
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
        
        currentHealth = maxHealth;
        startPosition = transform.position;
        currentState = EnemyState.Patrolling;
        isAggroed = false;
        canAttack = true;
        
        SetNewPatrolTarget();
    }
    
    private void UpdateAI()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                UpdatePatrolling();
                break;
            case EnemyState.Chasing:
                UpdateChasing();
                break;
            case EnemyState.Attacking:
                UpdateAttacking();
                break;
        }
    }
    
    private void UpdatePatrolling()
    {
        if (player != null && distanceToPlayer <= detectionRange)
        {
            ChangeState(EnemyState.Chasing);
            return;
        }
        
        float distanceToTarget = Vector3.Distance(transform.position, patrolTarget);
        if (distanceToTarget <= stoppingDistance)
        {
            SetNewPatrolTarget();
        }
    }
    
    private void UpdateChasing()
    {
        if (player == null)
        {
            ChangeState(EnemyState.Patrolling);
            return;
        }
        
        if (distanceToPlayer > deaggroRange)
        {
            ChangeState(EnemyState.Patrolling);
            return;
        }
        
        if (distanceToPlayer <= attackRange)
        {
            ChangeState(EnemyState.Attacking);
            return;
        }
        
        targetPosition = player.position;
    }
    
    private void UpdateAttacking()
    {
        if (player == null)
        {
            ChangeState(EnemyState.Patrolling);
            return;
        }
        
        if (distanceToPlayer > attackRange)
        {
            ChangeState(EnemyState.Chasing);
            return;
        }
        
        if (canAttack && Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
        }
        
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    private void ChangeState(EnemyState newState)
    {
        if (Time.time < lastStateChange + stateChangeCooldown && newState != EnemyState.Dead)
            return;
        
        currentState = newState;
        lastStateChange = Time.time;
    }
    
    private void MoveEnemy()
    {
        if (currentState == EnemyState.Attacking || currentState == EnemyState.Dead)
            return;
        
        Vector3 target = targetPosition;
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;
        
        float distanceToTarget = Vector3.Distance(transform.position, target);
        
        if (distanceToTarget > stoppingDistance)
        {
            Vector3 moveVector = direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position + moveVector);
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }
    
    private void SetNewPatrolTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        patrolTarget = startPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);
        targetPosition = patrolTarget;
    }
    
    private void PerformAttack()
    {
        if (player == null)
            return;
        
        lastAttackTime = Time.time;
        canAttack = false;
        
        if (distanceToPlayer <= attackRange)
        {
            Player playerComponent = player.GetComponent<Player>();
            if (playerComponent != null)
            {
                int damage = CalculateDamage();
                playerComponent.TakeDamage(damage);
            }
        }
        
        StartCoroutine(ResetAttackCooldown());
    }
    
    private int CalculateDamage()
    {
        int baseDamage = attackPower;
        
        int finalDamage = baseDamage + Random.Range(-3, 4);
        return Mathf.Max(1, finalDamage);
    }
    
    public int GetDamage()
    {
        return attackPower;
    }
    
    public void TakeDamage(int damage)
    {
        if (currentState == EnemyState.Dead)
            return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        if (!isAggroed && player != null)
        {
            SetAggroed(true);
            ChangeState(EnemyState.Chasing);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void UpdateDistanceToPlayer()
    {
        if (player != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, player.position);
        }
        else
        {
            distanceToPlayer = float.MaxValue;
        }
    }
    
    private void CheckAggroState()
    {
        if (player == null)
        {
            SetAggroed(false);
            return;
        }
        
        if (isAggroed)
        {
            aggroTimer += Time.deltaTime;
            
            if (distanceToPlayer > deaggroRange || aggroTimer >= aggroDuration)
            {
                if (distanceToPlayer > deaggroRange)
                {
                    SetAggroed(false);
                    if (currentState == EnemyState.Chasing || currentState == EnemyState.Attacking)
                    {
                        ChangeState(EnemyState.Patrolling);
                    }
                }
            }
        }
        else
        {
            if (distanceToPlayer <= aggroRange)
            {
                SetAggroed(true);
                ChangeState(EnemyState.Chasing);
            }
        }
    }
    
    private void SetAggroed(bool aggroed)
    {
        isAggroed = aggroed;
        aggroTimer = 0f;
        
        if (aggroed)
        {
            moveSpeed *= 1.2f;
        }
        else
        {
            moveSpeed = moveSpeed / 1.2f;
        }
    }
    
    private void HandleTriggerInteraction(Collider other)
    {
        Player playerComponent = other.GetComponent<Player>();
        if (playerComponent != null && currentState != EnemyState.Dead)
        {
            SetAggroed(true);
            ChangeState(EnemyState.Chasing);
        }
    }
    
    private void HandleCollisionInteraction(Collision collision)
    {
        Player playerComponent = collision.gameObject.GetComponent<Player>();
        if (playerComponent != null && currentState == EnemyState.Attacking)
        {
            Vector3 pushDirection = (transform.position - collision.transform.position).normalized;
            if (rb != null)
            {
                rb.AddForce(pushDirection * 3f, ForceMode.Impulse);
            }
        }
    }
    
    private void Die()
    {
        if (currentState == EnemyState.Dead)
            return;
        
        currentState = EnemyState.Dead;
        
        if (gameManager != null)
        {
            gameManager.UnregisterEnemy();
        }
        
        StartCoroutine(DestroyAfterDelay(2f));
    }
    
    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
