using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase;
using UnityEngine.UI;



public class miscellaneousClass : MonoBehaviour
{
    public Firebase.Database.Query ref1;
    //public DatabaseReference ref1;
    //public DatabaseReference ref2;
    public Firebase.Database.Query ref2;
    public string matchId;
    public string myId;
    public string mod;
    public Text t;


    public async void getLatestMessage()
    {
        Firebase.Database.DatabaseReference chatRef = FirebaseDatabase.DefaultInstance.RootReference.Child("chatRooms");
        string chatRoomKey;
        if (string.Compare(myId, matchId) < 0)
        {
            chatRoomKey = myId + matchId + mod;
        }
        else
        {
            chatRoomKey = matchId + myId + mod;
        }
        Firebase.Database.DataSnapshot chat = await chatRef.Child(chatRoomKey).GetValueAsync();
        Firebase.Database.DataSnapshot checkMessage = await chatRef.Child(chatRoomKey).OrderByKey().LimitToLast(1).GetValueAsync();
        if (!checkMessage.Exists)
        {
            t.text = " No messages";
        }
        if (chat.Exists)
        {
            ref1 = chatRef.Child(chatRoomKey).OrderByKey().LimitToLast(1);
            ref1.ChildAdded += HandleChildAdded;
        }

        
        this.ref2 = chatRef.Child(chatRoomKey);
        this.ref2.ChildChanged += HandleNewMessageForDisplay;
        

    }

    public void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        MessageClass messageData = JsonUtility.FromJson<MessageClass>(args.Snapshot.GetRawJsonValue());
        t.text = " " + messageData.messageText;
    }

    public void HandleNewMessageForDisplay(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        MessageClass messageData = JsonUtility.FromJson<MessageClass>(args.Snapshot.GetRawJsonValue());
        t.text = " " + messageData.messageText;

    }




}
