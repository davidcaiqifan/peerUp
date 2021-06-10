using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using UnityEngine.UI;
using Firebase.Extensions;
using System.Transactions;
using System;
using System.Threading.Tasks;

public class MatchChecker : MonoBehaviour
{
    public Firebase.Auth.FirebaseAuth auth;
    public Firebase.Auth.FirebaseUser user;
    Firebase.Database.DatabaseReference roomRef;
    Firebase.Database.DatabaseReference rootRef;
    public GameObject matchScreen;
    public Text matchName;
    public Text course;
    public Text description;

    public GameObject loadingCircle;
    public GameObject matchButton;
    public GameObject moduleInputButton;
    public GameObject cancelSearch;

    // Start is called before the first frame update
    /*async void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        roomRef = FirebaseDatabase.DefaultInstance.RootReference.Child("Rooms");
        rootRef = FirebaseDatabase.DefaultInstance.RootReference;
        if (await checkMatchFindingStatus())
        {
            checkRooms();
        }
        
    }*/
    async void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        roomRef = FirebaseDatabase.DefaultInstance.RootReference.Child("Rooms");
        rootRef = FirebaseDatabase.DefaultInstance.RootReference;
        if (await checkMatchFindingStatus())
        {
            checkRooms();
        }
        else
        {
            moduleInputButton.SetActive(true);
            matchButton.SetActive(true);
        }

    }

    /*public async void checkRooms()
    {
        Boolean found = false;
        Firebase.Database.DataSnapshot roomData = await roomRef.GetValueAsync();
        if(roomData == null)
        {
        }
        else
        {
            foreach(Firebase.Database.DataSnapshot room in roomData.Children)
            {
                MatchRoom matchRoom = JsonUtility.FromJson<MatchRoom>(room.GetRawJsonValue());
                if (matchRoom.pax == "2" && (matchRoom.user1 == user.UserId || matchRoom.user2 == user.UserId))
                {
                    string roomKey = room.Key;
                    matchScreen.SetActive(true);
                    Player userInfo = await getMatchInfo(matchRoom.user2);
                    matchName.text = userInfo.displayName;
                    course.text = userInfo.course;
                    description.text = userInfo.description;
                    updateMatchList(matchRoom.user2, matchRoom.mod);
                    await roomRef.Child(roomKey).RemoveValueAsync();
                    updateMatchFindingStatus("0");
                    found = true;
                }
            }
        }
        if(!found)
        {
            button1.SetActive(false);
            loadingCircle.SetActive(true);
            cancelSearch.SetActive(true);
        }
    }*/
    public async void checkRooms()
    {
        Boolean found = false;
        Firebase.Database.DataSnapshot roomData = await roomRef.GetValueAsync();
        if (roomData == null)
        {
        }
        else
        {
            foreach (Firebase.Database.DataSnapshot room in roomData.Children)
            {
                MatchRoom matchRoom = JsonUtility.FromJson<MatchRoom>(room.GetRawJsonValue());
                if (matchRoom.pax == "2" && (matchRoom.user1 == user.UserId || matchRoom.user2 == user.UserId))
                {
                    string roomKey = room.Key;
                    matchScreen.SetActive(true);
                    Player userInfo = await getMatchInfo(matchRoom.user2);
                    matchName.text = userInfo.displayName;
                    course.text = userInfo.course;
                    description.text = userInfo.description;
                    updateMatchList(matchRoom.user2, matchRoom.mod);
                    await roomRef.Child(roomKey).RemoveValueAsync();
                    updateMatchFindingStatus("0");
                    found = true;
                }
            }
        }
        if (!found)
        {
            //button1.SetActive(false);
            loadingCircle.SetActive(true);
            cancelSearch.SetActive(true);
        }
        else
        {
            moduleInputButton.SetActive(true);
            matchButton.SetActive(true);
        }
    }
    public async void updateMatchList(string matchIdUpdate, string selectedMod)
    {
        IDictionary<string, object> updates = new Dictionary<string, object>();
        IDictionary<string, object> matchAndMod = new Dictionary<string, object>();
        matchAndMod.Add("match", matchIdUpdate);
        matchAndMod.Add("mod", selectedMod);
        updates.Add(matchIdUpdate + selectedMod, matchAndMod);
        //rootRef.Child("Users").Child("User" + user.UserId).Child("matches").Child("match" + i).UpdateChildrenAsync(updates);
        await rootRef.Child("Users").Child("User" + user.UserId).Child("matches").UpdateChildrenAsync(updates);
    }

    public async Task<Player> getMatchInfo(string matchId)
    {
        DataSnapshot matchInfo = await rootRef.Child("Users").Child("User" + matchId).GetValueAsync();
        return JsonUtility.FromJson<Player>(matchInfo.GetRawJsonValue());
    }

    public async Task<Boolean> checkMatchFindingStatus()
    {
        Firebase.Database.DataSnapshot matchFindingStatus = await rootRef.Child("Users").Child("User" + user.UserId).Child("isMatching").GetValueAsync();
        if (matchFindingStatus.GetRawJsonValue() == "\"1\"")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void updateMatchFindingStatus(string findStatus)
    {
        IDictionary<string, object> updates = new Dictionary<string, object>();
        updates.Add("isMatching", findStatus);
        rootRef.Child("Users").Child("User" + user.UserId).UpdateChildrenAsync(updates);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
