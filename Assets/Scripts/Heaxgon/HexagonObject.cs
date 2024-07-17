using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HexagonObject : MonoBehaviour
{
    [SerializeField] protected GameObject _gameObject;

    /// <summary>
    /// 该地块现在的Object
    /// </summary>
    public GameObject GameObject
    {
        get
        {
            if (_gameObject == null)
            {
                Debug.LogWarning("返回Object为空");
            }
            return _gameObject;
        }
        set
        {
            if (value != null)
            {
                _gameObject = value;
                foreach (Transform child in transform)
                {
                    if (_gameObject == null || child.gameObject != _gameObject)
                    {
                        child.gameObject.SetActive(false);
                    }
                    else
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    public void OnEnable()
    {
        GameObject = GetObject();
    }

    protected abstract GameObject GetObject();
   
}
