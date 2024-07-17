using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class PlayerSpawner : NetworkBehaviour, INetworkRunnerCallbacks
    {
        public static PlayerSpawner Instance;

        private  GameManager gameManager = null;
        private NetworkRunner runner = null;

        [SerializeField] private NetworkObject playerCastlePrefab = null;

        [Header("船菜单")]
        public BoatDefMenu boatDefMenu;

        [Header("建筑菜单")]
        public BuildingMenu buildingDefMenu;

        private void Start()
        {
            Instance = this;

            gameManager = GameManager.Instance;

            runner = gameManager.Runner;

            runner.AddCallbacks(this);

            SpawnAllPlayers();
        }

        /// <summary>
        /// 只有Host会处理所有进入房间的玩家，生成他们对应的派系主城，和初始船
        /// </summary>
        private async void SpawnAllPlayers()
        {
            // 只有服务器可以生成玩家或者船只
            if (!runner.CanSpawn) return;

            // 选择派系出生点
            int playersNumber = gameManager.PlayerList_NetworkData.Count;
            Hexagon[] birthPlace = MapManager.Instance.GetRandomLandHexagon(Vector3.zero, 25, playersNumber);
            int i = 0;

            // 生成主城
            foreach (var it in gameManager.PlayerList_NetworkData)
            {
                var playerRef = it.Key;
                var playerData = it.Value;
                var spwanPositon = birthPlace[i++].center;

                await runner.SpawnAsync(playerCastlePrefab, spwanPositon, Quaternion.identity, playerRef);
            }

            // 生成初始船
            foreach (var it in gameManager.PlayerList_NetworkData)
            {
                var playerRef = it.Key;
                var playerData = it.Value;

                const int BOATDEF_INDEX_TEST = 0;

                BoatDef boatDef = boatDefMenu.boatDefs[BOATDEF_INDEX_TEST];

                PlayerCastle castle = gameManager.PlayerList_Castle[playerRef];

                Vector3 spwanPositon = MapManager.Instance.FindNearSeaHexagon(castle.transform.position).center;

                await runner.SpawnAsync(boatDef.prefab, spwanPositon, Quaternion.identity, playerRef);
            }
        }

        #region - RPC -

        /// <summary>
        /// 根据定义生成一艘船
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="playerRef"></param>
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_SpawnBoat(string name, Vector3 position, Quaternion rotation, PlayerRef playerRef, RpcInfo rpcInfo = default)
        {
            Debug.LogWarning($"{rpcInfo.Source} 发送rpc RPC_SpawnBoat");

            if(!runner.CanSpawn)
            {
                Debug.LogError($"{runner.LocalPlayer} 不可以生成物件但尝试生成");
            }

            foreach(var def in boatDefMenu.boatDefs)
            {
                if (def.name == name)
                {
                    runner.SpawnAsync(def.prefab, position, rotation, playerRef);
                    return;
                }
            }
            Debug.LogError("未找到这个名字的船定义");
        }

        /// <summary>
        /// 根据定义生成一个建筑
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="playerRef"></param>
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_SpawnBuilding(string name, Vector3 position, Quaternion rotation, PlayerRef playerRef, RpcInfo rpcInfo = default)
        {
            Debug.LogWarning($"{rpcInfo.Source} 发送rpc RPC_SpawnBuilding");

            if (!runner.CanSpawn)
            {
                Debug.LogError($"{runner.LocalPlayer} 不可以生成物件但尝试生成");
            }

            foreach (var def in buildingDefMenu.buildingsDefs)
            {
                if (def.name == name)
                {
                    runner.SpawnAsync(def.prefab, position, rotation, playerRef);
                    return;
                }
            }
            Debug.LogError("未找到这个名字的建筑定义");
        }

        #endregion

        #region Player Join and Left

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.LogWarning($"玩家 {player.PlayerId} 进入游戏");
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
        {
            Debug.LogWarning($"玩家 {player.PlayerId} 离开游戏");
        }

        #endregion

        #region - Input -

        private InputData myInput = new InputData();

        public void SetInput(InputData set, PlayerRef player)
        {
            if (player != gameManager.playerRef_local)
            {
                Debug.LogWarning("没有输入权限");
                return;
            }
                
            myInput = set;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                myInput.Buttons.Set(MyButtons.Test, true);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            //低滴答率的轮询输入
            input.Set(myInput);

            myInput = default;
        }

        #endregion

        #region -- Not Used --
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

        #endregion
    }
}