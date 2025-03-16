using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Game/Character")]
public class CharacterData : ScriptableObject
{
    public Character character;
}

[System.Serializable]
public class Character
{
    [SerializeField] string characterName;
    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;
    [SerializeField] int currentMana;
    [SerializeField] int maxMana;
    [SerializeField] int baseDamage;
    [SerializeField] int defend;
    [SerializeField] bool onDefend;
    [SerializeField] int currentSpeed;
    [SerializeField] int speed;
    [SerializeField] List<SkillData> skills;
    [Header("Battle Component")]
    [SerializeField] Sprite characterBaseBattleSprite;

    public string CharacterName { get => characterName; set => characterName = value; }
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int CurrentMana { get => currentMana; set => currentMana = value; }
    public int MaxMana { get => maxMana; set => maxMana = value; }
    public int BaseDamage { get => baseDamage; set => baseDamage = value; }
    public int Defend { get => defend; set => defend = value; }
    public bool OnDefend { get => onDefend; set => onDefend = value; }
    public int CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }
    public int Speed { get => speed; set => speed = value; }
    public List<SkillData> Skills { get => skills; set => skills = value; }
    public Sprite CharacterBaseBattleSprite { get => characterBaseBattleSprite; set => characterBaseBattleSprite = value; }

}
