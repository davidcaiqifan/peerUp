using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class ModuleInput : MonoBehaviour
{
    public GameObject moduleInputPanel;
    private string theText;
    public GameObject Mod1;
    public InputField inputMod;
    public GameObject loadingCircle;
    public GameObject button1;
    public GameObject button2;
    public GameObject cancelSearch;
    public static ModuleInput instance;
    private string user;
    public Firebase.Auth.FirebaseAuth auth;
    public Firebase.Auth.FirebaseUser firebaseUser;
    public static ModuleInput staticMI = new ModuleInput();

    private void Awake()
    {
      instance = this;
    }
    void Start()
    {
        user = PlayerPrefs.GetString("studentID");
        Mod1.GetComponent<InputField>().text = PlayerPrefs.GetString(user + "savedMod");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        firebaseUser = auth.CurrentUser;
    }

    public void saveMod()
    {
        theText = inputMod.text.ToUpper();
        Mod1.GetComponent<InputField>().text = theText;
        moduleInputPanel.SetActive(false);
        PlayerPrefs.SetString(user + "savedMod", theText);
        Firebase.Database.DatabaseReference userRef = FirebaseDatabase.DefaultInstance.RootReference.Child("Users").Child("User" + firebaseUser.UserId);
        IDictionary<string, object> updates = new Dictionary<string, object>();
        updates.Add("selectedMod", Mod1.GetComponent<InputField>().text);
        userRef.UpdateChildrenAsync(updates);
    }

    public void backButton()
    {
        moduleInputPanel.SetActive(false);
    }
    public void moduleButton()
    {
        if (cancelSearch.activeInHierarchy == true)
        {
            loadingCircle.SetActive(false);
            cancelSearch.SetActive(false);
            button2.SetActive(true);
        }
            moduleInputPanel.SetActive(true);
        
    }
    public void findMatch()
    {
        if (Mod1.GetComponent<InputField>().text != "")
        {
            button1.SetActive(false);
            loadingCircle.SetActive(true);
            cancelSearch.SetActive(true);
        }
    }

    public void cancelMatch()
    {
        loadingCircle.SetActive(false);
        cancelSearch.SetActive(false);
        button1.SetActive(true);
        button2.SetActive(true);      
    }

}
