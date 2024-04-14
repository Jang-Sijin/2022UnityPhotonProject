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

    private FirebaseAuth _auth; // �α���, ȸ������ � ���
    private FirebaseUser _user; // ������ �Ϸ�� ���� ����

    void Start()
    {
        _auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    // ���� ȸ������
    public void Create()
    {
        _auth.CreateUserWithEmailAndPasswordAsync(_userEmail.text, _userPassword.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                // ȸ������ ��� - ��Ʈ��ũ ������ ����ų� ������ �߻��� ��� �� ��� ��ư�� ���� ���
                Debug.LogError("[�����] ȸ������ ���");
                return;
            }
            if (task.IsFaulted)
            {
                // ȸ������ ���� => �̹� ���Ե� �̸��� / ȸ������ ���� ������ ����� ���                
                Debug.LogError("[�����] ȸ������ ����");

                return;
            }

            // ȸ������ �Ϸ�
            var newUser = task.Result;
            Debug.Log($"{_userEmail.text}, {_userPassword.text} ȸ������ �Ϸ�");
        });
    }

    public void Login()
    {
        _auth.SignInWithEmailAndPasswordAsync(_userEmail.text, _userPassword.text).ContinueWith(task =>
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

    // �̸��� �ߺ� Ȯ�� �޼���    
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
                // �ߺ��� �̸����� �ִ� ���
                _createUserIDText.text = $"�ߺ��� �̸����� �����մϴ�.";
                _createUserIDPanel.gameObject.SetActive(true);
                Debug.LogError("This email is already registered.");
            }
            else
            {
                // �ߺ��� �̸����� ���� ���
                _createUserIDText.text = $"��� ������ �̸����Դϴ�.";
                _createUserIDPanel.gameObject.SetActive(true);
                Debug.Log("No user found with this email.");
            }
        });
    }
}
