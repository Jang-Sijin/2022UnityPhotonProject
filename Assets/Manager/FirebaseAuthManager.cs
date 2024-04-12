using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using Firebase.Extensions;
using Firebase;
using System;

public class FirebaseAuthManager : MonoBehaviour
{
    public InputField UserEmail;
    public InputField UserPassword;

    private FirebaseAuth _auth; // 로그인, 회원가입 등에 사용
    private FirebaseUser _user; // 인증이 완료된 유저 정보

    void Start()
    {
        _auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    // 유저 회원가입
    public void Create()
    {
        _auth.CreateUserWithEmailAndPasswordAsync(UserEmail.text, UserPassword.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                // 회원가입 취소
                Debug.LogError("[장시진] 회원가입 취소");
                return;
            }
            if (task.IsFaulted)
            {
                // 회원가입 실패 => 이미 가입된 이메일 / 비밀번호 너무 간단함 등
                Debug.LogError("[장시진] 회원가입 실패");
                return;
            }

            // 회원가입 완료
            var newUser = task.Result;
            Debug.Log($"{UserEmail.text}, {UserPassword.text} 회원가입 완료");
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
