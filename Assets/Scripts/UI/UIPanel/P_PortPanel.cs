using ChenChen_Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class P_PortPanel : PanelBase
    {
        private readonly static string path = "Prefabs/UI/Panel/PortPanel";

        private B_Port port = null;

        private Text text_needWorkforce = null;
        private Text text_currentProduceBoat = null;

        private Button button_accpet;

        private int _index_boatdef = 0;
        private int index_boatdef
        {
            get { return _index_boatdef; }
            set
            {
                _index_boatdef = value;
                _index_boatdef = (_index_boatdef + 6) % 6;
            }
        }

        public P_PortPanel(B_Port port) : base(new UIType(path))
        {
            this.port = port;
            port.current_produce_boat_def = PlayerSpawner.Instance.boatDefMenu.boatDefs[index_boatdef];
        }

        public override void OnEnter()
        {
            if (port.HasInputAuthority)
            {
                UITool.TryGetChildComponentByName<Button>("上一个").onClick.AddListener(() =>
                {
                    index_boatdef--;
                    port.current_produce_boat_def = PlayerSpawner.Instance.boatDefMenu.boatDefs[index_boatdef];
                });
                UITool.TryGetChildComponentByName<Button>("下一个").onClick.AddListener(() =>
                {
                    index_boatdef++;
                    port.current_produce_boat_def = PlayerSpawner.Instance.boatDefMenu.boatDefs[index_boatdef];
                });
                UITool.TryGetChildComponentByName<Button>("确定").onClick.AddListener(() =>
                {
                    port.ProduceBoat();
                });
                text_needWorkforce = UITool.TryGetChildComponentByName<Text>("所需生产力");
            }
            else
            {

            }
            text_currentProduceBoat = UITool.TryGetChildComponentByName<Text>("当前生产的船");
            UITool.TryGetChildComponentByName<Button>("Close").onClick.AddListener(() =>
            {
                UIManager.Instance.PM.RemovePanel(this);
            });
        }

        public override void OnUpdate()
        {
            if (text_currentProduceBoat != null)
            {
                text_currentProduceBoat.text = port.current_produce_boat_def.name;
            }
            if (text_needWorkforce != null)
            {
                text_needWorkforce.text = port.current_produce_boat_def.costForProduce.ToString();
            }
            if(button_accpet != null)
            {
                button_accpet.interactable = port.unlock;
            }
        }
    }
}