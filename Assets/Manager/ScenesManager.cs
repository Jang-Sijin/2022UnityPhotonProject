using System.Collections;
using System.Collections.Generic;using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviourPunCallbacks // MonoBehaviour
{
    // 불러올 로딩 씬
    public string loadingSceneName;

    public string loadSceneName;
    public string unLoadSceneName;
    // 불러올 다음 씬
    public string nextSceneName { get; set; }

    private Scene[] SceneSetup;

    private bool checkSceneLoding = false;
    
    public static ScenesManager instance; // Scenes Manager를 싱글톤으로 관리
    private void Awake()
    {
        // Scenes Manager 싱글톤 설정
        if (instance == null)
        {
            // SceneSetup = new Scene[SceneManager.sceneCount];
                
            instance = this;
            DontDestroyOnLoad(instance);
        } 
        else
        { 
            Destroy(gameObject);  
        }
    }

    private void Start()
    {
        // for (int scene = 0; scene < SceneManager.sceneCount; scene++)
        // {
        //     SceneSetup[scene] = SceneManager.GetSceneAt(scene);
        // }
        
        // SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Single);
        // Debug.Log($"[장시진] 로딩씬 로드중...");
        
        // StartCoroutine(TitleScene());
        
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 40;
        PhotonNetwork.SerializationRate = 20;
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Update()
    {
        if (checkSceneLoding == true)
        {
            StartCoroutine(LoadingScene());
            checkSceneLoding = false;
        }
    }

    public void LoadInGameScene()
    {
        checkSceneLoding = true;
        nextSceneName = loadSceneName;
    }

    public void LoadScene(string _loadSceneName)
    {
        checkSceneLoding = true;
        nextSceneName = _loadSceneName;
    }
    
    public void UnLoadScene()
    {
        checkSceneLoding = true;
        nextSceneName = unLoadSceneName;
    }

    IEnumerator LoadingScene()
    {
        // 프로그램은 현재 장면이 실행될 때 백그라운드로 장면을 로드합니다.
        // 이 기능은 로딩 화면을 만드는 데 특히 유용합니다.
        // 씬빌드를 사용하여 씬(scene)을 로드할 수도 있습니다.
        // 이 경우 장면2에는 씬(scene)Build가 있습니다.
        // 빌드 설정에 표시된 대로 1의 인덱스입니다.
        PhotonNetwork.LoadLevel(nextSceneName);
        yield return null;
        // AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        Debug.Log($"[장시진] 다음씬 로드중...");

        // 비동기 장면이 완전히 로드될 때까지 기다립니다.
        // while (!asyncLoad.isDone)
        // {
        //     yield return null;
        // }
    }
}
