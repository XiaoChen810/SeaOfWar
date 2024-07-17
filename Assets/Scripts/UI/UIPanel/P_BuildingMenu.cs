using ChenChen_Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class P_BuildingMenu : PanelBase
    {
        private static readonly string path = "Prefabs/UI/Panel/BuildingMenu";
        private static readonly string path_btn = "Prefabs/UI/Button/BuildingBtn";

        public P_BuildingMenu() : base(new UIType(path))
        {
        }

        private BuildingMenu _menu = null;
        private GameObject content;
        private GameObject buildingBtn;

        public override void OnEnter()
        {
            // 从BuildingManager加载所有建筑
            _menu = BuildingManager.Instance.Menu;

            content = UITool.GetChildByName("Content");
            buildingBtn = Resources.Load(path_btn) as GameObject;
            
            foreach(var b_def in _menu.buildingsDefs)
            {
                GameObject newBtn = GameObject.Instantiate(buildingBtn, content.transform);
                newBtn.transform.GetComponentInChildren<Text>().text = b_def.name;
                if(newBtn.TryGetComponent<Button>(out Button btn))
                {
                    btn.onClick.AddListener(() =>
                    {
                        BuildingManager.Instance.OpenBuildMode(b_def);
                    });
                }
            }

            //UITool.TryGetChildComponentByName<Button>("Close").onClick.AddListener(() =>
            //{
            //    UIManager.Instance.PM.RemovePanel(this);
            //});
        }
    }
}