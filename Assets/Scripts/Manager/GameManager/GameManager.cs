using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using ChenChen_Core;
using ChenChen_Lobby;

public class GameManager : SingletonMono<GameManager>
{
    [Header("������Ϣ")]
    // ����ҽ��뷿���ʱ��ͳ�ʼ����
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
    /// �����˷�������ҵ� PlayerNetworkData 
    /// <para> Key����ұ�ʶ PlayerRef </para>
    /// Value: ����������ϴ�������� ��PlayerNetworkData��
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
        // ֻ�з��������ж�
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

        // Host Ҫ���ж�
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
            Debug.Log("��ʼ��Ϸ, ���س���--��GamePlay");

            foreach(var playerData in PlayerList_NetworkData.Values)
            {
                playerData.IsReady = false;
            }

            await Runner.LoadScene("GamePlay");

            gameIsStart = true;
        }
        else
        {
            Debug.Log("����һ�δ׼��");
        }
    }

    private PlayerCastle flagRound; // ����Ƿ��Ѿ�������������һ��
    
    public void NewRound()
    {
        Debug.LogWarning($"��֪ͨ�µ�һȦ�ֿ�ʼ��");     

        OnNewRound?.Invoke();
    }

    private void Update()
    {  
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnNewRound?.Invoke();
        }

        // ֻ��Host���ж�����ж�˳��
        if (runner.IsServer && gameIsStart)
        {
            if (!gameIsReady && CheckAllPlayersIsReady())
            {
                Debug.LogWarning("��������ж�˳��");

                playerActionQueue.Clear();

                // ���趨������ҵ��ж�˳��
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

            // �����ǰ��ҽ����غϣ����л�����һ����ң���֪ͨȫ�����
            if (gameIsReady && playerActionQueue.Peek().IsEndRound)
            {
                playerActionQueue.Enqueue(playerActionQueue.Dequeue());

                foreach(var playerCastle in PlayerList_Castle.Values)
                {
                    playerCastle.RPC_SetNextRound(playerActionQueue.Peek().Object.InputAuthority);

                    // ��������һ���֣�����֪ͨ
                    if (playerActionQueue.Peek() == flagRound)
                    {
                        playerCastle.RPC_NewRound();
                    }
                }
            }
        }
    }

    #region - ����ӳ� -

    private GUIStyle _guiStyle;
    private double latency = 0;

    private void Start()
    {
        Screen.SetResolution(1024, 738, FullScreenMode.Windowed); 

        // ��ʼ�� GUI ��ʽ
        _guiStyle = new GUIStyle();
        _guiStyle.fontSize = 24; // �����С
        _guiStyle.normal.textColor = Color.green; // ������ɫ
        _guiStyle.fontStyle = FontStyle.Bold; // ������ʽ

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
                // ��ȡ��ǰ���ӳ�
                latency = Runner.GetPlayerRtt(playerRef_local);
                latency *= 1000;
            }

            // ÿ����һ��
            yield return new WaitForSeconds(1f);
        }
    }
    #endregion

    #endregion
}
