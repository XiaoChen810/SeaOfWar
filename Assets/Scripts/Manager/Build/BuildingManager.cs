using ChenChen_UI;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class BuildingManager : SingletonMono<BuildingManager>
    {        
        private bool open;
        private PanelBase temp;
        
        /// <summary>
        /// �Ƿ����ڽ�����
        /// </summary>
        public bool OnBuilding => open;

        public BuildingMenu Menu;

        public event Action<BuildingDef> OnOpenBuildMode;
        public event Action OnCloseBuildMode;

        public PanelBase OpenPanel()
        {
            if (!open)
            {
                temp = new P_BuildingMenu();
                UIManager.Instance.PM.AddPanel(temp, false);
                open = true;
                return temp;
            }
            return null;
        }
        public void ClosePanel()
        {
            if (open)
            {
                UIManager.Instance.PM.RemovePanel(temp);
                open = false;
            }
                
        }

        public void OpenBuildMode(BuildingDef building)
        {
            OnOpenBuildMode?.Invoke(building);
            StartCoroutine(BuildModeCo(building));
        }

        public void CloseBuildMode()
        {
            OnCloseBuildMode?.Invoke();
        }

        IEnumerator BuildModeCo(BuildingDef buildingDef)
        {
            HexagonGrid hg = MapManager.Instance.HG;
            GameObject obj = Instantiate(buildingDef.prefab, transform);
            
            // ����Ǹۿڣ���Ҫ�����ж��Ƿ�����
            bool isPort = false;
            isPort = obj.TryGetComponent<B_Port>(out B_Port port);

            while (obj != null)
            {
                yield return null;
                Hexagon hex = hg.FindHexagon_MousePoint();
                if (hex != null)
                {
                    obj.transform.position = hex.center;
                    obj.transform.position += new Vector3(0, 0.05f, 0);
                }

                // ˳ʱ��
                if (Input.GetKeyDown(KeyCode.E))
                {
                    obj.transform.Rotate(new Vector3(0, 60, 0), Space.Self);
                }
                // ��ʱ��
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    obj.transform.Rotate(new Vector3(0, -60, 0), Space.Self);
                }
                // ����
                if (Input.GetMouseButtonDown(0))
                {
                    if(GameManager.Instance.LocalPlayerCastle.Productivity < Building.costProductivity) continue;

                    if(isPort)
                    {
                        if (hg.FindHexagon(port.produce_boat_birthplace.position).type != Hexagon.Type.water)
                        {
                            Debug.LogWarning("�ۿڳ���������ں���");
                            continue;
                        }
                    }
                    else
                    {
                        if (hex.type != Hexagon.Type.grass)
                        {
                            Debug.LogWarning("����ֻ����½����");
                            continue;
                        }
                    }

                    // ������б�
                    RpcInfo rpcInfo = new RpcInfo() { Source = GameManager.Instance.playerRef_local };
                    PlayerRef myRef = GameManager.Instance.playerRef_local;
                    PlayerSpawner.Instance.RPC_SpawnBuilding(buildingDef.name, obj.transform.position, obj.transform.rotation, myRef, rpcInfo);

                    var pc = GameManager.Instance.LocalPlayerCastle;
                    if (pc.HasInputAuthority)
                    {
                        pc.RPC_AddProductivity(-5, pc.Object.InputAuthority);
                    }

                    Destroy(obj);
                    break;
                }
                // ȡ��
                if (Input.GetMouseButtonDown(1))
                {
                    Destroy(obj);
                    break;
                }
            }

            if (obj == null)
            {
                Debug.LogError("��ʼ����������Ϊ��");
            }

            CloseBuildMode();
        }
    }
}