using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    private static bool applicationIsQutting = false;
    public static T Instance
    {
        get
        {
            if (applicationIsQutting)
            {
                return instance;
            }
            if (instance == null && Application.isPlaying) 
            {
                instance = FindAnyObjectByType<T>();
                if(instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).ToString();
                    Debug.Log("Create a Singleton: " + obj.name);
                    DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<T>();
                }
            }
            if (instance == null && !Application.isPlaying)
            {
                instance = FindAnyObjectByType<T>();
            }
            return instance;
        }

        protected set { instance = value; }
    }

    protected virtual void Awake()
    {
        if(instance != null && instance != this as T)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this as T;
        }
    }

    private void OnDestroy()
    {
        applicationIsQutting = true;
    }
}
