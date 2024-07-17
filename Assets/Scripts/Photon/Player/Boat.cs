using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using Fusion;
using ChenChen_Core;
using StylizedWater2;

public class Boat : NetworkBehaviour 
{
    // -------- Argument ----------
    [SerializeField] public BoatDef _def;   
    
    #region - Network -

    [Networked]
    public int UID { get; set; }

    [Networked]
    public bool CanMove { get; set; }

    [Networked]
    public bool CanAttack { get; set; }

    [Networked, OnChangedRender(nameof(OnDestinationChanged))]
    public Vector3 Destination { get; set; }

    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public int Health { get; set; }

    #endregion

    #region -- Changed --

    private void OnHealthChanged()
    {
        int Fixed = Mathf.Clamp(Health, 0, _def.maxHealth);
        if( Health != Fixed ) 
        { 
            Health = Fixed;
        }
    }

    private void OnDestinationChanged()
    {
        if (Vector3.Distance(transform.position, Destination) > 0.1f)
        {
            reachDestination = false;
        }
    }

    #endregion

    #region -- RPC --

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_SetSelfInfo(int uid)
    {
        UID = uid;
        CanMove = true;
        CanAttack = true;
        Health = _def.maxHealth;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_SetDestination(Vector3 destination)
    {
        Debug.Log($"设置了 船：{UID} 的目标位置");

        Destination = destination;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_SetCanAttack(bool value)
    {
        CanAttack = value;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_SetHealth(int value)
    {
        Health = value;
    }

    // 客户端发送攻击的消息给主机
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendAttackMessage(Boat a, Boat b)
    {
        RPC_RelayAttackMessage(a, b);
    }

    // 主机发送攻击消息给全部客户端
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayAttackMessage(Boat a, Boat b)
    {
        Debug.Log($"{a.Object.InputAuthority} 对 {b.Object.InputAuthority} 发起了攻击");

        // 如果受到攻击的是我自己
        if (b == this && b.Object.HasInputAuthority)
        {
            int finalDamage = a._def.damage - b._def.defend;
            b.RPC_SetHealth(b.Health - finalDamage);
            b.CreateSplashEffect();
        }

        // 如果发起攻击的是我自己
        if (a == this && a.Object.HasInputAuthority)
        {
            a.RPC_SetCanAttack(false);
        }
    }

    #endregion

    [Header("非网络参数")]
    [SerializeField] private List<Vector3> path = new List<Vector3>();
    [SerializeField] private int indexPath = -1;
    [SerializeField] private bool reachDestination = true;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float viewRange = 12;

    [SerializeField] private float shakeStrength = 10;
    [SerializeField] private Transform body;

    [SerializeField] private GameObject bigSplashEffectPrefab;

    private Vector3 lastPosition;
    private float flashRange = 0.5f;

    private GameManager gameManager;
    private NetworkTransform nt;

    #region - 属性器 -

    public Vector3 Position => transform.position;
    public float HealthPercentage => Health / _def.maxHealth;
    public int MovementRange => gameManager.LocalPlayerCastle.Coal / _def.costCoalOneStep;
    public int AttackRange => _def.attackRange;
    public int Damage => _def.damage;
    public int Defend => _def.defend;
    public int CostOneStep => _def.costCoalOneStep;

    #endregion

    #region - 生命周期 -
    public override void Spawned()
    {
        gameManager = GameManager.Instance;
        nt = GetComponent<NetworkTransform>();

        if (gameManager.PlayerList_Boats.ContainsKey(Object.InputAuthority))
        {
            gameManager.PlayerList_Boats[Object.InputAuthority].Add(this);
        }
        else
        {
            gameManager.PlayerList_Boats.Add(Object.InputAuthority, new List<Boat> { this });
        }

        // 当被生成时，如果是自己所属的船只，则执行相应的操作
        if(Object.HasInputAuthority)
        {
            // 初始化数据
            RPC_SetSelfInfo(GetInstanceID());
            Animation(1, shakeStrength);
        }           
    }

    public override void FixedUpdateNetwork()
    {
        if (reachDestination) return;

        if (path.Count == 0)
        {
            path = SeekManager.Instance.GetPath(transform.position, Destination);

            if (path.Count > 0)
                indexPath = 0;
        }

        if (indexPath < 0 || indexPath >= path.Count) return;

        Vector3 point = path[indexPath];

        if (Vector3.Distance(transform.position, point) > 0.1f)
        {
            Vector3 direction = (point - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            var rot = Quaternion.Slerp(transform.rotation, lookRotation, Runner.DeltaTime * speed);
            var pos = Vector3.MoveTowards(transform.position, point, Runner.DeltaTime * speed);
            nt.Teleport(pos, rot);
        }
        else
        {
            indexPath++;
            if (indexPath >= path.Count)
            {
                indexPath = -1;
                path.Clear();
                reachDestination = true;              
            }
        }
    }

    private void Update()
    {
        if(!reachDestination && Vector3.Distance(lastPosition,transform.position) > flashRange)
        {
            lastPosition = transform.position;
            MapManager.Instance.HG.SeeFlog(transform.position, viewRange);
        }
    }

    #endregion

    #region - 事件 -

    private void OnEnable()
    {
        GameManager.Instance.OnNewRound += OnNewRound;
    }

    private void OnNewRound()
    {
        if (Object.HasInputAuthority)
        {
            RPC_SetCanAttack(true);
        }      
    }

    private void OnDisable()
    {
        GameManager.Instance.OnNewRound -= OnNewRound;
    }

    #endregion

    private void OnMouseDown()
    {
        // 通知已经玩家点击了这艘船
        PlayerInputManager.Instance.MosueSelect(this.gameObject);
    }

    public void Animation(float duration, float strength)
    {
        Vector3 origin = transform.position;
        Vector3 start = origin + Vector3.down * 1;
        Vector3 end = origin;

        body.position = start;
        body.DOMove(end, duration);
        body.DOShakeRotation(duration + 0.75f, strength);
    }

    private void CreateSplashEffect()
    {
        // 实例化水花粒子特效
        GameObject splashEffect = Instantiate(bigSplashEffectPrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = splashEffect.GetComponent<ParticleSystem>();

        if (ps != null)
        {
            // 设置粒子系统在播放完后自动销毁
            var main = ps.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
        }
    }

}

