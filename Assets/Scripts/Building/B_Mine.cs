using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    /// <summary>
    /// 挖取煤矿 -- 提高船的行动力
    /// </summary>
    public class B_Mine : Building
    {
        [Header("独有属性")]
        public int provide_coal_one_round = 5;

        protected override void OnNewRound()
        {
            // 每轮提供一定的煤炭
            if (Object.HasInputAuthority)
            {
                gameManager.LocalPlayerCastle.RPC_AddCoal(provide_coal_one_round, Object.InputAuthority);
            }
        }
    }
}