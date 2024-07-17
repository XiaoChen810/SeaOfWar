using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using ChenChen_Core;
using static UnityEngine.Rendering.DebugUI;

public class PlayerCastle : NetworkBehaviour
{ 
    [Networked] public float Hp { get; set; }
    [Networked] public int Productivity { get; set; }
    [Networked] public int Coal { get; set; }
    [Networked, OnChangedRender(nameof(OnTechnologyChanged))] public int Technology { get; set; }
    [Networked] public bool IsMyRound { get; set; }
    [Networked] public bool IsEndRound { get; set; }

    [SerializeField] private float maxHp = 100;
    [SerializeField] private float viewRange = 50;
    [SerializeField] private int curentLevel = 0;
    [SerializeField] private int perLevelOfTechnical = 10;

    private GameManager gameManager;

    public int UnlockBoatLevel => curentLevel;
    public float HpValue => Hp / maxHp;

    public override void Spawned()
    {
        gameManager = GameManager.Instance;
        gameManager.PlayerList_Castle.Add(Object.InputAuthority, this);

        if (this.Object.HasInputAuthority)
        {
            RPC_SetSelfInfo(maxHp, 10, 10, 0);

            gameManager.PlayerList_NetworkData[gameManager.playerRef_local].RPC_SetIsReady(true);

            // �۽���������Լ�
            PlayerInputManager.Instance.Camera.FocusOnTarget(this.transform);

            // ��ȡ�Լ��ܱߵ�һЩ��Ұ
            MapManager.Instance.HG.SeeFlog(transform.position, viewRange);
        }
        else
        {
            // ��ȡ��������ܱߵ�һ����Ұ
            MapManager.Instance.HG.SeeFlog(transform.position, 8);
        }      
    }

    private void OnMouseDown()
    {
        PlayerInputManager.Instance.MosueSelect(this.gameObject);
    }

    #region - RPC -

    #region - �ͻ��˷���Ϣ������� -

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    private void RPC_SetSelfInfo(float hp, int productivity, int coal, int tech)
    {
        Hp = hp;
        Productivity = productivity;
        Coal = coal;
        Technology = tech;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_AddProductivity(int value, PlayerRef who)
    {   
        foreach (var pc in gameManager.PlayerList_Castle)
        {
            if (pc.Key == who)
            {
                pc.Value.Productivity += value;
                Debug.Log($"�������޸��� {who} �����ǵ���������{pc.Value.Productivity}");
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_AddCoal(int value, PlayerRef who)
    {     
        foreach (var pc in gameManager.PlayerList_Castle)
        {
            if (pc.Key == who)
            {
                pc.Value.Coal += value;
                Debug.Log($"�������޸��� {who} �����ǵ�ú̿������{pc.Value.Coal}");
            }
        }    
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_AddTechnology(int value, PlayerRef who)
    {      
        foreach (var pc in gameManager.PlayerList_Castle)
        {
            if (pc.Key == who)
            {
                pc.Value.Technology += value;
                Debug.Log($"�������޸��� {who} �����ǵĿƼ�ֵ��{pc.Value.Technology}");
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SetIsEndRound(PlayerRef who)
    {
        foreach (var pc in gameManager.PlayerList_Castle)
        {
            if (pc.Key == who)
            {
                pc.Value.IsMyRound = false;
                pc.Value.IsEndRound = true;
                Debug.Log($"{who} ���غϽ�������Ϣ�������������Ļغ��ѽ���");
            }
        }
    }

    #endregion

    #region - ����������Ϣ֪ͨ���Կͻ��� -

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_SetNextRound(PlayerRef nextPlayer)
    {
        if (nextPlayer == this.Object.InputAuthority)
        {
            IsMyRound = true;
            IsEndRound = false;
        }

        PlayerInputManager.Instance.Camera.FocusOnTarget(GameManager.Instance.PlayerList_Castle[nextPlayer].gameObject.transform);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_NewRound()
    {
        gameManager.NewRound();
    }

    #endregion

    #endregion

    #region - Changed - 

    private void OnTechnologyChanged()
    {
        curentLevel = Technology / perLevelOfTechnical;
        curentLevel = Mathf.Clamp(curentLevel, 1, 5);
    }

    #endregion
}
