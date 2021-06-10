using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;

public class DataBridge : MonoBehaviour
{
   
private Player data;
    public InputField usernameInput;//, passwordInput;
    private string dataURL = "https://peerup-1f6c7.firebaseio.com/";
    public DatabaseReference databaseReference;
    public Firebase.Auth.FirebaseAuth auth;
    public Firebase.Auth.FirebaseUser user;

    // Handle initialization of the necessary firebase modules:
    /*public void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }*/

    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(dataURL);
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void registerData()
    {
        //InitializeFirebase();
        user = auth.CurrentUser;
        data = new Player(usernameInput.text, user.UserId);
        string jsonData = JsonUtility.ToJson(data);
        databaseReference.Child("Users").Child("User" + user.UserId).
            SetRawJsonValueAsync(jsonData);

    }
}
