using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class Player
{
    public string userId;
    public string studentNumber;
    public string password;
    public string displayName;
    public string course;
    public string description;
    public string selectedMod;
    public string matchStatus;
    public string isMatching;
    public string avatarID;
   // public Matches matches;
    public string mostRecentMatch;
    public string reward;
    [Serializable]
    public class Value
    {
        public string match;
        public string mod;
        public Value()
        {
            this.match = "";
            this.mod = "";
        }
    }

    [Serializable]
    public class Matches
    {
        
        public Value match1;
        public Value match2;
        public Value match3;
        public Value match4;
        public Value match5;
        public Matches()
        {
            match1 = new Value();
            match2 = new Value();
            match3 = new Value();
            match4 = new Value();
            match5 = new Value();
        }
    }
    
    public Player() { }
    public Player(string name, string uId)
    {
        studentNumber = name;
        this.userId = uId;
        displayName = "";
        this.course = "";
        this.description = "";
        this.matchStatus = "0";
        this.isMatching = "0";
        this.selectedMod = "";
        //this.matches = new Matches();
        this.avatarID = "0";
        this.mostRecentMatch = "";
        this.reward = "";
    }

    public Player(string name, string displayName, string course, string description)
    {
        studentNumber = name;
        displayName = name;
        this.course = course;
        this.description = description;
    }
}
