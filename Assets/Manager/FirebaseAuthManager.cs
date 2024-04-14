using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using Firebase.Extensions;
using Firebase;
using System;
using TMPro;
using Unity.VisualScripting;

public class FirebaseAuthManager : MonoBehaviour
{
    [SerializeField]
    private InputField _userEmail;
    [SerializeField]
    private InputField _userPassword;

    [SerializeField]
    private GameObject _createUserIDPanel;
    [SerializeField]
    private Text _createUserIDText;

    private FirebaseAuth _auth; // 로그인, 회원가입 등에 사용
    private FirebaseUser _user; // 인증이 완료된 유저 정보

    void Start()
    {
        _auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    // 유저 회원가입
    public void Create()
    {
        _auth.CreateUserWithEmailAndPasswordAsync(_userEmail.text, _userPassword.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                // 회원가입 취소 - 네트워크 연결이 끊기거나 문제가 발생한 경우 및 취소 버튼을 누른 경우
                Debug.LogError("[장시진] 회원가입 취소");
                return;
            }
            if (task.IsFaulted)
            {
                // 회원가입 실패 => 이미 가입된 이메일 / 회원가입 실패 로직이 실행된 경우                
                Debug.LogError("[장시진] 회원가입 실패");

                return;
            }

            // 회원가입 완료
            var newUser = task.Result;
            Debug.Log($"{_userEmail.text}, {_userPassword.text} 회원가입 완료");
        });
    }

    public void Login()
    {
        _auth.SignInWithEmailAndPasswordAsync(_userEmail.text, _userPassword.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                // 로그인 취소

                return;
            }
            if (task.IsFaulted)
            {
                // 로그인 실패 => 이메일, 비밀번호가 등록된 계정 정보와 일치하지 않음

                return;
            }

            var newUser = task.Result;
        });
    }

    public void LogOut()
    {
        _auth.SignOut();
        Debug.Log("로그아웃");
    }

    // 이메일 중복 확인 메서드    
    public void CheckEmail()
    {
        _auth.FetchProvidersForEmailAsync(_userEmail.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("FetchProvidersForEmailAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("FetchProvidersForEmailAsync encountered an error: " + task.Exception);
                return;
            }

            List<string> providers = (List<string>)task.Result;

            if (providers != null && providers.Count > 0)
            {
                // 중복된 이메일이 있는 경우
                _createUserIDText.text = $"중복된 이메일이 존재합니다.";
                _createUserIDPanel.gameObject.SetActive(true);
                Debug.LogError("This email is already registered.");
            }
            else
            {
                // 중복된 이메일이 없는 경우
                _createUserIDText.text = $"사용 가능한 이메일입니다.";
                _createUserIDPanel.gameObject.SetActive(true);
                Debug.Log("No user found with this email.");
            }
        });
    }
}
