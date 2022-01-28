using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int LocalPlayerRoomNumber;
    public string LocalPlayerName;
    // public int LocalPlayerKill;
    // public int LocalPlayerDeath;
    // public float LocalPlayerTotalDamage;

    public static PlayerManager instance; // Scenes Manager를 싱글톤으로 관리
    private void Awake()
    {
        // Scenes Manager 싱글톤 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        } 
        else
        { 
            Destroy(gameObject);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
