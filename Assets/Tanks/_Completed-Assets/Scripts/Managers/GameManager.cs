using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Complete
{
    public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        public PhotonView PV;

        public int m_NumRoundsToWin = 5;            // The number of rounds a single player has to win to win the game.
        public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
        public float m_EndDelay = 5f;               // The delay between the end of RoundPlaying and RoundEnding phases.
        public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases.
        public Text m_ReadyText;
        public Text m_StartText;
        public Text m_EndText;
        public Text m_WaitText;
        public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
        // public GameObject m_TankPrefab;             // Reference to the prefab the players will control.
        public TankManager[] m_MyTank;               // A collection of managers for enabling and disabling different aspects of the tanks.

        public List<Transform> SpawnPoints;


        private int m_gameStart = 0;
        private int m_RoundNumber;                  // Which round the game is currently on.
        private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
        private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.
        private TankManager m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won.
        private TankManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.

        public int m_NetworkAlivePlayerCount; // 플레이어는 1 ~ 4명


        const float k_MaxDepenetrationVelocity = float.PositiveInfinity;


        private void Start()
        {
            m_NetworkAlivePlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;

            // 생존 플레이어 수를 초기화합니다.
            InGameAlivePlayers.instance.AlivePlayerCount = m_NetworkAlivePlayerCount;

            // Debug.Log($"{m_NetworkAlivePlayerCount}");
            // This line fixes a change to the physics engine.
            Physics.defaultMaxDepenetrationVelocity = k_MaxDepenetrationVelocity;

            // Create the delays so they only have to be made once.
            m_StartWait = new WaitForSeconds(m_StartDelay);
            m_EndWait = new WaitForSeconds(m_EndDelay);

            SpawnAllTanks(); // 플레이어 스폰 설정
            SetCameraTargets(); // 카메라 설정

            RoundStarting();
            StartCoroutine(GameLoop());
        }

        private void SpawnAllTanks()
        {
            // For all the tanks...
            for (int i = 0; i < 1; i++)
            {
                // ... create them, set their player number and references needed for control.

                // 플레이어 캐릭터 생성 (Photon)
                // m_MyTank[i].m_Instance = PhotonNetwork.Instantiate("CompleteTank", 
                //     SpawnPoints[0].position, SpawnPoints[0].rotation) as GameObject;
                // Instantiate(m_TankPrefab, m_MyTank[i].m_SpawnPoint.position, m_MyTank[i].m_SpawnPoint.rotation) as GameObject;

                m_MyTank[i].m_Instance = PhotonNetwork.Instantiate("CompleteTank",
                    SpawnPoints[PlayerManager.instance.LocalPlayerRoomNumber].position, SpawnPoints[PlayerManager.instance.LocalPlayerRoomNumber].rotation) as GameObject;

                m_MyTank[i].m_PlayerNumber = i + 1;
                m_MyTank[i].Setup();
            }
        }

        private void SetCameraTargets()
        {
            // Create a collection of transforms the same size as the number of tanks.
            Transform[] targets = new Transform[m_MyTank.Length];

            // For each of these transforms...
            for (int i = 0; i < targets.Length; i++)
            {
                // ... set it to the appropriate tank transform.
                targets[i] = m_MyTank[i].m_Instance.transform;
            }

            // These are the targets the camera should follow.
            m_CameraControl.m_Targets = targets;
        }


        // This is called from start and will run each phase of the game one after another.
        private IEnumerator GameLoop()
        {
            // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
            yield return StartCoroutine(RoundPlaying());

            // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
            if (InGameAlivePlayers.instance.AlivePlayerCount <= 1)
            {
                m_EndText.gameObject.SetActive(true);
                m_WaitText.gameObject.SetActive(true);
                yield return StartCoroutine(RoundEnding());
            }

            // if (GameObject.FindWithTag("Player") == null)
            // {
            //     PhotonNetwork.LeaveRoom();
            //     ScenesManager.instance.LoadScene("1.TitleScene");
            // }

            // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
            // if (m_GameWinner != null)
            // {
            //     // If there is a game winner, restart the level.
            //     ScenesManager.instance.LoadScene("1.TitleScene");
            // }
            // else
            // {
            if (m_GameWinner == null)
            {
                // If there isn't a winner yet, restart this coroutine so the loop continues.
                // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
                StartCoroutine(GameLoop());
            }
            // }
        }

        public void ExitButton()
        {
            PhotonNetwork.LeaveRoom();
            ScenesManager.instance.LoadScene("1.TitleScene");
        }

        private void RoundStarting() //snapshot
        {
            StartCoroutine(ShowReadyText());
        }

        private IEnumerator ShowReadyText()
        {
            m_WaitText.text = null;

            m_MyTank[0].m_Movement.m_MoveActive = false;
            m_MyTank[0].m_Shooting.m_ShootingActive = false;

            Debug.Log($"{m_gameStart}");
            m_ReadyText.gameObject.SetActive(true);
            m_WaitText.gameObject.SetActive(true);
            for (int i = (int)m_StartDelay; i > 0; i--)
            {
                m_WaitText.text = $"\n\n{i}";
                yield return new WaitForSeconds(1.0f);
            }
            m_ReadyText.gameObject.SetActive(false);
            m_WaitText.gameObject.SetActive(false);
            m_StartText.gameObject.SetActive(true);

            yield return new WaitForSeconds(2.0f);
            m_StartText.gameObject.SetActive(false);

            m_MyTank[0].m_Movement.m_MoveActive = true;
            m_MyTank[0].m_Shooting.m_ShootingActive = true;
            // Once the tanks have been created and the camera is using them as targets, start the game.
        }

        private IEnumerator RoundPlaying()
        {
            // As soon as the round begins playing let the players control the tanks.
            EnableTankControl();

            // Clear the text from the screen.
            m_MessageText.text = string.Empty;

            // While there is not one tank left...
            // 탱크가 1개도 남지 않은 상태에서...
            while (!OneTankLeft())
            {
                // ... return on the next frame.
                yield return null;
            }
        }


        private IEnumerator RoundEnding()
        {
            m_EndText.text = null;
            m_WaitText.text = null;

            // Stop tanks from moving.
            DisableTankControl();

            // 남아있는 플레이어를 승자로 설정
            m_GameWinner = GetGameWinner();

            if (m_GameWinner != null)
            {
                // m_GameWinner의 탱크가 있는 플레이어의 닉네임을 가져옵니다.
                string winnerName = m_GameWinner.m_Instance.GetComponent<PhotonView>().Owner.NickName;

                // 모든 클라이언트에 RPC로 승자의 닉네임을 전달합니다.
                PV.RPC("DisplayWinnerName", RpcTarget.All, winnerName);
            }

            for (int i = 5; i > 0; i--)
            {
                m_WaitText.text = $"\n\n{i}";
                yield return new WaitForSeconds(1.0f);
            }
            m_WaitText.gameObject.SetActive(false);
            m_EndText.gameObject.SetActive(false);

            ScenesManager.instance.LoadScene("1.TitleScene");

            // [Legacy]
            //PV.RPC("GetWinnerPlayerName", RpcTarget.All);
            //    for (int i = 5; i > 0; i--)
            //    {
            //        m_WaitText.text = $"\n\n{i}";
            //        yield return new WaitForSeconds(1.0f);
            //    }
            //    m_WaitText.gameObject.SetActive(false);
            //    m_EndText.gameObject.SetActive(false);
            //
            //    ScenesManager.instance.LoadScene("1.TitleScene");

            // // Clear the winner from the previous round.
            // m_RoundWinner = null;
            //
            // // See if there is a winner now the round is over.
            // m_RoundWinner = GetRoundWinner ();
            //
            // // If there is a winner, increment their score.
            // if (m_RoundWinner != null)
            //     m_RoundWinner.m_Wins++;

            // Now the winner's score has been incremented, see if someone has one the game.

            // Get a message based on the scores and whether or not there is a game winner and display it.
            // string message = EndMessage ();
            // m_MessageText.text = message;


            // Wait for the specified length of time until yielding control back to the game loop.
            // yield return m_EndWait;
        }

        [PunRPC]
        private void DisplayWinnerName(string winnerName)
        {
            m_EndText.text = $"{winnerName} Win!!!\nGame End\n";
        }

        [PunRPC]
        private void GetWinnerPlayerName()
        {
            if (m_MyTank[0].m_Instance.GetComponent<TankHealth>().m_StartingHealth > 0)
            {
                m_EndText.text = $"{PlayerManager.instance.LocalPlayerName} Win!!!\nGame End\n";
            }
        }

        // This is used to check if there is one or fewer tanks remaining and thus the round should end.
        private bool OneTankLeft()
        {
            // Start the count of tanks left at zero.
            int numTanksLeft = 0;

            // Go through all the tanks...
            for (int i = 0; i < m_MyTank.Length; i++)
            {
                // ... and if they are active, increment the counter.
                if (m_MyTank[i].m_Instance.activeSelf)
                    numTanksLeft++;
            }

            // If there are one or fewer tanks remaining return true, otherwise return false.
            return numTanksLeft <= 1;
        }


        // This function is to find out if there is a winner of the round.
        // This function is called with the assumption that 1 or fewer tanks are currently active.
        private TankManager GetRoundWinner()
        {
            // Go through all the tanks...
            for (int i = 0; i < m_MyTank.Length; i++)
            {

                // ... and if one of them is active, it is the winner so return it.
                if (m_MyTank[i].m_Instance.activeSelf)
                    return m_MyTank[i];
            }

            // If none of the tanks are active it is a draw so return null.
            return null;
        }

        private TankManager GetGameWinner()
        {
            // 플레이어 중에서 마지막까지 살아남은 탱크를 승자로 결정합니다.
            for (int i = 0; i < m_MyTank.Length; i++)
            {
                if (m_MyTank[i].m_Instance.activeSelf)
                    return m_MyTank[i];
            }

            // 승자가 없으면 null 반환
            return null;
        }
        
        private string EndMessage()
        {           
            string message = "DRAW!";
            
            if (m_RoundWinner != null)
                message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";
            
            message += "\n\n\n\n";
            
            for (int i = 0; i < m_MyTank.Length; i++)
            {
                message += m_MyTank[i].m_ColoredPlayerText + ": " + m_MyTank[i].m_Wins + " WINS\n";
            }
            
            if (m_GameWinner != null)
                message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

            return message;
        }
        
        private void ResetAllTanks()
        {
            for (int i = 0; i < m_MyTank.Length; i++)
            {
                if (PV.IsMine)
                {
                    m_MyTank[i].Reset(SpawnPoints[PlayerManager.instance.LocalPlayerRoomNumber]);
                }
            }
        }


        private void EnableTankControl()
        {
            for (int i = 0; i < m_MyTank.Length; i++)
            {
                m_MyTank[i].EnableControl();
            }
        }


        private void DisableTankControl()
        {
            for (int i = 0; i < m_MyTank.Length; i++)
            {
                m_MyTank[i].DisableControl();
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(m_NetworkAlivePlayerCount);
            }
            else
            {
                // Network player, receive data
                this.m_NetworkAlivePlayerCount = (int)stream.ReceiveNext();
            }
        }

        // 플레이어가 나갔을 때 호출되는 콜백
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);

            // 남아있는 생존자 수를 업데이트합니다.
            InGameAlivePlayers.instance.AlivePlayerCount--;

            // 생존자가 1명만 남았다면 게임을 종료하고 승리 처리를 합니다.
            if (InGameAlivePlayers.instance.AlivePlayerCount == 1)
            {
                StartCoroutine(HandleSinglePlayerLeft());
            }
        }

        private IEnumerator HandleSinglePlayerLeft()
        {
            // 남아있는 플레이어를 승자로 설정합니다.
            m_GameWinner = GetGameWinner();

            if (m_GameWinner != null)
            {
                string winnerName = m_GameWinner.m_Instance.GetComponent<PhotonView>().Owner.NickName;
                PV.RPC("DisplayWinnerName", RpcTarget.All, winnerName);
            }

            // 승리 후 대기 시간
            for (int i = 5; i > 0; i--)
            {
                m_WaitText.text = $"\n\n{i}";
                yield return new WaitForSeconds(1.0f);
            }

            // 방으로 돌아가기
            if (PhotonNetwork.IsMasterClient)
            {
                // 마스터 클라이언트가 방을 종료하고 로비로 이동합니다.
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene("1.TitleScene"); // 로비 씬으로 전환
            }
            else
            {
                // 다른 클라이언트들도 방을 나갑니다.
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene("1.TitleScene"); // 로비 씬으로 전환
            }
        }
    }
}