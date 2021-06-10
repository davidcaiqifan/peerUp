using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Unity.Editor;
using Firebase;
using Firebase.Extensions;

public class userProfile : MonoBehaviour
{
    [SerializeField]
    private Image face;
    [SerializeField]
    private GameObject[] interactables;
    [SerializeField]
    private Text success;
    internal static userProfile instance;
    private int avatarIndex;
    private void Awake()
    {
       instance = this;
    }
    public void function()
    {
        foreach (GameObject g in interactables)
        {
            Selectable selectable = g.GetComponent<Selectable>();
            selectable.interactable = !selectable.interactable;
            success.text = "";
        }
    }

    public void saveFunction()
    {
        foreach (GameObject g in interactables)
        {
          
            if(g.transform.name != "Face")
            {
                Selectable selectable = g.GetComponent<Selectable>();
                selectable.interactable = false;
            }

        }
        success.text = "Profile saved successfully!";
    }

    /*public InputField exp;
    public InputField displayName;
    public InputField course;
    public InputField description;*/
    private Object[] sprites;
    public Text test;
    public InputField testInput;
    public InputField testInput2;
    public InputField testInput3;
    public InputField displayName;
    [SerializeField]
    
    //[SerializeField]
    private InputField course;
    [SerializeField]
    private InputField description;
    [SerializeField]
    private InputField exp;
    string sN;
    private string dataURL = "https://peerup-1f6c7.firebaseio.com/";

    public Firebase.Auth.FirebaseAuth auth;
    public Firebase.Auth.FirebaseUser user;
    DatabaseReference databaseReference;


    private void Update()
    {
        face.sprite = (Sprite)sprites[PlayerPrefs.GetInt("avatar", avatarIndex)];
    }

    public void changeData()
    {
            
        IDictionary<string, object> updates = new Dictionary<string, object>();
        
                updates.Add("course", course.text);
                updates.Add("displayName", displayName.text);
                updates.Add("description", description.text);
                updates.Add("avatarID", PlayerPrefs.GetInt("avatar", 0).ToString());
                avatarIndex = PlayerPrefs.GetInt("avatar", 0);
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
                    saveFunction();
                }

            }));
  
      
    }

    private void Start()
    {
        sprites = Resources.LoadAll("Avatars", typeof(Sprite));
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        print(user.UserId);
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://peerup-1f6c7.firebaseio.com/");
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
                    print(userData);
                    sN = userInfo.studentNumber;
                    test.text = sN;
                    testInput.text = userInfo.displayName;
                    testInput2.text = userInfo.course;
                    testInput3.text = userInfo.description;
                    avatarIndex = int.Parse(userInfo.avatarID);
                    print(sN); 
                    print("done");
                }
            }));
    }


}
