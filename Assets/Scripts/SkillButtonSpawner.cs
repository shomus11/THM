using UnityEngine;

public class SkillButtonSpawner : BaseObjectPooling
{
    [SerializeField]
    GameObject skillPrefabs;

    [SerializeField] Transform skillButtonParent;
    [SerializeField] int skillButtonPoolSize;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnInitializationPoolObject(skillButtonPoolSize, pooledObjectList, skillPrefabs, skillButtonParent);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
