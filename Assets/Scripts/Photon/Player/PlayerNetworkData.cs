using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace ChenChen_Core
{
    /// <summary>
    /// 玩家在网络上传输的数据
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

            transform.SetParent(gameManager.transform); // 设置子物体，避免消失

            gameManager.PlayerList_NetworkData.Add(Object.InputAuthority, this);
            gameManager.UpdatePlayerList();

            // 只有有输入权限才能发送RPC
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