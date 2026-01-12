using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    protected virtual bool IsDDOL => true;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에서 이미 존재하는 인스턴스 찾기
                _instance = FindObjectOfType<T>();

                // 없으면 새 GameObject 생성해서 붙이기
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                    var singleton = _instance as Singleton<T>;
                    if (singleton != null && singleton.IsDDOL)
                    {
                        DontDestroyOnLoad(obj);
                    }
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if(IsDDOL)DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}

