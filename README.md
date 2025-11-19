# Project EtiPuf - Unity Game Project

## Overview

This is a complete Unity game project demonstrating Object-Oriented Programming (OOP) principles with C#. The project includes a fully functional game system with player movement, enemy AI, collectable items, game management, and UI systems.

## Game Description

A simple action game where the player must collect items, defeat enemies, and reach a target score before time runs out. The game features multiple difficulty levels, round progression, and a scoring system with multipliers.

## OOP Structure

### Class Hierarchy and Relationships

The project implements a well-structured OOP design with clear separation of concerns:

```
GameManager (Singleton-like coordinator)
    ├── Manages: Player, Enemy[], Item[], UIManager
    ├── Controls: Game state, scoring, timing, win/lose conditions
    └── Methods: AddScore(), StartGame(), RestartGame(), etc.

Player (MonoBehaviour)
    ├── Interacts with: Enemy, Item, GameManager, UIManager
    ├── Manages: Movement, health, stamina, combat
    └── Methods: Attack(), TakeDamage(), CollectItem(), MoveTo(), etc.

Enemy (MonoBehaviour)
    ├── Interacts with: Player, GameManager
    ├── Manages: AI behavior, patrolling, chasing, attacking
    └── Methods: TakeDamage(), PerformAttack(), MoveTo(), etc.

Item (MonoBehaviour)
    ├── Interacts with: Player, GameManager
    ├── Manages: Collection, effects, animation
    └── Methods: UseItem(), CollectItem(), GetItemValue(), etc.

UIManager (MonoBehaviour)
    ├── Interacts with: GameManager, Player
    ├── Manages: All UI updates, button events
    └── Methods: UpdateScoreUI(), UpdateHealthUI(), ShowWinScreen(), etc.
```

## Class Details

### 1. GameManager.cs

**Purpose**: Central game coordinator managing game flow, scoring, timing, and state.

**Fields (≥5)**:
- `isGameActive`, `isGamePaused`, `hasWon`, `hasLost`, `currentRound`
- `playerScore`, `scoreMultiplier`, `targetScore`, `enemiesKilled`, `itemsCollected`
- `gameTimer`, `roundDuration`, `timeRemaining`, `timerActive`, `bonusTimePerKill`
- `currentDifficulty`, `difficultyMultiplier`, `enemiesToSpawn`, `itemsToSpawn`, `spawnRate`
- `uiManager`, `player`, `activeEnemies`, `activeItems`

**Methods (≥5)**:
- `StartGame()` / `StartGame(DifficultyLevel)` - Overloaded method
- `AddScore(int)` / `AddScore(int, string)` - Overloaded method
- `GetScore()` - Returns a value
- `GetTimeRemaining()` - Returns a value
- `RestartGame()`, `EndGame()`, `TogglePause()`
- `RegisterEnemy()`, `UnregisterEnemy()`, `RegisterItem()`, `UnregisterItem()`
- `NextRound()`, `WinGame()`, `LoseGame(string)`

**Key Interactions**:
- `gameManager.AddScore(points)` - Called by Player/Item
- `gameManager.UnregisterEnemy(enemy)` - Called when enemy dies
- `gameManager.UnregisterItem(item)` - Called when item collected
- `gameManager.LoseGame(reason)` - Called by Player on death

### 2. Player.cs

**Purpose**: Handles player movement, combat, health management, and item collection.

**Fields (≥5)**:
- `moveSpeed`, `sprintSpeed`, `rotationSpeed`, `jumpForce`, `isGrounded`
- `maxHealth`, `currentHealth`, `maxStamina`, `currentStamina`, `staminaRegenRate`
- `attackDamage`, `attackRange`, `attackCooldown`, `lastAttackTime`, `canAttack`
- `itemCount`, `weaponCount`, `hasWeapon`, `currentWeapon`, `ammoCount`
- `isSprinting`, `isMoving`, `moveDirection`, `currentSpeed`, `isDead`

**Methods (≥5)**:
- `Attack()` / `Attack(Enemy)` - Method with parameters, overloaded
- `MoveTo(Vector3)` / `MoveTo(Vector3, float)` - Overloaded method
- `GetHealth()` - Returns a value
- `GetStamina()` - Returns a value
- `GetDistanceTo(Vector3)` - Returns a value
- `TakeDamage(int)`, `Heal(int)`, `CollectItem(Item)`
- `EquipWeapon(string)`, `ResetPlayer()`

**Key Interactions**:
- `player.Attack(enemy)` - Attacks specific enemy
- `player.TakeDamage(damage)` - Called by Enemy
- `player.CollectItem(item)` - Called when collecting items
- `player.Heal(amount)` - Called by Item effects

**Unity Integration**:
- `Start()`, `Update()`, `FixedUpdate()`
- `OnTriggerEnter()`, `OnCollisionEnter()`

### 3. Enemy.cs

**Purpose**: Implements enemy AI behavior with patrolling, chasing, and attacking states.

**Fields (≥5)**:
- `maxHealth`, `currentHealth`, `attackDamage`, `moveSpeed`, `detectionRange`
- `currentState`, `patrolRadius`, `startPosition`, `patrolTarget`, `stateChangeCooldown`
- `attackRange`, `attackCooldown`, `lastAttackTime`, `canAttack`, `attackPower`
- `targetPosition`, `isMoving`, `stoppingDistance`, `rotationSpeed`, `moveDirection`
- `aggroRange`, `deaggroRange`, `isAggroed`, `aggroDuration`, `aggroTimer`

**Methods (≥5)**:
- `TakeDamage(int)` - Returns a value (bool)
- `GetDamage()` - Returns a value
- `GetHealth()` - Returns a value
- `GetDistanceTo(Vector3)` - Returns a value
- `MoveTo(Vector3)` / `MoveTo(Vector3, float)` - Method with parameters, overloaded
- `ChangeState(EnemyState)` / `ChangeState(EnemyState, float)` - Overloaded method
- `PerformAttack()`, `SetAggroed(bool)`

**Key Interactions**:
- `enemy.TakeDamage(damage)` - Called by Player.Attack()
- `enemy.PerformAttack()` - Attacks Player
- Registered with GameManager on spawn

**Unity Integration**:
- `Start()`, `Update()`, `FixedUpdate()`
- `OnTriggerEnter()`, `OnCollisionEnter()`

### 4. Item.cs

**Purpose**: Represents collectable items with various effects (health, weapons, boosts).

**Fields (≥5)**:
- `itemType`, `itemName`, `itemValue`, `isCollectable`, `isConsumedOnUse`
- `rotationSpeed`, `bobSpeed`, `bobAmount`, `startPosition`, `hasAnimation`
- `healAmount`, `damageBoost`, `speedBoost`, `effectDuration`, `hasTemporaryEffect`
- `collectionRange`, `autoCollect`, `autoCollectRange`, `isCollected`, `lifetime`
- `targetPlayer`, `gameManager`, `spawnTime`

**Methods (≥5)**:
- `UseItem(Player)` / `UseItem(Player, int)` - Method with parameters, overloaded
- `GetItemValue()` - Returns a value
- `GetDistanceTo(Vector3)` - Returns a value
- `CanBeCollected()` - Returns a value
- `CollectItem(Player)`, `SetItemType(ItemType)`, `SetItemValue(int)`

**Key Interactions**:
- `item.UseItem(player)` - Called by Player when collecting
- `item.CollectItem(player)` - Triggered on collision/trigger
- Registered with GameManager on spawn

**Unity Integration**:
- `Start()`, `Update()`
- `OnTriggerEnter()`, `OnCollisionEnter()`

### 5. UIManager.cs

**Purpose**: Manages all UI updates, displays, and user interface interactions.

**Fields (≥5)**:
- `scoreText`, `healthText`, `timerText`, `roundText`, `gameOverText`
- `healthBarFill`, `staminaBarFill`, `gameOverPanel`, `winPanel`, `losePanel`
- `startButton`, `restartButton`, `pauseButton`, `resumeButton`, `quitButton`
- `mainMenuPanel`, `gameUIPanel`, `pausePanel`, `winScreenPanel`, `loseScreenPanel`
- `healthColorGood`, `healthColorWarning`, `healthColorDanger`, `healthWarningThreshold`, `healthDangerThreshold`
- `scoreUpdateSpeed`, `healthBarUpdateSpeed`, `animateUI`, `animationDuration`, `animationScale`
- `displayedScore`, `targetScore`, `displayedHealth`, `targetHealth`, `isUpdatingScore`

**Methods (≥5)**:
- `UpdateScoreUI(int)` / `UpdateScoreUI(int, bool)` - Method with parameters, overloaded
- `UpdateHealthUI(int, int)` / `UpdateHealthUI(float, float, bool)` - Overloaded method
- `ShowWinScreen(bool)` / `ShowWinScreen(bool, string)` - Overloaded method
- `GetDisplayedScore()` - Returns a value
- `GetFormattedTime(float)` - Returns a value
- `GetHealthPercentage()` - Returns a value
- `UpdateTimerUI(float)`, `UpdateRoundUI(int)`, `ShowMessage(string, float)`

**Key Interactions**:
- `uiManager.UpdateScoreUI(score)` - Called by GameManager
- `uiManager.UpdateHealthUI(health, maxHealth)` - Called by Player
- `uiManager.ShowWinScreen(true)` - Called by GameManager
- `uiManager.ShowLoseScreen(true, reason)` - Called by GameManager

## C# Language Concepts Used

### Control Structures
- **if/else**: Used extensively for state checks, conditionals, and decision making
- **switch/case**: Used in Enemy state machine and Item type handling
- **loops**: 
  - `foreach` loops for iterating through Lists (enemies, items)
  - `for` loops in various utility methods

### Collections
- **List<T>**: 
  - `List<Enemy> activeEnemies` in GameManager
  - `List<Item> activeItems` in GameManager

### Math Library
- `Mathf.Max()`, `Mathf.Min()`, `Mathf.CeilToInt()`, `Mathf.FloorToInt()`
- `Vector3.Distance()` for distance calculations
- `Quaternion.Slerp()` for smooth rotations
- `Random.Range()` for random values

### Flow Control
- **break**: Used in switch statements
- **continue**: Used in loops when skipping iterations
- **return**: Used for early exits and value returns

### Debugging
- `Debug.Log()`: Used throughout for logging game events
- `Debug.LogWarning()`: Used for warnings

### Encapsulation
- **public**: Methods meant for external access (e.g., `Attack()`, `TakeDamage()`)
- **private**: Internal methods and fields (e.g., `InitializePlayer()`, `UpdateAI()`)
- **Properties**: Used for read-only access (e.g., `CurrentHealth`, `IsGameActive`)

## Unity Integration

### Required Unity Lifecycle Methods
All scripts implement:
- `Start()` - Initialization
- `Update()` - Per-frame updates
- `OnTriggerEnter()` - Trigger-based interactions
- `OnCollisionEnter()` - Collision-based interactions

### Additional Unity Methods
- `FixedUpdate()` - Physics-based updates (Player, Enemy)
- `OnDestroy()` - Cleanup (implicit through Destroy calls)

## Scene Setup Instructions

### 1. Create Game Objects

**Player**:
- Create a Cube (GameObject > 3D Object > Cube)
- Name it "Player"
- Add `Player.cs` script component
- Add Rigidbody component
- Set position to (0, 1, 0)
- Add a Collider (Box Collider)

**Enemy Prefab**:
- Create a Sphere (GameObject > 3D Object > Sphere)
- Name it "Enemy"
- Add `Enemy.cs` script component
- Add Rigidbody component
- Add a Collider (Sphere Collider)
- Set material color to red
- Drag to Prefabs folder to create prefab

**Item Prefab**:
- Create a Capsule (GameObject > 3D Object > Capsule)
- Name it "Item"
- Add `Item.cs` script component
- Add a Collider (Capsule Collider) - set as Trigger
- Set material color to green
- Drag to Prefabs folder to create prefab

### 2. Create UI Canvas

**Canvas Setup**:
- Create Canvas (GameObject > UI > Canvas)
- Add `UIManager.cs` script component

**UI Elements**:
- Create TextMeshPro - Text (UI) for Score
- Create TextMeshPro - Text (UI) for Health
- Create TextMeshPro - Text (UI) for Timer
- Create TextMeshPro - Text (UI) for Round
- Create Image for Health Bar (set Image Type to Filled)
- Create Button for Start
- Create Button for Restart
- Create Button for Pause

**Assign References**:
- In UIManager inspector, drag all UI elements to their respective fields

### 3. Create GameManager

- Create Empty GameObject
- Name it "GameManager"
- Add `GameManager.cs` script component
- Assign Player, UIManager references in inspector

### 4. Camera Setup

- Position Main Camera at (0, 5, -10)
- Rotate to look at origin
- Or use Cinemachine for better camera control

### 5. Ground Plane

- Create Plane (GameObject > 3D Object > Plane)
- Scale to (10, 1, 10)
- Position at (0, 0, 0)
- Add a material for visibility

## How to Play

### Controls
- **WASD / Arrow Keys**: Move player
- **Left Shift**: Sprint (consumes stamina)
- **Space**: Jump
- **Left Mouse / E**: Attack
- **F**: Interact with items

### Objectives
1. Collect items to gain health and score points
2. Defeat enemies to earn points and bonus time
3. Reach the target score before time runs out
4. Survive enemy attacks and manage your health

### Game Flow
1. **Start State**: Main menu with Start button
2. **Main Play**: 
   - Player moves and collects items
   - Enemies patrol and chase player
   - Score increases with items and kills
   - Timer counts down
3. **Win Condition**: Reach target score
4. **Lose Condition**: Time runs out or player health reaches 0
5. **Restart**: Use Restart button to play again

### Difficulty Levels
- **Easy**: More items, longer time, fewer enemies
- **Medium**: Balanced gameplay
- **Hard**: Fewer items, shorter time, more enemies

## Class Diagrams (ASCII)

```
┌─────────────────────────────────────┐
│         GameManager                 │
├─────────────────────────────────────┤
│ - isGameActive: bool                │
│ - playerScore: int                  │
│ - timeRemaining: float              │
│ - currentRound: int                 │
│ - currentDifficulty: DifficultyLevel│
│ - activeEnemies: List<Enemy>        │
│ - activeItems: List<Item>           │
├─────────────────────────────────────┤
│ + StartGame()                       │
│ + StartGame(DifficultyLevel)        │
│ + AddScore(int)                     │
│ + AddScore(int, string)             │
│ + GetScore(): int                   │
│ + RestartGame()                     │
│ + RegisterEnemy(Enemy)              │
│ + UnregisterEnemy(Enemy)            │
│ + RegisterItem(Item)                │
│ + UnregisterItem(Item)              │
└─────────────────────────────────────┘
           │         │         │
           │         │         │
           ▼         ▼         ▼
    ┌──────────┐ ┌──────┐ ┌──────────┐
    │  Player  │ │Enemy │ │   Item   │
    └──────────┘ └──────┘ └──────────┘
           │         │         │
           │         │         │
           └─────────┴─────────┘
                     │
                     ▼
              ┌──────────────┐
              │  UIManager   │
              └──────────────┘

┌─────────────────────────────────────┐
│            Player                    │
├─────────────────────────────────────┤
│ - currentHealth: int                │
│ - currentStamina: int               │
│ - moveSpeed: float                  │
│ - attackDamage: int                 │
│ - hasWeapon: bool                   │
├─────────────────────────────────────┤
│ + Attack()                          │
│ + Attack(Enemy)                     │
│ + TakeDamage(int)                   │
│ + Heal(int)                         │
│ + CollectItem(Item)                 │
│ + MoveTo(Vector3)                   │
│ + MoveTo(Vector3, float)            │
│ + GetHealth(): int                  │
│ + GetStamina(): int                 │
│ + GetDistanceTo(Vector3): float    │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│            Enemy                     │
├─────────────────────────────────────┤
│ - currentHealth: int               │
│ - currentState: EnemyState          │
│ - moveSpeed: float                  │
│ - attackPower: int                  │
│ - isAggroed: bool                   │
├─────────────────────────────────────┤
│ + TakeDamage(int): bool             │
│ + GetDamage(): int                  │
│ + GetHealth(): int                  │
│ + MoveTo(Vector3)                   │
│ + MoveTo(Vector3, float)            │
│ + ChangeState(EnemyState)           │
│ + ChangeState(EnemyState, float)    │
│ + GetDistanceTo(Vector3): float    │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│            Item                      │
├─────────────────────────────────────┤
│ - itemType: ItemType                │
│ - itemName: string                  │
│ - itemValue: int                    │
│ - isCollectable: bool               │
│ - isCollected: bool                 │
├─────────────────────────────────────┤
│ + UseItem(Player)                   │
│ + UseItem(Player, int)              │
│ + CollectItem(Player)               │
│ + GetItemValue(): int               │
│ + GetDistanceTo(Vector3): float    │
│ + CanBeCollected(): bool            │
│ + SetItemType(ItemType)             │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│          UIManager                   │
├─────────────────────────────────────┤
│ - scoreText: TextMeshProUGUI        │
│ - healthText: TextMeshProUGUI      │
│ - timerText: TextMeshProUGUI       │
│ - healthBarFill: Image              │
│ - displayedScore: int               │
├─────────────────────────────────────┤
│ + UpdateScoreUI(int)                │
│ + UpdateScoreUI(int, bool)          │
│ + UpdateHealthUI(int, int)          │
│ + UpdateHealthUI(float, float, bool)│
│ + UpdateTimerUI(float)              │
│ + ShowWinScreen(bool)               │
│ + ShowWinScreen(bool, string)       │
│ + GetDisplayedScore(): int          │
│ + GetFormattedTime(float): string   │
│ + GetHealthPercentage(): float      │
└─────────────────────────────────────┘
```

## Interaction Flow Examples

### Player Attacks Enemy
```
Player.Attack(enemy)
    └─> Enemy.TakeDamage(damage)
        └─> Returns bool (killed?)
            └─> If killed: GameManager.UnregisterEnemy(enemy)
                └─> GameManager.AddScore(10)
                    └─> UIManager.UpdateScoreUI(score)
```

### Player Collects Item
```
Player.OnTriggerEnter(Item)
    └─> Player.CollectItem(item)
        └─> Item.UseItem(player)
            └─> Player.Heal(amount) or Player.EquipWeapon(name)
                └─> UIManager.UpdateHealthUI(health, maxHealth)
            └─> GameManager.UnregisterItem(item)
                └─> GameManager.AddScore(5)
                    └─> UIManager.UpdateScoreUI(score)
```

### Enemy AI Behavior
```
Enemy.Update()
    └─> UpdateAI()
        ├─> If Patrolling: Check for player in range
        │   └─> If detected: ChangeState(Chasing)
        ├─> If Chasing: Move towards player
        │   └─> If in attack range: ChangeState(Attacking)
        └─> If Attacking: PerformAttack()
            └─> Player.TakeDamage(damage)
```

## Project Structure

```
Assets/
├── Scripts/
│   ├── GameManager.cs
│   ├── Player.cs
│   ├── Enemy.cs
│   ├── Item.cs
│   └── UIManager.cs
├── Prefabs/
│   ├── Enemy.prefab
│   └── Item.prefab
├── Scenes/
│   └── SampleScene.unity
└── Settings/
    └── (Unity settings files)
```

## Requirements Checklist

✅ **OOP Structure**
- ✅ 4+ interacting classes (GameManager, Player, Enemy, Item, UIManager)
- ✅ Each class has ≥5 fields
- ✅ Each class has ≥5 methods
- ✅ Methods with parameters
- ✅ Methods returning values
- ✅ Overloaded methods
- ✅ Clear class-to-class interactions

✅ **C# Language Concepts**
- ✅ if/else statements
- ✅ loops (foreach, for)
- ✅ Arrays/List<T>
- ✅ Math library (Mathf, Vector3, Random)
- ✅ break/continue
- ✅ Random
- ✅ Debug.Log
- ✅ public/private encapsulation

✅ **Unity Integration**
- ✅ Start() methods
- ✅ Update() methods
- ✅ OnTriggerEnter() methods
- ✅ OnCollisionEnter() methods
- ✅ Prefab support (instructions provided)
- ✅ UI elements (instructions provided)
- ✅ Complete game loop (Start → Play → Win/Lose)

✅ **Bonus Elements**
- ✅ Difficulty levels
- ✅ Enemy/item spawning via Lists
- ✅ Round system / score multiplier
- ✅ UI buttons (Start / Restart)

## Notes

- All scripts are fully commented with XML documentation
- Code follows C# naming conventions
- Methods are organized with region blocks
- Unity-specific attributes (SerializeField, Header) are used for inspector organization
- The project is designed to be easily extensible

## Future Enhancements

Potential additions:
- Sound effects and music
- Particle effects for combat
- More enemy types with different behaviors
- Weapon system with multiple weapon types
- Power-up system
- Save/Load functionality
- Leaderboard system

---

**Project Created**: 2024
**Unity Version**: 6000.0.60f1
**Language**: C#

