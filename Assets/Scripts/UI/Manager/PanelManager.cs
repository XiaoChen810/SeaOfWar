using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    /// <summary>
    /// 管理面板的
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
        /// 添加新面板，默认暂停原先顶层的面板，把新面板压入栈
        /// </summary>
        /// <param name="nextPanel"></param>
        /// <param name="stopCurrentPanel"> 是否暂停顶层面板 </param>
        public void AddPanel(PanelBase nextPanel, bool stopCurrentPanel = true)
        {
            if(_panelsStack.Count > 0 && stopCurrentPanel)
            {
                PanelBase currentPanel = _panelsStack.Peek();
                currentPanel.OnPause();
            }

            // 获取或创建nextPanel
            GameObject nextPanelObject = _manager.GenerateSingleUI(nextPanel.UIType);
            // 初始化nextPanel的UITool
            nextPanel.Init(new UITool(nextPanelObject));
            // 初始化nextPanel的PanelManager为自己
            nextPanel.Init(this);
            // 初始化nextPanel的UIManager
            nextPanel.Init(_manager);
            // 把nextPanel压入栈
            _panelsStack.Push(nextPanel);
            // 调用nextPanel进入时的方法
            nextPanel.OnEnter();
        }

        /// <summary>
        /// 移除面板, 如果移除的是顶层的面板，则会尝试恢复下一个面板
        /// </summary>
        /// <param name="removedPanel"> 默认为空，即移除顶层面板 </param>
        public bool RemovePanel(PanelBase removedPanel = null)
        {
            bool flag = false;
            // 移除顶点面板
            if(_panelsStack.Count > 0)
            {
                if (removedPanel == null || removedPanel == _panelsStack.Peek())
                {
                    _panelsStack.Pop().OnExit();
                    // 移除后尝试恢复下一个面板解除暂停
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
        /// 获取顶层面板
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
        /// 面板数量是否为空
        /// </summary>
        /// <returns></returns>
        public bool PanelSpace()
        {
            return _panelsStack.Count == 0;
        }
    }
}