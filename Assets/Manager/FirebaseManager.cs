using UnityEngine;
using Firebase.Auth;
using System;

public class FirebaseManager
{
    #region Singleton
    private static FirebaseManager _instance = null;
    public static FirebaseManager Instance
    { 
        get 
        {
            if (_instance == null)
            {
                _instance = new FirebaseManager();
            }

            return _instance; 
        } 
    }
    #endregion

    public Action<bool> LoginState;

    private FirebaseAuth _auth; // 로그인, 회원가입 등에 사용
    private FirebaseUser _user; // 인증이 완료된 유저 정보   

    public void Init()
    {
        _auth = FirebaseAuth.DefaultInstance;
        _auth.StateChanged += OnChanged;
    }

    // 유저 접속 상태 변경 여부를 확인
    private void OnChanged(object sender, EventArgs e)
    { 
        // 최근 접속한 유저가 아닌 경우
        if(_auth.CurrentUser != _user)
        {
            bool signed = (_auth.CurrentUser != _user && _auth.CurrentUser != null);

            if (!signed && _user != null)
            {
                Debug.Log("로그아웃");
            }

            _user = _auth.CurrentUser;
            if(signed)
            {
                Debug.Log("로그인");
            }
        }
    }

    public void CreateUserAccount(string userEmail, string userPassword)
    {
        _auth.CreateUserWithEmailAndPasswordAsync(userEmail, userPassword).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                // 회원가입 취소

                return;
            }
            if (task.IsFaulted)
            {
                // 회원가입 실패 => 이미 가입된 이메일 / 비밀번호 너무 간단함 등

                return;
            }

            var newUser = task.Result;
        });
    }

    public void LoginUserAccount(string userEmail, string userPassword)
    {
        _auth.SignInWithEmailAndPasswordAsync(userEmail, userPassword).ContinueWith(task =>
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
}
