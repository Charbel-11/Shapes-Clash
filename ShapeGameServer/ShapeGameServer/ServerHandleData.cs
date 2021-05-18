using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using MySql;
namespace ShapeGameServer
{
    public class ServerHandleData
    {
        public static int totalLen = 0; //Debug variable
        public delegate void PacketF(int ConnectionID, byte[] Data);    //Signature of functions of type (int, byte[])
        public static Dictionary<int, PacketF> PacketListener;          //Maps functionID -> function; read-only hence thread-safe

        public static void InitializePacketListener()
        {
            PacketListener = new Dictionary<int, PacketF> {
                { (int)ClientPackages.CLogin, HandleLogin },
                { (int)ClientPackages.CNewAccount, HandleRegister },
                { (int)ClientPackages.CBattleOnline, HandleBattleOnline },
                { (int)ClientPackages.CGetProbability, HandleGiveProbability },
                { (int)ClientPackages.CGetOtherPlayerChoice, HandleGiveOtherPlayerChoice },
                { (int)ClientPackages.CSendChoice, HandleReceivingChoice },
                { (int)ClientPackages.CEndGame, HandleEndGame },
                { (int)ClientPackages.CDebug, HandleDebug },
                { (int)ClientPackages.CLeaderboard, HandleSendingLeaderboard },
                { (int)ClientPackages.CGetAbilities, HandleGivingAbilities },
                { (int)ClientPackages.CMainMenu, HandleMainMenu },
                { (int)ClientPackages.CGetStats, HandleGivingStats },
                { (int)ClientPackages.CEmotes, HandleEmotes },
                { (int)ClientPackages.CAddFriend, HandleAddingFriend },
                { (int)ClientPackages.CFriendsList, HandleFriendList },
                { (int)ClientPackages.CCheckFriend, HandleFriendCheck },
                { (int)ClientPackages.CSpectate, HandleSpectate },
                { (int)ClientPackages.CLifePoints, HandleLP },
                { (int)ClientPackages.CChest, HandleChest },
                { (int)ClientPackages.CChestOpening, HandleChestOpening },
                { (int)ClientPackages.CReconnect, HandleReconnecting },
                { (int)ClientPackages.CAdReward, HandleAdReward }
            };
        }

        public static bool HandleAuth(int ConnectionID)
        {
            if (ServerTCP.ClientObjects[ConnectionID].buffer.Length() < 12)
            {
                ServerTCP.ClientObjects[ConnectionID].CloseConnection();
                return false;
            }

            int len = ServerTCP.ClientObjects[ConnectionID].buffer.ReadInteger();
            int id1 = ServerTCP.ClientObjects[ConnectionID].buffer.ReadInteger();
            int id2 = ServerTCP.ClientObjects[ConnectionID].buffer.ReadInteger();
            if (len == 8 && id1 == 1000000007 && id2 == 5793973)
            {
                ServerTCP.ClientObjects[ConnectionID].authenticated = true;
                Console.WriteLine(ConnectionID + " was successfully authenticated");
                return true;
            }
            else
            {
                ServerTCP.ClientObjects[ConnectionID].CloseConnection();
                Console.WriteLine(ConnectionID + " is fake");
                return false;
            }
        }

        //Makes sure that we get every byte of a given packet and then the next packet
        //In case of packets > 4096 bytes, handledata will be called mutliple times
        //and only when all data are here (length>=plength), the loop will be entered
        public static void HandleData(int ConnectionID, byte[] data)        //Static method is fine since each thread has its own stack space
        {
            try
            {
                foreach (byte bb in data) { Console.Write(bb + " "); }
                Console.Write('\n');
                foreach (byte bb in data) { Console.Write((char)bb); }
                Console.Write('\n');

                if (data == null) { Console.WriteLine("No data..."); return; }

                int pLength = 0;    //Packet length
                byte[] buffer = (byte[])data.Clone();   //To avoid shallow copies

                if (!ServerTCP.ClientObjects.ContainsKey(ConnectionID)) { return; }
                if (ServerTCP.ClientObjects[ConnectionID].buffer == null)
                    ServerTCP.ClientObjects[ConnectionID].buffer = new ByteBuffer();

                ServerTCP.ClientObjects[ConnectionID].buffer.WriteBytes(buffer);
                if (!ServerTCP.ClientObjects[ConnectionID].authenticated)
                {
                    bool a = HandleAuth(ConnectionID);
                    if (!a) { return; }
                    if (ServerTCP.ClientObjects[ConnectionID].buffer.Length() == 0) { return; }
                }

                if (ServerTCP.ClientObjects[ConnectionID].buffer.Length() < 4)
                {
                    Console.WriteLine("Buffer is too empty");
                    ServerTCP.ClientObjects[ConnectionID].buffer.Clear();
                    return;
                }

                if (!ServerTCP.ClientObjects.ContainsKey(ConnectionID)) { return; }
                pLength = ServerTCP.ClientObjects[ConnectionID].buffer.ReadInteger(false);  //Advances only when the whole packet is here
                while (pLength >= 4 && pLength <= ServerTCP.ClientObjects[ConnectionID].buffer.Length() - 4)    //-4 since readPos hasn't advanced yet
                {
                    ServerTCP.ClientObjects[ConnectionID].buffer.ReadInteger();
                    totalLen += pLength; Console.WriteLine("pLength = " + pLength + "   " + "totalLength = " + totalLen);

                    int packageID = ServerTCP.ClientObjects[ConnectionID].buffer.ReadInteger();
                    pLength -= 4;
                    data = ServerTCP.ClientObjects[ConnectionID].buffer.ReadBytes(pLength);

                    //Call the appropriate function in case of a correct packageID
                    if (PacketListener.TryGetValue(packageID, out PacketF packet))
                        packet.Invoke(ConnectionID, data);
                    else { Console.WriteLine("Wrong function ID"); pLength = 0; break; }

                    if (!ServerTCP.ClientObjects.ContainsKey(ConnectionID)) { return; }

                    pLength = 0;
                    if (ServerTCP.ClientObjects[ConnectionID].buffer.Length() >= 4)
                        pLength = ServerTCP.ClientObjects[ConnectionID].buffer.ReadInteger(false);
                }

                if (pLength < 4 && ServerTCP.ClientObjects.ContainsKey(ConnectionID)) { ServerTCP.ClientObjects[ConnectionID].buffer.Clear(); }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
       
        private static void HandleRegister(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            string username = buffer.ReadString();
            if(username[0] == '/')
            {
                string User = Database.CheckSecretCode(username);
                if (Database.Online(User))
                {
                    ServerTCP.PACKET_SendMessage(ConnectionID, "ABU");
                    return;
                }
                else if (ClientObject.ConnectionIDs.TryGetValue(username, out int ConnID))
                {
                    ServerTCP.PACKET_SendMessage(ConnID, "DN"); //Tell the game client to not let the player reconnect
                }

                if (User != "")
                    ServerTCP.PACKET_Login(ConnectionID, User);
                else
                    ServerTCP.PACKET_SendMessage(ConnectionID, "WrongSecretCode");
                return;
            }
            if (Database.AccountExists(username))
            {
                ServerTCP.PACKET_SendMessage(ConnectionID, "Taken");
                return;
            }

            Database.NewAccount(username);
            Console.WriteLine("Player {0} successfully logged into his account", username);
            ServerTCP.ClientObjects[ConnectionID].Username = username;

            //If username already here, remove than add otherwise just add
            ClientObject.ConnectionIDs.TryRemove(username, out int prevID);
            ClientObject.ConnectionIDs.TryAdd(username, ConnectionID);
            
            Database.SetOnline(username, true);
            Database.UpdateProfile(username);
            ServerTCP.PACKET_Login(ConnectionID, username);
            buffer.Dispose();
        }
        private static void HandleLogin(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            string username = buffer.ReadString();
            if (username == "N")
            {
                ServerTCP.PACKET_SendMessage(ConnectionID, "Create Username");
                return;
            }
            else if (!Database.AccountExists(username))
            {
                ServerTCP.PACKET_SendMessage(ConnectionID, "Wrong Username");
                return;
            }

            if(Database.Online(username))
            {
                ServerTCP.PACKET_SendMessage(ConnectionID, "ABU");
                return;
            }
            else if (ClientObject.ConnectionIDs.TryGetValue(username, out int ConnID))
            {
                ServerTCP.PACKET_SendMessage(ConnID, "DN"); //Tell the game client to not let the player reconnect
            }

            Console.WriteLine("Player {0} successfully logged into his account", username);
            ServerTCP.ClientObjects[ConnectionID].Username = username;

            ClientObject.ConnectionIDs.TryRemove(username, out int prevID);
            ClientObject.ConnectionIDs.TryAdd(username, ConnectionID);
            
            int Option = 0;
            string rec = "";
            if (ClientObject.UsernamesDisc.TryGetValue(username, out int[] Ar))
            {
                if(Ar[1] == -1)
                {
                    rec = RoomInstance.instance.HandleBotReconnect(Ar[0], ConnectionID);
                    Option = 2;
                }
                else if(RoomInstance._room[Ar[0]].ConnectionID[1] != 0 && RoomInstance._room[Ar[0]].ConnectionID[0] != 0)
                {
                    Option = 1;
                }
                else
                {
                    ClientObject.UsernamesDisc.TryRemove(username, out int[] aa);
                }
            }
            Database.SetOnline(username, true);
            Database.UpdateLastLogin(username);
            Database.UpdateLastSpin(username, true);
            ServerTCP.PACKET_Login(ConnectionID, username);
            if (Option == 1) { ServerTCP.PACKET_SendMessage(Ar[1], "Reconnect", ConnectionID); } //Message sent to the still online player
            else if(Option == 2) { ServerTCP.PACKET_SendMessage(ConnectionID, "ReconnectBot", Ar[0], rec); ClientObject.UsernamesDisc.TryRemove(username, out int[] aa); }
            buffer.Dispose();
        }
        private static void HandleBattleOnline(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            string username = buffer.ReadString();
            int ShapeChosenID = buffer.ReadInteger();
            int ShapeChosenID2 = buffer.ReadInteger();
            int GameMode = buffer.ReadInteger();
            string AbilitiesID = buffer.ReadString();
            string AbilitiesID2 = buffer.ReadString();
            int SuperID = buffer.ReadInteger();
            int SuperID2 = buffer.ReadInteger();
            int Choice = buffer.ReadInteger();
            int SkinID = buffer.ReadInteger();
            int SkinID2 = buffer.ReadInteger();
            if (Choice == 0)
            {
                RoomInstance.instance.JoinOrCreateRoom(ConnectionID, username, ShapeChosenID, ShapeChosenID2, AbilitiesID, AbilitiesID2, SuperID, SuperID2, GameMode, SkinID, SkinID2);
            }
            else if (Choice == 1)
            {
                string OtherUsername = buffer.ReadString();
                ClientObject.ConnectionIDs.TryGetValue(OtherUsername, out int OtherConnID);
                int RoomNum = RoomInstance.instance.FriendlyBattle(ConnectionID, username, ShapeChosenID, ShapeChosenID2, AbilitiesID, AbilitiesID2, SuperID, SuperID2, OtherConnID, SkinID, SkinID2);
                ServerTCP.PACKET_FriendlyBattle(OtherConnID, RoomNum, username);
            }
            else if (Choice == 2)
            {
                int Roomnum = buffer.ReadInteger();
                RoomInstance.instance.JoinFriendlyBattle(ConnectionID, username, ShapeChosenID, ShapeChosenID2, AbilitiesID, AbilitiesID2, Roomnum, SuperID, SuperID2, SkinID, SkinID2);
            }
            buffer.Dispose();
        }
        private static void HandleGiveProbability(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int OtherPlayerConnectionID = buffer.ReadInteger();
            //int Range = buffer.ReadInteger();
            string Username = buffer.ReadString();
            string OtherUsername = buffer.ReadString();
            bool Disc = buffer.ReadBool();
            Random rand = new Random();
            int Probability = rand.Next(0, 101);
            if (!Disc)
            {
                ServerTCP.PACKET_SendProbability(ConnectionID, 1, Probability);
                if (!ClientObject.UsernamesDisc.ContainsKey(OtherUsername))
                    ServerTCP.PACKET_SendProbability(OtherPlayerConnectionID, 2, Probability);

                int room = ServerTCP.TempPlayers[ConnectionID].Room;
                if (room != 0)
                {
                    if (RoomInstance._room[room].SpectatorIDs.Count != 0)
                    {
                        foreach (int c in RoomInstance._room[room].SpectatorIDs)
                        {
                            ServerTCP.PACKET_SendProbability(c, 1, Probability, Username);
                        }
                    }
                }
            }
            else
            {
                ServerTCP.PACKET_SendProbability(ConnectionID, 2, Probability);

                int room = ServerTCP.TempPlayers[ConnectionID].Room;
                if (room != 0)
                {
                    if (RoomInstance._room[room].SpectatorIDs.Count != 0)
                    {
                        foreach (int c in RoomInstance._room[room].SpectatorIDs)
                        {
                            ServerTCP.PACKET_SendProbability(c, 1, Probability, OtherUsername);
                        }
                    }
                }
            }
            buffer.Dispose();
        }
        private static void HandleGiveOtherPlayerChoice(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int OtherPlayerConnectionID = buffer.ReadInteger();
            ServerTCP.PACKET_GetChoice(OtherPlayerConnectionID);
            buffer.Dispose();
        }
        private static void HandleReceivingChoice(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            bool Choicedone = buffer.ReadBool();
            int IDChosen = buffer.ReadInteger();
            int OtherPlayerConnectionID = buffer.ReadInteger();
            string Username = buffer.ReadString();
            string OtherPlayerUsername = buffer.ReadString();
            bool Player2 = buffer.ReadBool();

            if (OtherPlayerConnectionID != -1 && !ClientObject.UsernamesDisc.ContainsKey(OtherPlayerUsername))
            {
               ServerTCP.PACKET_SendingChoice(OtherPlayerConnectionID, Choicedone, IDChosen);
            }

            int room = ServerTCP.TempPlayers[ConnectionID].Room;
            if (RoomInstance._room[room].SpectatorIDs.Count != 0)
            { 
                int ID2 = 0;
                if(Player2)
                    ID2 = buffer.ReadInteger();
                foreach (int c in RoomInstance._room[room].SpectatorIDs)
                {
                    ServerTCP.PACKET_SendingChoice(c, Choicedone, IDChosen, true, Username, Player2, ID2);
                }
            }
            buffer.Dispose();
        }
        private static void HandleEndGame(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int LooserConnID = buffer.ReadInteger();
            string Username = buffer.ReadString();
            if (LooserConnID == -1)
            {
                int shapeID = buffer.ReadInteger();
                int playerWon = buffer.ReadInteger();
                int botPP = buffer.ReadInteger();
                if(playerWon == 0)
                {
                    RoomInstance.instance.LeaveRoom(ConnectionID, 1, ServerTCP.TempPlayers[ConnectionID].Room);
                    Database.UpdateProfile(Username);
                    buffer.Dispose();
                    return;
                }
                string pp1str = Database.GetPP(Username);
                string[] pp1Array = pp1str.Split(',');
                int[] pp1Arrayint = new int[pp1Array.Length];
                int j = 0;
                foreach (string c in pp1Array)
                {
                    Int32.TryParse(c, out pp1Arrayint[j]);
                    j++;
                }
                int pp1 = pp1Arrayint[shapeID];
                int ppdiff = pp1 - botPP; ppdiff = playerWon == 1 ? ppdiff : -ppdiff;             
                int ppAdded = 0;
                if (ppdiff >= 10) { ppAdded = 1; }
                if (ppdiff < 10 && ppdiff >= 5) { ppAdded = 2; }
                if (ppdiff < 5 && ppdiff >= 0) { ppAdded = 3; }
                if (ppdiff < 0 && ppdiff >= -5) { ppAdded = 4; }
                if (ppdiff < -5 && ppdiff >= -10) { ppAdded = 5; }
                if (ppdiff < -10) { ppAdded = 6; }
                ppAdded = playerWon == 1 ? ppAdded : -ppAdded;
                Database.SetWinstreak(Username, playerWon == 1 ? 1 : 0);
                Database.SetRecentIDs(Username, playerWon == 1 ? shapeID : -1);
                RoomInstance.instance.LeaveRoom(ConnectionID, 1, ServerTCP.TempPlayers[ConnectionID].Room);
                Database.SetPP(Username, pp1 + ppAdded, shapeID);
                if (!ClientObject.UsernamesDisc.ContainsKey(Username))
                    ServerTCP.PACKET_Endgame(ConnectionID, Username);
                Database.UpdateProfile(Username);
                buffer.Dispose();
                return;
            }
            string OtherPlayerUsername = buffer.ReadString();
            int ShapeID = buffer.ReadInteger();
            int OtherShapeID = buffer.ReadInteger();
            int GameMode = buffer.ReadInteger();
            bool Friendly = buffer.ReadBool();
            bool Draw = buffer.ReadBool();
            if (Friendly || Draw)
            {
                /*
                Database.SetReplay(Username, Replay + "/" + 0);
                string[] Components1 = Replay.Split('/');
                string PlaceHolder1;
                PlaceHolder1 = Components1[0];
                Components1[0] = Components1[1];
                Components1[1] = PlaceHolder1;
                PlaceHolder1 = Components1[2];
                Components1[2] = Components1[3];
                Components1[3] = PlaceHolder1;
                PlaceHolder1 = Components1[4];
                Components1[4] = Components1[5];
                Components1[5] = PlaceHolder1;
                PlaceHolder1 = Components1[6];
                Components1[6] = Components1[7];
                Components1[7] = PlaceHolder1;
                PlaceHolder1 = Components1[8];
                Components1[8] = Components1[9];
                Components1[9] = PlaceHolder1;
                PlaceHolder1 = Components1[10];
                Components1[10] = Components1[11];
                Components1[11] = PlaceHolder1;
                Components1[12] = Username;
                PlaceHolder1 = Components1[13];
                Components1[13] = Components1[14];
                Components1[14] = PlaceHolder1;
                PlaceHolder1 = Components1[15];
                Components1[15] = Components1[16];
                Components1[16] = PlaceHolder1;
                PlaceHolder1 = Components1[17];
                Components1[17] = Components1[18];
                Components1[18] = PlaceHolder1;
                Replay = string.Join("/", Components1);
                Database.SetReplay(OtherPlayerUsername, Replay + "/" + 0);
                if(!ClientObject.UsernamesDisc.ContainsKey(Username))
                    ServerTCP.PACKET_Replay(ConnectionID, Username);
                if (!ClientObject.UsernamesDisc.ContainsKey(OtherPlayerUsername))
                    ServerTCP.PACKET_Replay(LooserConnID, OtherPlayerUsername);
                */
                if (Friendly)
                    RoomInstance.instance.LeaveFriendly(ConnectionID);
                else
                {
                    RoomInstance.instance.LeaveRoom(ConnectionID, GameMode, ServerTCP.TempPlayers[ConnectionID].Room);
                    Database.UpdateProfile(Username);
                    Database.UpdateProfile(OtherPlayerUsername);
                }
                return;
            }
            string PP1str = Database.GetPP(Username);
            string[] PP1Array = PP1str.Split(',');
            int[] PP1Arrayint = new int[PP1Array.Length];
            int i = 0;
            foreach (string c in PP1Array)
            {
                Int32.TryParse(c, out PP1Arrayint[i]);
                i++;
            }
            int PP1 = PP1Arrayint[ShapeID];
            string PP2str = Database.GetPP(OtherPlayerUsername);
            string[] PP2Array = PP2str.Split(',');
            int[] PP2Arrayint = new int[PP2Array.Length];
            i = 0;
            foreach (string c in PP2Array)
            {
                Int32.TryParse(c, out PP2Arrayint[i]);
                i++;
            }
            int PP2 = PP2Arrayint[OtherShapeID];
            int PPdiff = PP1 - PP2;                 
            int PPAdded = 0;
            if (PPdiff >= 10) { PPAdded = 1; }                         
            if (PPdiff < 10 && PPdiff >= 5) { PPAdded = 2; }
            if (PPdiff < 5 && PPdiff >= 0) { PPAdded = 3; }
            if (PPdiff < 0 && PPdiff >= -5) { PPAdded = 4; }
            if (PPdiff < -5 && PPdiff >= -10) { PPAdded = 5; }
            if (PPdiff < -10) { PPAdded = 6; }    
            if (GameMode == 2) { PPAdded = 0; }         //2v2 is not competitve
            #region OldReplayCode
            /*Database.SetReplay(Username, Replay + "/" + PPAdded);
            string[] Components = Replay.Split('/');
            string PlaceHolder;
            PlaceHolder = Components[0];
            Components[0] = Components[1];
            Components[1] = PlaceHolder;
            PlaceHolder = Components[2];
            Components[2] = Components[3];
            Components[3] = PlaceHolder;
            PlaceHolder = Components[4];
            Components[4] = Components[5];
            Components[5] = PlaceHolder;
            PlaceHolder = Components[6];
            Components[6] = Components[7];
            Components[7] = PlaceHolder;
            PlaceHolder = Components[8];
            Components[8] = Components[9];
            Components[9] = PlaceHolder;
            PlaceHolder = Components[10];
            Components[10] = Components[11];
            Components[11] = PlaceHolder;
            Components[12] = Username;
            PlaceHolder = Components[13];
            Components[13] = Components[14];
            Components[14] = PlaceHolder;
            PlaceHolder = Components[15];
            Components[15] = Components[16];
            Components[16] = PlaceHolder;
            PlaceHolder = Components[17];
            Components[17] = Components[18];
            Components[18] = PlaceHolder;
            Replay = string.Join("/", Components);
            Database.SetReplay(OtherPlayerUsername, Replay + "/" + -PPAdded);*/
            #endregion
            Database.SetWinstreak(Username, 1);
            Database.SetWinstreak(OtherPlayerUsername, 0);
            Database.SetRecentIDs(Username, ShapeID);
            Database.SetRecentIDs(OtherPlayerUsername, -1);
            RoomInstance.instance.LeaveRoom(ConnectionID, GameMode, ServerTCP.TempPlayers[ConnectionID].Room);
            Database.SetPP(Username, PP1 + PPAdded, ShapeID);
            Database.SetPP(OtherPlayerUsername, PP2 - PPAdded, OtherShapeID);
            if (!ClientObject.UsernamesDisc.ContainsKey(Username))
                ServerTCP.PACKET_Endgame(ConnectionID, Username);
            if (!ClientObject.UsernamesDisc.ContainsKey(OtherPlayerUsername))
                ServerTCP.PACKET_Endgame(LooserConnID, OtherPlayerUsername);
            Database.UpdateProfile(Username);
            Database.UpdateProfile(OtherPlayerUsername);
            buffer.Dispose();
        }
        private static void HandleDebug(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            string msg = buffer.ReadString();
            if (msg == "No")
            {
                string Username = buffer.ReadString();
                int OtherConnectionID;
                ClientObject.ConnectionIDs.TryGetValue(Username, out OtherConnectionID);
                RoomInstance.instance.LeaveFriendly(OtherConnectionID, true);
                return;
            }
            else if (msg == "SpectatorOut")
            {
                if (RoomInstance._room[ServerTCP.TempPlayers[ConnectionID].SpecRoom].SpectatorIDs.Count != 0)
                {
                    RoomInstance._room[ServerTCP.TempPlayers[ConnectionID].SpecRoom].SpectatorIDs.Remove(ConnectionID);
                    ServerTCP.TempPlayers[ConnectionID].Spectating = false;
                    ServerTCP.TempPlayers[ConnectionID].SpecRoom = 0;
                }
            }
            else if (msg == "ResetW")
            {
                string Username = buffer.ReadString();
                Database.SetRecentIDs(Username, -1);
                Database.SetWinstreak(Username, 0);
                return;
            }
            else if (msg == "SendChest")
            {
                string Username = buffer.ReadString();
                int ChestNum = buffer.ReadInteger();
                ServerTCP.PACKET_Chest(ConnectionID, Database.GetChest(Username, ChestNum), ChestNum);
                return;
            }
            else if (msg == "Test")
            {
                return;
            }
            else if (msg == "GiveLP")
            {
                int ConnID = buffer.ReadInteger();
                ServerTCP.PACKET_SendMessage(ConnID, "LifePoints", ConnectionID,"",buffer.ReadInteger());
                return;
            }
            else if (msg == "CheckIP")
            {
                string[] IPUsername = buffer.ReadString().Split('/');
                if (ServerTCP.ClientObjects[ConnectionID].OldUsername != IPUsername[1])
                {
                    ServerTCP.PACKET_SendMessage(ConnectionID, "Connect");
                    ServerTCP.ClientObjects[ConnectionID].StopReading = true;
                    ServerTCP.ClientObjects[ConnectionID].CloseConnection();
                }
                ServerTCP.IPDictionary.TryRemove(IPUsername[0], out ClientObject ws);
                return;
            }
            else if (msg == "Profile")
            {
                string Username = buffer.ReadString();
                int version = buffer.ReadInteger();
                ServerTCP.PACKET_Profile(ConnectionID, Username, version);
            }
            else if (msg == "LastDeck")
            {
                string[] LastDeckAr = buffer.ReadString().Split(',');
                string[] newArr = new string[] { LastDeckAr[1], LastDeckAr[2], LastDeckAr[3], LastDeckAr[4], LastDeckAr[5], LastDeckAr[6], LastDeckAr[7], LastDeckAr[8] };
                Database.SetLastDeck(LastDeckAr[0], String.Join(",", newArr));
            }
            else if (msg == "OC")   //Online Check
            {
                int pNum = buffer.ReadInteger();
                int room = ServerTCP.TempPlayers[ConnectionID].Room;
                if (room > 0) { RoomInstance._room[room].OnlineCheck[pNum - 1] = true; }
                buffer.Dispose();
                return;
            }
            else if (msg == "ShapeChoice")
            {
                Database.SetLvl(buffer.ReadString(), 1, buffer.ReadInteger());
                ServerTCP.PACKET_SendMessage(ConnectionID, "Done");
                buffer.Dispose();

                return;
            }
            else if (msg == "RF")    //Remove Friend
            {
                string[] Usernames = buffer.ReadString().Split(':');
                string Friends = Database.GetFriends(Usernames[0], true);
                string[] List = Friends.Split(',');
                string[] newList = new string[List.Length - 1];
                int i = 0;
                foreach (string c in List)
                {
                    if (!c.Equals(Usernames[1])) { newList[i++] = c; }
                }
                Database.AddFriend(Usernames[0], String.Join(",", newList), true);
                ServerTCP.PACKET_GiveList(ConnectionID, Database.GetFriends(Usernames[0]));
                buffer.Dispose();
                return;
            }
            else if (msg == "BotReconnectString")
            {
                string ReconnectString = buffer.ReadString();
                RoomInstance._room[buffer.ReadInteger()].ReconnectString = ReconnectString;
                buffer.Dispose();
                return;
            }
            else if (msg == "BotSpectate")
            {               
                string[] BotArray = buffer.ReadString().Split('|');
                Room R = RoomInstance._room[buffer.ReadInteger()];
                Int32.TryParse(BotArray[3], out int ID); Int32.TryParse(BotArray[0], out int PP); Int32.TryParse(BotArray[1], out int Lvl);
                R.ConnectionID[1] = -1; R.Usernames.Add(-1, BotArray[4]); R.ShapeChosenIDs.Add(-1, ID); R.PPs[1] = PP; R.Levels[1] = Lvl; R.BotAbilities = BotArray[2];
                buffer.Dispose();
                return;
            }
            else if(msg == "Offline")
            {
                string username = buffer.ReadString();
                if (Database.AccountExists(username))
                {
                    Database.SetOnline(username, false);
                }
                buffer.Dispose();
                return;
            }
            else if(msg == "Online")
            {
                string username = buffer.ReadString();
                if (Database.AccountExists(username))
                {
                    Database.SetOnline(username, true);
                }
                buffer.Dispose();
                return;
            }
            Console.WriteLine(msg);
            buffer.Dispose();
        }
        private static void HandleSendingLeaderboard(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int ShapeID = buffer.ReadInteger();
            ServerTCP.PACKET_Leaderboard(ConnectionID, ShapeID);
            buffer.Dispose();
        }
        private static void HandleGivingAbilities(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            string username = buffer.ReadString();
            ServerTCP.PACKET_GiveAbilities(ConnectionID, username);
            buffer.Dispose();
        }
        private static void HandleMainMenu(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            bool Searching = buffer.ReadBool();
            int GameMode = buffer.ReadInteger();
            if (Searching)
            {
                if (ServerTCP.TempPlayers[ConnectionID].Room > 0)
                    RoomInstance.instance.LeaveRoom(ConnectionID, GameMode, ServerTCP.TempPlayers[ConnectionID].Room);
                else
                    ServerTCP.TempPlayers[ConnectionID].Stop = true;
            }
            else
            {
                if (ServerTCP.TempPlayers[ConnectionID].Room > 0)
                {
                    RoomInstance.instance.LeaveRoom(ConnectionID, GameMode, ServerTCP.TempPlayers[ConnectionID].Room);
                }
            }
            buffer.Dispose();
        }
        private static void HandleGivingStats(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            ServerTCP.PACKET_GiveStats(ConnectionID);
            buffer.Dispose();
        }
        private static void HandleEmotes(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int ID = buffer.ReadInteger();
            int ConnID = buffer.ReadInteger();
            string Username = buffer.ReadString();
            ServerTCP.PACKET_Emotes(ConnID, ID);
            if (Username != "")
            {
                /*if (RoomInstance._room[ServerTCP.TempPlayers[ConnectionID].Room].SpectatorIDs != "")
                {
                    string[] Spec = RoomInstance._room[ServerTCP.TempPlayers[ConnectionID].Room].SpectatorIDs.Split(',');
                    int[] Spectators = new int[Spec.Length];
                    int i = 0;
                    foreach (string s in Spec)
                    {
                        Int32.TryParse(s, out Spectators[i]);
                        i++;
                    }
                    foreach (int c in Spectators)
                    {
                        ServerTCP.PACKET_Emotes(c,ID, Username);
                    }
                }*/

                int room = ServerTCP.TempPlayers[ConnectionID].Room;
                if (RoomInstance._room[room].SpectatorIDs.Count != 0)
                {
                    foreach (int c in RoomInstance._room[room].SpectatorIDs)
                    {
                        ServerTCP.PACKET_Emotes(c, ID, Username);
                    }
                }
            }
            buffer.Dispose();
        }
        private static void HandleAddingFriend(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            string Code = buffer.ReadString();
            string Username = buffer.ReadString();
            string Friend = Database.GetUsername(Code);
            string Friends = Database.GetFriends(Username, true);
            string[] List = Friends.Split(',');

            if (Friend == "") { ServerTCP.PACKET_SendMessage(ConnectionID, "IC"); }
            else
            {
                foreach (string c in List)
                {
                    if (c == Friend)
                    {
                        ServerTCP.PACKET_SendMessage(ConnectionID, "FAA");
                        buffer.Dispose();
                        return;
                    }
                }

                ServerTCP.PACKET_SendMessage(ConnectionID, "FA");
                Database.AddFriend(Username, Friend);
                ServerTCP.PACKET_GiveList(ConnectionID, Database.GetFriends(Username));
            }
            buffer.Dispose();
        }
        private static void HandleFriendList(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            string Username = buffer.ReadString();
            string Friends = Database.GetFriends(Username);
            ServerTCP.PACKET_GiveList(ConnectionID, Friends);
            buffer.Dispose();
        }
        private static void HandleFriendCheck(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            string Username = buffer.ReadString();
            string PlayerUsername = buffer.ReadString();

            if (!Database.Online(Username)) { ServerTCP.PACKET_SendMessage(ConnectionID, "UN"); return; }
            bool inGame = false;
            if (ClientObject.ConnectionIDs.ContainsKey(Username))
                inGame = ServerTCP.TempPlayers[ClientObject.ConnectionIDs[Username]].inMatch;
            if (inGame) { ServerTCP.PACKET_SendMessage(ConnectionID, "UN"); return; }

            string[] Friends = Database.GetFriends(Username, true).Split(',');
            buffer.Dispose();
            foreach (string c in Friends)
            {
                if (c == PlayerUsername)
                {
                    ServerTCP.PACKET_SendMessage(ConnectionID, "Yes");
                    return;
                }
            }
            ServerTCP.PACKET_SendMessage(ConnectionID, "No");
        }
        private static void HandleSpectate(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            string Username = buffer.ReadString();      //Username of the in-game player
            int ConnID = ClientObject.ConnectionIDs[Username];

            if (ServerTCP.TempPlayers[ConnID].inMatch)
            {
                int RoomNum = ServerTCP.TempPlayers[ConnID].Room;
                Room curR = RoomInstance._room[RoomNum];
                if (curR.ConnectionID[0] == ConnID)
                    ServerTCP.PACKET_Spectate(ConnectionID, curR.Usernames[ConnID], curR.Usernames[curR.ConnectionID[1]],
                        ConnID, curR.ConnectionID[1], curR.ShapeChosenIDs[ConnID], curR.ShapeChosenIDs[curR.ConnectionID[1]],
                        curR.PPs[0], curR.PPs[1], curR.Levels[0], curR.Levels[1], curR.Bot? curR.BotAbilities : "");
                else
                    ServerTCP.PACKET_Spectate(ConnectionID, curR.Usernames[ConnID], curR.Usernames[curR.ConnectionID[0]],
                        ConnID, curR.ConnectionID[0], curR.ShapeChosenIDs[ConnID], curR.ShapeChosenIDs[curR.ConnectionID[0]],
                        curR.PPs[1], curR.PPs[0], curR.Levels[1], curR.Levels[0]);

                RoomInstance._room[RoomNum].SpectatorIDs.Add(ConnectionID);
                Console.WriteLine("SpectatorID added " + ConnectionID + " to room number " + RoomNum);
                ServerTCP.TempPlayers[ConnectionID].Spectating = true;
                ServerTCP.TempPlayers[ConnectionID].SpecRoom = RoomNum;
            }
            else { ServerTCP.PACKET_SendMessage(ConnectionID, "NIG"); }
            buffer.Dispose();
        }
        private static void HandleLP(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int LP1 = buffer.ReadInteger();
            int LP2 = buffer.ReadInteger();
            int ConnID = buffer.ReadInteger();
            string ReconnectString = buffer.ReadString();
            ServerTCP.PACKET_LP(ConnID, LP1, LP2, ReconnectString,buffer.ReadInteger(),buffer.ReadInteger(),buffer.ReadInteger(),buffer.ReadInteger());
            buffer.Dispose();
        }
        private static void HandleChest(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int Time = buffer.ReadInteger();
            string Username = buffer.ReadString();
            int ChestNum = buffer.ReadInteger();
            Database.SetChest(Username, Time, ChestNum);
            buffer.Dispose();
        }
        private static void HandleChestOpening(int ConnectionID, byte[] data)
        {            
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            string Username = buffer.ReadString();
            int Gold = buffer.ReadInteger();
            int Redbolts = buffer.ReadInteger();
            int Diamonds = buffer.ReadInteger();
            string Abilities = buffer.ReadString();
            string Super100 = buffer.ReadString();
            string Super200 = buffer.ReadString();
            string Level = buffer.ReadString();
            string ChestSlots = buffer.ReadString();
            string XP = buffer.ReadString();
            string Bg = buffer.ReadString();
            string Emotes = buffer.ReadString();
            string Skins = buffer.ReadString();
            string TrophyRoad = buffer.ReadString();
            if (buffer.ReadBool()) { Database.SetDailyRewards(Username, 0); Database.UpdateLastSpin(Username, false); }
            Database.SetAdditionalRewards(Username, buffer.ReadString());
            Database.SetGold(Username, Gold);
            Database.SetRedbolts(Username, Redbolts);
            Database.SetDiamonds(Username, Diamonds);
            Database.SetAbilities(Username, Abilities);
            Database.SetSuper100(Username, Super100);
            Database.SetSuper200(Username, Super200);
            Database.SetLvl(Username, -1, -1, Level);
            Database.SetChestSlots(Username, ChestSlots);
            Database.SetXP(Username, -1, -1, XP);
            Database.SetBackgrounds(Username, Bg);
            Database.SetEmotes(Username, Emotes);
            Database.SetSkins(Username, Skins);
            Database.SetTrophyRoad(Username, TrophyRoad);
            Database.UpdateProfile(Username);
            buffer.Dispose();
        }
        private static void HandleReconnecting(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int OtherLife = buffer.ReadInteger();
            int OtherID = 0;
            int Index = 0;

            Room room = RoomInstance._room[ServerTCP.TempPlayers[ConnectionID].Room];
            foreach (int ID in room.ConnectionID)
            {
                if (ID != ConnectionID) { OtherID = ID; break; }
                else { Index++; }
            }
            room.Usernames.TryGetValue(OtherID, out string Username);
            if (OtherLife == -1)
            {
                ClientObject.UsernamesDisc.TryRemove(Username, out int[] aba);
                return;
            }
            int Life = buffer.ReadInteger();
            int OtherEP = buffer.ReadInteger();
            int EP = buffer.ReadInteger();
            int Round = buffer.ReadInteger();
            string OtherChoices = buffer.ReadString();
            string Choices = buffer.ReadString();
            string OtherProbs = buffer.ReadString();
            string Probs = buffer.ReadString();
            int Mode = buffer.ReadInteger();
            int ConnID = buffer.ReadInteger();
            string Username2 = buffer.ReadString();
            string ReconnectString = buffer.ReadString();
            int ChoiceID = buffer.ReadInteger();

            room.ConnectionID[Index] = ConnID;
            room.Usernames.Remove(OtherID);
            room.Usernames.Add(ConnID, Username);
            int ShapeID = room.ShapeChosenIDs[OtherID];
            room.ShapeChosenIDs.Remove(OtherID);
            room.ShapeChosenIDs.Add(ConnID, ShapeID);
            ServerTCP.PACKET_Reconnect(ConnID, Life, OtherLife, EP, OtherEP, Round, Choices, OtherChoices, Probs, OtherProbs, Mode, Index, Username2, ConnectionID,ReconnectString,ChoiceID);
            ClientObject.UsernamesDisc.TryRemove(Username, out int[] aa);
            ServerTCP.TempPlayers[ConnID].inMatch = true;
            ServerTCP.TempPlayers[ConnID].Room = ServerTCP.TempPlayers[ConnectionID].Room;
            buffer.Dispose();
        }
        private static void HandleAdReward(int ConnectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            string Username = buffer.ReadString();
            int Gold = buffer.ReadInteger();
            int Redbolts = buffer.ReadInteger();
            int Diamonds = buffer.ReadInteger();
            int timeLeftBeforeReset = buffer.ReadInteger();
            Database.SetGold(Username, Gold);
            Database.SetRedbolts(Username, Redbolts);
            Database.SetDiamonds(Username, Diamonds);
            if (timeLeftBeforeReset != -1)      //it is -1 when we just want to update gold/rb/d
            {
                string AdsRem = buffer.ReadString();
                Database.SetAdsRem(Username, timeLeftBeforeReset, AdsRem);
            }
            buffer.Dispose();
        }
    }
}