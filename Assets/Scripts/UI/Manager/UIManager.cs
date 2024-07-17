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
        /// 获取单个的一个UI对象
        /// </summary>
        /// <param name="type"> UI对象的类型数据 </param>
        /// <returns></returns>
        public GameObject GenerateSingleUI(UIType type)
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                Debug.LogError("画布对象不存在");
                return null;
            }
            // 尝试在场景中寻找
            GameObject newUI = GameObject.Find(type.Name);
            if(newUI == null)
            {
                // 生成
                newUI = Instantiate(Resources.Load<GameObject>(type.Path), canvas.transform);
                newUI.name = type.Name;
            }
            return newUI;
        }
    }
}