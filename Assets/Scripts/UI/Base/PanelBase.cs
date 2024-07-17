using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    /// <summary>
    /// UI面板的基类
    /// </summary>
    public abstract class PanelBase
    {
        public UIType UIType { get; private set; }

        public UITool UITool { get; private set; }

        /// <summary>
        /// 自己所在的PanelManager
        /// </summary>
        public PanelManager PanelManager { get; private set; }

        public UIManager UIManager { get; private set; }

        // 定义回调函数的委托类型
        public delegate void Callback();

        // 声明面板创建和退出的回调函数
        private Callback onEnterCallback;
        private Callback onExitCallback;

        public bool IsStopping { get; private set; }

        /// <summary>
        ///  带回调函数的构造函数
        /// </summary>
        /// <param name="UIType"></param>
        /// <param name="onEnter"></param>
        /// <param name="onExit"></param>
        protected PanelBase(UIType UIType,Callback onEnter,Callback onExit)
        {
            this.UIType = UIType;
            this.onEnterCallback = onEnter;
            this.onExitCallback = onExit;
        }

        /// <summary>
        /// 使用UIType构造函数，但一般接受静态路径作为参数<br></br>
        /// 示例用法：
        /// <code>
        /// static readonly string path = "UI/Panel/DynamicPath";
        /// public Panel() : base(new UIType(path)) { }
        /// </code>
        /// </summary>
        protected PanelBase(UIType uIType)
        {
            UIType = uIType;
        }

        /// <summary>
        ///  初始化UITool
        /// </summary>
        /// <param name="uITool"></param>
        public void Init(UITool uITool)
        {
            UITool = uITool;
        }

        /// <summary>
        ///  初始化PanelManager
        /// </summary>
        /// <param name="panelManager"></param>
        public void Init(PanelManager panelManager)
        {
            PanelManager = panelManager;
        }

        /// <summary>
        ///  初始化UIManager
        /// </summary>
        /// <param name="uiManager"></param>
        public void Init(UIManager uiManager)
        {
            UIManager = uiManager;
        }

        /// <summary>
        /// 创建时的调用，默认调用进入回调
        /// </summary>
        public virtual void OnEnter()
        {
            onEnterCallback?.Invoke();
        }

        public virtual void OnUpdate()
        {

        }

        /// <summary>
        /// 默认将GameObject SetActive to false
        /// </summary>
        public virtual void OnPause()
        {
            if (UITool.TryGetOrAddComponent<Transform>(out Transform trans))
            {
                trans.gameObject.SetActive(false);
            }
            IsStopping = true;
        }

        /// <summary>
        /// 默认将GameObject SetActive to true
        /// </summary>
        public virtual void OnResume()
        {
            if (UITool.TryGetOrAddComponent<Transform>(out Transform trans))
            {
                trans.gameObject.SetActive(true);
            }
            IsStopping = false;
        }

        /// <summary>
        /// 删除时的调用，默认删除GameObject 和 调用退出回调函数
        /// </summary>
        public virtual void OnExit()
        {
            onExitCallback?.Invoke();
            if (UITool.TryGetOrAddComponent<Transform>(out Transform trans))
            {
                GameObject.Destroy(trans.gameObject);
            }
        }

        /// <summary>
        /// 延迟触发退出函数
        /// </summary>
        /// <param name="time"></param>
        public virtual void OnExit(float time)
        {
            UIManager.StartCoroutine(DelayExitCo(time));
            IEnumerator DelayExitCo(float time)
            {
                yield return new WaitForSeconds(time);
                OnExit();
            }
        }
    }
}