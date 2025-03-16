using System.Collections.Generic;
using UnityEngine;

public class BaseObjectPooling : MonoBehaviour
{
    public List<GameObject> pooledObjectList;
    public void SpawnInitializationPoolObject(int poolSize, List<GameObject> listTarget, GameObject prefabsTarget, Transform parent = null)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = parent == null ? Instantiate(prefabsTarget) : Instantiate(prefabsTarget, parent.position, Quaternion.identity);
            go.transform.parent = parent;
            go.SetActive(false);
            listTarget.Add(go);
        }
    }

    public void SpawnInitializationPoolObject(int poolSize, List<GameObject> listTarget, List<GameObject> prefabsTarget, Transform parent = null)
    {
        for (int i = 0; i < poolSize; i++)
        {
            int random = Random.Range(0, prefabsTarget.Count);
            GameObject go = parent == null ? Instantiate(prefabsTarget[random]) : Instantiate(prefabsTarget[random], parent.position, Quaternion.identity);
            go.SetActive(false);
            listTarget.Add(go);
        }
    }

    public virtual GameObject GetPooledObject(object type)
    {
        return null;
    }

    public virtual GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjectList.Count; i++)
        {
            if (!pooledObjectList[i].activeInHierarchy)
            {
                return pooledObjectList[i];
            }
        }
        return null;
    }
    public virtual GameObject GetRandomizePooledObject()
    {
        int randomIndex = Random.Range(0, pooledObjectList.Count);
        for (int i = 0; i < pooledObjectList.Count; i++)
        {
            if (!pooledObjectList[randomIndex].activeInHierarchy)
            {
                return pooledObjectList[randomIndex];
            }
            randomIndex = Random.Range(0, pooledObjectList.Count);
        }
        return null;
    }
}