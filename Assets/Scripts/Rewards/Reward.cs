using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reward : MonoBehaviour
{
    [SerializeField]
    private Text text;
    [SerializeField]
    private Image rewardImage;
    // Start is called before the first frame update
    private string dataURL = "https://peerup-1f6c7.firebaseio.com/";
    public Firebase.Auth.FirebaseAuth auth;
    public Firebase.Auth.FirebaseUser user;
    private Object[] sprites;
    void Start()
    {
        sprites = Resources.LoadAll("Rewards", typeof(Sprite));
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        FirebaseDatabase.DefaultInstance.GetReferenceFromUrl(dataURL).Child("Users").Child("User" + user.UserId).GetValueAsync()
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
              DataSnapshot snapshot = task.Result;
              string userData = snapshot.GetRawJsonValue();
              Player userInfo = JsonUtility.FromJson<Player>(userData);
              text.text = "StudentID: " + userInfo.displayName;
              text.text += "\nMatchID: " + userInfo.mostRecentMatch;
              if(userInfo.reward != "") rewardImage.sprite = (Sprite)sprites[int.Parse(userInfo.reward)];
          }
      }));
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
