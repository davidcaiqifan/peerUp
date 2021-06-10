using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]

public class MatchRoom
{
    public string user1;
    public string user2;
    public string mod;
    public string pax;
    public MatchRoom() {}
    public MatchRoom(string user, string mod)
    {
        this.user1 = user;
        this.mod = mod;
        this.pax = "1";
    }
    

}
