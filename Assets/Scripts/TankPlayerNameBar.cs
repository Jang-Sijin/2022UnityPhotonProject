using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class TankPlayerNameBar : MonoBehaviour
{
    public PhotonView PV;
    public TextMeshPro playerName;
    
    [SerializeField]
    private Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        if (Camera.main is { }) cam = Camera.main.transform;

        if (PV.IsMine)
        {
            // 플레이어 자신은 이름을 Green Bar로 출력한다.
            playerName.color = Color.green;
        }

        // PV.RPC("SetPlayerName", RpcTarget.All);
        playerName.text = PV.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        // PV.RPC("SetCameraRotation", RpcTarget.All);
        var rotation = cam.rotation;
        playerName.transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

    [PunRPC]
    void SetPlayerName()
    {
        playerName.text = PV.Owner.NickName;
    }

    [PunRPC]
    void SetCameraRotation()
    {
        var rotation = cam.rotation;
        playerName.transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);   
    }
}
