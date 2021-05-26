using System;
using System.Collections.Generic;
using UnityEngine;


class TempOpponent
{
    public string Username;
    public string Username2;
    public int ConnectionID;
    public int ConnectionID2;
    public int ShapeID, ShapeID12;              //Shape ID of the opponent
    public int ShapeID2, ShapeID22;             //Shape ID of first player in replays/spectate
    public int ShapeIDUser, ShapeID2User;       //Shape ID of the current user
    public int Probability;
    public int Probability1;
    public int Probability2;
    public int Probability21;
    public bool GotProb1 = false;
    public bool GotProb11 = false;
    public bool GotProb2 = false;
    public bool GotProb21 = false;
    public string[] AbLevelArray; 
    public string[] AbLevelArray2;
    public string[] Super100;
    public string[] Super1002;
    public string[] Super200;
    public string[] Super2002;
    public string Leaderboard;
    public string Friends;
    public string FriendsOnline;
    public string FriendsPP;
    public string FriendsShapeIDs;
    public bool Lbupdated = false;
    public bool FLupdated = false;
    public int[] AbIDs;
    public int[] AbIDs2;
    public int SuperID;
    public int SuperID2;
    public int OpPP;
    public int OpPP2;
    public int OpLvl;
    public int OpLvl2;
    public bool FriendlyBattle = false;
    public bool CheckDone = false;
    public bool Accepting = false;
    public int RoomNum = 0;
    public int[] Choices1;
    public int[] Choices2;
    public int[] Probs1;
    public int[] Probs2;
    public int[] Passives = new int[8];
    public int[] Passives2 = new int[8];
    public bool Replay = false;
    public int SkinID;
    public int SkinID2;
    public delegate void Method();
    public Dictionary<int, Method> Abilities = new Dictionary<int, Method>();
    public Dictionary<int, Method> Abilities12 = new Dictionary<int, Method>();
    public Dictionary<int, Method> Abilities2 = new Dictionary<int, Method>();
    public Dictionary<int, string> AbNames = new Dictionary<int, string>();
    public List<int> SpecAbs = new List<int>();
    public string FriendsLvl;
    public string FriendsInGame;
    public int BotRoomNum;

    //Reconnect Stuff
    public int LP1;
    public int LP2;
    public int EP1;
    public int EP2;
    public int Round;
    public int ChoiceID;

    //Profile
    public bool GotProfile = false;
    public bool changed;
    public string profile;

    public static TempOpponent Opponent = new TempOpponent();

    public void Reset(bool ReplayBool = false)
    {
        BotRoomNum = 0;
        LP1 = 0;
        LP2 = 0;
        EP1 = 0;
        EP2 = 0;
        Round = 0;
        ChoiceID = -1;
        SkinID = 0;
        SkinID2 = 0;
        Username = "";
        Username2 = "";
        ConnectionID = 0;
        ShapeID = 0;
        ShapeID12 = 0;
        ConnectionID2 = 0;
        ShapeID2 = 0;
        ShapeID22 = 0;
        ShapeIDUser = 0;
        Probability = 0;
        Probability1 = 0;
        Probability2 = 0;
        Probability21 = 0;
        GotProb1 = false;
        GotProb11 = false;
        GotProb2 = false;
        GotProb21 = false;
        GotProfile = false;
        SuperID = 0;
        Array.Clear(AbLevelArray, 0, AbLevelArray.Length);
        Array.Clear(Super100, 0, Super100.Length);
        Array.Clear(Super200, 0, Super200.Length);
        Array.Clear(Passives, 0, Passives.Length);
        Abilities.Clear();
        Abilities12.Clear();
        Abilities.Remove(0);
        Abilities12.Remove(0);
        if (!GameMaster.Spectate && !GameMaster.Replay && GameMaster.Online)
        {
            Array.Clear(AbIDs, 0, AbIDs.Length);
            Array.Clear(AbIDs2, 0, AbIDs2.Length);
            AbNames.Clear();
            SpecAbs.Clear();
        }
        OpPP = 0;
        OpPP2 = 0;
        OpLvl = 0;
        OpLvl2 = 0;
        if (GameMaster.Spectate)
        {
            Array.Clear(AbLevelArray2, 0, AbLevelArray.Length);
            Array.Clear(Super1002, 0, Super100.Length);
            Array.Clear(Super2002, 0, Super200.Length);
            Abilities2.Clear();
            Abilities2.Remove(0);
            Array.Clear(Passives2, 0, Passives.Length);
        }
        if (ReplayBool)
        {
            Array.Clear(Choices1, 0, Choices1.Length);
            Array.Clear(Choices2, 0, Choices2.Length);
            Array.Clear(Probs1, 0, Probs1.Length);
            Array.Clear(Probs2, 0, Probs2.Length);
            Array.Clear(AbLevelArray2, 0, AbLevelArray.Length);
            Array.Clear(Super1002, 0, Super100.Length);
            Array.Clear(Super2002, 0, Super200.Length);
            Abilities2.Clear();
            Abilities2.Remove(0);
            Replay = false;
        }
    }
}

