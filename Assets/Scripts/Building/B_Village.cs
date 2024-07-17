using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    /// <summary>
    /// ��������� -- ���������Լӿ촬�Ľ��� 
    /// </summary>
    public class B_Village : Building
    {
        [Header("��������")]
        public int provide_workfores_one_round = 3;

        protected override void OnNewRound()
        {
            // ÿ���ṩ��Ӧ��������
            if (Object.HasInputAuthority)
            {
                RpcInfo rpc = new RpcInfo() { Source = Object.InputAuthority };
                gameManager.PlayerList_Castle[Object.InputAuthority].RPC_AddProductivity(provide_workfores_one_round, Object.InputAuthority);
            }            
        }
    }
}