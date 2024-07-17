using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : new()
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
                Debug.Log("Create new instance: " + instance.ToString());
            }
            return instance;
        }

        protected set { instance = value; }
    }
}
