using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI.Utiliy;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public GameObject BattleSystemObject;
    public List<GameObject> playerParty;
    public List<GameObject> enemyParty;

    public PlayerController playerController;
    public EnemyController enemyController;

    [SerializeField] Character characterTurn;

    [Header("CharacterData")]
    public List<Character> playerPartyData;
    public List<Character> enemyPartyData;
    [SerializeField] List<Character> allCombatants = new List<Character>();

    [Header("CharacterTurn")]
    public TextMeshProUGUI playerTurnText;
    public Queue<Character> turnQueue;
    public GameObject characterActionPanel;
    public GameObject skillActionPanel;

    [Header("BattleAction Component")]
    [SerializeField] int actionSelected = 0;
    [SerializeField] int actionMax = 0;
    [SerializeField] List<Button> actionButton;
    [SerializeField] bool onActionSelection = false;

    [Header("BattleSkill Component")]
    [SerializeField] int skillSelected = 0;
    [SerializeField] int skillMax = 0;
    [SerializeField] bool onSkillSelection = false;
    [SerializeField] SkillButtonSpawner skillButtonSpawner;
    [SerializeField] List<Button> skillButton;


    [Header("on Battle Select Target")]
    [SerializeField] bool onBattleSelectTarget = false;
    [SerializeField] int partySelected = 0;
    [SerializeField] int enemypartySelected = 0;

    [Header("Game Over properties")]
    [SerializeField] Canvas gameoverCanvas;
    [SerializeField] List<Button> gameoverButton;
    [SerializeField] int gameoverSelected = 0;
    [SerializeField] bool gameover;



    [Header("Ekstra")]
    [SerializeField] Canvas actionCanvas;
    [SerializeField] Transform targetIndicator;
    //[SerializeField] int enemyDefeated = 0;
    //[SerializeField] int partyDefeated = 0;
    [SerializeField] bool canInput = true;
    public List<GameObject> skillPrefabs;
    public string bgmTarget;


    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actionButton = new List<Button>(characterActionPanel.GetComponentsInChildren<Button>());
        actionMax = actionButton.Count;
        InitButtonListener();
        BattleSystemObject.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
        {
            OnGameOver();
        }
#endif
    }

    public void ListCharacters(List<Character> playerParties, List<Character> enemiesParties)
    {

    }
    public void InitButtonListener()
    {
        for (int i = 0; i < actionButton.Count; i++)
        {
            switch (i)
            {
                case 0: //Attack
                    actionButton[i].onClick.AddListener(OnAttackCharacterSelect);
                    break;
                case 1: //Skill
                    actionButton[i].onClick.AddListener(OpenSkillAction);
                    break;

                case 2: // Defend
                    actionButton[i].onClick.AddListener(Defend);
                    break;

                case 3: // Run
                    actionButton[i].onClick.AddListener(Run);
                    break;
            }
        }
        for (int i = 0; i < gameoverButton.Count; i++)
        {
            switch (i)
            {
                case 0: //Attack
                    gameoverButton[i].onClick.AddListener(Retry);
                    break;
                case 1: //Skill
                    gameoverButton[i].onClick.AddListener(QuitGame);
                    break;
            }
        }
    }
    void Retry()
    {
        Debug.Log("Retry");
        UITransition.instance.FadeOut("ExploringScene");
    }
    void QuitGame()
    {
        Debug.Log("Quit Game");
        UITransition.instance.FadeOut(true);
    }

    public void InitBattle(PlayerController player = null, EnemyController enemy = null, bool playerAttacked = false, bool ambush = false)
    {
        SoundManager.Instance.PlayBGM(bgmTarget);
        GameplayManager.instance.gameState = GameState.battle;
        GameplayManager.instance.SetEnemyMove(false);
        GameplayManager.instance.SetPlayerMove(false);

        //partyDefeated = 0;
        //enemyDefeated = 0;
        playerController = player;
        enemyController = enemy;
        playerPartyData = playerController.playerParty;
        enemyPartyData = enemyController.enemyParty;
        SetPartySpeed(playerAttacked, ambush);
        SetBattleCharacter();
        InitializeTurnQueue();
        BattleSystemObject.gameObject.SetActive(true);
        StartCoroutine(InitTurn());
    }

    void SetPartySpeed(bool playerAttacked = false, bool ambush = false)
    {
        //set current speed or battle speed
        for (int i = 0; i < playerPartyData.Count; i++)
        {
            int battleSpeed = playerPartyData[i].Speed;
            int result = 0;
            if (ambush)
            {
                result = (int)(battleSpeed * .5f);

                Debug.Log("Player Ambush Attack");
            }
            else if (playerAttacked)
            {
                result = -(int)(battleSpeed * .5f);

                Debug.Log("Player Got Attacked");
            }
            else
            {
                result = 0;
                Debug.Log("Player Normal Attack");
            }

            playerPartyData[i].CurrentSpeed = result + battleSpeed;
        }
    }
    void SetEnemyPartySpeed()
    {
        for (int i = 0; i < enemyPartyData.Count; i++)
        {
            int battleSpeed = enemyPartyData[i].Speed;
            enemyPartyData[i].CurrentSpeed = battleSpeed;
        }
    }

    public void SetBattleCharacter()
    {
        // deactive first
        foreach (var obj in playerParty) obj.SetActive(false);
        foreach (var obj in enemyParty) obj.SetActive(false);

        for (int i = 0; i < playerPartyData.Count; i++)
        {
            if (playerPartyData[i].CurrentHealth > 0)
            {
                playerParty[i].SetActive(true);
                playerParty[i].GetComponentInChildren<SpriteRenderer>().sprite = playerPartyData[i].CharacterBaseBattleSprite;
            }
        }

        for (int i = 0; i < enemyPartyData.Count; i++)
        {
            if (enemyPartyData[i].CurrentHealth > 0)
            {
                enemyParty[i].SetActive(true);
                enemyParty[i].GetComponentInChildren<SpriteRenderer>().sprite = enemyPartyData[i].CharacterBaseBattleSprite;
            }
        }


    }

    public void OnBattleFinished()
    {

    }

    private void InitializeTurnQueue()
    {
        // Combine all characters into one queue
        allCombatants = new List<Character>();
        allCombatants.AddRange(playerPartyData);
        allCombatants.AddRange(enemyPartyData);

        // Sort based on character Speed
        allCombatants.Sort((a, b) => b.CurrentSpeed.CompareTo(a.CurrentSpeed));

        turnQueue = new Queue<Character>();

        // Add to queue
        foreach (Character character in allCombatants)
        {
            turnQueue.Enqueue(character);
        }
    }
    IEnumerator InitTurn()
    {
        yield return new WaitForSeconds(1f);
        StartTurn();
    }
    private void StartTurn()
    {
        if (GameplayManager.instance.gameState == GameState.battle)
        {
            Back();
            if (turnQueue.Count == 0)
            {
                InitializeTurnQueue();
            }

            Character currentCharacter = turnQueue.Dequeue();
            Debug.Log($"{currentCharacter.CharacterName}'s turn!");

            if (playerPartyData.Contains(currentCharacter))
            {
                PlayerTurn(currentCharacter);
            }
            else
            {
                EnemyTurn(currentCharacter);
            }
        }
    }

    private void PlayerTurn(Character player)
    {
        characterTurn = player;
        Debug.Log($"Player {player.CharacterName}'s turn. Choose an action.");
        // show BattleAction
        playerTurnText.text = $"{player.CharacterName} turn";
        characterActionPanel.gameObject.SetActive(true);
        //SelectActionButton();
        SelectButton(actionButton, actionSelected);
        onActionSelection = true;
        // Initialize Skill Action

        // Wait for player input: Attack, Skill, Defend, etc.
    }

    private void EnemyTurn(Character enemy)
    {
        HideUI(false);
        characterTurn = enemy;
        Debug.Log($"Enemy {enemy.CharacterName} attacks!");
        characterActionPanel.gameObject.SetActive(false);
        Character target = playerPartyData[0];
        // Example attack logic (attacking the first player member)
        if (playerPartyData.Count > 0)
        {
            int rand = Random.Range(0, playerPartyData.Count);
            target = playerPartyData[rand];
        }
        bool skill = false;
        int randomSkill = Random.Range(0, 1000);
        if (randomSkill < 300)
        {
            skill = true;
        }
        else
        {
            skill = false;
        }
        Attack(enemy, target, skill);
        //EndTurn();
    }

    private void EndTurn()
    {
        onActionSelection = false;
        onSkillSelection = false;
        //if (playerParty.Count == 0)
        //{
        //    Debug.Log("Game Over! You lost.");
        //    return;
        //}
        //if (enemyParty.Count == 0)
        //{
        //    BackToExplore();
        //    Debug.Log("Victory! You won the battle.");
        //    return;
        //}
        StartTurn();
    }

    #region Battle Action


    private void RemoveCharacter(Character character)
    {
        if (playerPartyData.Contains(character))
            playerPartyData.Remove(character);
        else if (enemyPartyData.Contains(character))
            enemyPartyData.Remove(character);

        Debug.Log($"{character.CharacterName} has been removed from battle.");
    }
    public void CheckParty()
    {
        int enemyDefeated = 0;
        int partyDefeated = 0;
        for (int i = 0; i < enemyPartyData.Count; i++)
        {
            if (enemyPartyData[i].CurrentHealth <= 0)
            {
                enemyDefeated++;
            }
        }

        for (int i = 0; i < playerPartyData.Count; i++)
        {
            if (playerPartyData[i].CurrentHealth <= 0)
            {
                partyDefeated++;
            }
        }

        if (enemyDefeated >= enemyPartyData.Count)
        {
            BackToExplore();
        }
        else if (partyDefeated >= playerPartyData.Count)
        {
            //Show GameOverUI
            OnGameOver();
        }
        else
        {
            canInput = true;
            EndTurn();
        }

    }

    void OnGameOver()
    {
        canInput = false;
        gameover = true;
        SelectButton(gameoverButton, gameoverSelected);
        gameoverCanvas.gameObject.SetActive(true);
    }

    void BackToExplore()
    {
        GameplayManager.instance.gameState = GameState.exploring;
        GameplayManager.instance.PlayExploringBGM();
        enemyController.gameObject.SetActive(false);
        EnemySpawner.instance.DestroyEnemy();
        actionCanvas.gameObject.SetActive(false);
        targetIndicator.gameObject.SetActive(false);
        skillActionPanel.gameObject.SetActive(false);
        characterActionPanel.gameObject.SetActive(false);
        onSkillSelection = false;
        onBattleSelectTarget = false;
        onActionSelection = false;
        GameplayManager.instance.SetEnemyMove(true);
        GameplayManager.instance.SetPlayerMove(true);
        BattleSystemObject.gameObject.SetActive(false);
    }
    public void Attack(Character attacker, Character target, bool skillattack = false)
    {
        StartCoroutine(AttackSequence(attacker, target, skillattack));
        //EndTurn();
    }
    IEnumerator AttackSequence(Character attacker, Character target, bool skillattack = false)
    {
        GameObject damageEffect;
        GameObject go;
        int skillNum = 0;
        int index = 0;
        canInput = false;
        HideUI(false);

        // check if skill attack or not for spawn Animation Object
        if (!skillattack)
        {
            damageEffect = skillPrefabs.FirstOrDefault(obj => obj.name == "Attack");
            SoundManager.Instance.PlaySE($"Attack");
        }
        else
        {
            if (playerPartyData.Contains(attacker))
            {
                Debug.Log("player fixed");
                skillNum = skillSelected;

            }
            else
            {

                Debug.Log("enemy random");
                skillNum = Random.Range(0, attacker.Skills.Count);
                //damageEffect = attacker.Skills[Random.Range(0, attacker.Skills.Count)].skillPrefab;
            }
            damageEffect = attacker.Skills[skillNum].skillPrefab;
            SoundManager.Instance.PlaySE($"{attacker.Skills[skillNum].skillName}");
        }

        // check target if in player or enemy party
        if (playerPartyData.Contains(target))
        {
            index = playerPartyData.IndexOf(target);
            go = Instantiate(damageEffect, playerParty[index].transform.position, Quaternion.identity);
        }
        else
        {
            index = enemyPartyData.IndexOf(target);
            go = Instantiate(damageEffect, enemyParty[index].transform.position, Quaternion.identity);
        }
        go.SetActive(true);


        yield return new WaitForSeconds(1f);

        int damage = 0;
        if (target.OnDefend)
        {
            if (skillattack)
            {
                Debug.Log($"{attacker.Skills[skillNum].skillName} used ");
                damage = Mathf.Max(attacker.Skills[skillNum].damage + attacker.BaseDamage - target.Defend, 1);
            }
            else
                damage = Mathf.Max(attacker.BaseDamage - target.Defend, 1);

            target.OnDefend = false;
        }
        else
        {
            if (skillattack)
            {
                Debug.Log($"{attacker.Skills[skillNum].skillName} used ");
                damage = Mathf.Max(attacker.Skills[skillNum].damage + attacker.BaseDamage, 1);
            }
            else
                damage = Mathf.Max(attacker.BaseDamage, 1);
        }

        target.CurrentHealth -= damage;

        Debug.Log($"{attacker.CharacterName} attacked {target.CharacterName} for {damage} damage! and now health were {target.CurrentHealth}");

        if (target.CurrentHealth <= 0)
        {
            Debug.Log($"{target.CharacterName} has been defeated!");
            //RemoveCharacter(target);
        }
        CheckParty();
        go.SetActive(false);
        Destroy(go);
        //if (!enemyPartyData.Contains(attacker))
        //{
        //    HideUI(true);
        //    Back();
        //}
        //canInput = true;
    }

    public void Run()
    {
        if (Random.Range(0, 100) < 50) // 50% chance to escape
        {
            Debug.Log($"successfully ran away!");
            BackToExplore();
            //Do Run
        }
        else
        {
            Debug.Log($"failed to escape!");
            CheckParty();
        }
    }

    public void Defend()
    {
        StartCoroutine(DefendSequence());
        //StartCoroutine(RemoveDefenseBoost(defender, defenseBoost));
    }
    IEnumerator DefendSequence()
    {
        canInput = false;
        HideUI(false);
        Character defender = characterTurn;
        GameObject defendEffect = skillPrefabs.FirstOrDefault(obj => obj.name == "Defend");
        int index = playerPartyData.IndexOf(defender);
        GameObject go = Instantiate(defendEffect, playerParty[index].transform.position, Quaternion.identity);
        SoundManager.Instance.PlaySE("Defend");
        go.SetActive(true);
        yield return new WaitForSeconds(1f);

        //int defenseBoost = defender.Defend / 2;
        defender.OnDefend = true;
        Debug.Log($"{defender.CharacterName} is defending!");
        CheckParty();
        go.SetActive(false);
        Destroy(go);
        HideUI(true);
        Back();
        canInput = true;
    }

    public void ActionSelection(InputAction.CallbackContext context)
    {
        if (canInput && !gameover)
        {
            if (context.started)
            {
                #region Action Selection
                if (onActionSelection && !onSkillSelection && !onBattleSelectTarget)
                {

                    if (context.ReadValue<Vector2>().y > 0)
                    {
                        if (actionSelected != 0)
                        {
                            actionSelected -= 1;
                        }
                    }
                    if (context.ReadValue<Vector2>().y < 0)
                    {
                        if (actionSelected != actionMax - 1)
                        {
                            actionSelected += 1;
                        }

                    }
                    //SelectActionButton();
                    SelectButton(actionButton, actionSelected);

                }
                #endregion

                #region skill Selection
                if (onSkillSelection && !onBattleSelectTarget)
                {

                    if (context.ReadValue<Vector2>().y > 0)
                    {
                        if (skillSelected != 0)
                        {
                            skillSelected -= 1;
                        }
                    }
                    if (context.ReadValue<Vector2>().y < 0)
                    {
                        if (skillSelected != skillMax - 1)
                        {
                            skillSelected += 1;
                        }

                    }
                    SelectButton(skillButton, skillSelected);

                }
                #endregion

                #region Battle Target Selection
                if (onBattleSelectTarget)
                {
                    if (context.ReadValue<Vector2>().y > 0)
                    {
                        if (playerPartyData.Contains(characterTurn))
                        {
                            if (enemypartySelected != 0)
                            {
                                enemypartySelected -= 1;
                            }
                        }
                        else
                        {
                            if (partySelected != 0)
                            {
                                partySelected -= 1;
                            }
                        }
                        SelectingCharacter();
                    }
                    if (context.ReadValue<Vector2>().y < 0)
                    {
                        if (playerPartyData.Contains(characterTurn))
                        {
                            if (enemypartySelected != enemyPartyData.Count - 1)
                            {
                                enemypartySelected += 1;
                            }
                        }
                        else
                        {
                            if (partySelected != playerPartyData.Count - 1)
                            {
                                partySelected += 1;
                            }
                        }
                        SelectingCharacter();

                    }
                }
                #endregion
            }
        }
        else if (!canInput && gameover)
        {
            #region Gameover Selection

            if (context.started)
                if (context.ReadValue<Vector2>().y > 0)
                {
                    if (gameoverSelected != 0)
                    {
                        gameoverSelected -= 1;
                    }
                }
                else if (context.ReadValue<Vector2>().y < 0)
                {
                    if (gameoverSelected != gameoverButton.Count - 1)
                    {
                        gameoverSelected += 1;
                    }

                }
            SelectButton(gameoverButton, gameoverSelected);
            #endregion
        }

    }
    public void SelectAction(InputAction.CallbackContext context)
    {
        if (canInput && !gameover)
        {
            if (GameplayManager.instance.gameState == GameState.battle)
                SoundManager.Instance.PlaySE("Submit");

            if (onActionSelection && !onSkillSelection)
            {
                if (context.started)
                {
                    Debug.Log("Select Action");
                    actionButton[actionSelected].onClick.Invoke();
                }
            }
            else if (onSkillSelection && !onBattleSelectTarget || onSkillSelection && onBattleSelectTarget)
            {
                if (context.started)
                {
                    Debug.Log("Select the skill Action");
                    skillButton[skillSelected].onClick.Invoke();
                }
            }
        }
        else if (!canInput && gameover)
        {
            if (context.started)
            {
                gameoverButton[gameoverSelected].onClick.Invoke();
            }
        }
    }

    public void OpenSkillAction()
    {
        Debug.Log("open Skill");
        //playerTurnText.text = $" skill";
        onSkillSelection = true;
        skillActionPanel.gameObject.SetActive(true);
        characterActionPanel.gameObject.SetActive(false);

        //instantiate Skill Button
        InitSkillButton();
    }

    void InitSkillButton()
    {
        skillButton = new List<Button>();
        for (int i = 0; i < characterTurn.Skills.Count; i++)
        {
            Button buttonSkill = skillButtonSpawner.GetPooledObject().GetComponent<Button>();
            buttonSkill.gameObject.SetActive(true);
            buttonSkill.onClick.RemoveAllListeners();
            buttonSkill.GetComponent<RectTransform>().DOScale(Vector3.one, 0f);
            buttonSkill.GetComponentInChildren<TextMeshProUGUI>().text = characterTurn.Skills[i].skillName;
            skillButton.Add(buttonSkill);
            buttonSkill.onClick.AddListener(SelectSkill);
        }
        skillMax = skillButton.Count;
        SelectButton(skillButton, skillSelected);
    }
    void HideSkillButton()
    {
        for (int i = 0; i < skillButton.Count; i++)
        {
            skillButton[i].gameObject.SetActive(false);
        }
        skillButton = new List<Button>();
    }

    void SelectSkill()
    {
        Debug.Log($"memilih skill {characterTurn.Skills[skillSelected].skillName}");
        OnSkillCharacterSelect();
    }
    public void BackMenu(InputAction.CallbackContext context)
    {
        if (canInput)
        {
            SoundManager.Instance.PlaySE("Cancel");
            if (context.started)
            {
                Back();
            }
        }

    }
    void Back()
    {
        if (onSkillSelection && !onBattleSelectTarget)
        {
            actionCanvas.gameObject.SetActive(true);
            targetIndicator.gameObject.SetActive(false);
            skillActionPanel.gameObject.SetActive(false);
            characterActionPanel.gameObject.SetActive(true);
            HideSkillButton();
            onSkillSelection = false;

        }
        else if (onBattleSelectTarget)
        {
            actionCanvas.gameObject.SetActive(true);
            targetIndicator.gameObject.SetActive(false);
            skillActionPanel.gameObject.SetActive(false);
            characterActionPanel.gameObject.SetActive(true);
            HideSkillButton();
            onSkillSelection = false;
            onBattleSelectTarget = false;
        }
        else
        {
            actionCanvas.gameObject.SetActive(true);
            targetIndicator.gameObject.SetActive(false);
            skillActionPanel.gameObject.SetActive(false);
            characterActionPanel.gameObject.SetActive(true);
            HideSkillButton();
            onSkillSelection = false;
            onBattleSelectTarget = false;
        }
    }

    public void OnAttackCharacterSelect()
    {
        BattleCharacterSelect();
    }
    public void OnSkillCharacterSelect()
    {
        BattleCharacterSelect(true);
    }

    public void BattleCharacterSelect(bool skillDamage = false)
    {
        if (!onBattleSelectTarget)
        {
            Debug.Log("select Target");
            enemypartySelected = 0;
            partySelected = 0;
            onBattleSelectTarget = true;
            actionCanvas.gameObject.SetActive(false);
            skillActionPanel.gameObject.SetActive(false);
            characterActionPanel.gameObject.SetActive(false);
            targetIndicator.gameObject.SetActive(true);

            if (playerPartyData.Contains(characterTurn))
                targetIndicator.transform.position = enemyParty[enemypartySelected].transform.position + Vector3.up;
            else
                targetIndicator.transform.position = playerParty[partySelected].transform.position + Vector3.up;
        }
        else
        {
            if (!skillDamage)
            {
                Attack(characterTurn, enemyPartyData[enemypartySelected]);
            }
            else
            {
                Debug.Log("skill damage");
                Attack(characterTurn, enemyPartyData[enemypartySelected], true);
            }

        }

    }
    public void SelectingCharacter()
    {
        if (playerPartyData.Contains(characterTurn))
            targetIndicator.transform.position = enemyParty[enemypartySelected].transform.position + Vector3.up;
        else
            targetIndicator.transform.position = playerParty[partySelected].transform.position + Vector3.up;
    }

    public void SelectButton(List<Button> targetButton, int targetSelected)
    {
        for (int i = 0; i < targetButton.Count; i++)
        {
            targetButton[i].GetComponent<Image>().color = targetButton[i].colors.normalColor;
        }
        targetButton[targetSelected].GetComponent<Image>().color = targetButton[targetSelected].colors.highlightedColor;
    }
    public void HideUI(bool set)
    {
        actionCanvas.gameObject.SetActive(set);
        targetIndicator.gameObject.SetActive(set);
    }
    #endregion

}
