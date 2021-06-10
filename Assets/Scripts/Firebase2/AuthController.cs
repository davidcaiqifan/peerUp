using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Extensions;
using Firebase.Analytics;
using Firebase.Database;
using System.Globalization;
using Firebase;
using Firebase.Unity.Editor;

public class AuthController : MonoBehaviour
{
    public Text statusPrintLogin;
    public Text statusPrintSignUp;
    public InputField emailInput, passwordInput, newEmailInput, newPasswordInput, newPasswordInput2;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
    private Player data;
    public InputField usernameInput;
    //private string dataURL = "https://peerup-1f6c7.firebaseio.com/";
    public DatabaseReference databaseReference;
    private DatabaseReference reference;

    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

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
                //SceneManager.LoadSceneAsync("PostMenu");
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }
    public void Login()
    {
        Firebase.Auth.Credential credential =
        Firebase.Auth.EmailAuthProvider.GetCredential(emailInput.text, passwordInput.text);
        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            Firebase.Auth.FirebaseUser user = auth.CurrentUser;
            if (task.IsCanceled)
            {
                Debug.Log("cancelled");
                statusPrintLogin.text = task.Exception.GetBaseException().Message.ToString();
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("faulted");
                statusPrintLogin.text = task.Exception.GetBaseException().Message.ToString();
                return;
            }
            if (task.IsCompleted)
            {

                Debug.Log("completed");
                checkIfEmailVerified(user);
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    user.DisplayName, user.UserId);
            }

        });
    }
    public void verifyEmail()
    {
        user.SendEmailVerificationAsync().ContinueWithOnMainThread(task3 =>
        {
            statusPrintSignUp.text = "Sending verification email...";
            if (task3.IsCanceled)
            {
                statusPrintSignUp.text = task3.Exception.GetBaseException().Message.ToString();
                return;
            }
            if (task3.IsFaulted)
            {
                Debug.Log("faulted");
                statusPrintSignUp.text = task3.Exception.GetBaseException().Message.ToString();
                return;
            }
            if (task3.IsCompleted)
            {
                statusPrintSignUp.text = "Verification email sent.";
                Debug.Log("done");
            }
        });
    }

    private void checkIfEmailVerified(FirebaseUser user)
    {

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://peerup-1f6c7.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        if (user.IsEmailVerified)
        {
            PlayerPrefs.SetString("emailNUS", emailInput.text);
            PlayerPrefs.SetString("password", passwordInput.text);
            statusPrintLogin.text = "Login Success!";
            SceneManager.LoadScene("PostMenu");
        }
        else
        {
            statusPrintLogin.text = "Please verify email first.";
        }
    }

    public void RegisterUser()
    {
        if(newPasswordInput.text != newPasswordInput2.text)
        {

            statusPrintSignUp.text = "Passwords different";
            return;
        }

        int start = newEmailInput.text.IndexOf('@');
        string nusMail = emailInput.text;
        if (newEmailInput.text.Length - start < 10 || newEmailInput.text.Substring(start + 1, 9) != "u.nus.edu")
        {
            statusPrintSignUp.text = "Use only nus email";
            return;
        }

        FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(newEmailInput.text, newPasswordInput.text).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Firebase.FirebaseException e =
                task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                GetErrorMessage((AuthError)e.ErrorCode);        
                return;
            }
            if (task.IsFaulted)
            {
                Firebase.FirebaseException e =
                task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                GetErrorMessage((AuthError)e.ErrorCode);
                return;
            }

            if(task.IsCompleted)
            {
                /* statusPrintSignUp.text = "Signup Success!";
                 print("Signup Success!"); 
                 SceneManager.LoadSceneAsync("PostMenu");*/
                data = new Player(usernameInput.text, user.UserId);
                string jsonData = JsonUtility.ToJson(data);
                print(jsonData);
                databaseReference.Child("Users").Child("User" + user.UserId).
                    SetRawJsonValueAsync(jsonData);
                verifyEmail();

            }
        });
        //print("displayname" + auth.CurrentUser.DisplayName);
    }

    
     
    public void GetErrorMessage(AuthError errorCode)
    {
        string msg = "";
        msg = errorCode.ToString();
        print(msg);
        statusPrintLogin.text = msg;
        statusPrintSignUp.text = msg;

       
    }

    private void Start()
    {
        /*await Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;       
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
            
        });*/
        InitializeFirebase();
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    

}
