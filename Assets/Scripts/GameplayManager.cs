using System.Collections.Generic;
using UnityEngine;
public enum GameState
{
    exploring,
    battle,
}
public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;
    public GameState gameState;

    public string bgmTarget;

    public PlayerController[] players;
    public List<EnemyController> enemies;

    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        players = FindObjectsOfType<PlayerController>();
        EnemyController[] enemy = FindObjectsOfType<EnemyController>();
        foreach (EnemyController item in enemy)
        {
            enemies.Add(item);
        }
        PlayExploringBGM();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PlayExploringBGM()
    {
        SoundManager.Instance.PlayBGM(bgmTarget);
    }
    public void SetPlayerMove(bool set)
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].CanMove = set;
        }
    }
    public void SetEnemyMove(bool set)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].CanMove = set;
        }
    }
    public void AddEnemy(EnemyController target)
    {
        enemies.Add(target);
    }

}
