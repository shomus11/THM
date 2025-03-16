using UnityEngine;


public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = FindAnyObjectByType<T>();
            if (_instance != this)
            {
                _instance = null;
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                Init();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Init()
    {

    }
}
