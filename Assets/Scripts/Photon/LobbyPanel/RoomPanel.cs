using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_Lobby
{
    public class RoomPanel : MonoBehaviour
    {
        [SerializeField] private LobbyManager lobbyManager;
        private GameManager gameManager;
        private NetworkRunner runner;

        [Header("Text")]
        [SerializeField] private Text playerName;
        [SerializeField] private Text roomName;

        [Header("Button")]
        [SerializeField] private Button ready_Btn;
        [SerializeField] private Button canelReady_Btn;
        [SerializeField] private Button play_Btn;

        [SerializeField] private RoomList roomList;

        private void OnEnable()
        {
            gameManager = GameManager.Instance;
            runner = gameManager.Runner;

            playerName.text = gameManager.playerName_local;
            roomName.text = gameManager.roomName_local;

            ready_Btn.onClick.AddListener(OnClickReadyButton);
            canelReady_Btn.onClick.AddListener(OnClickCanelReadyButton);
            play_Btn.onClick.AddListener(OnClickPlayButton);

            GameManager.Instance.OnCheckPlayerIsReady += Instance_OnCheckPlayerIsReady;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnCheckPlayerIsReady -= Instance_OnCheckPlayerIsReady;
        }

        private void Instance_OnCheckPlayerIsReady(bool obj)
        {
            play_Btn.gameObject.SetActive(obj);
        }

        private void OnClickReadyButton()
        {
            if (gameManager.PlayerList_NetworkData.TryGetValue(runner.LocalPlayer, out var playerNetworkData))
            {
                playerNetworkData.RPC_SetIsReady(true);

                canelReady_Btn.gameObject.SetActive(true);
                ready_Btn.gameObject.SetActive(false);
            }
        }

        private void OnClickCanelReadyButton()
        {
            if (gameManager.PlayerList_NetworkData.TryGetValue(runner.LocalPlayer, out var playerNetworkData))
            {
                playerNetworkData.RPC_SetIsReady(false);
                canelReady_Btn.gameObject.SetActive(false);
                ready_Btn.gameObject.SetActive(true);
            }
        }

        private void OnClickPlayButton()
        {
            Debug.Log("点击准备按钮");
            gameManager.StartGame();
        }
    }
}