using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class B_Institute : Building
    {
        [Header("独有属性")]
        public int provide_knowledge_one_round;

        protected override void OnNewRound()
        {
            // 每轮提供相应的科技点
            if (Object.HasInputAuthority)
            {
                gameManager.PlayerList_Castle[Object.InputAuthority].RPC_AddTechnology(provide_knowledge_one_round, Object.InputAuthority);
            }
        }
    }
}