using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
public class messageData : MonoBehaviour
{
    public string roomId;
    public string messageId;
    public void deleteMessage()
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("chatRooms").Child(roomId).Child(messageId).RemoveValueAsync();
    }

    public void dropDown(int value)
    {
        if (value == 2)
        {
            deleteMessage();
            Destroy(this);
        }
    }
}
