using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;

public class FirebaseAuthManager : MonoBehaviour
{
    public InputField UserEmail;
    public InputField UserPassword;

    private FirebaseAuth _auth; // 로그인, 회원가입 등에 사용
    private FirebaseUser _user; // 인증이 완료된 유저 정보

    void Start()
    {
        _auth = FirebaseAuth.DefaultInstance;
    }    

    public void Create()
    {
        _auth.CreateUserWithEmailAndPasswordAsync(UserEmail.text, UserPassword.text).ContinueWith(task =>
        {
            if(task.IsCanceled)
            {
                // 회원가입 취소

                return;
            }
            if(task.IsFaulted)
            {
                // 회원가입 실패 => 이미 가입된 이메일 / 비밀번호 너무 간단함 등

                return;
            }

            var newUser = task.Result;
        });
    }

    public void Login()
    {
        _auth.SignInWithEmailAndPasswordAsync(UserEmail.text, UserPassword.text).ContinueWith(task =>
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
