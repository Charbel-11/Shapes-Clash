using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;
using MySql.Data.MySqlClient; 
using System.Threading;
using System.Threading.Tasks;

namespace ShapeGameServer
{
    class ServerTCP
    {
        private static TcpListener ServerSocket;    //The server basically
        public static ConcurrentDictionary<int, ClientObject> ClientObjects;
        public static ConcurrentDictionary<int, TempPlayer> TempPlayers;
        public static ConcurrentDictionary<string, ClientObject> IPDictionary;
        private static string AbilitiesStatsStr, Super100StatsStr, Super200StatsStr;
        private static string ShopStr;
        public static string ShapePriceStr;
        public static string ChestSlotStr;
        public static string LevelStatsStr;
        public static string AbilitiesRarety;
        public static string EmotesPrice, BgPrice, SkinPrice;
        public static string CoinPrice, CoinReward;
        public static string Super100Shop, Super200Shop;
        public static string PassivesShop;
        public static string PassivesStats;
        public static string TrophyRoad;
        public static string addrew;
        public static int C = 0;
        private static Random R = new Random();

        public static void InitializeServer()
        {
            AbilitiesStoringClass.InitializeAbilitiesArray();
            AbilitiesStatsStr = AbilitiesStoringClass.TransformToString(AbilitiesStoringClass.AbilitiesArray);
            PassivesStats = AbilitiesStoringClass.TransformToString(AbilitiesStoringClass.PassivesArray);
            Super100StatsStr = AbilitiesStoringClass.TransformToStringSuper100();
            Super200StatsStr = AbilitiesStoringClass.TransformToStringSuper200();
            ShopStr = AbilitiesStoringClass.TransformToStringShop(AbilitiesStoringClass.AbilitiesShopArray);
            ShapePriceStr = AbilitiesStoringClass.TransformToStringShape();
            ChestSlotStr = AbilitiesStoringClass.TransformToStringChestSlot();
            LevelStatsStr = AbilitiesStoringClass.TransformToStringLevel();
            AbilitiesRarety = String.Join(",", AbilitiesStoringClass.AbilitiesRarety);
            EmotesPrice = String.Join(",", AbilitiesStoringClass.EmotesPrice);
            Super100Shop = AbilitiesStoringClass.TransformToStringShop(AbilitiesStoringClass.Super100ShopArray);
            Super200Shop = AbilitiesStoringClass.TransformToStringShop(AbilitiesStoringClass.Super200ShopArray);
            PassivesShop = AbilitiesStoringClass.TransformToStringShop(AbilitiesStoringClass.PassivesShopArray);
            BgPrice = String.Join(",", AbilitiesStoringClass.BgPrice);
            SkinPrice = String.Join(",", AbilitiesStoringClass.SkinPrice);
            CoinPrice = String.Join(",", AbilitiesStoringClass.CoinPrice);
            CoinReward = String.Join(",", AbilitiesStoringClass.CoinReward);
            TrophyRoad = AbilitiesStoringClass.TransformToStringShop(AbilitiesStoringClass.TrophyRoadArray);
            addrew = String.Join(",", AbilitiesStoringClass.AdditionalRewardsArray);

            InitializeClientObjects();
            InitializeServerSocket();
        }
        public static MySqlConnection CreateSQLConnection()
        {
            MySqlConnection Connection;
            string user = "root";
            string password = "";
            string server = "localhost";
            string database = "shapegame";
            string connectionString = "SERVER=" + server + ";" +
                "DATABASE=" + database + ";" +
                "UID=" + user + ";" +
                "PASSWORD=" + password + ";";
            Connection = new MySqlConnection(connectionString);
            try { Connection.Open(); }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            return Connection;
        }
        private static void InitializeClientObjects()
        {
            ClientObjects = new ConcurrentDictionary<int, ClientObject>();
            TempPlayers = new ConcurrentDictionary<int, TempPlayer>();
            IPDictionary = new ConcurrentDictionary<string, ClientObject>();
            ClientObject.ConnectionIDs = new ConcurrentDictionary<string, int>();

            for (int i = 1; i < Constants.Max_Rooms; i++)
            {
                RoomInstance.unusedRooms.Enqueue(i);
                RoomInstance._room[i] = new Room();
                RoomInstance._room[i].roomIndex = i;
                RoomInstance._room[i].SpectatorIDs = new List<int>();
            }
        }
        private static void InitializeServerSocket()
        {
            ServerSocket = new TcpListener(IPAddress.Any, 8080);
            ServerSocket.Start();
            ServerSocket.BeginAcceptTcpClient(ClientConnectCallback, null); //Once we get a connection, ClientConnectCallback is called
        }
        private static void ClientConnectCallback(IAsyncResult result)
        {
            try
            {
                TcpClient tempClient = ServerSocket.EndAcceptTcpClient(result); //Accepts the connection and creates its corresponding TcpClient
                ServerSocket.BeginAcceptTcpClient(ClientConnectCallback, null); //Open the connection again for other players

                string[] IPs = tempClient.Client.RemoteEndPoint.ToString().Split(':');
                if (IPDictionary.TryGetValue(IPs[0], out ClientObject C))
                {
                    C.Socket = tempClient;

                    C.Socket.NoDelay = true;
                    C.Socket.ReceiveBufferSize = 4096;
                    C.Socket.SendBufferSize = 4096;

                    C.ReceiveBuffer = new byte[4096];
                    C.myStream = C.Socket.GetStream();

                    C.IP = tempClient.Client.RemoteEndPoint.ToString();
                    C.myStream.BeginRead(C.ReceiveBuffer, 0, C.Socket.ReceiveBufferSize, C.ReceiveCallback, null);

                    PACKET_SendMessage(C.ConnectionID, "CheckIP", 0, IPs[0]);

                    Console.WriteLine("Connection incoming from {0}", C.IP);
                    return;
                }
                Random ran = new Random();
                int curID = ran.Next();     //random signed int
                while (curID == 0 || ClientObjects.ContainsKey(curID)) { curID = ran.Next(); }

                ClientObjects[curID] = new ClientObject(tempClient, curID);
                TempPlayers[curID] = new TempPlayer();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }        
        }

        public static void SendDataTo(int ConnectionID, byte[] data)
        {
            if (!ClientObjects.ContainsKey(ConnectionID)) { Console.WriteLine("No such connectionID"); return; }
            if (ClientObjects[ConnectionID].Socket == null)
            {
                ClientObjects[ConnectionID].BufferList.Add(data);       
                return;
            }

            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger(data.GetUpperBound(0) - data.GetLowerBound(0) + 1);
            buffer.WriteBytes(data); 
            byte[] tmp = buffer.ToArray();

            Console.WriteLine("Send data to " + ConnectionID + "  PacketLength is: " + (data.GetUpperBound(0) - data.GetLowerBound(0) + 1)+ "  ByteArray length is: " + tmp.Length);

            ClientObjects[ConnectionID].myStream.BeginWrite(tmp, 0, tmp.Length, null, null);
            buffer.Dispose();
        }
        public static void PACKET_SendMessage(int ConnectionID, string msg, int ConnID = 0, string IP = "", int LPID = -1)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SMsg);
            buffer.WriteString(msg);
            if (ConnID != 0) { buffer.WriteInteger(ConnID); }
            if (IP != "") { buffer.WriteString(IP); }
            if (LPID != -1) { buffer.WriteInteger(LPID); }
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_Login(int ConnectionID, string username)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SLogin);
            buffer.WriteString(username);
            buffer.WriteInteger(Database.GetGold(username));
            buffer.WriteString(Database.GetLevel(username));
            buffer.WriteString(Database.GetXP(username));
            buffer.WriteString(Database.GetPP(username));
            buffer.WriteString(Database.GetAbilities(username));
            buffer.WriteString(Database.GetSuper100(username));
            buffer.WriteString(Database.GetSuper200(username));
            buffer.WriteString(AbilitiesStatsStr);
            buffer.WriteString(Super100StatsStr);
            buffer.WriteString(Super200StatsStr);
            buffer.WriteString(ShopStr);
            buffer.WriteString(ShapePriceStr);
            buffer.WriteString(ChestSlotStr);
            buffer.WriteString(Database.GetCode(username));
            buffer.WriteString(Database.GetPassives(username));
            //buffer.WriteString(Database.GetReplay(username));
            buffer.WriteInteger(Database.GetWinstreak(username));
            buffer.WriteString(Database.GetRecentIDs(username));
            buffer.WriteString(Database.GetChestSlots(username));
            buffer.WriteString(LevelStatsStr);
            buffer.WriteInteger(Database.GetRedbolts(username));
            buffer.WriteInteger(Database.GetDiamonds(username));
            buffer.WriteString(AbilitiesRarety);
            buffer.WriteString(Database.GetBackgrounds(username));
            buffer.WriteString(PassivesStats);
            buffer.WriteString(PassivesShop);
            buffer.WriteString(BgPrice);
            buffer.WriteString(EmotesPrice);
            buffer.WriteString(SkinPrice);
            buffer.WriteString(CoinPrice);
            buffer.WriteString(CoinReward);
            buffer.WriteString(Super100Shop);
            buffer.WriteString(Super200Shop);
            buffer.WriteString(Database.GetSkins(username));
            buffer.WriteString(Database.GetEmotes(username));
            buffer.WriteString(TrophyRoad);
            buffer.WriteString(Database.GetTrophyRoad(username));
            buffer.WriteString(Database.GetMaxPP(username));
            buffer.WriteString(Database.GetAdsRem(username));
            buffer.WriteString(Database.GetSecretCode(username));
            buffer.WriteInteger(Database.GetConsecutiveDays(username));
            buffer.WriteInteger(Database.GetDailyRewards(username));
            buffer.WriteString(addrew);
            buffer.WriteString(Database.GetAdditionalRewards(username));
            buffer.WriteInteger(AbilitiesStoringClass.GameVersion);
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_SendToMatchmaking(int ConnectionID, string otherplayerusername, int OtherPlayerConnectionID, int OtherPlayerShapeChosenID, int OtherPlayerShapeID2, string AbIDs, string AbIDs2, int OpPP,int OpPP2, int OpLvl, int OpLvl2, int OtherPlayerSuperID, int OtherPlayerSuperID2, int SkinID, int SkinID2, int Mode = 0,bool Bot = false)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SSendToBattle);
            if (Bot)
            {
                buffer.WriteString("PassThePhone");
                buffer.WriteString(AbilitiesStoringClass.botNames[R.Next(0, AbilitiesStoringClass.botNames.Length)] + R.Next(0,10).ToString());
                buffer.WriteInteger(TempPlayers[ConnectionID].Room);
                SendDataTo(ConnectionID, buffer.ToArray());
                buffer.Dispose();
                return;
            }
            buffer.WriteString("OnlineBattleScene");
            buffer.WriteString(otherplayerusername);
            buffer.WriteInteger(OtherPlayerConnectionID);
            buffer.WriteInteger(OtherPlayerShapeChosenID);
            buffer.WriteInteger(OtherPlayerShapeID2);
            buffer.WriteString(Database.GetAbilities(otherplayerusername));
            buffer.WriteString(Database.GetSuper100(otherplayerusername));
            buffer.WriteString(Database.GetSuper200(otherplayerusername));
            buffer.WriteString(Database.GetPassives(otherplayerusername));
            buffer.WriteString(AbIDs);
            buffer.WriteString(AbIDs2);
            buffer.WriteInteger(OpPP);
            buffer.WriteInteger(OpPP2);
            buffer.WriteInteger(OpLvl);
            buffer.WriteInteger(OpLvl2);
            buffer.WriteInteger(OtherPlayerSuperID);
            buffer.WriteInteger(OtherPlayerSuperID2);
            buffer.WriteInteger(Mode);
            buffer.WriteInteger(ConnectionID);
            buffer.WriteInteger(SkinID);
            buffer.WriteInteger(SkinID2);
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_SendProbability(int ConnectionID, int PlayerNumber, int Probability, string Username = "")
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SGiveProbability);
            buffer.WriteInteger(Probability);
            buffer.WriteInteger(PlayerNumber);
            buffer.WriteString(Username);
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_GetChoice(int ConnectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SGetChoice);
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_SendingChoice(int ConnectionID, bool Choicedone, int IDchosen, bool Spectate = false, string Username = "", bool Player2 = false, int ID2 = 0)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SSendChoice);
            buffer.WriteBool(Choicedone);
            buffer.WriteInteger(IDchosen);
            buffer.WriteBool(Spectate);
            if (Spectate)
            {
                buffer.WriteString(Username);
                buffer.WriteBool(Player2);
                if (Player2) { buffer.WriteInteger(ID2); }         
            }
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_Endgame(int ConnectionID, string Username)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SEndgame);
            buffer.WriteString(Database.GetPP(Username));
            //buffer.WriteString(Database.GetReplay(Username));
            buffer.WriteInteger(Database.GetWinstreak(Username));
            buffer.WriteString(Database.GetRecentIDs(Username));
            buffer.WriteString(Database.GetMaxPP(Username));
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_Leaderboard(int ConnectionID, int ShapeID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SLeaderboard);
            buffer.WriteString(Database.GetRanking(ShapeID));
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_Disconnect(int ConnectionID, int PP1, int PP2, string Username, string Otherplayerusername, int ShapeIDdisc, int ShapeIDOther)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SDisconnect);
            int PPdiff = PP1 - PP2;
            if (PPdiff >= 10)
            {
                Database.SetPP(Username, PP1 + 1, ShapeIDOther);
                Database.SetPP(Otherplayerusername, PP2 - 1, ShapeIDdisc);
            }
            else if (PPdiff < 10 && PPdiff >= 5)
            {
                Database.SetPP(Username, PP1 + 2, ShapeIDOther);
                Database.SetPP(Otherplayerusername, PP2 - 2, ShapeIDdisc);
            }
            else if (PPdiff < 5 && PPdiff >= 0)
            {
                Database.SetPP(Username, PP1 + 3, ShapeIDOther);
                Database.SetPP(Otherplayerusername, PP2 - 3, ShapeIDdisc);
            }
            else if (PPdiff < 0 && PPdiff >= -5)
            {
                Database.SetPP(Username, PP1 + 4, ShapeIDOther);
                Database.SetPP(Otherplayerusername, PP2 - 4, ShapeIDdisc);
            }
            else if (PPdiff < -5 && PPdiff >= -10)
            {
                Database.SetPP(Username, PP1 + 5, ShapeIDOther);
                Database.SetPP(Otherplayerusername, PP2 - 5, ShapeIDdisc);
            }
            else if (PPdiff < -10)
            {
                Database.SetPP(Username, PP1 + 6, ShapeIDOther);
                Database.SetPP(Otherplayerusername, PP2 - 6, ShapeIDdisc);
            }
            string NewPP = Database.GetPP(Username);
            Database.SetWinstreak(Username, 1);
            Database.SetWinstreak(Otherplayerusername, 0);
            Database.SetRecentIDs(Username, ShapeIDOther);
            Database.SetRecentIDs(Otherplayerusername, -1);
            buffer.WriteString(NewPP);
            buffer.WriteInteger(Database.GetWinstreak(Username));
            buffer.WriteString(Database.GetRecentIDs(Username));
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_GiveAbilities(int ConnectionID, string Username)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SGiveAbilities);
            buffer.WriteString(Database.GetAbilities(Username));
            buffer.WriteString(Database.GetSuper100(Username));
            buffer.WriteString(Database.GetSuper200(Username));
            buffer.WriteInteger(Database.GetGold(Username));
            buffer.WriteString(Database.GetLevel(Username));
            buffer.WriteString(Database.GetXP(Username));
            buffer.WriteString(Database.GetPP(Username));
            buffer.WriteString(Database.GetPassives(Username));
            buffer.WriteInteger(Database.GetRedbolts(Username));
            buffer.WriteInteger(Database.GetDiamonds(Username));
            buffer.WriteString(Database.GetBackgrounds(Username));
            buffer.WriteString(Database.GetSkins(Username));
            buffer.WriteString(Database.GetEmotes(Username));
            buffer.WriteString(Database.GetTrophyRoad(Username));
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_GiveStats(int ConnectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SGiveStats);
            buffer.WriteString(AbilitiesStatsStr);
            buffer.WriteString(Super100StatsStr);
            buffer.WriteString(Super200StatsStr);
            buffer.WriteString(ShopStr);
            buffer.WriteString(ShapePriceStr);
            buffer.WriteString(ChestSlotStr);
            buffer.WriteString(LevelStatsStr);
            buffer.WriteString(AbilitiesRarety);
            buffer.WriteString(PassivesStats);
            buffer.WriteString(PassivesShop);
            buffer.WriteString(BgPrice);
            buffer.WriteString(EmotesPrice);
            buffer.WriteString(SkinPrice);
            buffer.WriteString(CoinPrice);
            buffer.WriteString(CoinReward);
            buffer.WriteString(Super100Shop);
            buffer.WriteString(Super200Shop);
            buffer.WriteString(TrophyRoad);
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_Emotes(int ConnectionID, int ID, string Username = "")
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SEmotes);
            buffer.WriteInteger(ID);
            buffer.WriteString(Username);
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_GiveList(int ConnectionID, string List)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SGiveList);
            string[] FO = List.Split('/');
            string[] Friends = FO[0].Split(',');
            string[] PPs = new string[Friends.Length];
            string[] lvl = new string[Friends.Length];
            int[] shape = new int[Friends.Length];
            int[] inGame = new int[Friends.Length];
            int i = 0;
            foreach (string c in Friends)
            {
                if (ClientObject.ConnectionIDs.ContainsKey(c))
                    inGame[i] = TempPlayers[ClientObject.ConnectionIDs[c]].inMatch ? 1 : 0;
                else
                    inGame[i] = 0;

                string allPP = Database.GetPP(c);
                string allLvls = Database.GetLevel(c);
                string lastDeck = Database.GetLastDeck(c);
                if (lastDeck == "N")
                {
                    string[] lvls = allLvls.Split(',');
                    for(int kk = 0; kk < 4; kk++)
                    {
                        if (lvls[kk] != "0") { shape[i] = kk; break; }
                    }
                }
                else
                    Int32.TryParse(lastDeck.Split(',')[0], out shape[i]);

                lvl[i] = allLvls.Split(',')[shape[i]];
                PPs[i] = allPP.Split(',')[shape[i]];
                i++;
            }
            string PPTotals = String.Join(",", PPs);
            buffer.WriteString(FO[0]);
            buffer.WriteString(FO[1]);
            buffer.WriteString(PPTotals);
            buffer.WriteString(String.Join(",", lvl));
            buffer.WriteString(String.Join(",", inGame));
            buffer.WriteString(String.Join(",", shape));
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_FriendlyBattle(int ConnectionID, int RoomNum, string Username)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SFriendly);
            buffer.WriteInteger(RoomNum);
            buffer.WriteString(Username);
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_Spectate(int ConnectionID, string Username, string Username2, int ConnectionID1, int ConnectionID2, int ShapeID1, int ShapeID2, int PP1, int PP2, int Lvl1, int Lvl2,string BotAbs = "")
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SSpectate);
            buffer.WriteString(Username);
            buffer.WriteString(Username2);
            buffer.WriteInteger(ConnectionID1);
            buffer.WriteInteger(ConnectionID2);
            buffer.WriteInteger(ShapeID1);
            buffer.WriteInteger(ShapeID2);
            buffer.WriteString(Database.GetAbilities(Username));
            buffer.WriteString(ConnectionID2 == -1 ? BotAbs : Database.GetAbilities(Username2));
            buffer.WriteString(Database.GetSuper100(Username));
            buffer.WriteString(ConnectionID2 == -1 ? Database.GetSuper100(Username) : Database.GetSuper100(Username2));
            buffer.WriteString(Database.GetSuper200(Username));
            buffer.WriteString(ConnectionID2 == -1 ? Database.GetSuper200(Username) : Database.GetSuper200(Username2));
            buffer.WriteInteger(PP1);
            buffer.WriteInteger(PP2);
            buffer.WriteInteger(Lvl1);
            buffer.WriteInteger(Lvl2);
            buffer.WriteString(Database.GetPassives(Username));
            buffer.WriteString(ConnectionID2 == -1 ? Database.GetPassives(Username) : Database.GetPassives(Username2));
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_LP(int ConnectionID, int LP1, int LP2, string Rec,int EP1,int EP2,int round,int LPID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SLP);
            buffer.WriteInteger(LP1);
            buffer.WriteInteger(LP2);
            buffer.WriteString(Rec);
            buffer.WriteInteger(EP1); buffer.WriteInteger(EP2); buffer.WriteInteger(round);
            buffer.WriteInteger(LPID);
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_Chest(int ConnectionID, int Time, int ChestNum)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SChest);
            buffer.WriteInteger(Time);
            buffer.WriteInteger(ChestNum);
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_Replay(int ConnectionID, string Username)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SReplay);
            buffer.WriteString(Database.GetReplay(Username));
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public static void PACKET_Reconnect(int ConnectionID, int LP1, int LP2, int EP1, int EP2, int Round, string Choices1, string Choices2, string Probs1, string Probs2, int Mode, int Index, string Username2, int OtherConnID, string ReconnectString, int ChoiceID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SReconnect);
            buffer.WriteInteger(LP1);
            buffer.WriteInteger(LP2);
            buffer.WriteInteger(EP1);
            buffer.WriteInteger(EP2);
            buffer.WriteInteger(Round);
            buffer.WriteString(Choices1);
            buffer.WriteString(Choices2);
            buffer.WriteString(Probs1);
            buffer.WriteString(Probs2);
            buffer.WriteInteger(Mode);
            buffer.WriteString(Username2);
            buffer.WriteInteger(OtherConnID);
            int Index2 = (Index == 0 ? 1 : 0);

            Room room = RoomInstance._room[TempPlayers[OtherConnID].Room];
            buffer.WriteInteger(room.ShapeChosenIDs[OtherConnID]);
            //buffer.WriteInteger(OtherPlayerShapeID2);
            buffer.WriteString(Database.GetAbilities(Username2));
            buffer.WriteString(Database.GetSuper100(Username2));
            buffer.WriteString(Database.GetSuper200(Username2));
            buffer.WriteString(Database.GetPassives(Username2));
            buffer.WriteString(room.AbIDs[Index2]);
            buffer.WriteInteger(room.PPs[Index2]);
            buffer.WriteInteger(room.Levels[Index2]);
            buffer.WriteInteger(room.SuperIDs[Index2]);
            buffer.WriteInteger(ConnectionID);
            if (Index2 == 0)
                buffer.WriteInteger(room.SkinArray[Index2]);
            else
                buffer.WriteInteger(room.SkinArray[Index2 + 1]);
            //buffer.WriteInteger(SkinID2);
            buffer.WriteString(ReconnectString);
            buffer.WriteInteger(ChoiceID);

            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        } 
        public static void PACKET_Profile(int ConnectionID, string Username, int v)
        {
            string p = Database.GetProfile(Username);
            string[] Profile = p.Split('|');
            Int32.TryParse(Profile[0], out int Version);
            bool Outdated = (Version != v);

            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackages.SProfile);
            buffer.WriteBool(Outdated);
            if (Outdated) { buffer.WriteString(p); }
            SendDataTo(ConnectionID, buffer.ToArray());
            buffer.Dispose();
        }
    }
}
