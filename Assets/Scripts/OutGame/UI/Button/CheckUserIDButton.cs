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
            // 미중복 아이디
            _errorText.GetComponent<Text>().text = "사용 가능한\n 아이디입니다.";
            _errorPanel_CreateUserID.SetActive(true);
        }
        else
        {
            _errorText.GetComponent<Text>().text = "사용 불가능한\n 아이디입니다.";
            _errorPanel_CreateUserID.SetActive(true);
        }
    }
}
