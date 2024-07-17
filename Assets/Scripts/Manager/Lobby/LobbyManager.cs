using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using ChenChen_Core;

namespace ChenChen_Lobby
{
    public enum PairState
    {
        Lobby = 0,
        InRoom = 1,
    }

    public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        private GameManager gameManager = null;

        private PairState pairState = PairState.Lobby;

        [SerializeField] private PlayerNetworkData playerNetworkDataPrefab = null;

        [Header("UI")]
        [SerializeField] private CreateOrJoinRoomPanel createOrJoinRoomPanel = null;
        [SerializeField] private RoomPanel roomPanel = null;
        [SerializeField] private LoadingPanel loadingPanel = null;

        public const int MAX_PLAYER_ONE_ROOM = 4;

        private async void Start()
        {
            SetPairState(PairState.Lobby);

            gameManager = GameManager.Instance;

            gameManager.Runner.AddCallbacks(this);

            //await JoinLobby(gameManager.Runner);
        }

        #region  Lobby

        public void SetPairState(PairState state)
        {
            pairState = state;

            switch (pairState)
            {
                case PairState.Lobby:
                    createOrJoinRoomPanel.gameObject.SetActive(true);
                    roomPanel.gameObject.SetActive(false);
                    break;
                case PairState.InRoom:
                    createOrJoinRoomPanel.gameObject.SetActive(false);
                    roomPanel.gameObject.SetActive(true);
                    break;
            }
        }

        public async Task JoinLobby(NetworkRunner runner)
        {
            loadingPanel.Loading();
            var result = await runner.JoinSessionLobby(SessionLobby.ClientServer);
            loadingPanel.EndLoading();

            if (!result.Ok)
            {
                Debug.LogError($"加入大厅失败: {result.ShutdownReason}");
            }
        }
       
        public async Task CreateRoom(string playerName, string roomName)
        {
            var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            var sceneInfo = new NetworkSceneInfo();
            if (scene.IsValid)
            {
                sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
            }

            StartGameArgs args = new StartGameArgs()
            {              
                GameMode = GameMode.Host,
                SessionName = roomName,
                PlayerCount = MAX_PLAYER_ONE_ROOM,
                Scene = scene,
                SceneManager = gameManager.Runner.GetComponent<INetworkSceneManager>()
            };

            loadingPanel.Loading();
            var result = await gameManager.Runner.StartGame(args);
            loadingPanel.EndLoading();

            if (result.Ok)
            {
                gameManager.roomName_local = roomName;
                gameManager.playerName_local = playerName;
                SetPairState(PairState.InRoom);
            }
            else
            {
                Debug.LogError($"创建房间失败: {result.ShutdownReason}");
            }
        }

        public async Task JoinRoom(string playerName, string roomName)
        {

            var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            var sceneInfo = new NetworkSceneInfo();
            if (scene.IsValid)
            {
                sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
            }

            StartGameArgs args = new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = roomName,
                PlayerCount = MAX_PLAYER_ONE_ROOM,
                Scene = scene,
                SceneManager = gameManager.Runner.GetComponent<INetworkSceneManager>()
            };


            loadingPanel.Loading();
            var result = await gameManager.Runner.StartGame(args);
            loadingPanel.EndLoading();

            if (result.Ok)
            {
                gameManager.roomName_local = roomName;
                gameManager.playerName_local = playerName;
                SetPairState(PairState.InRoom);
            }
            else
            {
                Debug.LogError($"加入房间失败: {result.ShutdownReason}");
            }

        }

        #endregion

        #region - Used Callbacks -

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Debug.Log("OnSessionListUpdated");
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                var data = runner.Spawn<PlayerNetworkData>(playerNetworkDataPrefab, Vector3.zero, Quaternion.identity, player);             
            }       
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (gameManager.PlayerList_NetworkData.TryGetValue(player, out PlayerNetworkData networkPlayerData))
            {
                runner.Despawn(networkPlayerData.Object);

                gameManager.PlayerList_NetworkData.Remove(player);
                gameManager.UpdatePlayerList();
            }
        }

        #endregion

        #region -- Not Used --
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { } 
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }   
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }   
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        #endregion
    }
}