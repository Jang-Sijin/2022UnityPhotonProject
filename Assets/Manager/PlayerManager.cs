using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public string LocalPlayerName;
    
    [SerializeField]
    private List<GameObject> m_PlayerTanks; // 모든 플레이어 저장

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

    public void PlayerAddList(GameObject player)
    {
        m_PlayerTanks.Add(player);
    }
    
    public void PlayerSetPosition(Transform[] spawnPoints)
    {
        for (int i = 0; i < m_PlayerTanks.Count; i++)
        {
            m_PlayerTanks[i].transform.position = spawnPoints[i].position;
            m_PlayerTanks[i].transform.rotation = spawnPoints[i].rotation;
        }
    }
}
