using System.Collections;
using System.Collections.Generic;
using UI.Utiliy;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public Vector2 patrolDirection = Vector2.right; // Set to (1,0) for horizontal or (0,1) for vertical
    [SerializeField] private Direction enemyDirection = Direction.Down;
    public float detectionRadius = 5f;
    public float chaseDelay = 1f;
    public float changeDirectionInterval = 3f;
    public float wonderingInterval = 1.5f;
    public Animator enemyAnimator;

    [Header("Enemy Party Data")]
    public List<CharacterData> party;
    public List<Character> enemyParty;

    float patrolSpeedTemp;
    private Rigidbody2D rb;
    private Transform player;
    private bool isChasing = false;
    private bool isWaitingToChase = false;
    private bool canChangeDirection = true;
    private Vector2 originalDirection;
    [SerializeField] bool canMove = true;

    public bool CanMove { get => canMove; set => canMove = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalDirection = patrolDirection;
        player = PlayerController.instance.transform;
        patrolSpeedTemp = patrolSpeed;
        InitializeParty();
        ChangeDirection();
        StartCoroutine(ChangeDirectionRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);

                if (distanceToPlayer <= detectionRadius && !isChasing && !isWaitingToChase)
                {
                    StartCoroutine(ChaseDelayRoutine());
                }
                else if (distanceToPlayer > detectionRadius && isChasing)
                {
                    isChasing = false;
                    canChangeDirection = true;
                }
            }
        }

    }
    void FixedUpdate()
    {
        if (canMove)
            if (!isChasing)
            {
                Patrol();
            }
            else
            {
                ChasePlayer();
            }
    }
    void Patrol()
    {
        enemyAnimator.SetFloat("dirX", patrolDirection.x);
        enemyAnimator.SetFloat("dirY", patrolDirection.y);
        SetDirection(patrolDirection);
        Vector2 mov = patrolDirection * patrolSpeed * Time.fixedDeltaTime;
        Vector2 newPosition = (Vector2)rb.position + mov;
        rb.MovePosition(newPosition);
        //rb.linearVelocity = patrolDirection * patrolSpeed;
    }
    void ChasePlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            enemyAnimator.SetFloat("dirX", direction.x);
            enemyAnimator.SetFloat("dirY", direction.y);
            //Debug.Log(direction);
            SetDirection(direction);
            Vector2 mov = direction * chaseSpeed * Time.fixedDeltaTime;
            Vector2 newPosition = (Vector2)rb.position + mov;
            rb.MovePosition(newPosition);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        if (dir.magnitude > 0f)
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                if (dir.x > 0)
                    enemyDirection = Direction.Right;
                else
                    enemyDirection = Direction.Left;
            }
            else
            {
                if (dir.y > 0)
                    enemyDirection = Direction.Up;
                else
                    enemyDirection = Direction.Down;
            }
            //animator.Play("Direction " + idleTemp);
        }
    }

    IEnumerator ChaseDelayRoutine()
    {
        isWaitingToChase = true;
        yield return new WaitForSeconds(chaseDelay);

        // If player is still in range, start chasing
        if (Vector2.Distance(transform.position, player.position) <= detectionRadius)
        {
            isChasing = true;
            canChangeDirection = false;
        }

        isWaitingToChase = false;
    }

    IEnumerator ChangeDirectionRoutine()
    {
        while (canMove)
        {
            yield return new WaitForSeconds(changeDirectionInterval);
            patrolSpeed = 0;
            yield return new WaitForSeconds(wonderingInterval);
            if (!isChasing && canChangeDirection) // Change direction only if not chasing
            {
                ChangeDirection();
            }
        }
    }
    void ChangeDirection()
    {
        patrolSpeed = patrolSpeedTemp;
        patrolDirection = Random.value > 0.5f ? Vector2.right : Vector2.up;
        if (Random.value > 0.5f) patrolDirection *= -1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (canMove)
        {
            if (collision.gameObject.tag == "Player")
            {
                PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();
                Debug.Log("Enemy Attacked Player, Battle Scene On!");
                StartCoroutine(StartBattle(player, this, false, true));
            }
            if (!isChasing)
            {
                // Reverse the patrol direction on collision
                patrolDirection *= -1;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canMove)
            if (collision.gameObject.tag == "PlayerAttack")
            {
                PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();
                Debug.Log($"Enemy got Attacked, Battle Scene On!");
                StartCoroutine(StartBattle(player, this, true, false));
            }

    }
    IEnumerator StartBattle(PlayerController player = null, EnemyController enemy = null, bool enemyAttacked = false, bool playerAttacked = false)
    {
        UITransition.instance.FadeOutFadeIn();
        yield return new WaitForSeconds(.5f);
        GoBattle(player, enemy, enemyAttacked, playerAttacked);
    }

    public void GoBattle(PlayerController player = null, EnemyController enemy = null, bool enemyAttacked = false, bool playerAttacked = false)
    {
        bool ambush = false;
        if (enemyAttacked)
        {
            ambush = IsPlayerBehind(enemy.transform, player.transform);
        }
        Debug.Log($"ambush is {ambush}");
        BattleManager.instance.InitBattle(player, enemy, playerAttacked, ambush);
    }

    bool IsPlayerBehind(Transform enemy, Transform player)
    {
        Vector3 enemyToPlayer = (player.position - enemy.position).normalized;
        Vector3 enemyForward = Vector3.zero;

        // Set forward direction based on the enum
        switch (enemyDirection)
        {
            case Direction.Up:
                enemyForward = Vector3.up;
                break;
            case Direction.Down:
                enemyForward = Vector3.down;
                break;
            case Direction.Left:
                enemyForward = Vector3.left;
                break;
            case Direction.Right:
                enemyForward = Vector3.right;
                break;
        }

        float dot = Vector3.Dot(enemyForward, enemyToPlayer);

        // If dot product is negative, the player is behind the enemy
        return dot < 0;
    }
    public void InitializeParty()
    {
        enemyParty.Clear();
        foreach (CharacterData characterData in party)
        {
            AddCharacterToParty(characterData);
        }
    }

    private void AddCharacterToParty(CharacterData characterData)
    {
        Character newCharacter = new Character
        {
            CharacterName = characterData.character.CharacterName,
            CurrentHealth = characterData.character.CurrentHealth,
            MaxHealth = characterData.character.MaxHealth,
            CurrentMana = characterData.character.CurrentMana,
            MaxMana = characterData.character.MaxMana,
            BaseDamage = characterData.character.BaseDamage,
            Defend = characterData.character.Defend,
            OnDefend = characterData.character.OnDefend,
            CurrentSpeed = characterData.character.Speed,
            Speed = characterData.character.Speed,
            Skills = new List<SkillData>(characterData.character.Skills), // Copy skills list
            CharacterBaseBattleSprite = characterData.character.CharacterBaseBattleSprite
        };

        enemyParty.Add(newCharacter);
    }
}
