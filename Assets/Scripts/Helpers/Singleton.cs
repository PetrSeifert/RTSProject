using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    protected static T instance;
    public static bool HasInstance => instance != null;
    public static T TryGetInstance() => HasInstance ? instance : null;
    public static T Current => instance;
    public static bool applicationQuiting;

    /// <summary>
    /// Singleton design pattern
    /// </summary>
    /// <value>The instance.</value>
    public static T Instance
    {
        get
        {
            if (applicationQuiting) return null;
            
            if (instance == null)
            {
                instance = FindObjectOfType<T> ();
                if (instance == null)
                {
                    GameObject obj = new();
                    obj.name = typeof(T).Name + "_AutoCreated";
                    instance = obj.AddComponent<T> ();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// On awake, we initialize our instance. Make sure to call base.Awake() in override if you need awake.
    /// </summary>
    protected virtual void Awake ()
    {
        InitializeSingleton();		
    }

    protected virtual void OnDestroy()
    {
        Debug.Log("Destroy");
        applicationQuiting = true;
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

        instance = this as T;
    }
}
