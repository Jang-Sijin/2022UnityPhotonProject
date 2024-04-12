using Firebase;
// using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/*

public class FirebaseDBManager : MonoBehaviour
{
    private string DBurl = "https://tanktutorial-gameproject-default-rtdb.firebaseio.com/";
    DatabaseReference reference;

    #region Singleton
    private static FirebaseDBManager _instance = null;
    public static FirebaseDBManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new FirebaseDBManager();
            }

            return _instance;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri(DBurl);

        // ������ ���̽� ����
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    
    public bool FindTextInDatabase(string userInput)
    {
        bool result = false;

        reference.GetValueAsync().ContinueWith(task =>
        {
            if(task.IsFaulted)
            {
                Debug.LogError("Firebase database error: " + task.Exception.ToString());
                result = false;
            }

            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // �����ͺ��̽��� ��� �����Ϳ� ���� �ݺ� ��ȸ
                foreach(DataSnapshot childSnapshot in snapshot.Children) 
                { 
                    // ����ڰ� �Է��� �ؽ�Ʈ�� �����ͺ��̽��� ���� ��ġ�ϴ��� Ȯ��
                    if(userInput.Equals(childSnapshot.Value.ToString()))
                    {
                        // ��ġ�ϴ� �����Ͱ� �߰ߵǸ� true ��ȯ
                        result = true;
                        break;
                    }
                }
            }
        });

        return result;
    }    

    public async Task<bool> CheckTextInDatabase(string userInput)
    {
        DataSnapshot snapshot = await reference.GetValueAsync();

        if (snapshot == null || snapshot.ChildrenCount == 0)
        {
            Debug.LogError("Firebase database is empty or null.");
            return false;
        }

        // �����ͺ��̽��� ��� �����Ϳ� ���� �ݺ�
        foreach (DataSnapshot childSnapshot in snapshot.Children)
        {
            // ����ڰ� �Է��� �ؽ�Ʈ�� �����ͺ��̽��� ���� ��ġ�ϴ��� Ȯ��
            if (userInput.Equals(childSnapshot.Value.ToString()))
            {
                // ��ġ�ϴ� �����Ͱ� �߰ߵǸ� true ��ȯ
                Debug.Log("Text found in database!");
                return true;
            }
        }

        // ��ġ�ϴ� �����Ͱ� ������ false ��ȯ
        Debug.Log("Text not found in database.");
        return false;
    }
}
*/