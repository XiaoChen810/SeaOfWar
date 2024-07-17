using UnityEngine;
using System.Collections.Generic;
using Fusion;
using DG.Tweening.Core.Easing;

namespace ChenChen_Core
{
    public abstract class Building : NetworkBehaviour, IBuilding
    {
        public static int costProductivity = 5;

        protected GameManager gameManager;
        protected NetworkTransform nt; 

        #region - 网络属性 -

        [Networked, OnChangedRender(nameof(OnHealthChanged))]
        public int Health { get; set; }

        #endregion

        [Header("建筑共有属性")]
        [SerializeField] protected BuildingDef _def;
        [SerializeField] protected int maxHealth = 10;

        #region - RPC -

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RPC_SetSelfInfo(int health)
        {
            Health = health;
        }

        #endregion

        #region - Changed -

        private void OnHealthChanged()
        {
            int Fixed = Mathf.Clamp(Health, 0, maxHealth);
            if (Health != Fixed) { Health = Fixed; }
        }

        #endregion

        public override void Spawned()
        {
            gameManager = GameManager.Instance;
            nt = GetComponent<NetworkTransform>();

            if (gameManager.PlayerList_Buildings.ContainsKey(Object.InputAuthority))
            {
                gameManager.PlayerList_Buildings[Object.InputAuthority].Add(this);
            }
            else
            {
                gameManager.PlayerList_Buildings.Add(Object.InputAuthority, new List<Building> { this });
            }

            // 当被生成时，如果是自己所属的阵营，则执行相应的操作
            if (Object.HasInputAuthority)
            {
                RPC_SetSelfInfo(maxHealth);
                GameManager.Instance.OnNewRound += OnNewRound;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);

            if (Object.HasInputAuthority)
            {
                GameManager.Instance.OnNewRound -= OnNewRound;
            }
        }

        // 当进入新的一轮后
        protected abstract void OnNewRound();
    }
}