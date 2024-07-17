using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_Lobby
{
    public class CreateOrJoinRoomPanel : MonoBehaviour
    {
        [SerializeField] private LobbyManager lobbyManager;

        [SerializeField] private InputField playerName_IF;
        [SerializeField] private InputField roomName_IF;
        private const int MIN_CHAR_FOR_NAME = 2;

        [SerializeField] private Button createRoom;
        [SerializeField] private Button joinRoom;

        private void Start()
        {
            createRoom.interactable = false;
            createRoom.onClick.AddListener(OnCreateRoom);

            joinRoom.interactable = false;
            joinRoom.onClick.AddListener(OnJoinRoom);

            playerName_IF.onValueChanged.AddListener(OnPlayerNameChanged);
            roomName_IF.onValueChanged.AddListener(OnRoomNameChanged);
        }

        private void OnPlayerNameChanged(string arg0)
        {
            var playerName = playerName_IF.text;
            var roomName = roomName_IF.text;
            createRoom.interactable = playerName.Length >= MIN_CHAR_FOR_NAME && roomName.Length >= MIN_CHAR_FOR_NAME;
            joinRoom.interactable = playerName.Length >= MIN_CHAR_FOR_NAME && roomName.Length >= MIN_CHAR_FOR_NAME;
        }

        private void OnRoomNameChanged(string arg0)
        {
            var playerName = playerName_IF.text;
            var roomName = roomName_IF.text;
            createRoom.interactable = playerName.Length >= MIN_CHAR_FOR_NAME && roomName.Length >= MIN_CHAR_FOR_NAME;
            joinRoom.interactable = playerName.Length >= MIN_CHAR_FOR_NAME && roomName.Length >= MIN_CHAR_FOR_NAME;
        }

        private async void OnCreateRoom()
        {
            var playerName = playerName_IF.text;
            var roomName = roomName_IF.text;

            if (playerName.Length >= MIN_CHAR_FOR_NAME && roomName.Length >= MIN_CHAR_FOR_NAME)
            {
                await lobbyManager.CreateRoom(playerName, roomName);
            }
        }

        private async void OnJoinRoom()
        {
            var playerName = playerName_IF.text;
            var roomName = roomName_IF.text;

            if (playerName.Length >= MIN_CHAR_FOR_NAME && roomName.Length >= MIN_CHAR_FOR_NAME)
            {
                await lobbyManager.JoinRoom(playerName, roomName);
            }
        }
    }
}