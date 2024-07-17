using ChenChen_Core;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class P_CastlePanel : PanelBase
    {
        private static readonly string path = "Prefabs/UI/Panel/CastlePanel";

        private PlayerCastle playerCastle;
        private bool canOperate;

        private PanelBase buildingPanel = null;

        private Slider healthBar;

        public P_CastlePanel(PlayerCastle playerCastle, bool canOperate) : base(new UIType(path))
        {
            this.playerCastle = playerCastle;
            this.canOperate = canOperate;
        }

        public override void OnEnter()
        {
            Button btn_b = UITool.TryGetChildComponentByName<Button>("Btn_BuildingPanel");

            UITool.TryGetChildComponentByName<Button>("Close").onClick.AddListener(() =>
            {
                if (buildingPanel != null)
                    BuildingManager.Instance.ClosePanel();
                UIManager.Instance.PM.RemovePanel(this);
            });

            healthBar = UITool.TryGetChildComponentByName<Slider>("HealthBar");

            if (!canOperate)
            {
                btn_b.gameObject.SetActive(false);
            }
            else
            {
                btn_b.onClick.AddListener(() =>
                {
                    if (buildingPanel != null)
                    {
                        BuildingManager.Instance.ClosePanel();
                        buildingPanel = null;
                    }
                    else
                    {
                        buildingPanel = BuildingManager.Instance.OpenPanel();
                    }
                });
            }
        }

        public override void OnUpdate()
        {
            healthBar.value = playerCastle.HpValue;
        }
    }
}
