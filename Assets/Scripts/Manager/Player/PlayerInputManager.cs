using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using ChenChen_UI;
using Fusion;

namespace ChenChen_Core
{
    public class PlayerInputManager : SingletonMono<PlayerInputManager>
    {
        [Header("组件")]
        public CameraController Camera;
        private LineRenderer lineRenderer;
        private GameManager gameManager;       
        [SerializeField] PlayerSpawner playerSpawner;

        [Header("UI")]
        [SerializeField] private Button endMyRound;
        [SerializeField] private Text roundText;
        [SerializeField] private Text productivityText;
        [SerializeField] private Text coalText;
        [SerializeField] private Text techText;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;

            gameManager = GameManager.Instance;

            endMyRound.onClick.AddListener(OnClickEndMyRound);
        }

        private void Update()
        {
            if(gameManager.LocalPlayerCastle != null)
            {
                roundText.text = gameManager.LocalPlayerCastle.IsMyRound ? "我的回合" : "其他人的回合";
                productivityText.text = gameManager.LocalPlayerCastle.Productivity.ToString();
                coalText.text = gameManager.LocalPlayerCastle.Coal.ToString();
                techText.text = gameManager.LocalPlayerCastle.Technology.ToString();
            }  
        }

        private void OnClickEndMyRound()
        {
            if (gameManager.LocalPlayerCastle.IsMyRound && gameManager.LocalPlayerCastle.HasInputAuthority)
            {
                gameManager.LocalPlayerCastle.RPC_SetIsEndRound(gameManager.LocalPlayerCastle.Object.InputAuthority);
            }         
        }

        public void MosueSelect(GameObject obj)
        {
            // 将镜头聚焦
            Camera.FocusOnTarget(obj.gameObject.transform);

            // 攻击船只
            if (obj.TryGetComponent<Boat>(out var boat))
            {
                SelectedBoat(boat);
            }
            // 攻击建筑
            if (obj.TryGetComponent<Building>(out var building))
            {

            }
            // 攻击主城
            if (obj.TryGetComponent<PlayerCastle>(out var castle))
            {
                SelectedCastle(castle);
            }
        }

        #region - 控制船只 - 

        private Boat _selectedBoat = null;
        private bool _inAttackAction = false;

        private void SelectedBoat(Boat boat)
        {
            if (_inAttackAction) return;

            _selectedBoat = boat;

            // 切换到该船只的UI面板
            if (UIManager.Instance.PM.TryGetTopPanel(out PanelBase top) && top.GetType() == typeof(P_BoatPanel))
            {
                UIManager.Instance.PM.RemovePanel(top);
            }

            // 只有选择的船是自己的，并且现在是自己的回合，才能操作
            bool openOperate = boat.Object.InputAuthority == gameManager.playerRef_local && gameManager.LocalPlayerCastle.IsMyRound;
            UIManager.Instance.PM.AddPanel(new P_BoatPanel(_selectedBoat, this, openOperate));
        }

        public void SelectedNextBoat()
        {

        }

        public void SelectedLastBoat()
        {

        }

        public void SelectToMove()
        {
            if (_selectedBoat != null && _selectedBoat.CanMove)
            {
                StopAllCoroutines();
                List<Hexagon> moveRange = MapManager.Instance.FindRangeSeaHexagon(_selectedBoat.Position, _selectedBoat.MovementRange);
                StartCoroutine(MoveAction(moveRange));
            }
            else
            {
                Debug.LogWarning("选择的船不可以移动");
            }
        }

        public void SelectToAttack()
        {
            if (_selectedBoat != null && _selectedBoat.CanAttack)
            {
                StopAllCoroutines();
                List<Hexagon> attackRange = MapManager.Instance.FindRangeHexagon(_selectedBoat.Position, _selectedBoat.AttackRange);
                StartCoroutine(AttackAction(attackRange));
            }
            else
            {
                Debug.LogWarning("选择的船不可以攻击");
            }
        }

        public void SelectToSkip()
        {
            _selectedBoat.CanAttack = false;
            _selectedBoat.CanMove = false;
        }

        IEnumerator MoveAction(List<Hexagon> range)
        {
            ShowRange(range, true);

            while (true)
            {
                yield return null;

                // 取消移动
                if (Input.GetMouseButton(1))
                {
                    break;
                }

                Hexagon hex = MapManager.Instance.HG.FindHexagon_MousePoint();
                if (hex != null && range.Contains(hex))
                {
                    DrawPath(SeekManager.Instance.GetPath(_selectedBoat.Position, hex.center, useSmoothPath: false));
                    // 正式移动
                    if (Input.GetMouseButtonDown(0))
                    {
                        Move(hex.center);
                        break;
                    }
                }
                else
                {
                    lineRenderer.positionCount = 0;
                }
            }

            lineRenderer.positionCount = 0;

            HideRange(range);

            // 绘制路径
            void DrawPath(List<Vector3> path)
            {
                if (path == null || path.Count == 0)
                {
                    lineRenderer.positionCount = 0;
                    return;
                }

                lineRenderer.positionCount = path.Count;
                Vector3[] positions = path.ToArray();
                for (int i = 0; i < positions.Length; i++)
                {
                    positions[i].y = 0.2f;
                }
                lineRenderer.SetPositions(positions);
            }
        }

        [Header("弹道")]
        public int segmentCount = 50;   // 切分数
        public float height = 5f;   // 弹道高度
        IEnumerator AttackAction(List<Hexagon> range)
        {
            Vector3 start = _selectedBoat.Position;
            _inAttackAction = true;

            ShowRange(range, false);

            while (true)
            {
                yield return null;

                // 取消攻击
                if (Input.GetMouseButton(1))
                {
                    break;
                }

                // 通过射线检测判断有没有指向能攻击的目标
                Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(ray,out hit))
                {
                    if (hit.collider.CompareTag("Building") || hit.collider.CompareTag("Boat"))
                    {
                        if (hit.collider.TryGetComponent<NetworkObject>(out NetworkObject hitObj) && !hitObj.HasInputAuthority)
                        {
                            Hexagon hex = MapManager.Instance.HG.FindHexagon_MousePoint();
                            if (hex != null && range.Contains(hex))
                            {
                                DrawParabola(start, hex.center);
                                // 正式攻击
                                if (Input.GetMouseButton(0))
                                {
                                    Attack(hitObj);
                                    break;
                                }
                                continue;
                            }
                        }
                    }
                }
                lineRenderer.positionCount = 0;
            }

            lineRenderer.positionCount = 0;
            _inAttackAction = false;

            HideRange(range);

            // 绘制弹道
            void DrawParabola(Vector3 vA, Vector3 vB)
            {
                lineRenderer.positionCount = segmentCount + 1;
                Vector3[] positions = new Vector3[segmentCount + 1];

                for (int i = 0; i <= segmentCount; i++)
                {
                    float t = i / (float)segmentCount;
                    positions[i] = CalculateBezierPoint(t, vA, (vA + vB) / 2 + Vector3.up * height, vB);
                }

                lineRenderer.SetPositions(positions);
            }
            // 贝塞尔曲线
            Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
            {
                float u = 1 - t;
                float tt = t * t;
                float uu = u * u;
                Vector3 p = uu * p0; // (1-t)^2 * P0
                p += 2 * u * t * p1; // 2 * (1-t) * t * P1
                p += tt * p2; // t^2 * P2
                return p;
            }
        }

        private void ShowRange(List<Hexagon> range, bool isShowMoveRange)
        {
            foreach(Hexagon it in range)
            {
                if(it.obj.TryGetComponent<HexagonWater>(out var w))
                {
                    w.Open(isShowMoveRange);
                }
            }
        }

        private void HideRange(List<Hexagon> range)
        {
            foreach (Hexagon it in range)
            {
                if (it.obj.TryGetComponent<HexagonWater>(out var w))
                {
                    w.Close();
                }
            }
        }

        // 正式移动
        private void Move(Vector3 position)
        {
            // 计算消耗的燃料
            int cost = SeekManager.Instance.GetPath(_selectedBoat.Position, position, useSmoothPath: false).Count - 1;
            cost *= _selectedBoat.CostOneStep;

            // 减少燃料库存
            var pc = gameManager.LocalPlayerCastle;
            if (pc.HasInputAuthority)
            {
                pc.RPC_AddCoal(-cost, pc.Object.InputAuthority);
            }

            // 设置目标点
            if (_selectedBoat.HasInputAuthority)
            {
                _selectedBoat.RPC_SetDestination(position);
            }

        }

        // 正式攻击
        private void Attack(NetworkObject target)
        {
            if (_selectedBoat.HasInputAuthority)
            {
                // 攻击船只
                if (target.TryGetBehaviour<Boat>(out var boat))
                {
                    _selectedBoat.RPC_SendAttackMessage(_selectedBoat, boat);
                }
                // 攻击建筑
                if (target.TryGetBehaviour<Building>(out var building))
                {

                }
                // 攻击主城
                if (target.TryGetBehaviour<PlayerCastle>(out var castle))
                {

                }
            }

        }

        #endregion

        #region - 控制主城 -

        private PlayerCastle _selectCastle;

        private void SelectedCastle(PlayerCastle playerCastle)
        {
            _selectCastle = playerCastle;

            // 切换到该主城的UI面板
            if (UIManager.Instance.PM.TryGetTopPanel(out PanelBase top) && top.GetType() == typeof(P_CastlePanel))
            {
                UIManager.Instance.PM.RemovePanel(top);
            }

            UIManager.Instance.PM.AddPanel(new P_CastlePanel(playerCastle, playerCastle.HasInputAuthority));
        }

        #endregion
    }
}