using ChenChen_Core;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class P_BoatPanel : PanelBase
    {
        private static readonly string path = "Prefabs/UI/Panel/BoatPanel";

        private Boat boat;
        private PlayerInputManager playerInputManager;
        private bool canOperate;

        public P_BoatPanel(Boat boat, PlayerInputManager playerInputManager, bool canOperate) : base(new UIType(path))
        {
            this.boat = boat;
            this.canOperate = canOperate;
            this.playerInputManager = playerInputManager;
        }

        private Slider healthbar;

        public override void OnEnter()
        {
            UITool.TryGetChildComponentByName<Text>("HealthAttribute").text = boat.Health.ToString();
            UITool.TryGetChildComponentByName<Text>("MoveAttribute").text = boat.MovementRange.ToString();                 
            UITool.TryGetChildComponentByName<Text>("AttackAttribute").text = boat.Damage.ToString();
            UITool.TryGetChildComponentByName<Text>("DefendAttribute").text = boat.Defend.ToString();

            healthbar = UITool.TryGetChildComponentByName<Slider>("Healthbar");
            healthbar.value = boat.HealthPercentage;

            if (!canOperate)
            {
                UITool.GetChildByName("Next").gameObject.SetActive(false);
                UITool.GetChildByName("Last").gameObject.SetActive(false);
                UITool.GetChildByName("Move").gameObject.SetActive(false);
                UITool.GetChildByName("Attack").gameObject.SetActive(false);
                UITool.GetChildByName("Skip").gameObject.SetActive(false);
            }
            // Button
            UITool.TryGetChildComponentByName<Button>("Next").onClick.AddListener(() =>
            {
                playerInputManager.SelectedNextBoat();
            });
            UITool.TryGetChildComponentByName<Button>("Last").onClick.AddListener(() =>
            {
                playerInputManager.SelectedLastBoat();
            });
            UITool.TryGetChildComponentByName<Button>("Move").onClick.AddListener(() =>
            {
                playerInputManager.SelectToMove();
            });
            UITool.TryGetChildComponentByName<Button>("Attack").onClick.AddListener(() =>
            {
                playerInputManager.SelectToAttack();
            });
            UITool.TryGetChildComponentByName<Button>("Skip").onClick.AddListener(() =>
            {
                playerInputManager.SelectToSkip();
            });
            UITool.TryGetChildComponentByName<Button>("Close").onClick.AddListener(() =>
            {
                UIManager.Instance.PM.RemovePanel(this);
            });
        }

        public override void OnUpdate()
        {
            healthbar.value = boat.HealthPercentage;
        }
    }
}