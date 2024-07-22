using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using ChenChen_Core;
using ChenChen_Lobby;

public class GameManager : SingletonMono<GameManager>
{
    [Header("房间信息")]
    // 当玩家进入房间的时候就初始化了
    public string playerName_local;
    public string roomName_local;
    public PlayerRef playerRef_local => Runner.LocalPlayer;

    #region - Network -

    [SerializeField] private NetworkRunner runner = null;
    public NetworkRunner Runner
    {
        get
        {
            if (runner == null)
            {
                GameObject runnerObject = new GameObject("NetworkRunner");

                runner = runnerObject.AddComponent<NetworkRunner>();

                runnerObject.AddComponent<NetworkSceneManagerDefault>();

                runner.ProvideInput = true;               
            }
            return runner;
        }
    }

    /// <summary>
    /// 储存了房间内玩家的 PlayerNetworkData 
    /// <para> Key：玩家标识 PlayerRef </para>
    /// Value: 玩家在网络上传输的数据 （PlayerNetworkData）
    /// </summary>
    public Dictionary<PlayerRef, PlayerNetworkData> PlayerList_NetworkData = new Dictionary<PlayerRef, PlayerNetworkData>();
    public Dictionary<PlayerRef, PlayerCastle> PlayerList_Castle = new Dictionary<PlayerRef, PlayerCastle>();
    public Dictionary<PlayerRef, List<Boat>> PlayerList_Boats = new Dictionary<PlayerRef, List<Boat>>();
    public Dictionary<PlayerRef, List<Building>> PlayerList_Buildings = new Dictionary<PlayerRef, List<Building>>();

    private Queue<PlayerCastle> playerActionQueue = new Queue<PlayerCastle>();

    public PlayerCastle LocalPlayerCastle => PlayerList_Castle.ContainsKey(playerRef_local) ? PlayerList_Castle[playerRef_local] : null;

    public event Action OnPlayerListUpdated = null;
    public event Action<bool> OnCheckPlayerIsReady = null;
    public event Action OnNewRound = null;

    private bool gameIsStart = false;
    private bool gameIsReady = false;

    private bool CheckAllPlayersIsReady()
    {
        // 只有服务器做判断
        if (!Runner.IsServer) return false;

        foreach (var playerData in PlayerList_NetworkData.Values)
        {
            if (!playerData.IsReady) return false;
        }

        return true;
    }

    public void UpdatePlayerList()
    {
        OnPlayerListUpdated?.Invoke();

        // Host 要做判断
        if (runner.IsServer)
        {
            bool allReady = CheckAllPlayersIsReady();

            OnCheckPlayerIsReady?.Invoke(allReady);
        }        
    }

    public async void StartGame()
    {
        if (CheckAllPlayersIsReady())
        {
            Debug.Log("开始游戏, 加载场景--“GamePlay");

            foreach(var playerData in PlayerList_NetworkData.Values)
            {
                playerData.IsReady = false;
            }

            await Runner.LoadScene("GamePlay");

            gameIsStart = true;
        }
        else
        {
            Debug.Log("有玩家还未准备");
        }
    }

    private PlayerCastle flagRound; // 标记是否已经进行了完整的一轮
    
    public void NewRound()
    {
        Debug.LogWarning($"被通知新的一圈轮开始了");     

        OnNewRound?.Invoke();
    }

    private void Update()
    {  
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnNewRound?.Invoke();
        }

        // 只有Host会判断玩家行动顺序
        if (runner.IsServer && gameIsStart)
        {
            if (!gameIsReady && CheckAllPlayersIsReady())
            {
                Debug.LogWarning("设置玩家行动顺序");

                playerActionQueue.Clear();

                // 先设定所有玩家的行动顺序
                foreach (var playerCastle in PlayerList_Castle.Values)
                {
                    playerCastle.IsEndRound = false;
                    playerCastle.IsMyRound = false;
                    playerActionQueue.Enqueue(playerCastle);
                }

                flagRound = playerActionQueue.Peek();
                playerActionQueue.Peek().IsMyRound = true;

                gameIsReady = true;
            }

            // 如果当前玩家结束回合，则切换到下一个玩家，并通知全部玩家
            if (gameIsReady && playerActionQueue.Peek().IsEndRound)
            {
                playerActionQueue.Enqueue(playerActionQueue.Dequeue());

                foreach(var playerCastle in PlayerList_Castle.Values)
                {
                    playerCastle.RPC_SetNextRound(playerActionQueue.Peek().Object.InputAuthority);

                    // 如果完成了一整轮，额外通知
                    if (playerActionQueue.Peek() == flagRound)
                    {
                        playerCastle.RPC_NewRound();
                    }
                }
            }
        }
    }

    #region - 检查延迟 -

    private GUIStyle _guiStyle;
    private double latency = 0;

    private void Start()
    {
        Screen.SetResolution(1024, 738, FullScreenMode.Windowed); 

        // 初始化 GUI 样式
        _guiStyle = new GUIStyle();
        _guiStyle.fontSize = 24; // 字体大小
        _guiStyle.normal.textColor = Color.green; // 文字颜色
        _guiStyle.fontStyle = FontStyle.Bold; // 字体样式

        StartCoroutine(CheckLatency());
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 40), "Ping: " + latency.ToString("0.00") + " ms", _guiStyle);
    }

    private IEnumerator CheckLatency()
    {
        while (true)
        {
            if (Runner != null && playerRef_local != PlayerRef.None)
            {
                // 获取当前的延迟
                latency = Runner.GetPlayerRtt(playerRef_local);
                latency *= 1000;
            }

            // 每秒检查一次
            yield return new WaitForSeconds(1f);
        }
    }
    #endregion

    #endregion
}
