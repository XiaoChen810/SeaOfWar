using ChenChen_UI;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class B_Port : Building
    {
        [Header("独有属性")]
        public BoatDef current_produce_boat_def;
        public Transform produce_boat_birthplace;

        public bool unlock => current_produce_boat_def.needTechnologyLevel < GameManager.Instance.LocalPlayerCastle.UnlockBoatLevel;

        public void ProduceBoat()
        {
            // 自己的朝向，然后偏转90度
            RpcInfo rpcInfo = new RpcInfo() { Source = this.Object.InputAuthority };

            Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 60, 0));
            PlayerSpawner.Instance.RPC_SpawnBoat(current_produce_boat_def.name, produce_boat_birthplace.position, rotation, Object.InputAuthority, rpcInfo);
        }

        protected void OnMouseDown()
        {

            if (!BuildingManager.Instance.OnBuilding)
            {
                UIManager.Instance.PM.AddPanel(new P_PortPanel(this));
            }
            
        }

        protected override void OnNewRound()
        {
            // 什么都不做
        }
    }
}