using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    /// <summary>
    /// 管理UI面板里面的组件工具, 包括通过路径或名字获取子组件，子物体
    /// </summary>
    public class UITool
    {
        // 当前的面板
        GameObject currentPanel;
        
        /// <summary>
        /// 面板自己对应的GameObject
        /// </summary>
        public GameObject MyGameObject
        {
            get { return currentPanel; }
        }

        public UITool(GameObject currentPanel)
        {
            this.currentPanel = currentPanel;
        }

        /// <summary>
        /// 获取或者添加组件
        /// </summary>
        /// <typeparam name="T">组件类型，例如Button</typeparam>
        /// <returns></returns>
        public bool TryGetOrAddComponent<T>(out T component) where T : Component
        {
            component = null;
            if (currentPanel == null) return false;
            if (currentPanel.GetComponent<T>() == null)
            {
                currentPanel.AddComponent<T>();
            }
            component = currentPanel.GetComponent<T>();
            return component != null;
        }

        /// <summary>
        ///  通过名字获取子物体
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public GameObject GetChildByName(string Name)
        {
            Transform child = FindChildRecursively(currentPanel.transform, Name);
            if (child != null)
            {
                return child.gameObject;
            }

            Debug.LogWarning("找不到该组件" + Name);
            return null;
        }

        private Transform FindChildRecursively(Transform parent, string childName)
        {
            Transform child = parent.Find(childName);
            if (child != null)
            {
                return child;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform foundChild = FindChildRecursively(parent.GetChild(i), childName);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }

            return null;
        }

        /// <summary>
        ///  通过名字获取一个子物体的组件，例如获取<see href="Button"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentName"></param>
        /// <returns></returns>
        public T TryGetChildComponentByName<T>(string componentName) where T : Component
        {
            GameObject child = GetChildByName(componentName);
            if (child != null)
            {
                if (child.GetComponent<T>() == null) child.AddComponent<T>();

                return child.GetComponent<T>();
            }

            Debug.LogWarning("No Find The Component name of : " + componentName);
            return null;
        }

        /// <summary>
        /// 通过路径获取子物体
        /// </summary>
        /// <param name="path">路径，使用/分割</param>
        /// <returns>查找到的子物体</returns>
        public GameObject GetChildByPath(string path)
        {
            Transform child = FindChildByPathRecursively(currentPanel.transform, path);
            if (child != null)
            {
                return child.gameObject;
            }

            Debug.LogWarning("找不到该路径下的子物体：" + path);
            return null;
        }

        private Transform FindChildByPathRecursively(Transform parent, string path)
        {
            string[] pathSegments = path.Split('/');
            Transform child = parent;

            foreach (string segment in pathSegments)
            {
                child = child.Find(segment);
                if (child == null)
                {
                    Debug.LogWarning("找不到路径中的子物体：" + segment);
                    return null;
                }
            }

            return child;
        }

        /// <summary>
        ///  通过路径获取一个子物体的组件，例如获取<see href="Button"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentPath"></param>
        /// <returns></returns>
        public T TryGetChildComponentByPath<T>(string componentPath) where T : Component
        {
            GameObject child = GetChildByPath(componentPath);
            if (child != null)
            {
                if (child.GetComponent<T>() == null) child.AddComponent<T>();

                return child.GetComponent<T>();
            }

            Debug.LogWarning("No Find The Component name of : " + componentPath);
            return null;
        }
    }
}