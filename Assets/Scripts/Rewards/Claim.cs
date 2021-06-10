using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;
public class Claim : MonoBehaviour
{
    private string dataURL = "https://peerup-1f6c7.firebaseio.com/";
    public Firebase.Auth.FirebaseAuth auth;
    public Firebase.Auth.FirebaseUser user;

    private void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
   /*     FirebaseDatabase.DefaultInstance.GetReferenceFromUrl(dataURL).Child("Users").Child("User" + user.UserId).Child("reward").GetValueAsync().ContinueWithOnMainThread((task => {
            if (task.IsCanceled)
            {
                return;
            }
            if (task.IsFaulted)
            {
                return;
            }
            if (task.IsCompleted)
            {
                
                DataSnapshot snapshot = task.Result;
                string userData = snapshot.GetRawJsonValue();
                Player userInfo = JsonUtility.FromJson<Player>(userData);
                if (userInfo.reward.Length <= 0)
                {
                    Debug.Log("unactivated");
                    gameObject.SetActive(false);
                }
            }

        }));*/
        GetComponent<Button>().onClick.AddListener(changeData);
      
        
    }
    public void changeData()
    {
       
   
        IDictionary<string, object> updates = new Dictionary<string, object>();
        updates.Add("reward", "");
        FirebaseDatabase.DefaultInstance.GetReferenceFromUrl(dataURL).Child("Users").Child("User" + user.UserId).UpdateChildrenAsync(updates)
             .ContinueWithOnMainThread((task => {
                 if (task.IsCanceled)
                 {
                     return;
                 }
                 if (task.IsFaulted)
                 {
                     return;
                 }
                 if (task.IsCompleted)
                 {
                     print("Claimed");
                     gameObject.SetActive(false);
                 }

             }));


    }
}
