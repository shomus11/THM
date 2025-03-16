using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Game/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public int damage;
    public int manaCost;
    public bool isHeal = false;
}