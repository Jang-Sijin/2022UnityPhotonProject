﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    
    [Header("DisconnectPanel")]
    public GameObject DisconnectPanel;
    public InputField NickNameInput;
    public GameObject ErrorPanel;

    [Header("LobbyPanel")]
    public GameObject LobbyPanel;
    public InputField RoomInput;
    public Text WelcomeText;
    public Text LobbyInfoText;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public Text ListText;
    public Text RoomInfoText;
    public Text[] ChatText;
    public InputField ChatInput;
    public GameObject GameStartButton;
    public GameObject ReadyButton;
    public GameObject ReadyCancelButton;
    public GameObject StartErrorPanel;

    [Header("ETC")]
    public Text StatusText;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;
    
    
    public static NetworkManager instance; // Network Manager을 싱글톤으로 관리

    #region 서버연결
    // void Awake() => Screen.SetResolution(960, 540, false);
    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        // Sound Manager 싱글톤 설정
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(instance);
        } 
        else
        { 
            Destroy(gameObject);  
        }
    }

    private void Start()
    {
        if (PhotonNetwork.NetworkClientState.ToString() == "Joined")
        {
            // 로컬 플레이어가 LocalNumber을 할당받는다.
            SetPlayerLocalNumber();

            // 방(Room) 화면
            RoomPanel.SetActive(true);
            RoomRenewal();
            ChatInput.text = "";
            for (int i = 0; i < ChatText.Length; i++)
            {
                ChatText[i].text = "";
            }
        
            // 플레이어가 룸 마스터이면 게임 시작 버튼 패널을 활성화,
            // 마스터가 아닌 플레이어들은 레디 버튼 패널이 활성화 되도록 적용
            if(PlayerManager.instance.LocalPlayerRoomNumber == 0)
            {
                GameStartButton.SetActive(true);
            }
            else
            {
                ReadyButton.SetActive(true);
            }

            // 방(Room) BGM 재생
            SoundManager.instance.BackGroundSoundPlay(RoomPanel);
        }
    }

    void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
        LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";
    }

    public void Connect()
    {
        if (NickNameInput.text != "")
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            ErrorPanel.SetActive(true);
        }
    }

    public void ErrorPanelClose()
    {
        ErrorPanel.SetActive(false);
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        // 로비 화면
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);
        if (PlayerManager.instance.LocalPlayerName == "")
        {
            PhotonNetwork.LocalPlayer.NickName = PlayerManager.instance.LocalPlayerName = NickNameInput.text;
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = NickNameInput.text = PlayerManager.instance.LocalPlayerName;   
        }
        WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다";
        myList.Clear();
        
        // 로비 BGM 재생
        SoundManager.instance.BackGroundSoundPlay(LobbyPanel);
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        // 로그인 화면
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
        
        // 로그인 BGM 재생
        SoundManager.instance.BackGroundSoundPlay(DisconnectPanel);
    }
    #endregion


    #region 방리스트 갱신
    // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }
    
    void MyListRenewal()
    {
        // 최대페이지
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;
    
        // 이전, 다음버튼
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;
    
        // 페이지에 맞는 리스트 대입
        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion
    
    
    #region 채팅
    public void Send()
    {
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
    }
    
    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string msg)
    {
        if (ChatText[0].text != "") // 첫번 째 칸에 저장된 메시지가 있을 때
        {
            for (int i = ChatText.Length - 1; i > 0; i--)   // 메시지의 내용을 윗칸으로 한칸씩 옮긴다.
            {
                ChatText[i].text = ChatText[i - 1].text;
            }
            ChatText[0].text = msg;
        }
        else if(ChatText[0].text == "")// // 첫번 째 칸에 저장된 메시지가 없을 때
        {
            ChatText[0].text = msg; // 그대로 메시지를 넣어준다.
        }
        else
        {
            Debug.Log($"[장시진] ChatText[0].text 예외 발생");
        }
    }
    #endregion
    

    #region 방
    public void CreateRoom() => PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text, new RoomOptions { MaxPlayers = 4 });
    
    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PlayerManager.instance.LocalPlayerRoomNumber = 0;
    }

    public override void OnJoinedRoom()
    {
        // 로컬 플레이어가 LocalNumber을 할당받는다.
        SetPlayerLocalNumber();

        // 방(Room) 화면
        RoomPanel.SetActive(true);
        RoomRenewal();
        ChatInput.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
        
        // 플레이어가 룸 마스터이면 게임 시작 버튼 패널을 활성화, 마스터가 아닌 플레이어들은 레디 버튼 패널이 활성화 되도록 적용
        if(PlayerManager.instance.LocalPlayerRoomNumber == 0)
        {
            GameStartButton.SetActive(true);
        }
        else
        {
            ReadyButton.SetActive(true);
        }

        // 방(Room) BGM 재생
        SoundManager.instance.BackGroundSoundPlay(RoomPanel);
    }

    public void ReadyButtonClick()
    {
        ReadyButton.SetActive(false);
        ReadyCancelButton.SetActive(true);
    }
    
    public void ReadyCancelButtonClick()
    {
        ReadyCancelButton.SetActive(false);
        ReadyButton.SetActive(true);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        GameStartButton.SetActive(true);
        ReadyButton.SetActive(false);
        ReadyCancelButton.SetActive(false);
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); } 
    
    public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 로컬 플레이어가 LocalNumber을 할당받는다.
        SetPlayerLocalNumber();
            
        RoomRenewal();
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");
    }
    
    void RoomRenewal()
    {
        ListText.text = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대";
    }

    void SetPlayerLocalNumber()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PlayerManager.instance.LocalPlayerName == PhotonNetwork.PlayerList[i].NickName)
            {
                PlayerManager.instance.LocalPlayerRoomNumber = i;
            }
        }
    }
    
    public void LoadInGameScene()
    {
        if (PhotonNetwork.PlayerList.Length <= 1)
        {
            StartErrorPanel.SetActive(true);
        }
        else
        {
            ScenesManager.instance.LoadInGameScene();
        }
    }
    
    public void StartErrorPanelClose()
    {
        StartErrorPanel.SetActive(false);
    }
    
    #endregion
}