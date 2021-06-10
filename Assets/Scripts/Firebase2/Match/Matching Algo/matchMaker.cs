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

public class matchMaker : MonoBehaviour
{
    public Firebase.Auth.FirebaseAuth auth;
    public Firebase.Auth.FirebaseUser user;
    public InputField selectedMod;
    Firebase.Database.DatabaseReference rootRef;
    Firebase.Database.DatabaseReference roomRef;
    public GameObject matchScreen;
    public Text matchName;
    public Text course;
    public Text description;
    public string matchId;
    public Boolean madeRoom;
    public string roomKey;
    public string finalMatchId; 
    public Firebase.Database.DataSnapshot matchListData;
    public Boolean mld;
    public Text warningText;

    async void Start()
    {    
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        roomRef = FirebaseDatabase.DefaultInstance.RootReference.Child("Rooms");
        rootRef = FirebaseDatabase.DefaultInstance.RootReference;
        //matchListData = await getMatches();
        matchListData = await rootRef.Child("Users").Child("User" + user.UserId).Child("matches").GetValueAsync();        
        mld = matchListData.Exists;

    }

    async void HandleMatchListChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        //Do something with the data in args.Snapshot          
        string roomData = args.Snapshot.GetRawJsonValue();
        //print(roomData);
        matchListData = await rootRef.Child("Users").Child("User" + user.UserId).Child("matches").GetValueAsync();
        mld = matchListData.Exists;
        //matchListData = await getMatches();
        //print(matchListData.GetRawJsonValue());     
    }

    void HandleChildChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        //Do something with the data in args.Snapshot
        string roomData = args.Snapshot.GetRawJsonValue(); 
        if (madeRoom == false)
        {

        }
        else
        {
            if (roomData == "2")
            {
            }
            //if the child updated is the 2nd userid
            else if (roomData != "2")
            {
                print("Match Found :" + roomData);
                MatchRoom matchRoom = JsonUtility.FromJson<MatchRoom>(roomData);
                matchId = matchRoom.user2;
                finalMatchId = matchRoom.user2;
                print(matchId);
                rootRef.Child("Users").Child("User" + finalMatchId).GetValueAsync().ContinueWithOnMainThread((item =>
                {
                    if (item.IsCanceled)
                    {
                        return;
                    }
                    if (item.IsFaulted)
                    {

                        return;
                    }
                    if (item.IsCompleted && item.Result.Exists)
                    {                      
                        matchScreen.SetActive(true);
                        DataSnapshot snapshot = item.Result;
                        string userData = snapshot.GetRawJsonValue();
                        print(userData);
                        Player userInfo = JsonUtility.FromJson<Player>(userData);
                        matchName.text = userInfo.displayName;
                        course.text = userInfo.course;
                        description.text = userInfo.description;
                        updateMatchList(matchId, userInfo.displayName);
                        updateMatchFindingStatus("0");
                        FirebaseDatabase.DefaultInstance.RootReference.Child("Rooms").Child(roomKey).RemoveValueAsync();
                        ModuleInput.instance.cancelMatch();
                    }
                }));

            }
        }
    }

    public async void matchFunction()
    {
        Firebase.Database.DataSnapshot check = await rootRef.Child("Users").Child("User" + user.UserId).Child("matches").GetValueAsync();
        if (check.ChildrenCount < 5)
        {
            if (selectedMod.GetComponent<InputField>().text != "")
            {
                ModuleInput.instance.findMatch();
                updateMatchFindingStatus("1");
                var matchListRef = FirebaseDatabase.DefaultInstance
                    .GetReference("Users").Child("User" + user.UserId).Child("matches"); 
                matchListRef.ChildAdded += HandleMatchListChanged;
                findRoom(user.UserId, selectedMod.text); 
                var reference = FirebaseDatabase.DefaultInstance
                    .GetReference("Rooms").Child(roomKey);
                reference.ChildChanged += HandleChildChanged;
            }
            else
            {
                warningText.text = "Please input a mod!";
            }
        }
        else
        {
            warningText.text = "You already have five matches!";
        }
           
    }
    //transactional method to find a room that has the desired mod
    public void findRoom(string userId, string mod) 
    {   
        roomRef.RunTransaction(mutableData =>
        {
            List<object> rooms = mutableData.Value as List<object>;
            Dictionary<string, object> roomUpdates =
                        new Dictionary<string, object>();
            if (rooms == null)
            {
                rooms = new List<object>();
                roomUpdates["user1"] = userId;
                roomUpdates["user2"] = "";
                roomUpdates["mod"] = mod;
                roomUpdates["pax"] = "1";
                rooms.Add(roomUpdates);
                mutableData.Value = rooms;
                string text = rooms.ToString();
                madeRoom = true;
            }
            else if (rooms != null)
            {
                foreach (var room in rooms)
                {
                    if (!(room is Dictionary<string, object>)) continue;
                    string matchIdDetails = (string)((Dictionary<string, object>)room)["user1"];

                    string roomPax = (string)
                      ((Dictionary<string, object>)room)["pax"];
                    string roomMod = (string)
                      ((Dictionary<string, object>)room)["mod"];
                    print("checkRoom");
                    print(matchListData.Exists);
                    if (roomMod == selectedMod.text && roomPax != "2")
                    {
                        if(mld)
                        {
                            if(checkUserAndModNew(matchIdDetails))
                            {
                                
                            }
                            else
                            {
                                break;
                            }
                        }
                        matchId = matchIdDetails; 
                        ((Dictionary<string, object>)room)["pax"] = "2";
                        ((Dictionary<string, object>)room)["user2"] = userId;
                        print("Match Found!");
                        madeRoom = false;
                        mutableData.Value = rooms;
                        ModuleInput.instance.cancelMatch();
                        return TransactionResult.Success(mutableData);    
                    }
                    print("checkDone");
                    
                }
                roomUpdates["user1"] = userId;
                roomUpdates["user2"] = "";
                roomUpdates["mod"] = mod;
                roomUpdates["pax"] = "1";
                rooms.Add(roomUpdates);
                mutableData.Value = rooms;
                print("No Room Matches");
                madeRoom = true;
            }
            return TransactionResult.Success(mutableData);
        }).ContinueWithOnMainThread(async task => {
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
                print(matchId);
                foreach (var child in snapshot.Children)
                {                                   
                    roomKey = child.Key;             
                }
                if (madeRoom == false)
                {             
                    await rootRef.Child("Users").Child("User" + matchId).GetValueAsync().ContinueWithOnMainThread((info =>
                    {
                        if (info.IsCanceled)
                        {
                            return;
                        }
                        if (info.IsFaulted)
                        {
                            return;
                        }
                        if (info.IsCompleted)
                        {
                            matchScreen.SetActive(true);
                            DataSnapshot snapshot1 = info.Result;                          
                            string userData1 = snapshot1.GetRawJsonValue();
                            //print("display" + matchId + "userData" + userData1);
                            Player userInfo = JsonUtility.FromJson<Player>(userData1);
                            matchName.text = userInfo.displayName;
                            course.text = userInfo.course;
                            description.text = userInfo.description;
                            updateMatchList(matchId, userInfo.displayName);
                            updateMatchFindingStatus("0");
                        }
                    }));
                    return;
                }
                
            }
        });
    }

    public Boolean checkUserAndModNew(string matchFoundId)
    {
        print("start check");
        print("matchListData : " + matchListData.GetRawJsonValue());
        Boolean result = true;
        if (matchListData.Exists)
        {
            print("matchListData : " + matchListData.GetRawJsonValue());
            foreach (Firebase.Database.DataSnapshot match in matchListData.Children)
            {
                string matchesData = match.GetRawJsonValue();
                print("matches data : " + matchesData);
                Player.Value matchInfo = JsonUtility.FromJson<Player.Value>(matchesData);
                if (matchInfo.match == matchFoundId && matchInfo.mod == selectedMod.text)
                {
                    print("already matched");
                    result = false;
                    break;
                }
                //print("new match");
            }
        }
        print(result);
        return result;
    }


    public async Task<Firebase.Database.DataSnapshot> getMatches()
    {
        Firebase.Database.DataSnapshot matchReturn = null;
        await rootRef.Child("Users").Child("User" + user.UserId).Child("matches").GetValueAsync().ContinueWithOnMainThread((task =>
        {
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
                matchReturn = snapshot;
                return;
            }
            
        }));
        return matchReturn;
    }
    //not working
    //updates match status of user
    public void updateMatchStatus(string matchStatus)
    {
        IDictionary<string, object> updates = new Dictionary<string, object>();
        updates.Add("matchStatus", matchStatus);
        rootRef.Child("Users").Child("User" + user.UserId).UpdateChildrenAsync(updates);
    }
   
    //updates matchList of users when match found
    public void updateMatchList(string matchIdUpdate, string matchDisplayName)
    {       
        IDictionary<string, object> updates = new Dictionary<string, object>();
        IDictionary<string, object> matchAndMod = new Dictionary<string, object>();
        matchAndMod.Add("match", matchIdUpdate);
        matchAndMod.Add("mod", selectedMod.text);
        updates.Add(matchIdUpdate + selectedMod.text, matchAndMod);
        //rootRef.Child("Users").Child("User" + user.UserId).Child("matches").Child("match" + i).UpdateChildrenAsync(updates);
        rootRef.Child("Users").Child("User" + user.UserId).Child("matches").UpdateChildrenAsync(updates);
        IDictionary<string, object> matchUpdate = new Dictionary<string, object>();
        matchUpdate.Add("mostRecentMatch", matchDisplayName);
        rootRef.Child("Users").Child("User" + user.UserId).UpdateChildrenAsync(matchUpdate);
    }
    public void updateMatchFindingStatus(string findStatus)
    {
        IDictionary<string, object> updates = new Dictionary<string, object>();
        updates.Add("isMatching", findStatus);
        rootRef.Child("Users").Child("User" + user.UserId).UpdateChildrenAsync(updates);
    }

    //returns true if user is finding match
    public async Task<Boolean> checkMatchFindingStatus()
    {
        Firebase.Database.DataSnapshot matchFindingStatus = await rootRef.Child("Users").Child("User" + user.UserId).Child("isMatching").GetValueAsync();      
        if(matchFindingStatus.GetRawJsonValue() == "\"1\"")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /*public async void cancelMatching()    
    {
        if (await checkMatchFindingStatus() == true)
        {
            updateMatchFindingStatus("0");
            await FirebaseDatabase.DefaultInstance.RootReference.Child("Rooms").Child(roomKey).RemoveValueAsync();
        }
        else
        {
        }
    }*/
    public async void cancelMatching()
    {

        if (await checkMatchFindingStatus() == true)
        {
            updateMatchFindingStatus("0");
            DataSnapshot roomSnapshot = await FirebaseDatabase.DefaultInstance.RootReference.Child("Rooms").OrderByChild("user1").EqualTo(user.UserId).LimitToFirst(1).GetValueAsync();
            foreach (DataSnapshot room in roomSnapshot.Children)
            {
                print(room.Key);
                if (room != null)
                {
                    await FirebaseDatabase.DefaultInstance.RootReference.Child("Rooms").Child(room.Key).RemoveValueAsync();
                }
            }

        }
    }
/*    public async void cancelMatchingWithoutDestroy()
    {
        if (await checkMatchFindingStatus() == true)
        {
            updateMatchFindingStatus("0");
            await FirebaseDatabase.DefaultInstance.RootReference.Child("Rooms").Child(roomKey).RemoveValueAsync();
        }
        else
        {
        }
    }*/

    public void matchScreenOff()
    {
        matchScreen.SetActive(false);
    }

    /*void OnApplicationQuit()
    {
        cancelMatching();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }*/

}
