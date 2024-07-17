using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    /// <summary>
    /// 提高生产力 -- 生产力可以加快船的建造 
    /// </summary>
    public class B_Village : Building
    {
        [Header("独有属性")]
        public int provide_workfores_one_round = 3;

        protected override void OnNewRound()
        {
            // 每轮提供相应的生产力
            if (Object.HasInputAuthority)
            {
                RpcInfo rpc = new RpcInfo() { Source = Object.InputAuthority };
                gameManager.PlayerList_Castle[Object.InputAuthority].RPC_AddProductivity(provide_workfores_one_round, Object.InputAuthority);
            }            
        }
    }
}