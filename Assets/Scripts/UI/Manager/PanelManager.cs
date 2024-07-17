using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    /// <summary>
    /// ��������
    /// </summary>
    public class PanelManager
    {
        private Stack<PanelBase> _panelsStack;

        private UIManager _manager;

        public PanelManager(UIManager manager)
        {
            _panelsStack = new Stack<PanelBase>();
            _manager = manager;
        }

        public void Update()
        {
            foreach (var panel in _panelsStack)
            {
                if(!panel.IsStopping)
                {
                    panel.OnUpdate();
                }
            }
        }

        /// <summary>
        /// �������壬Ĭ����ͣԭ�ȶ������壬�������ѹ��ջ
        /// </summary>
        /// <param name="nextPanel"></param>
        /// <param name="stopCurrentPanel"> �Ƿ���ͣ������� </param>
        public void AddPanel(PanelBase nextPanel, bool stopCurrentPanel = true)
        {
            if(_panelsStack.Count > 0 && stopCurrentPanel)
            {
                PanelBase currentPanel = _panelsStack.Peek();
                currentPanel.OnPause();
            }

            // ��ȡ�򴴽�nextPanel
            GameObject nextPanelObject = _manager.GenerateSingleUI(nextPanel.UIType);
            // ��ʼ��nextPanel��UITool
            nextPanel.Init(new UITool(nextPanelObject));
            // ��ʼ��nextPanel��PanelManagerΪ�Լ�
            nextPanel.Init(this);
            // ��ʼ��nextPanel��UIManager
            nextPanel.Init(_manager);
            // ��nextPanelѹ��ջ
            _panelsStack.Push(nextPanel);
            // ����nextPanel����ʱ�ķ���
            nextPanel.OnEnter();
        }

        /// <summary>
        /// �Ƴ����, ����Ƴ����Ƕ������壬��᳢�Իָ���һ�����
        /// </summary>
        /// <param name="removedPanel"> Ĭ��Ϊ�գ����Ƴ�������� </param>
        public bool RemovePanel(PanelBase removedPanel = null)
        {
            bool flag = false;
            // �Ƴ��������
            if(_panelsStack.Count > 0)
            {
                if (removedPanel == null || removedPanel == _panelsStack.Peek())
                {
                    _panelsStack.Pop().OnExit();
                    // �Ƴ����Իָ���һ���������ͣ
                    if (_panelsStack.Count > 0 && _panelsStack.Peek().IsStopping)
                    {
                        _panelsStack.Peek().OnResume();
                    }
                    flag = true;
                }
                else
                {
                    Stack<PanelBase> temp = new();
                    while(_panelsStack.Count > 0)
                    {
                        temp.Push(_panelsStack.Pop());
                        if(_panelsStack.Peek() == removedPanel)
                        {
                            _panelsStack.Pop().OnExit();
                            flag = true;
                            break;
                        }
                    }
                    while(temp.Count > 0)
                    {
                        _panelsStack.Push(temp.Pop());
                    }
                }
            }

            return flag;
        }

        /// <summary>
        /// ��ȡ�������
        /// </summary>
        public bool TryGetTopPanel(out PanelBase top)
        {
            _panelsStack.TryPeek(out var panel);
            if (panel != null)
            {
                top = panel;
                return true;
            }
            top = null;
            return false;
        }

        /// <summary>
        /// ��������Ƿ�Ϊ��
        /// </summary>
        /// <returns></returns>
        public bool PanelSpace()
        {
            return _panelsStack.Count == 0;
        }
    }
}