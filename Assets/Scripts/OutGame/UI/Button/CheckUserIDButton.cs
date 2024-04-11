using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CheckUserIDButton : MonoBehaviour
{
    [SerializeField]
    private GameObject _inputUserIDGobj;

    [SerializeField]
    private GameObject _errorPanel_CreateUserID;

    [SerializeField]
    private GameObject _errorText;

    public void ClickUserIDButton()
    {
        string inputUserName = _inputUserIDGobj.GetComponent<InputField>().text.ToString();
        Task<bool> result = false;

        result = FirebaseDBManager.Instance.FindTextInDatabase(inputUserName);

        if (result == false)
        {
            // ���ߺ� ���̵�
            _errorText.GetComponent<Text>().text = "��� ������\n ���̵��Դϴ�.";
            _errorPanel_CreateUserID.SetActive(true);
        }
        else
        {
            _errorText.GetComponent<Text>().text = "��� �Ұ�����\n ���̵��Դϴ�.";
            _errorPanel_CreateUserID.SetActive(true);
        }
    }
}
