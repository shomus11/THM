using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [SerializeField] float moveSpeed = 5.0f; // Kecepatan karakter
    [SerializeField] bool canMove;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D boundaryCollider;
    [SerializeField] private List<Collider2D> boundaryList = new List<Collider2D>();
    [SerializeField] private List<Collider2D> cantOverlapthisColliderList = new List<Collider2D>();
    [SerializeField] private Direction idleTemp = Direction.Down;
    [SerializeField] private Transform slashPoint;
    [SerializeField] private Animator slashAnimator;
    [SerializeField] private float slashAnimationLength;
    bool attacking = false;
    public List<CharacterData> party;
    public List<Character> playerParty;
    bool canInteract = false;

    private Vector2 lastValidPosition;
    Vector2 movement;
    public bool CanMove { get => canMove; set => canMove = value; }
    public bool CanInteract { get => canInteract; set => canInteract = value; }

    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RecalculatingBoundary();
        canMove = true;
        lastValidPosition = transform.position;
        slashPoint.gameObject.SetActive(false);
        InitializePlayerParty();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        if (canMove)
        {
            Movement();
            AnimationDirection();
        }
        else
        {
            //animator.Play("Idle " + idleTemp);
        }
    }

    private void InitializePlayerParty()
    {
        playerParty.Clear();
        foreach (CharacterData characterData in party)
        {
            AddCharacterToPlayerParty(characterData);
        }
    }
    public void AddPartyMember(CharacterData newMemberData)
    {
        // Prevent duplicates
        if (!party.Contains(newMemberData))
        {
            party.Add(newMemberData);
            AddCharacterToPlayerParty(newMemberData);
            Debug.Log($"{newMemberData.character.CharacterName} has joined the party!");
        }
        else
        {
            Debug.Log($"{newMemberData.character.CharacterName} is already in the party.");
        }
    }
    private void AddCharacterToPlayerParty(CharacterData characterData)
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

        playerParty.Add(newCharacter);
    }


    void AnimationDirection()
    {
        //Debug.Log(movement.magnitude);
        if (movement.magnitude > 0f)
        {
            animator.SetFloat("dirX", movement.x);
            animator.SetFloat("dirY", movement.y);
            animator.speed = 1;
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                if (movement.x > 0)
                    idleTemp = Direction.Right;
                else
                    idleTemp = Direction.Left;
            }
            else
            {
                if (movement.y > 0)
                    idleTemp = Direction.Up;
                else
                    idleTemp = Direction.Down;
            }
            //animator.Play("Direction " + idleTemp);
        }
        else
        {
            animator.speed = 0;
            // Jika tidak bergerak, berhenti bermain animasi
            //animator.Play("Idle " + idleTemp);
        }
    }
    void Movement()
    {
        Vector2 mov = new Vector2(movement.x, movement.y) * moveSpeed * Time.fixedDeltaTime;
        Vector2 newPosition = (Vector2)rb.position + mov;

        // Cek apakah posisi baru di dalam boundary
        if (!boundaryCollider.bounds.Contains(newPosition))
        {
            newPosition = boundaryCollider.ClosestPoint(newPosition);
        }

        // Cek apakah bertabrakan dengan collider yang tidak boleh dilewati
        Collider2D hitCollider = Physics2D.OverlapPoint(newPosition);
        if (hitCollider != null && cantOverlapthisColliderList.Contains(hitCollider))
        {
            // Jika bertabrakan, tetap di posisi terakhir yang valid
            rb.MovePosition(lastValidPosition);
            return;
        }

        // Jika posisi valid, update posisi terakhir yang valid
        lastValidPosition = newPosition;
        rb.MovePosition(newPosition);
    }
    public void Attack(InputAction.CallbackContext context)
    {
        if (canMove)
            if (context.started)
            {
                if (!attacking && !canInteract)
                    TriggerAttack();
            }
    }

    public void TriggerAttack()
    {
        SoundManager.Instance.PlaySE("Sword1");
        Vector2 playerPos = transform.position;
        Vector2 slashPos = Vector2.down;
        Vector3 SlashRotation = Vector3.zero;
        switch (idleTemp)
        {
            case Direction.Down:
                slashPos = playerPos + Vector2.down;
                SlashRotation = Vector3.forward * 90;
                break;
            case Direction.Up:
                slashPos = playerPos + Vector2.up;
                SlashRotation = Vector3.forward * -90;
                break;
            case Direction.Left:
                slashPos = playerPos + Vector2.left;
                SlashRotation = Vector3.forward * 0;
                break;
            case Direction.Right:
                slashPos = playerPos + Vector2.right;
                SlashRotation = (Vector3.forward + Vector3.right) * 180;
                break;
            default:
                break;
        }
        slashPoint.position = slashPos;
        slashPoint.eulerAngles = SlashRotation;
        StartCoroutine(Attacking());
    }

    IEnumerator Attacking()
    {
        attacking = true;
        slashPoint.gameObject.SetActive(true);
        slashAnimator.Play("Attack");
        yield return new WaitForSeconds(slashAnimationLength);
        slashPoint.gameObject.SetActive(false);
        attacking = false;
    }

    //float GetAnimationClipDuration(Animator anim, string name)
    //{
    //    if (anim == null || anim.runtimeAnimatorController == null) return 0f;

    //    foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
    //    {
    //        if (clip.name == name)
    //        {
    //            return clip.length; // Returns duration in seconds
    //        }
    //    }
    //    return 0f; // Return 0 if clip not found
    //}


    public void MovementInput(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void RecalculatingBoundary()
    {
        canMove = false;
        Vector3 playerPosition = transform.position;

        for (int i = 0; i < boundaryList.Count; i++)
        {
            if (boundaryList[i].bounds.Contains(playerPosition))
            {
                // Jika pemain berada dalam collider objek peta, set objek map yang dipilih ke objek ini
                Debug.Log(gameObject + " berada di area " + boundaryList[i]);
                boundaryCollider = boundaryList[i];
                // Keluar dari loop karena objek map sudah ditemukan
                break;
            }
            else
            {
                // Jika pemain tidak berada dalam collider objek peta, set objek map yang dipilih ke null
                Debug.Log(gameObject + " tidak berada di area " + boundaryList[i]);
                boundaryCollider = null;
            }
        }
    }
}
