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

    private FirebaseAuth _auth; // �α���, ȸ������ � ���
    private FirebaseUser _user; // ������ �Ϸ�� ���� ����   

    public void Init()
    {
        _auth = FirebaseAuth.DefaultInstance;
        _auth.StateChanged += OnChanged;
    }

    // ���� ���� ���� ���� ���θ� Ȯ��
    private void OnChanged(object sender, EventArgs e)
    { 
        // �ֱ� ������ ������ �ƴ� ���
        if(_auth.CurrentUser != _user)
        {
            bool signed = (_auth.CurrentUser != _user && _auth.CurrentUser != null);

            if (!signed && _user != null)
            {
                Debug.Log("�α׾ƿ�");
            }

            _user = _auth.CurrentUser;
            if(signed)
            {
                Debug.Log("�α���");
            }
        }
    }

    public void CreateUserAccount(string userEmail, string userPassword)
    {
        _auth.CreateUserWithEmailAndPasswordAsync(userEmail, userPassword).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                // ȸ������ ���

                return;
            }
            if (task.IsFaulted)
            {
                // ȸ������ ���� => �̹� ���Ե� �̸��� / ��й�ȣ �ʹ� ������ ��

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
                // �α��� ���

                return;
            }
            if (task.IsFaulted)
            {
                // �α��� ���� => �̸���, ��й�ȣ�� ��ϵ� ���� ������ ��ġ���� ����

                return;
            }

            var newUser = task.Result;
        });
    }

    public void LogOut()
    {
        _auth.SignOut();
        Debug.Log("�α׾ƿ�");
    }
}
