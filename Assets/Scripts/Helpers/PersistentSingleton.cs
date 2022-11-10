using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour	where T : Component
{
    public static bool HasInstance => instance != null;
    public static T Current => instance;
		
    protected static T instance;

    /// <summary>
    /// Singleton design pattern
    /// </summary>
    /// <value>The instance.</value>
    public static T Instance
    {
        get
        {
            if (instance) return instance;
            instance = FindObjectOfType<T> ();
            if (instance) return instance;
            GameObject obj = new()
            {
                name = typeof(T).Name + "_AutoCreated"
            };
            instance = obj.AddComponent<T> ();
            return instance;
        }
    }

    /// <summary>
    /// On awake, we check if there's already a copy of the object in the scene. If there's one, we destroy it.
    /// </summary>
    protected virtual void Awake ()
    {
        InitializeSingleton();
    }

    /// <summary>
    /// Initializes the singleton.
    /// </summary>
    protected virtual void InitializeSingleton()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (instance == null)
        {
            //If I am the first instance, make me the Singleton
            instance = this as T;
            DontDestroyOnLoad (transform.gameObject);
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if(this != instance)
            {
                Destroy(gameObject);
            }
        }
    }
}
