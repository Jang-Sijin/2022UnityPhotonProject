using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class InGameAlivePlayers : MonoBehaviour
{
    public int AlivePlayerCount;

    public static InGameAlivePlayers instance;
    private void Awake()
    {
        // Scenes Manager 싱글톤 설정
        if (instance == null)
        {
            instance = this;
        } 
        else
        { 
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        AlivePlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public void CheckAlivePlayers()
    {
        AlivePlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }
}
