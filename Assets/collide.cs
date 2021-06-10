using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class collide : MonoBehaviour
{
    private bool triggered;
    [SerializeField]
    private GameObject winscreen;
/*    [SerializeField]
    private GameObject spinButton;*/
    private string dataURL = "https://peerup-1f6c7.firebaseio.com/";
    public Firebase.Auth.FirebaseUser user;
    public Firebase.Auth.FirebaseAuth auth;
    private int itemIndex;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (triggered)
        {
            Debug.Log("triggered: " + collision.transform.name);
            winscreen.SetActive(true);
            winscreen.transform.GetChild(0).GetComponent<Text>().text = "You won " + collision.transform.name + "! Come down with your buddy to claim two sets of this prize for both of you!\n\n*Note both partners are to be present to claim reward*";
 /*           spinButton.SetActive(false);*/
            itemIndex = collision.transform.GetSiblingIndex();
            Debug.Log(itemIndex);
            changeData();
            gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if(Stay.instance.timer >= 5f)
        {
            triggered = true;
        }
    }


    public void changeData()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        IDictionary<string, object> updates = new Dictionary<string, object>();
        updates.Add("reward", itemIndex.ToString());
        FirebaseDatabase.DefaultInstance.GetReferenceFromUrl(dataURL).Child("Users").Child("User" + user.UserId).UpdateChildrenAsync(updates)
             .ContinueWith((task => {
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
                     print("okay");
                 }

             }));


    }
}
