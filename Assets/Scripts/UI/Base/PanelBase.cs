using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    /// <summary>
    /// UI���Ļ���
    /// </summary>
    public abstract class PanelBase
    {
        public UIType UIType { get; private set; }

        public UITool UITool { get; private set; }

        /// <summary>
        /// �Լ����ڵ�PanelManager
        /// </summary>
        public PanelManager PanelManager { get; private set; }

        public UIManager UIManager { get; private set; }

        // ����ص�������ί������
        public delegate void Callback();

        // ������崴�����˳��Ļص�����
        private Callback onEnterCallback;
        private Callback onExitCallback;

        public bool IsStopping { get; private set; }

        /// <summary>
        ///  ���ص������Ĺ��캯��
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
        /// ʹ��UIType���캯������һ����ܾ�̬·����Ϊ����<br></br>
        /// ʾ���÷���
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
        ///  ��ʼ��UITool
        /// </summary>
        /// <param name="uITool"></param>
        public void Init(UITool uITool)
        {
            UITool = uITool;
        }

        /// <summary>
        ///  ��ʼ��PanelManager
        /// </summary>
        /// <param name="panelManager"></param>
        public void Init(PanelManager panelManager)
        {
            PanelManager = panelManager;
        }

        /// <summary>
        ///  ��ʼ��UIManager
        /// </summary>
        /// <param name="uiManager"></param>
        public void Init(UIManager uiManager)
        {
            UIManager = uiManager;
        }

        /// <summary>
        /// ����ʱ�ĵ��ã�Ĭ�ϵ��ý���ص�
        /// </summary>
        public virtual void OnEnter()
        {
            onEnterCallback?.Invoke();
        }

        public virtual void OnUpdate()
        {

        }

        /// <summary>
        /// Ĭ�Ͻ�GameObject SetActive to false
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
        /// Ĭ�Ͻ�GameObject SetActive to true
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
        /// ɾ��ʱ�ĵ��ã�Ĭ��ɾ��GameObject �� �����˳��ص�����
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
        /// �ӳٴ����˳�����
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