using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    public class UIManager : SingletonMono<UIManager>
    {
        public PanelManager PM;

        protected override void Awake()
        {
            base.Awake();
            PM = new PanelManager(this);
        }

        private void Update()
        {
            PM.Update();
        }

        /// <summary>
        /// ��ȡ������һ��UI����
        /// </summary>
        /// <param name="type"> UI������������� </param>
        /// <returns></returns>
        public GameObject GenerateSingleUI(UIType type)
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                Debug.LogError("�������󲻴���");
                return null;
            }
            // �����ڳ�����Ѱ��
            GameObject newUI = GameObject.Find(type.Name);
            if(newUI == null)
            {
                // ����
                newUI = Instantiate(Resources.Load<GameObject>(type.Path), canvas.transform);
                newUI.name = type.Name;
            }
            return newUI;
        }
    }
}