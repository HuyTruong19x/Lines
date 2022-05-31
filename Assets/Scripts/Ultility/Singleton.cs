using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    [SerializeField]
    private bool _IsPersistent = true;
    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                var instances = FindObjectsOfType<T>();
                if (instances.Length > 0)
                {
                    if (instances.Length > 1)
                    {
                        for (int i = 1; i < instances.Length; i++)
                        {
                            Destroy(instances[i].gameObject);
                        }
                    }
                    _instance = instances[0];
                }
                else
                {
                    _instance = new GameObject($"{nameof(Singleton<T>)}{typeof(T)}").AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        var instances = FindObjectsOfType<T>();
        if (instances.Length > 1)
        {
            for (int i = 1; i < instances.Length; i++)
            {
                Debug.Log("<color=red>Already another " + this.name + " object, will destroy this </color>" + instances[i].GetInstanceID());
                Destroy(instances[i].gameObject);
            }
        }
        if (_IsPersistent)
        {
            DontDestroyOnLoad(gameObject);
        }
        OnAwake();
    }
    protected virtual void OnAwake() { }
}
