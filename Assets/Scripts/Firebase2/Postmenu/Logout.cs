using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;

public class Logout : MonoBehaviour
{
    public void signOutUser()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        print("Successfully Signed Out");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
