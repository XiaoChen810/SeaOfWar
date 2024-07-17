using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace ChenChen_Core
{
    /// <summary>
    /// ����������ϴ��������
    /// </summary>
    public class PlayerNetworkData : NetworkBehaviour
    {
        private GameManager gameManager;

        [Networked, OnChangedRender(nameof(OnPlayerNameChanged))]
        public string PlayerName { get; set; }

        [Networked, OnChangedRender(nameof(OnIsReadyChanged))]
        public NetworkBool IsReady { get; set; }

        public override void Spawned()
        {
            gameManager = GameManager.Instance;

            transform.SetParent(gameManager.transform); // ���������壬������ʧ

            gameManager.PlayerList_NetworkData.Add(Object.InputAuthority, this);
            gameManager.UpdatePlayerList();

            // ֻ��������Ȩ�޲��ܷ���RPC
            if (Object.HasInputAuthority)
            {
                RPC_SetPlayerName(gameManager.playerName_local);
            }
        }

        #region - RPC -

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void RPC_SetPlayerName(string name)
        {
            PlayerName = name;
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void RPC_SetIsReady(bool isReady)
        {
            IsReady = isReady;
        }

        #endregion

        #region - OnChanged Events -

        private void OnPlayerNameChanged()
        {
            GameManager.Instance.UpdatePlayerList();
        }
        private void OnIsReadyChanged()
        {
            GameManager.Instance.UpdatePlayerList();
        }

        #endregion
    }
}