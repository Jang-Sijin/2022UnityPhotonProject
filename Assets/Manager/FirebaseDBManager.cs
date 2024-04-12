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

        // 데이터 베이스 참조
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

                // 데이터베이스의 모든 데이터에 대해 반복 순회
                foreach(DataSnapshot childSnapshot in snapshot.Children) 
                { 
                    // 사용자가 입력한 텍스트와 데이터베이스의 값이 일치하는지 확인
                    if(userInput.Equals(childSnapshot.Value.ToString()))
                    {
                        // 일치하는 데이터가 발견되면 true 반환
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

        // 데이터베이스의 모든 데이터에 대해 반복
        foreach (DataSnapshot childSnapshot in snapshot.Children)
        {
            // 사용자가 입력한 텍스트와 데이터베이스의 값이 일치하는지 확인
            if (userInput.Equals(childSnapshot.Value.ToString()))
            {
                // 일치하는 데이터가 발견되면 true 반환
                Debug.Log("Text found in database!");
                return true;
            }
        }

        // 일치하는 데이터가 없으면 false 반환
        Debug.Log("Text not found in database.");
        return false;
    }
}
*/