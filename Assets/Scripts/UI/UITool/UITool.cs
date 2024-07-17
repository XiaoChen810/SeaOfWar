using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    /// <summary>
    /// ����UI���������������, ����ͨ��·�������ֻ�ȡ�������������
    /// </summary>
    public class UITool
    {
        // ��ǰ�����
        GameObject currentPanel;
        
        /// <summary>
        /// ����Լ���Ӧ��GameObject
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
        /// ��ȡ����������
        /// </summary>
        /// <typeparam name="T">������ͣ�����Button</typeparam>
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
        ///  ͨ�����ֻ�ȡ������
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

            Debug.LogWarning("�Ҳ��������" + Name);
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
        ///  ͨ�����ֻ�ȡһ�������������������ȡ<see href="Button"/>
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
        /// ͨ��·����ȡ������
        /// </summary>
        /// <param name="path">·����ʹ��/�ָ�</param>
        /// <returns>���ҵ���������</returns>
        public GameObject GetChildByPath(string path)
        {
            Transform child = FindChildByPathRecursively(currentPanel.transform, path);
            if (child != null)
            {
                return child.gameObject;
            }

            Debug.LogWarning("�Ҳ�����·���µ������壺" + path);
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
                    Debug.LogWarning("�Ҳ���·���е������壺" + segment);
                    return null;
                }
            }

            return child;
        }

        /// <summary>
        ///  ͨ��·����ȡһ�������������������ȡ<see href="Button"/>
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