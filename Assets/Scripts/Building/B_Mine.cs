using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    /// <summary>
    /// ��ȡú�� -- ��ߴ����ж���
    /// </summary>
    public class B_Mine : Building
    {
        [Header("��������")]
        public int provide_coal_one_round = 5;

        protected override void OnNewRound()
        {
            // ÿ���ṩһ����ú̿
            if (Object.HasInputAuthority)
            {
                gameManager.LocalPlayerCastle.RPC_AddCoal(provide_coal_one_round, Object.InputAuthority);
            }
        }
    }
}