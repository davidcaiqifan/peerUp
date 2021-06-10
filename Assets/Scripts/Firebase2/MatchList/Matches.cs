using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;

public class Matches : MonoBehaviour
{
    public Firebase.Auth.FirebaseAuth auth;
    public Firebase.Auth.FirebaseUser user;
    public GameObject matchDisplayPrefab;
    Firebase.Database.DatabaseReference matchRef;
    Firebase.Database.DatabaseReference chatRef;
    Firebase.Database.DatabaseReference userRef;
    public List<GameObject> buttonArray;
    public GameObject canvas;
    public GameObject matchView;   
    public GameObject messagePanel;
    public GameObject content;
    public GameObject blank;
    public GameObject myMessage;
    public GameObject theirMessage;
    public List<GameObject> messageArray;
    public Text messageToBeSent;
    public string chatKey;
    public string matchId;
    public InputField input;
    public GameObject back;
    public GameObject chatView;
    DatabaseReference messageRef;
    DatabaseReference matchListRef;
    private object[] sprites;
    async void Start()
    {
        sprites = Resources.LoadAll("Avatars", typeof(Sprite));
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        chatRef = FirebaseDatabase.DefaultInstance.RootReference.Child("chatRooms");
        matchRef = FirebaseDatabase.DefaultInstance.RootReference.Child("Users").Child("User" + user.UserId).Child("matches");
        userRef = FirebaseDatabase.DefaultInstance.RootReference.Child("Users");
        await matchRef.GetValueAsync().ContinueWithOnMainThread((async task =>
                {
                    if (task.IsCanceled)
                    {
                        return;
                    }
                    if (task.IsFaulted)
                    {

                        return;
                    }
                    if (task.IsCompleted && task.Result.Exists)
                    {
                        DataSnapshot snapshot = task.Result;
                        string MatchesData = snapshot.GetRawJsonValue();                      
                        foreach(DataSnapshot i in snapshot.Children)
                        {
                            Player.Value matchInfo = JsonUtility.FromJson<Player.Value>(i.GetRawJsonValue());                           
                            GameObject match = (GameObject)Instantiate(matchDisplayPrefab, matchView.transform);
                            match.AddComponent<miscellaneousClass>();                           
                            buttonArray.Add(match);
                            Firebase.Database.DataSnapshot matchUserInfo = await userRef.Child("User" + matchInfo.match).GetValueAsync();                    
                            Player matchedUser = JsonUtility.FromJson<Player>(matchUserInfo.GetRawJsonValue());
                            //print(matchedUser.displayName);
                            match.gameObject.name = matchedUser.userId + matchInfo.mod;
                            match.GetComponentInChildren<Text>().text = " " + matchedUser.displayName + "  | " + matchInfo.mod;
                            match.GetComponent<Button>().onClick.AddListener(() => TaskOnClick(matchInfo.match, matchInfo.mod));
                            Button[] matchViewButtons = match.GetComponentsInChildren<Button>();
                            foreach(Button b in matchViewButtons)
                            {
                                if(b.gameObject.name == "UnmatchButton")
                                {
                                    print("UnmatchButton");
                                    b.onClick.AddListener(() => unmatchPanel(matchInfo.match + matchInfo.mod, matchInfo.match, user.UserId + matchInfo.mod));
                                }
                            }
                            //match.GetComponentInChildren<Button>().onClick.AddListener(() => unmatchPanel(matchInfo.match + matchInfo.mod));
                            int avatarId = int.Parse(matchedUser.avatarID);
                            //print(avatarId);
                            Image[] imageDisplay = match.GetComponentsInChildren<Image>();
                            foreach (Image im  in imageDisplay)
                            {
                                if (im.gameObject.name == "ImageDisplay")
                                {
                                    im.sprite = (Sprite)sprites[avatarId];
                                }
                            }
                            //match.GetComponentInChildren<Image>().sprite = (Sprite)sprites[avatarId];
                            Text[] matchDisplay = match.GetComponentsInChildren<Text>();
                            foreach (Text m in matchDisplay)
                            {
                                if (m.gameObject.name == "NewestText")
                                {
                                    match.GetComponent<miscellaneousClass>().matchId = matchInfo.match;
                                    match.GetComponent<miscellaneousClass>().mod = matchInfo.mod;
                                    match.GetComponent<miscellaneousClass>().t = m;
                                    match.GetComponent<miscellaneousClass>().myId = user.UserId;
                                    match.GetComponent<miscellaneousClass>().getLatestMessage();
                                    //getLatestMessage(matchInfo.match, matchInfo.mod, m, match);
                                }
                            }
                        }
                    }
                }));
        matchListRef = FirebaseDatabase.DefaultInstance
                    .GetReference("Users").Child("User" + user.UserId).Child("matches");
        matchListRef.ChildRemoved += HandleMatchListChanged;
        
    }

    void HandleMatchListChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        print("childRemoved!");
        //Do something with the data in args.Snapshot          
        string matchKey = args.Snapshot.Key;
        print(matchKey);     
        foreach (GameObject g in buttonArray)
        {
            //print(g.gameObject.name);
            if (g.gameObject.name ==  matchKey)
            {
                GameObject buttonObject = g;
                if (buttonObject.GetComponent<miscellaneousClass>().ref1 != null)
                {
                    buttonObject.GetComponent<miscellaneousClass>().ref1.ChildAdded -= buttonObject.GetComponent<miscellaneousClass>().HandleChildAdded;
                }
                buttonObject.GetComponent<miscellaneousClass>().ref2.ChildChanged -= buttonObject.GetComponent<miscellaneousClass>().HandleNewMessageForDisplay;
                matchListRef.ChildRemoved -= HandleMatchListChanged;
                buttonArray.Remove(buttonObject);
                Destroy(buttonObject);
                break;
            }
        }
    }


    public GameObject uPanel;

    public void unmatchPanel(string matchNumber, string matchId, string otherMatchNumber)
    {
        print("voila");
        matchView.SetActive(false);
        uPanel.SetActive(true);
        Button[] yesAndNo = uPanel.GetComponentsInChildren<Button>();
        foreach(Button b in yesAndNo)
        {
            if(b.gameObject.name == "Yes")
            {
                b.onClick.AddListener(() => unmatch(matchNumber, matchId, otherMatchNumber));              
            }
            else
            {
                b.onClick.AddListener(() =>
                {
                    matchView.SetActive(true);
                    uPanel.SetActive(false);
                }             
                );
            }
        }
    }

    async void getLatestMessage(string matchId, string mod, Text t, GameObject matchDisplayObject)
    {
        string chatRoomKey;
        if (string.Compare(user.UserId, matchId) < 0)
        {
            chatRoomKey = user.UserId + matchId + mod;
        }
        else
        {
            chatRoomKey = matchId + user.UserId + mod;
        }
        Firebase.Database.DataSnapshot chat = await chatRef.Child(chatRoomKey).GetValueAsync();
        Firebase.Database.DataSnapshot checkMessage = await chatRef.Child(chatRoomKey).OrderByKey().LimitToLast(1).GetValueAsync();
        if (!checkMessage.Exists)
        {
            t.text = " No messages";
        }
        if (chat.Exists)
        {
            matchDisplayObject.GetComponent<miscellaneousClass>().ref1 = chatRef.Child(chatRoomKey).OrderByKey().LimitToLast(1);
            matchDisplayObject.GetComponent<miscellaneousClass>().ref1.ChildAdded += HandleChildAdded;           
        }

        void HandleChildAdded(object sender, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            MessageClass messageData = JsonUtility.FromJson<MessageClass>(args.Snapshot.GetRawJsonValue());
            t.text = " " + messageData.messageText;
        }
        matchDisplayObject.GetComponent<miscellaneousClass>().ref2 = chatRef.Child(chatRoomKey);
        matchDisplayObject.GetComponent<miscellaneousClass>().ref2.ChildChanged += HandleNewMessageForDisplay;
        void HandleNewMessageForDisplay(object sender, ChildChangedEventArgs args)
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

    

    async void TaskOnClick(string matchId, string mod)
    {
        back.SetActive(false);
        this.matchId = matchId;
        foreach (GameObject button in buttonArray)
        {
            button.SetActive(false);
        }
        string chatRoomKey;        
        if (string.Compare(user.UserId, matchId) < 0)
        {
            chatRoomKey = user.UserId + matchId + mod;
        }
        else
        {
            chatRoomKey = matchId + user.UserId + mod;
        }
        chatKey = chatRoomKey;
        Firebase.Database.DataSnapshot chat = await chatRef.OrderByKey().EqualTo(chatRoomKey).GetValueAsync();
        if (chat.Exists)
        {
            chat = await chatRef.OrderByKey().EqualTo(chatRoomKey).GetValueAsync();
            generateMessages(chat);
        }
        else
        {
            IDictionary<string, object> newRoom = new Dictionary<string, object>();
            newRoom.Add(chatRoomKey, "");
            await chatRef.UpdateChildrenAsync(newRoom);
        }

        messagePanel.SetActive(true);
        messageRef = FirebaseDatabase.DefaultInstance
            .GetReference("chatRooms").Child(chatRoomKey);
        messageRef.ChildChanged += HandleNewMessage;
    }

    public async void sendMessage()
    {
        string message = messageToBeSent.text;
        input.text = "";
        string timeKey = chatRef.Child(chatKey).Push().Key;
        string timestamp = DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString();
        IDictionary<string, object> setup = new Dictionary<string, object>();
        setup.Add("messageText", "");
        setup.Add("sender", "");
        setup.Add("receiver", "");
        setup.Add("timestamp", "");
        await chatRef.Child(chatKey).Child(timeKey).UpdateChildrenAsync(setup);
        IDictionary<string, object> sub = new Dictionary<string, object>();
        sub.Add("messageText", message);
        sub.Add("sender", user.UserId);
        sub.Add("receiver", matchId);
        sub.Add("timestamp", timestamp);
        await chatRef.Child(chatKey).Child(timeKey).UpdateChildrenAsync(sub);
    }
 
    void generateMessages(Firebase.Database.DataSnapshot data)
    {
        print(data.GetRawJsonValue());
      
        GameObject blankMessage = (GameObject)Instantiate(blank, new Vector2(0, 240), Quaternion.identity);
        //messageArray.Add(blankMessage);       
        foreach (Firebase.Database.DataSnapshot child in data.Children)
        {
            //string messagestuff = child.Children.ToString();
            foreach (Firebase.Database.DataSnapshot messageChild in child.Children)
            {     
                MessageClass messageData = JsonUtility.FromJson<MessageClass>(messageChild.GetRawJsonValue());
                GameObject message;
                if (messageData.sender == user.UserId)
                {                
                    message = (GameObject)Instantiate(myMessage, content.transform);
                    
                }
                else
                {
                    message = (GameObject)Instantiate(theirMessage, content.transform);                 
                }
                message.AddComponent<messageData>();
                message.GetComponentInChildren<Text>().text = messageData.messageText;
                var mD = message.GetComponentInChildren<messageData>();
                mD.messageId = messageChild.Key;           
                messageArray.Add(message);
            }
        }
        chatView.GetComponent<ScrollRect>().velocity = new Vector2(0f, 5000f);
    }


    void HandleNewMessage(object sender, ChildChangedEventArgs args)
          {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            print("new message" + args.Snapshot.GetRawJsonValue());
            //Do something with the data in args.Snapshot
            if (args.Snapshot.ChildrenCount < 1)
            {

            }
            else {  
                MessageClass messageData = JsonUtility.FromJson<MessageClass>(args.Snapshot.GetRawJsonValue());
                GameObject message;
                if (messageData.sender == user.UserId)
                {
                    message = (GameObject)Instantiate(myMessage, content.transform);

                }
                else
                {
                    message = (GameObject)Instantiate(theirMessage, content.transform);
                }
                message.AddComponent<messageData>();
                message.GetComponentInChildren<Text>().text = messageData.messageText;
                var mD = message.GetComponentInChildren<messageData>();
                mD.messageId = args.Snapshot.Key;
                messageArray.Add(message);
                chatView.GetComponent<ScrollRect>().velocity = new Vector2(0f, 1000f);
            }
        }
        
    public void backButton()
    {
        back.SetActive(true);
        messagePanel.SetActive(false);
        foreach (GameObject button in buttonArray)
        {
            button.SetActive(true);
        }
        foreach(GameObject child in messageArray)
        {
            Destroy(child);
        }
        messageRef.ChildChanged -= HandleNewMessage;
    }

    public async void unmatch(string matchNumber,string matchId, string otherMatchNumber)
    {
        matchView.SetActive(true);
        uPanel.SetActive(false);
        await matchRef.Child(matchNumber).RemoveValueAsync();
        await FirebaseDatabase.DefaultInstance.RootReference.Child("Users").Child("User" + matchId).Child("matches").Child(otherMatchNumber).RemoveValueAsync(); 
    }
        

}
