using UnityEngine;

public abstract class ASingleton<T> : MonoBehaviour where T : ASingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    Debug.LogError("No instance found in scene");
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"Duplicate instance detected on {gameObject.name}. Destroying this one");
            Destroy(this);
            return;
        }

        _instance = (T)this;
    }
}
