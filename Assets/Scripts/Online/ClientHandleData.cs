using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ClientHandleData
{
    private static ByteBuffer PlayerBuffer;
    public delegate void PacketF(byte[] Data);
    public static Dictionary<int, PacketF> PacketListener;
    private static int pLength;
    public static string ReconnectString;
    private static System.Random R = new System.Random();
    public static int GameVersion = 4; // Needs to be updated everytime we update the Game.

    public static void InitializePacketListener()
    {
        PacketListener = new Dictionary<int, PacketF>
        {
            { (int)ServerPackages.SMsg, HandleMessage },
            { (int)ServerPackages.SLogin, HandleLogin },
            { (int)ServerPackages.SSendToBattle, HandleGoingToBattle },
            { (int)ServerPackages.SGiveProbability, HandleGettingProbability },
            { (int)ServerPackages.SGetChoice, HandleGivingChoice },
            { (int)ServerPackages.SSendChoice, HandleReceivingChoice },
            { (int)ServerPackages.SEndgame, HandleEndgame },
            { (int)ServerPackages.SLeaderboard, HandleGettingLeaderboard },
            { (int)ServerPackages.SDisconnect, HandleDisconnect },
            { (int)ServerPackages.SGiveAbilities, HandleReceivingAbilities },
            { (int)ServerPackages.SGiveStats, HandleReceivingStats },
            { (int)ServerPackages.SEmotes, HandleEmotes },
            { (int)ServerPackages.SGiveList, HandleFriendsList },
            { (int)ServerPackages.SFriendly, HandleFriendly },
            { (int)ServerPackages.SSpectate, HandleSpectate },
            { (int)ServerPackages.SLP, HandleLP },
            { (int)ServerPackages.SChest, HandleChest },
            { (int)ServerPackages.SReplay, HandleReplay },
            { (int)ServerPackages.SReconnect, HandleReconnecting },
            { (int)ServerPackages.SProfile, HandleProfile }
        };
    }

    public static void HandleData(byte[] data)
    {
        string s = "", s2 = "";
        foreach (byte bb in data) { s = s + " " + bb.ToString(); s2 = s2 + " " + (char)bb; }
        Debug.Log(s + " or " + s2);
        if (data == null) { return; }

        int pLength = 0;    //Packet length
        byte[] buffer = (byte[])data.Clone();   //To avoid shallow copies

        if (PlayerBuffer == null) { PlayerBuffer = new ByteBuffer(); }

        PlayerBuffer.WriteBytes(buffer);
        if (PlayerBuffer.Length() < 4)  //Considers previously received data
        {
            Debug.Log("Buffer is too empty");
            PlayerBuffer.Clear();
            return;
        }

        pLength = PlayerBuffer.ReadInteger(false);      //Doesn't advance before all the packet is here
        while (pLength >= 4 && pLength <= PlayerBuffer.Length() - 4)
        {
            PlayerBuffer.ReadInteger();
            int packageID = PlayerBuffer.ReadInteger();
            pLength -= 4;
            data = PlayerBuffer.ReadBytes(pLength);

            Debug.Log("pLength = " + pLength + "   packageID = " + packageID);
            if (PacketListener.TryGetValue(packageID, out PacketF packet))
                packet.Invoke(data);
            else { Debug.Log("Wrong function ID"); break; }

            pLength = 0;
            if (PlayerBuffer.Length() >= 4)
                pLength = PlayerBuffer.ReadInteger(false);
        }

        if (pLength < 4) { PlayerBuffer.Clear(); }
    }
    private static void HandleMessage(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        string msg = buffer.ReadString();
        if (msg == "Yes")           //In my friend list
        {
            TempOpponent.Opponent.FriendlyBattle = true;
            TempOpponent.Opponent.CheckDone = true;
        }
        else if (msg == "No")       //Not in my friend list
        {
            TempOpponent.Opponent.FriendlyBattle = false;
            TempOpponent.Opponent.CheckDone = true;
        }
        else if (msg == "UN")       //Friend currently in-game or not online
        {
            GameObject.Find("Menu Manager").GetComponent<ValuesChange>().ShowMessage("Your friend is unavailable right now");
        }
        else if (msg == "BRD")      //Battle Request Denied
        {
            if (SceneManager.GetActiveScene().name == "MainMenuScene")
            {
                GameObject.Find("SelectionManager").GetComponent<SelectionManager>().rejectedFriendly();
            }
        }
        else if (msg == "LifePoints")
        {
            int LP1 = GameObject.Find("Player1").GetComponent<Shape_Player>().GetLife();
            int LP2 = GameObject.Find("Player2").GetComponent<Shape_Player>().GetLife();
            int ConnID = buffer.ReadInteger();
            ClientTCP.PACKAGE_LifePoints(LP1, LP2, ConnID,buffer.ReadInteger());
        }
        else if (msg == "Taken")
        {
            GameObject.Find("LoginPanel").GetComponent<Login>().taken();
        }
        else if (msg == "Leave")
        {
            SceneManager.LoadScene("MainMenuScene");
        }
        else if (msg == "Leave1")
        {
            if (SceneManager.GetActiveScene().name == "MainMenuScene")
            {
                GameObject Panel = GameObject.Find("Priority Messages").transform.Find("FriendlyBattleRequest").gameObject;
                if (Panel.activeSelf) { Panel.SetActive(false); }
                else if (TempOpponent.Opponent.Accepting)
                {
                    TempOpponent.Opponent.Accepting = false;
                    TempOpponent.Opponent.FriendlyBattle = false;
                    if (ValuesChange.SwitchScenes.TryGetValue("MainMenu", out UnityEngine.Events.UnityEvent Function))
                    {
                        Function.Invoke();
                    }
                }
                TempOpponent.Opponent.Accepting = false;
                TempOpponent.Opponent.FriendlyBattle = false;
                //Debug.Log("Friendly Battle Canceled");
            }
        }
        else if (msg == "Reconnect")
        {
            int OtherConnID = buffer.ReadInteger();
            if (!GameMaster.StopEvaluating)
            {
                ClientTCP.PACKAGE_Reconnect(OtherConnID);
                GameMaster.Disconnected = false;
            }
            else
            {
                GameMasterOnline GM = GameObject.Find("Game Manager").GetComponent<GameMasterOnline>();
                GM.ReconnectFunction(OtherConnID);
            }
        }
        else if (msg == "D")        //Other player disconnected
        {
            GameMaster.Disconnected = true;
            GameMaster.DisconnectedFirstTime = true;
            Shape_Player Player2 = GameObject.Find("Player2").GetComponent<Shape_Player>();
            if (!Player2.GetChoiceDone())
            {
                if (TempOpponent.Opponent.Abilities.TryGetValue(0, out TempOpponent.Method method))
                    method.Invoke();
            }
        }
        else if(msg == "D2")
        {
            
            GameObject.Find("Game Manager").GetComponent<GameMasterOnline>().Disc.SetActive(true);
            buffer.Dispose();
            return;
        }
        else if (msg == "Connect")
        {
            ClientTCP.Reconnecting = true;
            ClientTCP.InitializeClientSocket(ClientManager.serverIP, ClientManager.serverPort);
        }
        else if (msg == "CheckIP")
        {
            ClientTCP.PACKAGE_DEBUG("CheckIP", buffer.ReadString() + "/" + PlayerPrefs.GetString("Username"));
        }
        else if (msg == "OC")       //Online Check
        {
            ClientTCP.PACKAGE_DEBUG(msg, "", buffer.ReadInteger());
        }
        else if(msg == "SS")        //Stop Searching
        {
            if(SceneManager.GetActiveScene().name == "MainMenuScene" && GameObject.Find("SelectionManager"))
            {
                GameObject.Find("SelectionManager").GetComponent<SelectionManager>().Cancel.GetComponent<CancelSearch>().Cancel();
            }
        }
        else if(msg == "Done")
        {
            MainMenuController.JustRegistered = false;
            GameObject.Find("MainMenuController").GetComponent<MainMenuController>().Set();
        }
        else if (msg == "FAA") //Friend already added
        {
            AddFriend go = GameObject.Find("AddFriend").GetComponent<AddFriend>();
            go.added.gameObject.SetActive(false);
            go.invalid.gameObject.SetActive(true);
            go.invalid.GetComponent<Text>().text = "Friend Already Added";
        }
        else if (msg == "IC")   //Invalid friend Code
        {
            AddFriend go = GameObject.Find("AddFriend").GetComponent<AddFriend>();
            go.added.gameObject.SetActive(false);
            go.invalid.gameObject.SetActive(true);
            go.invalid.GetComponent<Text>().text = "Invalid Code";
        }
        else if (msg == "FA") //Friend added
        {
            GameObject.Find("AddFriend").GetComponent<AddFriend>().Added();
        }
        else if (msg == "NIG") //Friend not in game while we tried to spectate
        {
            GameObject.Find("Menu Manager").GetComponent<ValuesChange>().ShowMessage("Your friend is not in a game");
        }
        else if(msg == "ReconnectBot")
        {
            TempOpponent.Opponent.BotRoomNum = buffer.ReadInteger();
            ReconnectString = buffer.ReadString();
            string[] ReconnectStringBot = ReconnectString.Split(',');
            TempOpponent.Opponent.Username = ReconnectStringBot[75];
            TempOpponent.Opponent.AbLevelArray = ReconnectStringBot[77].Split('|');
            TempOpponent.Opponent.Super100 = PlayerPrefsX.GetStringArray("Super100Array");
            TempOpponent.Opponent.Super200 = PlayerPrefsX.GetStringArray("Super200Array");
            Int32.TryParse(ReconnectStringBot[76],out TempOpponent.Opponent.ShapeID);
            TempOpponent.Opponent.Passives = PlayerPrefsX.GetIntArray("PassivesArray");
            Int32.TryParse(ReconnectStringBot[78], out TempOpponent.Opponent.OpPP);
            TempOpponent.Opponent.OpLvl = PlayerPrefsX.GetIntArray("Level")[PlayerPrefs.GetInt("ShapeSelectedID")];
            TempOpponent.Opponent.Probs1 = SArrayToIArray(ReconnectStringBot[73].Split('/'));
            TempOpponent.Opponent.Probs2 = SArrayToIArray(ReconnectStringBot[74].Split('/'));
            TempOpponent.Opponent.Choices1 = SArrayToIArray(ReconnectStringBot[71].Split('/'));
            TempOpponent.Opponent.Choices2 = SArrayToIArray(ReconnectStringBot[72].Split('/'));
            PlayerPrefs.SetInt("BotOnline", 1);
            Int32.TryParse(ReconnectStringBot[68], out TempOpponent.Opponent.LP1);
            Int32.TryParse(ReconnectStringBot[69], out TempOpponent.Opponent.LP2);
            Int32.TryParse(ReconnectStringBot[70], out TempOpponent.Opponent.Round);
            GameMaster.Reconnect = true;
            SceneManager.LoadScene("PassThePhone");
            return;
        }
        else if(msg == "WrongSecretCode")
        {
            GameObject.Find("LoginPanel").GetComponent<Login>().WrongCode();
        }
        else if(msg == "DN")
        {
            PlayerPrefs.SetInt("DN", 1);
        }
        else if(msg == "ABU")
        {
            Debug.LogError("???");
            GameObject panel;
            if (panel = GameObject.Find("Canvas"))
            {
                panel.transform.Find("ABU").gameObject.SetActive(true);
            }
        }
        buffer.Dispose();
    }
    private static void HandleLogin(byte[] data)
    {
        if (GameObject.Find("LoginPanel")) GameObject.Find("LoginPanel").GetComponent<Login>().accepted();
        int i = 0;
        bool firsttime = true;
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        string Username = buffer.ReadString();
        int Gold = buffer.ReadInteger();
        string Level = buffer.ReadString();
        string[] Lvl1 = Level.Split(',');
        int[] LevelArray = new int[Lvl1.Length];
        foreach (string c in Lvl1)
        {
            if (firsttime)
            {
                i = 0;
                firsttime = false;
            }
            Int32.TryParse(c, out LevelArray[i]);
            i++;
        }
        firsttime = true;
        string XP = buffer.ReadString();
        string[] XP1 = XP.Split(',');
        int[] XPArray = new int[XP1.Length];
        foreach (string c in XP1)
        {
            if (firsttime)
            {
                i = 0;
                firsttime = false;
            }
            Int32.TryParse(c, out XPArray[i]);
            i++;
        }
        firsttime = true;
        string PP = buffer.ReadString();
        string[] PP1 = PP.Split(',');
        int[] PPArray = new int[PP1.Length];
        int AllPP = 0;
        foreach (string c in PP1)
        {
            if (firsttime)
            {
                i = 0;
                firsttime = false;
            }
            Int32.TryParse(c, out PPArray[i]);
            AllPP += PPArray[i];
            i++;
        }
        CloudOnce.Leaderboards.AllLb.SubmitScore(AllPP);
        firsttime = true;
        string Abilities = buffer.ReadString();
        string[] AbilitiesArray = Abilities.Split(',');
        string Super100 = buffer.ReadString();
        string[] Super100Array = Super100.Split(',');
        string Super200 = buffer.ReadString();
        string[] Super200Array = Super200.Split(',');
        string Stats = buffer.ReadString();
        string[] StatsArraytemp = Stats.Split('-');
        string Super100Stats = buffer.ReadString();
        string[] Super100StatsArraytemp = Super100Stats.Split('-');
        string Super200Stats = buffer.ReadString();
        string[] Super200StatsArraytemp = Super200Stats.Split('-');
        string Shop = buffer.ReadString();
        string ShapePriceStr = buffer.ReadString();
        string[] ShapePriceS = ShapePriceStr.Split(',');
        int[] ShapePrice = new int[ShapePriceS.Length];
        i = 0;
        foreach (string c in ShapePriceS)
        {
            Int32.TryParse(c, out ShapePrice[i]);
            i++;
        }
        string ChestSlotStr = buffer.ReadString();
        string[] ChestSlotPricesStr = ChestSlotStr.Split(',');
        int[] ChestSlotPrices = new int[ChestSlotPricesStr.Length];
        i = 0;
        foreach (string c in ChestSlotPricesStr)
        {
            Int32.TryParse(c, out ChestSlotPrices[i]);
            i++;
        }
        string Code = buffer.ReadString();
        i = 0;
        string Pas = buffer.ReadString();
        string[] PassivesStr = Pas.Split(',');
        int[] Passives = new int[PassivesStr.Length];
        foreach (string c in PassivesStr)
        {
            Int32.TryParse(c, out Passives[i]);
            i++;
        }
        /*string Replay = buffer.ReadString();
        string[] Replays = Replay.Split('|');
        string[] ReplayNew;
        if (Replays[0] != "N")
        {
            string[] Ar = Replays[0].Split('/');
            ReplayNew = new string[Ar.Length * Replays.Length];
            i = 0;
            foreach (string c in Replays)
            {
                string[] d = c.Split('/');
                foreach (string e in d)
                {
                    ReplayNew[i] = e;
                    i++;
                }
            }
        }
        else
        {
            ReplayNew = new string[] { "N" };
        }*/
        int Winstreak = buffer.ReadInteger();
        string IDs = buffer.ReadString();
        int[] RecentIDs;
        if (IDs == "N")
        {
            RecentIDs = PlayerPrefsX.GetIntArray("RecentIDs");
            if (RecentIDs.Length > 0 && RecentIDs[0] != -1) {
                ChestCode.OldRecentShapeIDs = PlayerPrefsX.GetIntArray("RecentIDs");
                ChestCode.OldWS = PlayerPrefs.GetInt("Winstreak");
                ChestCode.OpenIt = true;
            }
        }
        else
        {
            string[] RecentIDsStr = IDs.Split(',');
            RecentIDs = new int[RecentIDsStr.Length];
            i = 0;
            foreach (string d in RecentIDsStr)
            {
                Int32.TryParse(d, out RecentIDs[i]);
                i++;
            }
        }
        string ChestSlots = buffer.ReadString();
        string[] Slots = ChestSlots.Split(',');
        int[] SlotsAr = new int[Slots.Length];
        i = 0;
        foreach (string c in Slots)
        {
            Int32.TryParse(c, out SlotsAr[i]);
            i++;
        }
        string LevelStats = buffer.ReadString();
        int Redbolts = buffer.ReadInteger();
        int Diamonds = buffer.ReadInteger();
        string AbilitiesRarety = buffer.ReadString();
        string[] AbRarety = AbilitiesRarety.Split(',');
        int[] RaretyAr = new int[AbRarety.Length];
        i = 0;
        foreach (string c in AbRarety)
        {
            Int32.TryParse(c, out RaretyAr[i]);
            i++;
        }
        string Bg = buffer.ReadString();
        string[] BackgroundsStr = Bg.Split(',');
        int[] Backgrounds = new int[BackgroundsStr.Length];
        i = 0;
        foreach (string s in BackgroundsStr)
        {
            Int32.TryParse(s, out Backgrounds[i]);
            i++;
        }
        string PassiveStats = buffer.ReadString();
        string[] PassiveStatsArray = PassiveStats.Split('-');
        string PassivesShop = buffer.ReadString();
        string BgPrice = buffer.ReadString();
        string[] BgPriceStr = BgPrice.Split(',');
        int[] BgPriceAr = new int[BgPriceStr.Length];
        i = 0;
        foreach (string s in BgPriceStr)
        {
            Int32.TryParse(s, out BgPriceAr[i]);
            i++;
        }
        string EmotesPrice = buffer.ReadString();
        string[] EmotesPriceStr = EmotesPrice.Split(',');
        int[] EmotesPriceAr = new int[EmotesPriceStr.Length];
        i = 0;
        foreach (string s in EmotesPriceStr)
        {
            Int32.TryParse(s, out EmotesPriceAr[i]);
            i++;
        }
        string SkinsPrice = buffer.ReadString();
        string[] SkinsPriceStr = SkinsPrice.Split(',');
        int[] SkinsPriceAr = new int[SkinsPriceStr.Length];
        i = 0;
        foreach (string s in SkinsPriceStr)
        {
            Int32.TryParse(s, out SkinsPriceAr[i]);
            i++;
        }
        string CoinPrice = buffer.ReadString();
        string[] CoinPriceStr = CoinPrice.Split(',');
        int[] CoinPriceAr = new int[CoinPriceStr.Length];
        i = 0;
        foreach (string s in CoinPriceStr)
        {
            Int32.TryParse(s, out CoinPriceAr[i]);
            i++;
        }
        string CoinReward = buffer.ReadString();
        string[] CoinRewardStr = CoinReward.Split(',');
        int[] CoinRewardAr = new int[CoinRewardStr.Length];
        i = 0;
        foreach (string s in CoinRewardStr)
        {
            Int32.TryParse(s, out CoinRewardAr[i]);
            i++;
        }
        string Super100Shop = buffer.ReadString();
        string Super200Shop = buffer.ReadString();
        string SkinsUnlocked = buffer.ReadString();
        string[] SkinsUnlockedStr = SkinsUnlocked.Split(',');
        int[] SkinsUnlockedAr = new int[SkinsUnlockedStr.Length];
        i = 0;
        foreach (string s in SkinsUnlockedStr)
        {
            Int32.TryParse(s, out SkinsUnlockedAr[i]);
            i++;
        }
        string EmotesUnlocked = buffer.ReadString();
        string[] EmotesUnlockedStr = EmotesUnlocked.Split(',');
        int[] EmotesUnlockedAr = new int[EmotesUnlockedStr.Length];
        i = 0;
        foreach (string s in EmotesUnlockedStr)
        {
            Int32.TryParse(s, out EmotesUnlockedAr[i]);
            i++;
        }
        string TrophyRoad = buffer.ReadString();
        string TrophyRoadUnlocked = buffer.ReadString();
        string[] TrophyRoadUnlockedStr = TrophyRoadUnlocked.Split('-');
        string maxPPstr = buffer.ReadString();
        string[] maxPPstrAr = maxPPstr.Split(',');
        int[] maxPP = new int[maxPPstrAr.Length];
        i = 0;
        foreach(string s in maxPPstrAr)
        {
            Int32.TryParse(s, out maxPP[i]);
            i++;
        }
        string AdsRemStr = buffer.ReadString();
        string[] Dec = AdsRemStr.Split('/');
        Int32.TryParse(Dec[0], out int AdTimeBeforeReset);
        string[] AdsRemStrAr = Dec[1].Split(',');
        int[] AdsRem = new int[AdsRemStrAr.Length];
        i = 0;
        foreach (string s in AdsRemStrAr)
        {
            Int32.TryParse(s, out AdsRem[i]);
            i++;
        }
        string SecretCode = buffer.ReadString();
        MainMenuController.JustRegistered = true;
        i = 0;
        foreach(int j in LevelArray)
        {
            if (j > 0)
            {
                MainMenuController.JustRegistered = false;
                if (LevelArray[PlayerPrefs.GetInt("ShapeSelectedID")] < 1)
                    PlayerPrefs.SetInt("ShapeSelectedID", i);
                break;
            }
            i++;
        }
        int CDays = buffer.ReadInteger();
        int rew = buffer.ReadInteger();
        string[] addrewstatsstr = buffer.ReadString().Split(',');
        int[] addrewstats = new int[addrewstatsstr.Length];
        i = 0;
        foreach(string s in addrewstatsstr)
        {
            Int32.TryParse(s, out addrewstats[i]);
            i++;
        }
        string[] addrewstr = buffer.ReadString().Split(',');
        int[] addrew = new int[addrewstr.Length];
        i = 0;
        foreach (string s in addrewstr)
        {
            Int32.TryParse(s, out addrew[i]);
            i++;
        }
        if (GameVersion != buffer.ReadInteger())
        {
            GameObject.Find("ClientManager").GetComponent<ClientManager>().UpdatePanel.SetActive(true);
            return;
        }
        SelectionManager.online = true;
        PlayerPrefsX.SetIntArray("AdditionalRewardsStats", addrewstats);
        PlayerPrefsX.SetIntArray("AdditionalRewards", addrew);
        PlayerPrefs.SetInt("ConsDays", CDays);
        PlayerPrefs.SetInt("DailyReward", rew);
        PlayerPrefsX.SetIntArray("ChestSlots", SlotsAr);
        PlayerPrefsX.SetIntArray("RecentIDs", RecentIDs);
        PlayerPrefs.SetInt("Winstreak", Winstreak);
        PlayerPrefs.SetInt("Updated", 1);
        PlayerPrefs.SetInt("Gold", Gold);
        PlayerPrefs.SetInt("Redbolts", Redbolts);
        PlayerPrefs.SetInt("Diamonds", Diamonds);
        //PlayerPrefsX.SetStringArray("Replay", ReplayNew);
        PlayerPrefsX.SetIntArray("Level", LevelArray);
        PlayerPrefsX.SetIntArray("XP", XPArray);
        PlayerPrefs.SetString("Username", Username);
        PlayerPrefs.SetString("Code", Code);
        PlayerPrefs.SetString("Shop", Shop);
        PlayerPrefs.SetString("PassivesShop", PassivesShop);
        PlayerPrefs.SetString("Super100Shop", Super100Shop);
        PlayerPrefs.SetString("Super200Shop", Super200Shop);
        PlayerPrefs.SetString("TrophyRoad", TrophyRoad);
        PlayerPrefsX.SetIntArray("ShapePrice", ShapePrice);
        PlayerPrefsX.SetIntArray("ChestSlotPrices", ChestSlotPrices);
        PlayerPrefsX.SetIntArray("BgPriceAr", BgPriceAr);
        PlayerPrefsX.SetIntArray("EmotesPriceAr", EmotesPriceAr);
        PlayerPrefsX.SetIntArray("SkinsPriceAr", SkinsPriceAr);
        PlayerPrefsX.SetIntArray("CoinPriceAr", CoinPriceAr);
        PlayerPrefsX.SetIntArray("CoinRewardAr", CoinRewardAr);
        PlayerPrefsX.SetIntArray("PP", PPArray);
        PlayerPrefsX.SetStringArray("AbilitiesArray", AbilitiesArray);
        PlayerPrefsX.SetStringArray("Super100Array", Super100Array);
        PlayerPrefsX.SetStringArray("Super200Array", Super200Array);
        PlayerPrefsX.SetStringArray("StatsArray", StatsArraytemp);
        PlayerPrefsX.SetStringArray("PassiveStatsArray", PassiveStatsArray);
        PlayerPrefsX.SetStringArray("Super100StatsArray", Super100StatsArraytemp);
        PlayerPrefsX.SetStringArray("Super200StatsArray", Super200StatsArraytemp);
        PlayerPrefsX.SetStringArray("TrophyRoadUnlocked", TrophyRoadUnlockedStr);
        PlayerPrefsX.SetIntArray("PassivesArray", Passives);
        PlayerPrefsX.SetIntArray("SkinsUnlockedAr", SkinsUnlockedAr);
        PlayerPrefsX.SetIntArray("EmotesUnlockedAr", EmotesUnlockedAr);
        PlayerPrefs.SetString("LevelStats", LevelStats);
        PlayerPrefsX.SetIntArray("AbilitiesRarety", RaretyAr);
        PlayerPrefs.SetInt("Offline", 1);
        PlayerPrefsX.SetIntArray("Backgrounds", Backgrounds);
        PlayerPrefs.SetInt("BotOnline", 0);
        PlayerPrefsX.SetIntArray("MaxPP", maxPP);
        PlayerPrefsX.SetIntArray("AdsRem", AdsRem);
        PlayerPrefs.SetInt("AdTimeBeforeReset", AdTimeBeforeReset);
        PlayerPrefs.SetString("SecretCode", SecretCode);
        PlayerPrefs.Save();
        CloudOnce.CloudVariables.Username = Username;
        CloudOnce.CloudVariables.Gold = Gold;
        CloudOnce.CloudVariables.ShapeLvl = Level;
        CloudOnce.CloudVariables.PP = PP;
        CloudOnce.CloudVariables.Abilities = Abilities;
        CloudOnce.CloudVariables.Super100 = Super100;
        CloudOnce.CloudVariables.Super200 = Super200;
        CloudOnce.CloudVariables.XP = XP;
        //CloudOnce.CloudVariables.Replay = Replay;
        CloudOnce.CloudVariables.Code = Code;
        CloudOnce.CloudVariables.Passives = Pas;
        CloudOnce.CloudVariables.Stats = Stats;
        CloudOnce.CloudVariables.Super100Stats = Super100Stats;
        CloudOnce.CloudVariables.Super200Stats = Super200Stats;
        CloudOnce.CloudVariables.LevelStats = LevelStats;
        CloudOnce.CloudVariables.Redbolts = Redbolts;
        CloudOnce.CloudVariables.Diamonds = Diamonds;
        CloudOnce.CloudVariables.Backgrounds = Bg;
        CloudOnce.CloudVariables.Skins = SkinsUnlocked;
        CloudOnce.CloudVariables.Emotes = EmotesUnlocked;
        CloudOnce.CloudVariables.MaxPP = maxPPstr;
        CloudOnce.Cloud.Storage.Save();
        if (PlayerPrefs.GetInt("Tutorial") == 1)
            SceneManager.LoadScene("PassThePhone");
        else
            SceneManager.LoadScene("MainMenuScene");            //Don't do that if we are reconnecting though
        buffer.Dispose();
    }
    private static void HandleGoingToBattle(byte[] data)
    {
        GameObject go = GameObject.Find("CancelPanel");
        GameObject goP = go.transform.parent.Find("FoundPanel").gameObject;
        goP.SetActive(true);
        if (TempOpponent.Opponent.FriendlyBattle == true) { goP.GetComponentInChildren<Text>().text = "Your friend is ready!"; }
        else { goP.GetComponentInChildren<Text>().text = "Opponent Found!"; }
        go.SetActive(false);

        int i = 0;
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);       
        string SceneToLoad = buffer.ReadString();
        if (SceneToLoad == "PassThePhone")
        {
            TempOpponent.Opponent.Username = buffer.ReadString();
            TempOpponent.Opponent.Username = CheckBotUsername(TempOpponent.Opponent.Username);
            string[] Ar = PlayerPrefsX.GetStringArray("AbilitiesArray");
            int[] counter = new int[3] { 0, 0, 0 };
            foreach(string s in Ar)
            {
                if (s.Equals("0") || s.Equals("1"))
                    counter[0]++;
                else if (s.Equals("2"))
                    counter[1]++;
                else
                    counter[2]++;
            }
            System.Random R = new System.Random();
            TempOpponent.Opponent.AbLevelArray = new string[Ar.Length];
            TempOpponent.Opponent.Super100 = PlayerPrefsX.GetStringArray("Super100Array");
            TempOpponent.Opponent.Super200 = PlayerPrefsX.GetStringArray("Super200Array");
            TempOpponent.Opponent.ShapeID = R.Next(0, 4);
            TempOpponent.Opponent.Passives = PlayerPrefsX.GetIntArray("PassivesArray");
            TempOpponent.Opponent.OpPP = PlayerPrefsX.GetIntArray("PP")[TempOpponent.Opponent.ShapeIDUser] + R.Next(-14, 15);
            if(TempOpponent.Opponent.OpPP < 0) { TempOpponent.Opponent.OpPP = 0;}
            TempOpponent.Opponent.OpLvl = PlayerPrefsX.GetIntArray("Level")[TempOpponent.Opponent.ShapeIDUser];
            for (int j = 0; j < Ar.Length; j++)
            {
                int ran = R.Next(0, Ar.Length + 1);
                if (ran < counter[0])
                    TempOpponent.Opponent.AbLevelArray[j] = "1";
                else if (ran < counter[0] + counter[1])
                    TempOpponent.Opponent.AbLevelArray[j] = "2";
                else
                    TempOpponent.Opponent.AbLevelArray[j] = "3";
            }
            TempOpponent.Opponent.AbLevelArray[new int[] { 1, 22, 23, 42 }[TempOpponent.Opponent.ShapeID]] = TempOpponent.Opponent.OpLvl <= 2 ? "1" : (TempOpponent.Opponent.OpLvl <= 4 ? "2" : "3");
            PlayerPrefs.SetInt("BotOnline",1);
            TempOpponent.Opponent.BotRoomNum = buffer.ReadInteger();
            //Send info to server for spectate
            ClientTCP.PACKAGE_DEBUG("BotSpectate",String.Join("|", new String[] { TempOpponent.Opponent.OpPP.ToString(), TempOpponent.Opponent.OpLvl.ToString(), String.Join(",", TempOpponent.Opponent.AbLevelArray), TempOpponent.Opponent.ShapeID.ToString(), TempOpponent.Opponent.Username }), TempOpponent.Opponent.BotRoomNum);
            SceneManager.LoadScene(SceneToLoad);
            buffer.Dispose();
            return;
        }
        string OtherPlayerUsername = buffer.ReadString();
        int OtherPlayerConnectionID = buffer.ReadInteger();
        int OtherPlayerShapeChosenID = buffer.ReadInteger();
        int OtherPlayerShapeID2 = buffer.ReadInteger();//kk
        TempOpponent.Opponent.ShapeID = OtherPlayerShapeChosenID;
        TempOpponent.Opponent.ShapeID12 = OtherPlayerShapeID2;//kk
        TempOpponent.Opponent.Username = OtherPlayerUsername;
        TempOpponent.Opponent.ConnectionID = OtherPlayerConnectionID;
        string Abilities = buffer.ReadString();
        string[] AbilitiesArray = Abilities.Split(',');
        string Super100 = buffer.ReadString();
        string[] Super100Array = Super100.Split(',');
        string Super200 = buffer.ReadString();
        string[] Super200Array = Super200.Split(',');
        string Passives = buffer.ReadString();
        string[] PassivesArray = Passives.Split(',');
        string Abs = buffer.ReadString();
        string[] AbsAr = Abs.Split(',');
        string Abs2 = buffer.ReadString();
        string[] AbsAr2 = Abs2.Split(',');
        int OpPP = buffer.ReadInteger();
        int OpPP2 = buffer.ReadInteger();
        int OpLvl = buffer.ReadInteger();
        int OpLvl2 = buffer.ReadInteger();
        int SuperID = buffer.ReadInteger();
        int SuperID2 = buffer.ReadInteger();
        int Mode = buffer.ReadInteger();
        TempOpponent.Opponent.ConnectionID2 = buffer.ReadInteger();
        TempOpponent.Opponent.SkinID = buffer.ReadInteger();
        TempOpponent.Opponent.SkinID2 = buffer.ReadInteger();
        GameMaster.Mode = Mode;
        TempOpponent.Opponent.OpPP = OpPP;
        TempOpponent.Opponent.OpPP2 = OpPP2;
        TempOpponent.Opponent.OpLvl = OpLvl;
        TempOpponent.Opponent.OpLvl2 = OpLvl2;
        TempOpponent.Opponent.AbIDs = new int[AbsAr.Length];
        TempOpponent.Opponent.AbIDs2 = new int[AbsAr2.Length];
        foreach (string c in AbsAr)
        {
            Int32.TryParse(c, out TempOpponent.Opponent.AbIDs[i]);
            i++;
        }
        i = 0;
        foreach (string c in AbsAr2)
        {
            Int32.TryParse(c, out TempOpponent.Opponent.AbIDs2[i]);
            i++;
        }
        TempOpponent.Opponent.AbLevelArray = AbilitiesArray;
        TempOpponent.Opponent.Super100 = Super100Array;
        TempOpponent.Opponent.Super200 = Super200Array;
        TempOpponent.Opponent.SuperID = SuperID;
        TempOpponent.Opponent.SuperID2 = SuperID2;
        i = 0;
        foreach (string c in PassivesArray)
        {
            Int32.TryParse(c, out TempOpponent.Opponent.Passives[i]);
            i++;
        }
        SceneManager.LoadScene(SceneToLoad);
        buffer.Dispose();
    }
    private static void HandleGettingProbability(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        int prob = buffer.ReadInteger();
        int PlayerNumber = buffer.ReadInteger();
        string Username = buffer.ReadString();
        if (Username == "")
        {
            if (PlayerNumber == 1)
            {
                TempOpponent.Opponent.Probability = prob;
                TempOpponent.Opponent.GotProb1 = true;
            }
            else if (PlayerNumber == 2)
            {
                if (!TempOpponent.Opponent.GotProb2)
                {
                    TempOpponent.Opponent.Probability2 = prob;
                    TempOpponent.Opponent.GotProb2 = true;
                }
                else
                {
                    TempOpponent.Opponent.Probability21 = prob;
                    TempOpponent.Opponent.GotProb21 = true;
                }
            }
        }
        else
        {
            if (Username == TempOpponent.Opponent.Username)
            {
                if (!TempOpponent.Opponent.GotProb2)
                {
                    TempOpponent.Opponent.Probability2 = prob;
                    TempOpponent.Opponent.GotProb2 = true;
                }
                else
                {
                    TempOpponent.Opponent.Probability21 = prob;
                    TempOpponent.Opponent.GotProb21 = true;
                }
            }
            else if (Username == TempOpponent.Opponent.Username2)
            {
                if (!TempOpponent.Opponent.GotProb1)
                {
                    TempOpponent.Opponent.Probability = prob;
                    TempOpponent.Opponent.GotProb1 = true;
                }
                else
                {
                    TempOpponent.Opponent.Probability1 = prob;
                    TempOpponent.Opponent.GotProb11 = true;
                }
            }
        }
        buffer.Dispose();
    }
    private static void HandleGivingChoice(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        ClientTCP.PACKAGE_GiveChoice();
        buffer.Dispose();
    }
    private static void HandleReceivingChoice(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        bool ChoiceDone = buffer.ReadBool();
        int IDChosen = buffer.ReadInteger();
        bool Spectate = buffer.ReadBool();
        if (!Spectate)
        {
            if (IDChosen == 53) //53 is for switch shape
                GameObject.Find("Game Manager").GetComponent<GameMasterOnline>().switchShape2();
            else //Need cheeck
            {
                TempOpponent.Opponent.ChoiceID = IDChosen;      //Added now, OK?
                TempOpponent.Method method;
                if (GameMaster.Online && !GameObject.Find("Game Manager").GetComponent<GameMasterOnline>().p1Selected2)  //2v2, 2nd player2 choice
                {
                    if (IDChosen != -1 && TempOpponent.Opponent.Abilities12.TryGetValue(IDChosen, out method))
                        method.Invoke();

                    if (!ChoiceDone) GameObject.Find("Game Manager").GetComponent<GameMasterOnline>().player22.SetChoiceDone(ChoiceDone);
                    GameObject.Find("Game Manager").GetComponent<GameMasterOnline>().player22.SetIdOfAnimUSed(IDChosen);
                }
                else   
                {
                    if (IDChosen != -1 && TempOpponent.Opponent.Abilities.TryGetValue(IDChosen, out method))
                        method.Invoke();

                    if (!ChoiceDone)
                        GameObject.Find("Game Manager").GetComponent<GameMasterOnline>().player2.SetChoiceDone(ChoiceDone);
                    //GameObject.Find("Player2").GetComponent<Shape_Player>().SetIdOfAnimUSed(IDChosen);
                }
            }
        }
        else
        {
            TempOpponent.Method method;
            string Username = buffer.ReadString();
            bool Player2 = buffer.ReadBool();
            int ID2 = 0;
            if (Player2)
            {
                ID2 = buffer.ReadInteger();
            }
            if (Username == TempOpponent.Opponent.Username)
            {
                if (TempOpponent.Opponent.Abilities.TryGetValue(IDChosen, out method) && IDChosen != -1)
                {
                    method.Invoke();
                }
                if (!ChoiceDone)
                    GameObject.Find("Player2").GetComponent<Shape_Player>().SetChoiceDone(ChoiceDone);
                GameObject.Find("Player2").GetComponent<Shape_Player>().SetIdOfAnimUSed(IDChosen);
                if (Player2)
                {
                    if (TempOpponent.Opponent.Abilities2.TryGetValue(ID2, out method) && IDChosen != -1)
                    {
                        method.Invoke();
                    }
                    GameObject.Find("Player1").GetComponent<Shape_Player>().SetIdOfAnimUSed(ID2);
                    string Name;
                    if (ID2 > 100)
                    {
                        if (TempOpponent.Opponent.AbNames.TryGetValue(ID2, out Name))
                        {
                            if (!TempOpponent.Opponent.SpecAbs.Contains(ID2))
                            {
                                TempOpponent.Opponent.SpecAbs.Add(ID2);
                                if (GameMasterSpectate.SpecialAb.transform.Find("QMark").GetComponent<RectTransform>().localScale.x == 1)
                                {
                                    GameMasterSpectate.SpecialAb.transform.Find(Name).gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                                    GameMasterSpectate.SpecialAb.transform.Find("QMark").GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
                                }
                            }
                        }
                    }
                    else if (TempOpponent.Opponent.AbNames.TryGetValue(ID2, out Name) && ID2 < 100 && IDChosen < 100 && IDChosen != 0 && IDChosen != 1 && IDChosen != 22 && IDChosen != 23 && IDChosen != 42)
                    {
                        if (!TempOpponent.Opponent.SpecAbs.Contains(ID2))
                        {
                            TempOpponent.Opponent.SpecAbs.Add(ID2);
                            foreach (GameObject G in GameMasterSpectate.Abilities)
                            {
                                if (G.transform.Find("QMark").GetComponent<RectTransform>().localScale.x == 1)
                                {
                                    G.transform.Find(Name).gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                                    G.transform.Find("QMark").GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else if (Username == TempOpponent.Opponent.Username2)
            {
                if (TempOpponent.Opponent.Abilities2.TryGetValue(IDChosen, out method) && IDChosen != -1)
                {
                    method.Invoke();
                }
                if (!ChoiceDone)
                    GameObject.Find("Player1").GetComponent<Shape_Player>().SetChoiceDone(ChoiceDone);
                GameObject.Find("Player1").GetComponent<Shape_Player>().SetIdOfAnimUSed(IDChosen);
                if (Player2)
                {
                    if (TempOpponent.Opponent.Abilities.TryGetValue(ID2, out method) && IDChosen != -1)
                    {
                        method.Invoke();
                    }
                    GameObject.Find("Player2").GetComponent<Shape_Player>().SetIdOfAnimUSed(ID2);
                }
                string Name;
                if (IDChosen > 100)
                {
                    //Debug.Log("Entered the Special ability condition");
                    if (TempOpponent.Opponent.AbNames.TryGetValue(IDChosen, out Name))
                    {
                        if (!TempOpponent.Opponent.SpecAbs.Contains(IDChosen))
                        {
                            TempOpponent.Opponent.SpecAbs.Add(IDChosen);
                            if (GameMasterSpectate.SpecialAb.transform.Find("QMark").GetComponent<RectTransform>().localScale.x == 1)
                            {
                                //Debug.Log(Name);
                                GameMasterSpectate.SpecialAb.transform.Find(Name).gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                                GameMasterSpectate.SpecialAb.transform.Find("QMark").GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
                            }
                        }
                    }
                }
                else if (TempOpponent.Opponent.AbNames.TryGetValue(IDChosen, out Name) && IDChosen < 100 && IDChosen != 0 && IDChosen != 1 && IDChosen != 22 && IDChosen != 23 && IDChosen != 42)
                {
                    if (!TempOpponent.Opponent.SpecAbs.Contains(IDChosen))
                    {
                        TempOpponent.Opponent.SpecAbs.Add(IDChosen);
                        foreach (GameObject G in GameMasterSpectate.Abilities)
                        {
                            if (G.transform.Find("QMark").GetComponent<RectTransform>().localScale.x == 1)
                            {
                                G.transform.Find(Name).GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                                G.transform.Find("QMark").GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
                                break;
                            }
                        }
                    }
                }
            }
            GameObject.Find("Managers").GetComponentInChildren<GameMasterSpectate>().CheckChoices();
        }
        buffer.Dispose();
    }
    private static void HandleEndgame(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        string PP = buffer.ReadString();
        string[] PP1 = PP.Split(',');
        int[] PPArray = new int[PP1.Length];
        int i = 0;
        int AllPP = 0;
        foreach (string c in PP1)
        {
            Int32.TryParse(c, out PPArray[i]);
            AllPP += PPArray[i];
            i++;
        }
        CloudOnce.Leaderboards.AllLb.SubmitScore(AllPP);
        /*string Replay = buffer.ReadString();
        string[] Replays = Replay.Split('|');
        string[] ReplayNew;
        if (Replays[0] != "N")
        {
            string[] Ar = Replays[0].Split('/');
            ReplayNew = new string[Ar.Length * Replays.Length];
            i = 0;
            foreach (string c in Replays)
            {
                string[] d = c.Split('/');
                foreach (string e in d)
                {
                    ReplayNew[i] = e;
                    i++;
                }
            }
        }
        else
        {
            ReplayNew = new string[] { "N" };
        }*/
        int Winstreak = buffer.ReadInteger();
        string IDs = buffer.ReadString();
        int[] RecentIDs;
        if (IDs == "N")
        {
            RecentIDs = new int[] { -1 };
            if (PlayerPrefs.GetInt("Winstreak") != 0)
            {
                ChestCode.OldRecentShapeIDs = PlayerPrefsX.GetIntArray("RecentIDs");
                ChestCode.OldWS = PlayerPrefs.GetInt("Winstreak");
                ChestCode.OpenIt = true;
            }
        }
        else
        {
            string[] RecentIDsStr = IDs.Split(',');
            RecentIDs = new int[RecentIDsStr.Length];
            i = 0;
            foreach (string d in RecentIDsStr)
            {
                Int32.TryParse(d, out RecentIDs[i]);
                i++;
            }
        }
        string maxPPstr = buffer.ReadString();
        string[] maxPPstrAr = maxPPstr.Split(',');
        int[] maxPP = new int[maxPPstrAr.Length];
        i = 0;
        foreach (string s in maxPPstrAr)
        {
            Int32.TryParse(s, out maxPP[i]);
            i++;
        }
        PlayerPrefsX.SetIntArray("RecentIDs", RecentIDs);
        PlayerPrefs.SetInt("Winstreak", Winstreak);
        //PlayerPrefsX.SetStringArray("Replay", ReplayNew);
        PlayerPrefs.SetInt("Updated", 1);
        PlayerPrefsX.SetIntArray("PP", PPArray);
        PlayerPrefsX.SetIntArray("MaxPP", maxPP);
        PlayerPrefs.Save();
        buffer.Dispose();
    }
    public static void HandleGettingLeaderboard(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        string Leaderboard = buffer.ReadString();
        TempOpponent.Opponent.Leaderboard = Leaderboard;
        TempOpponent.Opponent.Lbupdated = true;
        buffer.Dispose();
    }
    public static void HandleDisconnect(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        string PP = buffer.ReadString();
        string[] PP1 = PP.Split(',');
        int[] PPArray = new int[PP1.Length];
        int i = 0;
        int AllPP = 0;
        foreach (string c in PP1)
        {
            Int32.TryParse(c, out PPArray[i]);
            AllPP += PPArray[i];
            i++;
        }
        CloudOnce.Leaderboards.AllLb.SubmitScore(AllPP);
        int Winstreak = buffer.ReadInteger();
        string IDs = buffer.ReadString();
        string[] RecentIDsStr = IDs.Split(',');
        int[] RecentIDs = new int[RecentIDsStr.Length];
        i = 0;
        foreach (string d in RecentIDsStr)
        {
            Int32.TryParse(d, out RecentIDs[i]);
            i++;
        }
        PlayerPrefsX.SetIntArray("RecentIDs", RecentIDs);
        PlayerPrefs.SetInt("Winstreak", Winstreak);
        PlayerPrefsX.SetIntArray("PP", PPArray);
        PlayerPrefs.Save();
        TempOpponent.Opponent.Reset();
        SceneManager.LoadScene("MainMenuScene");
        buffer.Dispose();
    }
    public static void HandleReceivingAbilities(byte[] data)
    {
        bool firsttime = true;
        int i = 0;
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        string Abilities = buffer.ReadString();
        string[] AbilitiesArray = Abilities.Split(',');
        string Super100 = buffer.ReadString();
        string[] Super100Array = Super100.Split(',');
        string Super200 = buffer.ReadString();
        string[] Super200Array = Super200.Split(',');
        int Gold = buffer.ReadInteger();
        string Level = buffer.ReadString();
        string[] Lvl1 = Level.Split(',');
        int[] LevelArray = new int[Lvl1.Length];
        foreach (string c in Lvl1)
        {
            if (firsttime)
            {
                i = 0;
                firsttime = false;
            }
            Int32.TryParse(c, out LevelArray[i]);
            i++;
        }
        firsttime = true;
        string XP = buffer.ReadString();
        string[] XP1 = XP.Split(',');
        int[] XPArray = new int[XP1.Length];
        foreach (string c in XP1)
        {
            if (firsttime)
            {
                i = 0;
                firsttime = false;
            }
            Int32.TryParse(c, out XPArray[i]);
            i++;
        }
        firsttime = true;
        string PP = buffer.ReadString();
        string[] PP1 = PP.Split(',');
        int[] PPArray = new int[PP1.Length];
        foreach (string c in PP1)
        {
            if (firsttime)
            {
                i = 0;
                firsttime = false;
            }
            Int32.TryParse(c, out PPArray[i]);
            i++;
        }
        firsttime = true;
        i = 0;
        string Pas = buffer.ReadString();
        string[] PassivesStr = Pas.Split(',');
        int[] Passives = new int[PassivesStr.Length];
        foreach (string c in PassivesStr)
        {
            Int32.TryParse(c, out Passives[i]);
            i++;
        }
        int Redbolts = buffer.ReadInteger();
        int Diamonds = buffer.ReadInteger();
        string Bg = buffer.ReadString();
        string[] BackgroundsStr = Bg.Split(',');
        int[] Backgrounds = new int[BackgroundsStr.Length];
        i = 0;
        foreach (string s in BackgroundsStr)
        {
            Int32.TryParse(s, out Backgrounds[i]);
            i++;
        }
        string SkinsUnlocked = buffer.ReadString();
        string[] SkinsUnlockedStr = SkinsUnlocked.Split(',');
        int[] SkinsUnlockedAr = new int[SkinsUnlockedStr.Length];
        i = 0;
        foreach (string s in SkinsUnlockedStr)
        {
            Int32.TryParse(s, out SkinsUnlockedAr[i]);
            i++;
        }
        string EmotesUnlocked = buffer.ReadString();
        string[] EmotesUnlockedStr = EmotesUnlocked.Split(',');
        int[] EmotesUnlockedAr = new int[EmotesUnlockedStr.Length];
        i = 0;
        foreach (string s in EmotesUnlockedStr)
        {
            Int32.TryParse(s, out EmotesUnlockedAr[i]);
            i++;
        }
        string TrophyRoadUnlocked = buffer.ReadString();
        string[] TrophyRoadUnlockedStr = TrophyRoadUnlocked.Split('-');
        PlayerPrefs.SetInt("Gold", Gold);
        PlayerPrefs.SetInt("Redbolts", Redbolts);
        PlayerPrefs.SetInt("Diamonds", Diamonds);
        PlayerPrefsX.SetIntArray("Level", LevelArray);
        PlayerPrefsX.SetIntArray("XP", XPArray);
        PlayerPrefsX.SetIntArray("PP", PPArray);
        PlayerPrefsX.SetStringArray("AbilitiesArray", AbilitiesArray);
        PlayerPrefsX.SetStringArray("Super100Array", Super100Array);
        PlayerPrefsX.SetStringArray("Super200Array", Super200Array);
        PlayerPrefsX.SetIntArray("PassivesArray", Passives);
        PlayerPrefsX.SetIntArray("Backgrounds", Backgrounds);
        PlayerPrefsX.SetIntArray("SkinsUnlockedAr", SkinsUnlockedAr);
        PlayerPrefsX.SetIntArray("EmotesUnlockedAr", EmotesUnlockedAr);
        PlayerPrefsX.SetStringArray("TrophyRoadUnlocked", TrophyRoadUnlockedStr);
        PlayerPrefs.Save();
        buffer.Dispose();
    }
    public static void HandleReceivingStats(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        string Stats = buffer.ReadString();
        string[] StatsArraytemp = Stats.Split('-');
        string Super100Stats = buffer.ReadString();
        string[] Super100StatsArraytemp = Super100Stats.Split('-');
        string Super200Stats = buffer.ReadString();
        string[] Super200StatsArraytemp = Super200Stats.Split('-');
        string Shop = buffer.ReadString();
        string ShapePriceStr = buffer.ReadString();
        string[] ShapePriceS = ShapePriceStr.Split(',');
        int[] ShapePrice = new int[ShapePriceS.Length];
        int i = 0;
        foreach (string c in ShapePriceS)
        {
            Int32.TryParse(c, out ShapePrice[i]);
            i++;
        }
        string ChestSlotStr = buffer.ReadString();
        string[] ChestSlotPricesStr = ChestSlotStr.Split(',');
        int[] ChestSlotPrices = new int[ChestSlotPricesStr.Length];
        i = 0;
        foreach (string c in ChestSlotPricesStr)
        {
            Int32.TryParse(c, out ChestSlotPrices[i]);
            i++;
        }
        string LevelStats = buffer.ReadString();
        string AbilitiesRarety = buffer.ReadString();
        string[] AbRarety = AbilitiesRarety.Split(',');
        int[] RaretyAr = new int[AbRarety.Length];
        i = 0;
        foreach (string c in AbRarety)
        {
            Int32.TryParse(c, out RaretyAr[i]);
            i++;
        }
        string PassiveStats = buffer.ReadString();
        string[] PassiveStatsArray = PassiveStats.Split('-');
        string PassivesShop = buffer.ReadString();
        string BgPrice = buffer.ReadString();
        string[] BgPriceStr = BgPrice.Split(',');
        int[] BgPriceAr = new int[BgPriceStr.Length];
        i = 0;
        foreach (string s in BgPriceStr)
        {
            Int32.TryParse(s, out BgPriceAr[i]);
            i++;
        }
        string EmotesPrice = buffer.ReadString();
        string[] EmotesPriceStr = EmotesPrice.Split(',');
        int[] EmotesPriceAr = new int[EmotesPriceStr.Length];
        i = 0;
        foreach (string s in EmotesPriceStr)
        {
            Int32.TryParse(s, out EmotesPriceAr[i]);
            i++;
        }
        string SkinsPrice = buffer.ReadString();
        string[] SkinsPriceStr = SkinsPrice.Split(',');
        int[] SkinsPriceAr = new int[SkinsPriceStr.Length];
        i = 0;
        foreach (string s in SkinsPriceStr)
        {
            Int32.TryParse(s, out SkinsPriceAr[i]);
            i++;
        }
        string CoinPrice = buffer.ReadString();
        string[] CoinPriceStr = CoinPrice.Split(',');
        int[] CoinPriceAr = new int[CoinPriceStr.Length];
        i = 0;
        foreach (string s in CoinPriceStr)
        {
            Int32.TryParse(s, out CoinPriceAr[i]);
            i++;
        }
        string CoinReward = buffer.ReadString();
        string[] CoinRewardStr = CoinReward.Split(',');
        int[] CoinRewardAr = new int[CoinRewardStr.Length];
        i = 0;
        foreach (string s in CoinRewardStr)
        {
            Int32.TryParse(s, out CoinRewardAr[i]);
            i++;
        }
        string Super100Shop = buffer.ReadString();
        string Super200Shop = buffer.ReadString();
        string TrophyRoad = buffer.ReadString();
        PlayerPrefsX.SetStringArray("StatsArray", StatsArraytemp);
        PlayerPrefsX.SetStringArray("Super100StatsArray", Super100StatsArraytemp);
        PlayerPrefsX.SetStringArray("Super200StatsArray", Super200StatsArraytemp);
        PlayerPrefs.SetString("Shop", Shop);
        PlayerPrefsX.SetIntArray("ShapePrice", ShapePrice);
        PlayerPrefsX.SetIntArray("ChestSlotPrices", ChestSlotPrices);
        PlayerPrefs.SetString("LevelStats", LevelStats);
        PlayerPrefsX.SetIntArray("AbilitiesRarety", RaretyAr);
        PlayerPrefs.SetString("PassivesShop", PassivesShop);
        PlayerPrefs.SetString("Super100Shop", Super100Shop);
        PlayerPrefs.SetString("Super200Shop", Super200Shop);
        PlayerPrefs.SetString("TrophyRoad", TrophyRoad);
        PlayerPrefsX.SetIntArray("BgPriceAr", BgPriceAr);
        PlayerPrefsX.SetIntArray("EmotesPriceAr", EmotesPriceAr);
        PlayerPrefsX.SetIntArray("SkinsPriceAr", SkinsPriceAr);
        PlayerPrefsX.SetIntArray("CoinPriceAr", CoinPriceAr);
        PlayerPrefsX.SetIntArray("CoinRewardAr", CoinRewardAr);
        PlayerPrefsX.SetStringArray("PassiveStatsArray", PassiveStatsArray);
        PlayerPrefs.Save();
        buffer.Dispose();
    }
    public static void HandleEmotes(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        int ID = buffer.ReadInteger();
        string Username = buffer.ReadString();
        GameMaster GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        if (Username == "" || Username == TempOpponent.Opponent.Username)
        {
            GameMaster.animatorPlayer2.SetInteger("EID", ID);
            GM.SetAnim(2);

        }
        else if (Username == TempOpponent.Opponent.Username2)
        {
            GameMaster.animatorPlayer1.SetInteger("EID", ID);
            GM.SetAnim(1);
        }
        buffer.Dispose();
    }
    public static void HandleFriendsList(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        string FriendsList = buffer.ReadString();
        string On = buffer.ReadString();
        string PP = buffer.ReadString();
        string lvl = buffer.ReadString();
        string inGame = buffer.ReadString();
        string shapeIDs = buffer.ReadString();
        TempOpponent.Opponent.Friends = FriendsList;
        TempOpponent.Opponent.FriendsOnline = On;
        TempOpponent.Opponent.FriendsPP = PP;
        TempOpponent.Opponent.FriendsLvl = lvl;
        TempOpponent.Opponent.FriendsInGame = inGame;
        TempOpponent.Opponent.FriendsShapeIDs = shapeIDs;
        TempOpponent.Opponent.FLupdated = true;
        buffer.Dispose();
    }
    public static void HandleFriendly(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        int RoomNum = buffer.ReadInteger();
        string Username = buffer.ReadString();
        TempOpponent.Opponent.Username = Username;
        TempOpponent.Opponent.RoomNum = RoomNum;
        if (SceneManager.GetActiveScene().name == "MainMenuScene")
        {
            GameObject Panel = GameObject.Find("Priority Messages").transform.Find("FriendlyBattleRequest").gameObject;
            Panel.SetActive(true);
            Panel.GetComponent<FriendlyRequest>().GetOn();
        }
        buffer.Dispose();
    }
    public static void HandleSpectate(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        string Username1 = buffer.ReadString();
        string Username2 = buffer.ReadString();
        int ConnectionID1 = buffer.ReadInteger();
        int ConnectionID2 = buffer.ReadInteger();
        int ShapeChosenID1 = buffer.ReadInteger();
        int ShapeChosenID2 = buffer.ReadInteger();
        TempOpponent.Opponent.ShapeID = ShapeChosenID2;
        TempOpponent.Opponent.ShapeID2 = ShapeChosenID1;
        TempOpponent.Opponent.Username = Username2;
        TempOpponent.Opponent.Username2 = Username1;
        TempOpponent.Opponent.ConnectionID = ConnectionID2;
        TempOpponent.Opponent.ConnectionID2 = ConnectionID1;
        string Abilities = buffer.ReadString();
        string Abilities2 = buffer.ReadString();
        string[] AbilitiesArray = Abilities.Split(',');
        string[] AbilitiesArray2 = Abilities2.Split(',');
        string Super100 = buffer.ReadString();
        string Super1002 = buffer.ReadString();
        string[] Super100Array = Super100.Split(',');
        string[] Super1002Array = Super1002.Split(',');
        string Super200 = buffer.ReadString();
        string Super2002 = buffer.ReadString();
        string[] Super200Array = Super200.Split(',');
        string[] Super2002Array = Super2002.Split(',');
        int PP1 = buffer.ReadInteger();
        int PP2 = buffer.ReadInteger();
        int Lvl1 = buffer.ReadInteger();
        int Lvl2 = buffer.ReadInteger();
        string Passives1 = buffer.ReadString();
        string[] PassivesStr = Passives1.Split(',');
        int[] PassivesAr1 = new int[PassivesStr.Length];
        int i = 0;
        foreach (string c in PassivesStr)
        {
            Int32.TryParse(c, out PassivesAr1[i]);
            i++;
        }
        string Passives2 = buffer.ReadString();
        string[] PassivesStr2 = Passives2.Split(',');
        int[] PassivesAr2 = new int[PassivesStr2.Length];
        i = 0;
        foreach (string c in PassivesStr2)
        {
            Int32.TryParse(c, out PassivesAr2[i]);
            i++;
        }
        TempOpponent.Opponent.OpPP = PP2;
        TempOpponent.Opponent.OpPP2 = PP1;
        TempOpponent.Opponent.OpLvl = Lvl2;
        TempOpponent.Opponent.OpLvl2 = Lvl1;
        TempOpponent.Opponent.AbLevelArray2 = AbilitiesArray;
        TempOpponent.Opponent.Super1002 = Super100Array;
        TempOpponent.Opponent.Super2002 = Super200Array;
        TempOpponent.Opponent.AbLevelArray = AbilitiesArray2;
        TempOpponent.Opponent.Super100 = Super1002Array;
        TempOpponent.Opponent.Super200 = Super2002Array;
        TempOpponent.Opponent.Passives = PassivesAr2;
        TempOpponent.Opponent.Passives2 = PassivesAr1;
        SceneManager.LoadScene("SpectateScene");
        buffer.Dispose();
    }
    public static void HandleLP(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        int LP1 = buffer.ReadInteger();
        int LP2 = buffer.ReadInteger();
        GameMaster GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        string Rec = buffer.ReadString();
        int EP1 = buffer.ReadInteger(); int EP2 = buffer.ReadInteger(); int round = buffer.ReadInteger();
        if(buffer.ReadInteger() == ClientTCP.LPID)
        {
            GM.player1.SetLife(LP1);
            GM.player2.SetLife(LP2);
            GM.round = round;
            GM.player1.SetEP(EP1); GM.player2.SetEP(EP2);
            GM.ReadReconnectString(Rec, true);
        }
        //give proof whether he's p1 or p2
        buffer.Dispose();
    }
    public static void HandleChest(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        int Time = buffer.ReadInteger();
        int ChestNum = buffer.ReadInteger();

        int[] TimerAr = PlayerPrefsX.GetIntArray("TimerAr");
        if (TimerAr.Length != 4) { TimerAr = new int[4] { 0, 0, 0, 0 }; }

        TimerAr[ChestNum] = Time;
        PlayerPrefsX.SetIntArray("TimerAr", TimerAr);
        ChestSlot.GotChest[ChestNum] = true;
        buffer.Dispose();
    }
    public static void HandleReplay(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        string Replay = buffer.ReadString();
        string[] Replays = Replay.Split('|');
        string[] ReplayNew;
        int i = 0;
        if (Replays[0] != "N")
        {
            string[] Ar = Replays[0].Split('/');
            ReplayNew = new string[Ar.Length * Replays.Length];
            foreach (string c in Replays)
            {
                string[] d = c.Split('/');
                foreach (string e in d)
                {
                    ReplayNew[i] = e;
                    i++;
                }
            }
        }
        else
        {
            ReplayNew = new string[] { "N" };
        }
        PlayerPrefsX.SetStringArray("Replay", ReplayNew);
        buffer.Dispose();
    }
    public static void HandleReconnecting(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        int LP1 = buffer.ReadInteger();
        int LP2 = buffer.ReadInteger();
        int EP1 = buffer.ReadInteger();
        int EP2 = buffer.ReadInteger();
        int Round = buffer.ReadInteger();
        string Choices1 = buffer.ReadString();
        string Choices2 = buffer.ReadString();
        string Probs1 = buffer.ReadString();
        string Probs2 = buffer.ReadString();
        int Mode = buffer.ReadInteger();
        string OtherUsername = buffer.ReadString();
        int OtherConnID = buffer.ReadInteger();

        int OtherPlayerShapeChosenID = buffer.ReadInteger();
        //int OtherPlayerShapeID2 = buffer.ReadInteger();
        TempOpponent.Opponent.ShapeID = OtherPlayerShapeChosenID;
        //TempOpponent.Opponent.ShapeID12 = OtherPlayerShapeID2;
        TempOpponent.Opponent.Username = OtherUsername;
        TempOpponent.Opponent.ConnectionID = OtherConnID;
        string Abilities = buffer.ReadString();
        string[] AbilitiesArray = Abilities.Split(',');
        string Super100 = buffer.ReadString();
        string[] Super100Array = Super100.Split(',');
        string Super200 = buffer.ReadString();
        string[] Super200Array = Super200.Split(',');
        string Passives = buffer.ReadString();
        string[] PassivesArray = Passives.Split(',');
        string Abs = buffer.ReadString();
        string[] AbsAr = Abs.Split(',');
        int OpPP = buffer.ReadInteger();
        int OpLvl = buffer.ReadInteger();
        int SuperID = buffer.ReadInteger();
        TempOpponent.Opponent.ConnectionID2 = buffer.ReadInteger();
        TempOpponent.Opponent.SkinID = buffer.ReadInteger();
        //TempOpponent.Opponent.SkinID2 = buffer.ReadInteger();
        GameMaster.Mode = Mode;
        TempOpponent.Opponent.OpPP = OpPP;
        TempOpponent.Opponent.OpLvl = OpLvl;
        TempOpponent.Opponent.AbIDs = new int[AbsAr.Length];
        TempOpponent.Opponent.AbIDs2 = PlayerPrefsX.GetIntArray("ActiveAbilityID" + PlayerPrefs.GetInt("ShapeSelectedID"));     //Is that OK?
        int i = 0;
        foreach (string c in AbsAr)
        {
            Int32.TryParse(c, out TempOpponent.Opponent.AbIDs[i]);
            i++;
        }
        TempOpponent.Opponent.AbLevelArray = AbilitiesArray;
        TempOpponent.Opponent.Super100 = Super100Array;
        TempOpponent.Opponent.Super200 = Super200Array;
        TempOpponent.Opponent.SuperID = SuperID;
        i = 0;
        foreach (string c in PassivesArray)
        {
            Int32.TryParse(c, out TempOpponent.Opponent.Passives[i]);
            i++;
        }
        TempOpponent.Opponent.LP1 = LP1;
        TempOpponent.Opponent.LP2 = LP2;
        TempOpponent.Opponent.EP1 = EP1;
        TempOpponent.Opponent.EP2 = EP2;
        TempOpponent.Opponent.Round = Round;
        //
        string[] PlaceHolder = Choices1.Split(',');
        int[] Holder = new int[PlaceHolder.Length];
        int j = 0;
        foreach (string d in PlaceHolder)
        {
            Int32.TryParse(d, out Holder[j]);
            j++;
        }
        TempOpponent.Opponent.Choices1 = Holder;
        //
        PlaceHolder = Choices2.Split(',');
        Holder = new int[PlaceHolder.Length];
        j = 0;
        foreach (string d in PlaceHolder)
        {
            Int32.TryParse(d, out Holder[j]);
            j++;
        }
        TempOpponent.Opponent.Choices2 = Holder;
        //
        PlaceHolder = Probs1.Split(',');
        Holder = new int[PlaceHolder.Length];
        j = 0;
        foreach (string d in PlaceHolder)
        {
            Int32.TryParse(d, out Holder[j]);
            j++;
        }
        TempOpponent.Opponent.Probs1 = Holder;
        //
        PlaceHolder = Probs2.Split(',');
        Holder = new int[PlaceHolder.Length];
        j = 0;
        foreach (string d in PlaceHolder)
        {
            Int32.TryParse(d, out Holder[j]);
            j++;
        }
        TempOpponent.Opponent.Probs2 = Holder;
        GameMaster.Reconnect = true;
        GameMaster.Disconnected = false;
        TempOpponent.Opponent.ShapeIDUser = PlayerPrefs.GetInt("ShapeSelectedID");
        ReconnectString = buffer.ReadString();
        TempOpponent.Opponent.ChoiceID = buffer.ReadInteger();
        SceneManager.LoadScene("OnlineBattleScene");
        buffer.Dispose();
    }
    public static void HandleProfile(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        bool Outdated = buffer.ReadBool();
        if (Outdated)
        {
            TempOpponent.Opponent.profile = buffer.ReadString();
        }
        TempOpponent.Opponent.changed = Outdated;
        TempOpponent.Opponent.GotProfile = true;
        buffer.Dispose();
    }

    public static string TransformToString(int[][] intArr)
    {
        string[][] AbilitiesArraystr = new string[intArr.Length][];
        string Abilities = "";
        bool firsttime = true;
        try
        {
            int i = 0;
            foreach (int[] c in intArr)
            {
                AbilitiesArraystr[i] = new string[7];
                for (int j = 0; j < 7; j++)
                {
                    AbilitiesArraystr[i][j] = c[j].ToString();
                }
                i++;
            }
            foreach (string[] d in AbilitiesArraystr)
            {
                if (firsttime)
                {
                    Abilities = String.Join(",", d);
                    firsttime = false;
                }
                else
                {
                    Abilities = Abilities + "-" + String.Join(",", d);
                }
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            throw;
        }
        return Abilities;
    }
    public static int[][][] TransformToArrayShop(string Shop)
    {
        string[] String1 = Shop.Split('|');
        string[][] String2 = new string[String1.Length][];
        int Count = 0;
        foreach (string c in String1)
        {
            String2[Count] = c.Split('/');
            Count++;
        }
        int[][][] ShopArray = new int[String2.Length][][];
        string[][][] String3 = new string[String2.Length][][];
        int Count2 = 0;
        foreach (string[] d in String2)
        {
            int Count3 = 0;
            ShopArray[Count2] = new int[d.Length][];
            foreach (string e in d)
            {
                int Count4 = 0;
                String3[Count2] = new string[d.Length][];
                String3[Count2][Count3] = e.Split(',');
                ShopArray[Count2][Count3] = new int[String3[Count2][Count3].Length];
                foreach (string Str in String3[Count2][Count3])
                {
                    Int32.TryParse(Str, out ShopArray[Count2][Count3][Count4]);
                    Count4++;
                }
                Count3++;
            }
            Count2++;
        }
        return ShopArray;
    }
    public static int[][] TransformStringArray(string[] array)
    {
        int i = 0;
        string[][] StatsArraystr = new string[array.Length][];
        int[][] StatsArray = new int[array.Length][];
        foreach (string c in array)
        {
            int j = 0;
            StatsArraystr[i] = c.Split(',');
            StatsArray[i] = new int[StatsArraystr[i].Length];
            foreach (string d in StatsArraystr[i])
            {
                Int32.TryParse(d, out StatsArray[i][j]);
                j++;
            }
            i++;
        }
        return StatsArray;
    }
    private static string CheckBotUsername(string username)
    {
        string[] botProfiles = PlayerPrefsX.GetStringArray("BotProfiles");
        foreach(string s in botProfiles)
        {
            while (s.Split('|')[1].Equals(username))
            {
                username = username + R.Next(0, 10);
            }
        }
        return username;
    }
    private static int[] SArrayToIArray(string[] Ar)
    {
        int[] newAr = new int[Ar.Length];
        for (int i = 0; i < Ar.Length; i++)
            Int32.TryParse(Ar[i], out newAr[i]);
        return newAr;
    }

}
