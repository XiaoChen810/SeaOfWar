using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class P_Message : PanelBase
    {
        private static readonly string path = "Prefabs/UI/Panel/Message";
        private Action onCanel;
        private Action onAccept;

        public P_Message(string titile, Action onCanel, Action onAccept) : base(new UIType(path))
        {
            UITool.TryGetChildComponentByName<Text>("Titile").text = titile;
            this.onCanel = onCanel;
            this.onAccept = onAccept;
        }

        public override void OnEnter()
        {
            Time.timeScale = 0;
            UITool.TryGetChildComponentByName<Button>("Canel").onClick.AddListener(() =>
            {
                Time.timeScale = 1;
                onCanel();           
            });
            UITool.TryGetChildComponentByName<Button>("Accept").onClick.AddListener(() =>
            {
                Time.timeScale = 1;
                onAccept();            
            });
        }
    }
}