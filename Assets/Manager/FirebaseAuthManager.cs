using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;

public class FirebaseAuthManager : MonoBehaviour
{
    public InputField UserEmail;
    public InputField UserPassword;

    private FirebaseAuth _auth; // �α���, ȸ������ � ���
    private FirebaseUser _user; // ������ �Ϸ�� ���� ����

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
                // ȸ������ ���

                return;
            }
            if(task.IsFaulted)
            {
                // ȸ������ ���� => �̹� ���Ե� �̸��� / ��й�ȣ �ʹ� ������ ��

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
