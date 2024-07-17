using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class B_Institute : Building
    {
        [Header("��������")]
        public int provide_knowledge_one_round;

        protected override void OnNewRound()
        {
            // ÿ���ṩ��Ӧ�ĿƼ���
            if (Object.HasInputAuthority)
            {
                gameManager.PlayerList_Castle[Object.InputAuthority].RPC_AddTechnology(provide_knowledge_one_round, Object.InputAuthority);
            }
        }
    }
}