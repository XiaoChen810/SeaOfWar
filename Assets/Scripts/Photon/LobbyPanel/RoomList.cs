using ChenChen_Core;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Lobby
{
    public class RoomList : MonoBehaviour
    {
        [SerializeField] private List<PlayerCell> playerCells = new List<PlayerCell>();

        private void Start()
        {
            GameManager.Instance.OnPlayerListUpdated += Instance_OnPlayerListUpdated;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnPlayerListUpdated -= Instance_OnPlayerListUpdated;
        }

        private void Instance_OnPlayerListUpdated()
        {
            foreach (PlayerCell cell in playerCells)
            {
                cell.ResetInfo();
            }

            int i = 0;

            foreach (var player in GameManager.Instance.PlayerList_NetworkData)
            {
                if (i < 4)
                {
                    PlayerNetworkData playerData = player.Value;

                    playerCells[i].SetInfo(playerData.PlayerName, playerData.IsReady);

                    i++;
                }
                else
                {
                    Debug.LogError("房间超出人数");
                }
            }
        }
    }
}