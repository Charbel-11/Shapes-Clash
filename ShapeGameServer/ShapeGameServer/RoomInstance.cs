using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGameServer
{
    class RoomInstance
    {
        public static ConcurrentDictionary<int, int> ppThere = new ConcurrentDictionary<int, int>();
        public static ConcurrentDictionary<int, int> ppThere2 = new ConcurrentDictionary<int, int>();   //For 2v2 shape games

        //Represents empty rooms
        public static ConcurrentQueue<int> unusedRooms = new ConcurrentQueue<int>();

        public static Room[] _room = new Room[Constants.Max_Rooms];
        public static RoomInstance instance = new RoomInstance();

        private Random R = new Random();

        //Returns the PP of a corresponding player which was already searching for a game
        public int SearchForPlayer(int curPP, int GameMode)
        {
            if (GameMode == 1)
            {
                if (ppThere.ContainsKey(curPP) && !_room[ppThere[curPP]].DontJoin)
                    return curPP;
                for (int j = 1; j < 15; j++)
                {
                    if (ppThere.ContainsKey(curPP - j) && !_room[ppThere[curPP - j]].DontJoin)
                        return curPP - j;
                    if (ppThere.ContainsKey(curPP + j) && !_room[ppThere[curPP + j]].DontJoin)
                        return curPP + j;
                }
            }
            else if (GameMode == 2)     //curPP and otherPP are the sum of 2 shape's PP in this game mode
            {
                if (ppThere2.ContainsKey(curPP) && !_room[ppThere2[curPP]].DontJoin)
                    return curPP;
                for (int j = 1; j < 15; j++)
                {
                    if (ppThere2.ContainsKey(curPP - j) && !_room[ppThere2[curPP - j]].DontJoin)
                        return curPP - j;
                    if (ppThere2.ContainsKey(curPP + j) && !_room[ppThere2[curPP + j]].DontJoin)
                        return curPP + j;
                }
            }
            return -1;
        }

        public void JoinOrCreateRoom(int connectionID, string username, int ShapeChosenID, int ShapeChosenID2, string AbilitiesID, string AbilitiesID2, int SuperID, int SuperID2, int GameMode, int SkinID, int SkinID2)
        { 
            if (ServerTCP.TempPlayers[connectionID].Room > 0)
            {
                Console.WriteLine("Player already in room");
                return;
            }
            if (ServerTCP.TempPlayers[connectionID].Stop)
            {
                ServerTCP.TempPlayers[connectionID].Stop = false;
                return;
            }

            int curPP = 0, curPP2 = -1, roomNum = -1;
            string curPPstr = Database.GetPP(username);
            string[] curPParray = curPPstr.Split(',');
            Int32.TryParse(curPParray[ShapeChosenID], out curPP);
            if (GameMode == 2)
            {
                Int32.TryParse(curPParray[ShapeChosenID2], out curPP2);
                curPP += curPP2;
            }

            int otherPP = SearchForPlayer(curPP, GameMode);
            bool foundMatch = otherPP != -1;

            if (ServerTCP.TempPlayers[connectionID].Stop)
            {
                ServerTCP.TempPlayers[connectionID].Stop = false;
                return;
            }

            if (foundMatch)
            {
                if (GameMode == 1)
                {
                    bool b = ppThere.TryRemove(otherPP, out roomNum);
                    if (!b) { foundMatch = false; }
                }
                else if (GameMode == 2)
                {
                    bool b = ppThere2.TryRemove(otherPP, out roomNum);
                    if (!b) { foundMatch = false; }
                }
                if (!foundMatch) { JoinOrCreateRoom(connectionID, username, ShapeChosenID, ShapeChosenID2, AbilitiesID, AbilitiesID2, SuperID, SuperID2, GameMode, SkinID, SkinID2); return; }

                ServerTCP.TempPlayers[_room[roomNum].ConnectionID[0]].inMatch = true;
                ServerTCP.TempPlayers[connectionID].inMatch = true;

                //Room already has the info of the other player, now we the current player's info
                _room[roomNum].ConnectionID[1] = connectionID;
                _room[roomNum].Usernames.Add(connectionID, username);
                _room[roomNum].ShapeChosenIDs.Add(connectionID, ShapeChosenID);
                _room[roomNum].ShapeChosenIDs2.Add(connectionID, ShapeChosenID2);
                _room[roomNum].AbIDs[1] = AbilitiesID;
                _room[roomNum].AbIDs2[1] = AbilitiesID2;
                _room[roomNum].PPs[1] = curPP;
                _room[roomNum].PPs2[1] = curPP2;
                _room[roomNum].SuperIDs[1] = SuperID;
                _room[roomNum].SuperIDs2[1] = SuperID2;
                _room[roomNum].GameMode = GameMode;
                _room[roomNum].SkinArray[2] = SkinID;
                _room[roomNum].SkinArray[3] = SkinID2;

                string level1 = Database.GetLevel(username);
                int levelShapeID1 = 0, levelShapeID2 = 0;
                string[] lvl1 = level1.Split(',');
                Int32.TryParse(lvl1[ShapeChosenID], out levelShapeID1);
                if (ShapeChosenID2 >= 0 && ShapeChosenID2 < 4) { Int32.TryParse(lvl1[ShapeChosenID2], out levelShapeID2); }
                _room[roomNum].Levels[1] = levelShapeID1;
                _room[roomNum].Levels2[1] = levelShapeID2;

                ServerTCP.TempPlayers[connectionID].Room = roomNum;
               
                Console.WriteLine("Room joined: " + roomNum + " | Player with " + curPP + " PP added: " + " connectionID: " + connectionID);

                ServerTCP.PACKET_SendMessage(connectionID, "OC", 2);
                ServerTCP.PACKET_SendMessage(_room[roomNum].ConnectionID[0], "OC", 1);
                Task.Delay(2000).ContinueWith(t => SendToBattle(roomNum));      

                return;
            }

            //Didn't find a match
            bool b2 = unusedRooms.TryDequeue(out roomNum);        //Get an unused room num
            if (!b2) { Console.WriteLine("All rooms are full..."); return; }

            _room[roomNum].ConnectionID[0] = connectionID;
            _room[roomNum].Usernames.Add(connectionID, username);
            _room[roomNum].ShapeChosenIDs.Add(connectionID, ShapeChosenID);
            _room[roomNum].ShapeChosenIDs2.Add(connectionID, ShapeChosenID2);
            _room[roomNum].AbIDs[0] = AbilitiesID;
            _room[roomNum].AbIDs2[0] = AbilitiesID2;
            _room[roomNum].PPs[0] = curPP;
            _room[roomNum].PPs2[0] = curPP2;
            _room[roomNum].SuperIDs[0] = SuperID;
            _room[roomNum].SuperIDs2[0] = SuperID2;
            _room[roomNum].GameMode = GameMode;
            _room[roomNum].SkinArray[0] = SkinID;
            _room[roomNum].SkinArray[1] = SkinID2;
            string Level1= Database.GetLevel(username);
            int LevelShapeID1, LevelShapeID2 = 0;
            string[] Lvl1= Level1.Split(',');
            Int32.TryParse(Lvl1[ShapeChosenID],out LevelShapeID1);
            if (ShapeChosenID2>=0 && ShapeChosenID2 < 4) { Int32.TryParse(Lvl1[ShapeChosenID2], out LevelShapeID2); }
            _room[roomNum].Levels[0] = LevelShapeID1;
            _room[roomNum].Levels2[0] = LevelShapeID2;

            ServerTCP.TempPlayers[connectionID].Room = roomNum;

            if (GameMode == 1) ppThere.TryAdd(curPP, roomNum);
            else if (GameMode == 2) ppThere2.TryAdd(curPP, roomNum);

            Console.WriteLine("Room created: " + roomNum + " | Player with " + curPP + " PP added: " + " connectionID: " + connectionID);

            if (GameMode == 1)
            {
                int Time = R.Next(11, 16) * 1000;
                Task.Delay(Time - 2000).ContinueWith(t => ServerTCP.PACKET_SendMessage(connectionID, "OC", 1));
                Task.Delay(Time).ContinueWith(t => MatchWithBot(roomNum, curPP, connectionID));
            }
        }

        private void MatchWithBot(int roomNum, int curPP, int connectionID)
        {
            TempPlayer curP = ServerTCP.TempPlayers[connectionID];
            if (curP.inMatch || curP.Room != roomNum)
                return;
            else if (!_room[roomNum].OnlineCheck[0])
            {
                LeaveRoom(connectionID, _room[roomNum].GameMode, roomNum);
                if (ServerTCP.ClientObjects[connectionID].Socket != null)
                    ServerTCP.PACKET_SendMessage(connectionID, "SS");
                return;
            }

            bool b = ppThere.TryRemove(curPP, out int roomN);
            if (!b) { return; }
            if (roomN != roomNum) { ppThere.TryAdd(curPP, roomN); return; }

            curP.inMatch = true;
            _room[roomNum].DontJoin = true;
            _room[roomNum].Bot = true;
            ServerTCP.PACKET_SendToMatchmaking(connectionID, "", -1, -1, -1, "", "", -1, -1, -1, -1, -1, -1, -1, -1, 0, true);
        }

        public string HandleBotReconnect(int roomNum, int connectionID)
        {
            TimeSpan timedifference = DateTime.Now - _room[roomNum].DiscRecTimes[0] ;
            TempPlayer curP = ServerTCP.TempPlayers[connectionID];
            curP.inMatch = true;
            curP.Room = roomNum;
            if (timedifference.TotalSeconds < 10)
            {
                _room[roomNum].ShapeChosenIDs.Add(connectionID, _room[roomNum].ShapeChosenIDs[_room[roomNum].ConnectionID[0]]);
                _room[roomNum].ShapeChosenIDs.Remove(_room[roomNum].ConnectionID[0]);
                _room[roomNum].Usernames.Add(connectionID, _room[roomNum].Usernames[_room[roomNum].ConnectionID[0]]);
                _room[roomNum].Usernames.Remove(_room[roomNum].ConnectionID[0]);
                _room[roomNum].ConnectionID[0] = connectionID;
                _room[roomNum].DiscRecTimes = new DateTime[2];
                return _room[roomNum].ReconnectString;
            }
            string[] ReconnectArray = _room[roomNum].ReconnectString.Split(',');
            Int32.TryParse(ReconnectArray[76], out int shapeID2);
            int[] ablevels1 = SArrayToIArray(Database.GetAbilities(_room[roomNum].Usernames[_room[roomNum].ConnectionID[0]]).Split(','));
            int lvl = _room[roomNum].Levels[0];
            int[] ablevels2 = SArrayToIArray(ReconnectArray[77].Split('|'));
            int attack =  AbilitiesStoringClass.AbilitiesArray[new int[] { 1, 22, 23, 42 }[shapeID2]][ablevels2[new int[] { 1, 22, 23, 42 }[shapeID2]] - 1] + AbilitiesStoringClass.LevelStatsArray[lvl][shapeID2][0];
            int roundsGone = (int)(timedifference.TotalSeconds / 7);
            List<bool>[] dmgdone = { new List<bool>(), new List<bool>() } ;
            Int32.TryParse(ReconnectArray[70], out int round);
            int EPToAdd = 0;
            Int32.TryParse(ReconnectArray[68], out int LP1);
            Int32.TryParse(ReconnectArray[69], out int LP2);
            bool botdisc = false;
            int[] matePow = new int[2];
            int[] mateLP = new int[2];
            Int32.TryParse(ReconnectArray[14], out matePow[0]);
            Int32.TryParse(ReconnectArray[15], out matePow[1]);
            Int32.TryParse(ReconnectArray[16], out mateLP[0]);
            Int32.TryParse(ReconnectArray[17], out mateLP[1]);
            if (ReconnectArray[12] == "1" && ReconnectArray[13] == "1")
            {
                int attack1 = AbilitiesStoringClass.LevelStatsArray[lvl][_room[roomNum].ShapeChosenIDs[_room[roomNum].ConnectionID[0]]][0] + matePow[0];
                attack += matePow[1] + AbilitiesStoringClass.LevelStatsArray[lvl][shapeID2][0];
                int diff = attack - attack1;
                int r = roundsGone;
                if(diff < 0)
                {
                    while(mateLP[1] > 0 && r > 0)
                    {
                        mateLP[1] += diff;
                        r--;
                        dmgdone[0].Add(false);
                        dmgdone[1].Add(false);
                    }
                    if(mateLP[1] <= 0)
                    {
                        ReconnectArray[13] = "0";
                        LP2 += mateLP[1]; mateLP[1] = 0; dmgdone[1].RemoveAt(dmgdone[1].Count - 1); dmgdone[1].Add(true);                     
                        diff -= matePow[1];
                        LP2 += r * diff;
                        while(r > 0)
                        {
                            dmgdone[1].Add(true);
                            dmgdone[0].Add(false);
                            r--;
                        }
                    }                   
                }
                else if(diff > 0)
                {
                    while (mateLP[0] > 0 && r > 0)
                    {
                        mateLP[0] -= diff;
                        r--;
                        dmgdone[0].Add(false);
                        dmgdone[1].Add(false);
                    }
                    if (mateLP[0] <= 0)
                    {
                        ReconnectArray[12] = "0";
                        LP1 += mateLP[0]; mateLP[0] = 0;
                        diff += matePow[0];
                        LP1 -= r * diff;
                        while(r > 0)
                        {
                            dmgdone[0].Add(true);
                            dmgdone[1].Add(false);
                            r--;
                        }
                    }
                }
                else
                {
                    botdisc = true;
                    diff -= (AbilitiesStoringClass.AbilitiesArray[new int[] { 1, 22, 23, 42 }[shapeID2]][ablevels2[new int[] { 1, 22, 23, 42 }[shapeID2]] - 1] + AbilitiesStoringClass.LevelStatsArray[lvl][shapeID2][0]);
                    while (mateLP[1] > 0 && r > 0)
                    {
                        mateLP[1] += diff;
                        r--;
                        dmgdone[0].Add(false);
                        dmgdone[1].Add(false);
                    }
                    if (mateLP[1] <= 0)
                    {
                        ReconnectArray[13] = "0";
                        LP2 += mateLP[1]; mateLP[1] = 0;
                        diff -= matePow[1];
                        LP2 += r * diff;
                        while (r > 0)
                        {
                            dmgdone[1].Add(true);
                            dmgdone[0].Add(false);
                            r--;
                        }
                    }
                }
            }
            else if(ReconnectArray[12] == "1")
            {
                int attack1 = AbilitiesStoringClass.LevelStatsArray[lvl][_room[roomNum].ShapeChosenIDs[_room[roomNum].ConnectionID[0]]][0] + matePow[0];
                int diff = attack - attack1;
                int r = roundsGone;
                if (diff < 0)
                {
                    LP2 += r * diff;
                    while (r > 0)
                    {
                        dmgdone[1].Add(true);
                        dmgdone[0].Add(false);
                        r--;
                    }
                }
                else if (diff > 0)
                {
                    while (mateLP[0] > 0 && r > 0)
                    {
                        mateLP[0] -= diff;
                        r--;
                        dmgdone[1].Add(false);
                        dmgdone[0].Add(false);
                    }
                    if (mateLP[0] <= 0)
                    {
                        ReconnectArray[12] = "0";
                        LP1 += mateLP[0]; mateLP[0] = 0;
                        diff += matePow[0];
                        LP1 -= r * diff;
                        while (r > 0)
                        {
                            dmgdone[1].Add(false);
                            dmgdone[0].Add(true);
                            r--;
                        }
                    }
                }
                else
                {
                    botdisc = true;
                    diff -= (AbilitiesStoringClass.AbilitiesArray[new int[] { 1, 22, 23, 42 }[shapeID2]][ablevels2[new int[] { 1, 22, 23, 42 }[shapeID2]] - 1] + AbilitiesStoringClass.LevelStatsArray[lvl][shapeID2][0]);
                    LP2 += r * diff;
                    while (r > 0)
                    {
                        dmgdone[1].Add(true);
                        dmgdone[0].Add(false);
                        r--;
                    }
                }
            }
            else
            {
                int r = roundsGone;
                if(ReconnectArray[13] == "1")
                    attack += matePow[1] + AbilitiesStoringClass.LevelStatsArray[lvl][shapeID2][0];
                LP1 -= r * attack;
                while (r > 0)
                {
                    dmgdone[0].Add(true);
                    dmgdone[1].Add(false);
                    r--;
                }
            }
            if(LP1 < 0) { LP1 = 0; }
            if(LP2 < 0) { LP2 = 0; }
            ReconnectArray[68] = LP1.ToString();
            ReconnectArray[69] = LP2.ToString();
            List<int> ChoicesP1 = new List<int>(SArrayToIArray(ReconnectArray[71].Split('/')));
            List<int> ChoicesP2 = new List<int>(SArrayToIArray(ReconnectArray[72].Split('/')));
            List<int> ProbsP1 = new List<int>(SArrayToIArray(ReconnectArray[73].Split('/')));
            List<int> ProbsP2 = new List<int>(SArrayToIArray(ReconnectArray[74].Split('/')));
            List<int>[] Probs = AddPassives(dmgdone, ProbsP1, ProbsP2, _room[roomNum].Usernames[_room[roomNum].ConnectionID[0]], _room[roomNum].ShapeChosenIDs[_room[roomNum].ConnectionID[0]], shapeID2);
            for (int j = 1; j <= roundsGone; j++)
            {
                if (round + j < 4)
                    EPToAdd++;
                else if (round + j < 9)
                {
                    EPToAdd += 2;
                }
                else if (round + j < 14)
                {
                    EPToAdd += 3;
                }
                else
                {
                    EPToAdd += 4;
                }
                ChoicesP1.Add(0);
                ChoicesP2.Add(botdisc ? 0 : new int[] { 1, 22, 23, 42 }[shapeID2]);
            }
            for (int i = 0; i < ReconnectArray.Length; i++)
            {
                if (i <= 11 || (i>=18 && i<=65))
                    ReconnectArray[i] = "0";
                else if(i == 66 || i == 67)
                {
                    Int32.TryParse(ReconnectArray[i], out int k);
                    k += EPToAdd;
                    ReconnectArray[i] = k.ToString();
                }
                else if(i == 16)
                {
                    ReconnectArray[i] = mateLP[0].ToString();
                }
                else if(i == 17)
                {
                    ReconnectArray[i] = mateLP[1].ToString();
                }
                else if (i == 68) { ReconnectArray[i] = LP1.ToString(); }
                else if(i == 69) { ReconnectArray[i] = LP2.ToString(); }
                else if (i == 70) { ReconnectArray[i] = (round + roundsGone).ToString(); }
                else if(i == 71) { ReconnectArray[i] = String.Join("/", ChoicesP1); }
                else if (i == 72) { ReconnectArray[i] = String.Join("/", ChoicesP2); }
                else if (i == 73) { ReconnectArray[i] = String.Join("/", Probs[0]); }
                else if (i == 74) { ReconnectArray[i] = String.Join("/", Probs[1]); }
            }
            _room[roomNum].ShapeChosenIDs.Add(connectionID, _room[roomNum].ShapeChosenIDs[_room[roomNum].ConnectionID[0]]);
            _room[roomNum].ShapeChosenIDs.Remove(_room[roomNum].ConnectionID[0]);
            _room[roomNum].Usernames.Add(connectionID, _room[roomNum].Usernames[_room[roomNum].ConnectionID[0]]);
            _room[roomNum].Usernames.Remove(_room[roomNum].ConnectionID[0]);
            _room[roomNum].ConnectionID[0] = connectionID;
            
            _room[roomNum].DiscRecTimes = new DateTime[2];
            _room[roomNum].ReconnectString = String.Join(",", ReconnectArray);
            return _room[roomNum].ReconnectString;
        }
        /* StartRoundCountCurseP2 = Ar[0];   //EVERYTHING IS INVERSED P1 IS P2 AND P2 IS P1
           StartRoundCountCurseP1 = Ar[1];  //Implement mate cases, take care of choices and prob
           StartRoundCountP2 = Ar[2];       //implement levels
           StartRoundCountP1 = Ar[3];
           StartRoundCountBurn2 = Ar[4];
           StartRoundCountBurn1 = Ar[5];  //string[] botRec = new string[] { player1.GetLife().ToString(), player2.GetLife().ToString(), round.ToString(), String.Join("/", ChoicesP1), String.Join("/", ChoicesP2), String.Join("/", ProbsP1), String.Join("/", ProbsP2), TempOpponent.Opponent.Username, shapeID2.ToString(), String.Join("|",TempOpponent.Opponent.AbLevelArray)};
           StartRoundCountFreeze2 = Ar[6];
           StartRoundCountFreeze1 = Ar[7];
           StartRoundCountStuckInPlace2 = Ar[8];
           StartRoundCountStuckInPlace1 = Ar[9];
           protectiveEarthEffectP2 = Ar[10] == 1;
           protectiveEarthEffectP1 = Ar[11] == 1;
           MateOn2 = Ar[12] == 1;
           MateOn1 = Ar[13] == 1;
           MatePow2 = Ar[14];
           MatePow1 = Ar[15];
           MateLP2 = Ar[16];
           MateLP1 = Ar[17];
           AssistCube2 = Ar[18] == 1;
           AssistCube1 = Ar[19] == 1;
           FireEnergy2 = Ar[20] == 1;
           FireEnergy1 = Ar[21] == 1;
           BluePlanet2 = Ar[22] == 1;
           BluePlanet1 = Ar[23] == 1;
           Doit2 = Ar[24] == 1;
           Doit1 = Ar[25] == 1;
           AssistPow2 = Ar[26];
           AssistPow1 = Ar[27];
           DoitFire2 = Ar[28] == 1;
           DoitFire1 = Ar[29] == 1;
           FirePow2 = Ar[30];
           FirePow1 = Ar[31];
           DoitWater2 = Ar[32] == 1;
           DoitWater1 = Ar[33] == 1;
           WaterPow2 = Ar[34];
           WaterPow1 = Ar[35];
           DoitDoubleEdge2 = Ar[36] == 1;
           DoitDoubleEdge1 = Ar[37] == 1;
           DoubleEdge2 = Ar[38] == 1;
           DoubleEdge1 = Ar[39] == 1;
           DoubleEdgeAtt2 = Ar[40];
           DoubleEdgeDef2 = Ar[41];
           DoubleEdgeAtt1 = Ar[42];
           DoubleEdgeDef1 = Ar[43];
           Snowing2 = Ar[44] == 1;
           Snowing1 = Ar[45] == 1;
           Healing2 = Ar[46] == 1;
           Healing1 = Ar[47] == 1;
           Heal2 = Ar[48];
           Heal1 = Ar[49];
           HealCount2 = Ar[50];
           HealCount1 = Ar[51];
           PoisonAir2 = Ar[52] == 1;
           PoisonAir1 = Ar[53] == 1;
           PoisonPow2 = Ar[54];
           PoisonPow1 = Ar[55];
           HelperSpheres2 = Ar[56];
           HelperSpheres1 = Ar[57];
           DoitPoison2 = Ar[58] == 1;
           DoitPoison1 = Ar[59] == 1;
           DoitSmoke2 = Ar[60] == 1;
           DoitSmoke1 = Ar[61] == 1;
           PPDrain2 = Ar[62];
           PPDrain1 = Ar[63];
           HardeningPow2 = Ar[64];
           HardeningPow1 = Ar[65];
           player2.SetEP(Ar[66]);
           player1.SetEP(Ar[67]);
        */
        private void SendToBattle(int roomNum)
        {
            bool bad1 = !_room[roomNum].OnlineCheck[0] || (_room[roomNum].ConnectionID[0] == 0);
            bool bad2 = !_room[roomNum].OnlineCheck[1] || (_room[roomNum].ConnectionID[1] == 0);
            if (bad1 && bad2)
            {
                //Clear the room and requeue it
                LeaveRoom(_room[roomNum].ConnectionID[0], _room[roomNum].GameMode, roomNum);
                if (ServerTCP.ClientObjects[_room[roomNum].ConnectionID[0]].Socket != null)
                    ServerTCP.PACKET_SendMessage(_room[roomNum].ConnectionID[0], "SS");
                if (ServerTCP.ClientObjects[_room[roomNum].ConnectionID[1]].Socket != null)
                    ServerTCP.PACKET_SendMessage(_room[roomNum].ConnectionID[1], "SS");

                return;
            }
            else if(!bad1 && bad2)
            {
                //Remove the Absent Player and requeue the room for searching
                #region ClearingTheRoom
                int otherConnectionID = _room[roomNum].ConnectionID[1];

                _room[roomNum].ConnectionID[1] = 0;
                _room[roomNum].Usernames.Remove(otherConnectionID);
                _room[roomNum].ShapeChosenIDs.Remove(otherConnectionID);
                if(_room[roomNum].ShapeChosenIDs2.ContainsKey(otherConnectionID))
                    _room[roomNum].ShapeChosenIDs2.Remove(otherConnectionID);
                _room[roomNum].AbIDs[1] = "";
                _room[roomNum].AbIDs2[1] = "";
                _room[roomNum].PPs[1] = 0;
                _room[roomNum].PPs2[1] = 0;
                _room[roomNum].SuperIDs[1] = 0;
                _room[roomNum].SuperIDs2[1] = 0;
                _room[roomNum].SkinArray[2] = 0;
                _room[roomNum].SkinArray[3] = 0;
                _room[roomNum].Levels[1] = 0;
                _room[roomNum].Levels2[1] = 0;
                if (ServerTCP.TempPlayers.ContainsKey(otherConnectionID))
                {
                    ServerTCP.TempPlayers[otherConnectionID].Room = 0;
                    ServerTCP.TempPlayers[otherConnectionID].Castbar = 0;
                    ServerTCP.TempPlayers[otherConnectionID].inMatch = false;
                }               
                if (ServerTCP.ClientObjects[otherConnectionID].Socket != null)
                    ServerTCP.PACKET_SendMessage(otherConnectionID, "SS");
                #endregion

                ServerTCP.TempPlayers[_room[roomNum].ConnectionID[0]].inMatch = false;
                _room[roomNum].OnlineCheck[0] = _room[roomNum].OnlineCheck[1] = false;
                if (_room[roomNum].GameMode == 1) { ppThere.TryAdd(_room[roomNum].PPs[0], roomNum); }
                else if (_room[roomNum].GameMode == 2) { ppThere2.TryAdd(_room[roomNum].PPs[0], roomNum); }
                return;
            }
            else if(bad1 && !bad2)
            {
                //Remove the Absent Player and requeue the room for searching
                #region Replacing Player1 with 2
                _room[roomNum].AbIDs[0] = _room[roomNum].AbIDs[1];
                _room[roomNum].AbIDs2[0] = _room[roomNum].AbIDs2[1];
                _room[roomNum].PPs[0] = _room[roomNum].PPs[1];
                _room[roomNum].PPs2[0] = _room[roomNum].PPs2[1];
                _room[roomNum].SuperIDs[0] = _room[roomNum].SuperIDs[1];
                _room[roomNum].SuperIDs2[0] = _room[roomNum].SuperIDs2[1];
                _room[roomNum].SkinArray[0] = _room[roomNum].SkinArray[2];
                _room[roomNum].SkinArray[1] = _room[roomNum].SkinArray[3];
                _room[roomNum].Levels[0] = _room[roomNum].Levels[1];
                _room[roomNum].Levels2[0] = _room[roomNum].Levels2[1];
                #endregion
                #region ClearingTheRoom
                int otherConnectionID = _room[roomNum].ConnectionID[0];
                _room[roomNum].ConnectionID[0] = _room[roomNum].ConnectionID[1];
                _room[roomNum].ConnectionID[1] = 0;

                _room[roomNum].Usernames.Remove(otherConnectionID);
                _room[roomNum].ShapeChosenIDs.Remove(otherConnectionID);
                if (_room[roomNum].ShapeChosenIDs2.ContainsKey(otherConnectionID))
                    _room[roomNum].ShapeChosenIDs2.Remove(otherConnectionID);
                _room[roomNum].AbIDs[1] = "";
                _room[roomNum].AbIDs2[1] = "";
                _room[roomNum].PPs[1] = 0;
                _room[roomNum].PPs2[1] = 0;
                _room[roomNum].SuperIDs[1] = 0;
                _room[roomNum].SuperIDs2[1] = 0;
                _room[roomNum].SkinArray[2] = 0;
                _room[roomNum].SkinArray[3] = 0;
                _room[roomNum].Levels[1] = 0;
                _room[roomNum].Levels2[1] = 0;
                if (ServerTCP.TempPlayers.ContainsKey(otherConnectionID))
                {
                    ServerTCP.TempPlayers[otherConnectionID].Room = 0;
                    ServerTCP.TempPlayers[otherConnectionID].Castbar = 0;
                    ServerTCP.TempPlayers[otherConnectionID].inMatch = false;
                }
                if (ServerTCP.ClientObjects[otherConnectionID].Socket != null)
                    ServerTCP.PACKET_SendMessage(otherConnectionID, "SS");
                #endregion

                ServerTCP.TempPlayers[_room[roomNum].ConnectionID[0]].inMatch = false;
                _room[roomNum].OnlineCheck[0] = _room[roomNum].OnlineCheck[1] = false;
                if (_room[roomNum].GameMode == 1) { ppThere.TryAdd(_room[roomNum].PPs[0], roomNum); }
                else if (_room[roomNum].GameMode == 2) { ppThere2.TryAdd(_room[roomNum].PPs[0], roomNum); }
                return;
            }

            int connectionID0 = _room[roomNum].ConnectionID[0];
            int connectionID1 = _room[roomNum].ConnectionID[1];
            _room[roomNum].Usernames.TryGetValue(connectionID1, out string username);
            _room[roomNum].ShapeChosenIDs.TryGetValue(connectionID1, out int ShapeChosenID);
            _room[roomNum].ShapeChosenIDs2.TryGetValue(connectionID1, out int ShapeChosenID2);

            _room[roomNum].Usernames.TryGetValue(connectionID0, out string OtherPlayerUsername);
            _room[roomNum].ShapeChosenIDs.TryGetValue(connectionID0, out int OtherPlayerShapeChosenID);
            _room[roomNum].ShapeChosenIDs2.TryGetValue(connectionID0, out int OtherPlayerShapeChosenID2);

            ServerTCP.PACKET_SendToMatchmaking(connectionID0, username, connectionID1, ShapeChosenID, ShapeChosenID2, _room[roomNum].AbIDs[1], _room[roomNum].AbIDs2[1], _room[roomNum].PPs[1], _room[roomNum].PPs2[1], _room[roomNum].Levels[1], _room[roomNum].Levels2[1], _room[roomNum].SuperIDs[1], _room[roomNum].SuperIDs2[1], _room[roomNum].SkinArray[2], _room[roomNum].SkinArray[3]);
            ServerTCP.PACKET_SendToMatchmaking(connectionID1, OtherPlayerUsername, connectionID0, OtherPlayerShapeChosenID, OtherPlayerShapeChosenID2, _room[roomNum].AbIDs[0], _room[roomNum].AbIDs2[0], _room[roomNum].PPs[0], _room[roomNum].PPs2[0], _room[roomNum].Levels[0], _room[roomNum].Levels2[0], _room[roomNum].SuperIDs[0], _room[roomNum].SuperIDs2[0], _room[roomNum].SkinArray[0], _room[roomNum].SkinArray[1]);
        }

        public void LeaveRoom(int connectionID, int GameMode, int roomNum, bool disc = false)
        {
            if (_room[roomNum].ConnectionID[0] == connectionID && connectionID != 0)
            {
                _room[roomNum].ConnectionID[0] = 0;

                string Username = _room[roomNum].Usernames[connectionID];
                int ShapeIDdisc = _room[roomNum].ShapeChosenIDs[connectionID];
                string PP2 = Database.GetPP(Username);
                string[] PP2array = PP2.Split(',');
                Int32.TryParse(PP2array[ShapeIDdisc], out int PP2int);

                _room[roomNum].PPs[0] = 0;
                _room[roomNum].PPs2[0] = 0;
                _room[roomNum].Levels[0] = 0;
                _room[roomNum].Levels2[0] = 0;
                _room[roomNum].SuperIDs[0] = 0;
                _room[roomNum].SuperIDs2[0] = 0;
                _room[roomNum].GameMode = 0;
                _room[roomNum].SkinArray[0] = -1;
                _room[roomNum].SkinArray[1] = -1;

                int otherConnectionID = _room[roomNum].ConnectionID[1];
                if (otherConnectionID != 0)
                {
                    _room[roomNum].ConnectionID[1] = 0;
                    if (disc)
                    {
                        string PP1 = Database.GetPP(_room[roomNum].Usernames[otherConnectionID]);
                        string[] PP1array = PP1.Split(',');
                        int ShapeIDOther = _room[roomNum].ShapeChosenIDs[otherConnectionID];
                        Int32.TryParse(PP1array[ShapeIDOther], out int PP1int);
                        ServerTCP.PACKET_Disconnect(otherConnectionID, PP1int, PP2int, _room[roomNum].Usernames[otherConnectionID], Username,ShapeIDdisc,ShapeIDOther);
                    }
                    if (ServerTCP.TempPlayers.ContainsKey(otherConnectionID))
                    {
                        ServerTCP.TempPlayers[otherConnectionID].Room = 0;
                        ServerTCP.TempPlayers[otherConnectionID].Castbar = 0;
                        ServerTCP.TempPlayers[otherConnectionID].inMatch = false;
                    }
                    if(_room[roomNum].Usernames.TryGetValue(otherConnectionID, out string OtherUsername))
                    {
                        ClientObject.UsernamesDisc.TryRemove(OtherUsername, out int[] aa);
                    }

                    _room[roomNum].PPs[1] = 0;
                    _room[roomNum].PPs2[1] = 0;
                    _room[roomNum].Levels[1] = 0;
                    _room[roomNum].Levels2[1] = 0;
                    _room[roomNum].SuperIDs[1] = 0;
                    _room[roomNum].SuperIDs2[1] = 0;
                    _room[roomNum].GameMode = 0;
                    _room[roomNum].SkinArray[2] = -1;
                    _room[roomNum].SkinArray[3] = -1;
                }
                else
                {
                    if (GameMode == 1) ppThere.TryRemove(PP2int, out int _);
                    else if (GameMode == 2) ppThere2.TryRemove(PP2int, out int __);

                    if (_room[roomNum].ConnectionIDTemp != -1)
                    {
                        ServerTCP.PACKET_SendMessage(_room[roomNum].ConnectionIDTemp, "Leave1");
                        _room[roomNum].ConnectionIDTemp = -1;
                    }
                }

                _room[roomNum].Usernames.Clear();
                _room[roomNum].ShapeChosenIDs.Clear();
                _room[roomNum].ShapeChosenIDs2.Clear();
            }
            else if (_room[roomNum].ConnectionID[1] == connectionID && connectionID != 0)
            {
                _room[roomNum].ConnectionID[1] = 0;

                if (disc)
                {
                    int ShapeIDdisc = _room[roomNum].ShapeChosenIDs[connectionID];
                    string PP2 = Database.GetPP(_room[roomNum].Usernames[connectionID]);
                    string[] PP2array = PP2.Split(',');
                    Int32.TryParse(PP2array[ShapeIDdisc], out int PP2int);
                    string Username = _room[roomNum].Usernames[connectionID];
                    string PP1 = Database.GetPP(_room[roomNum].Usernames[_room[roomNum].ConnectionID[0]]);
                    string[] PP1array = PP1.Split(',');
                    int PP1int;
                    int ShapeIDOther = _room[roomNum].ShapeChosenIDs[_room[roomNum].ConnectionID[0]];
                    Int32.TryParse(PP1array[ShapeIDOther], out PP1int);
                    ServerTCP.PACKET_Disconnect(_room[roomNum].ConnectionID[0], PP1int, PP2int, _room[roomNum].Usernames[_room[roomNum].ConnectionID[0]], Username,ShapeIDdisc,ShapeIDOther);
                }

                _room[roomNum].PPs[1] = 0;
                _room[roomNum].PPs2[1] = 0;
                _room[roomNum].Levels[1] = 0;
                _room[roomNum].Levels2[1] = 0;
                _room[roomNum].SuperIDs[1] = 0;
                _room[roomNum].SuperIDs2[1] = 0;
                _room[roomNum].SkinArray[2] = -1;
                _room[roomNum].SkinArray[3] = -1;

                int otherConnectionID = _room[roomNum].ConnectionID[0];
                if (otherConnectionID != 0)
                {
                    _room[roomNum].ConnectionID[0] = 0;
                    if (ServerTCP.TempPlayers.ContainsKey(otherConnectionID))
                    {
                        ServerTCP.TempPlayers[otherConnectionID].Room = 0;
                        ServerTCP.TempPlayers[otherConnectionID].Castbar = 0;
                        ServerTCP.TempPlayers[otherConnectionID].inMatch = false;
                    }
                    if (_room[roomNum].Usernames.TryGetValue(otherConnectionID, out string OtherUsername))
                    {
                        ClientObject.UsernamesDisc.TryRemove(OtherUsername, out int[] aa);
                    }

                    _room[roomNum].PPs[0] = 0;
                    _room[roomNum].PPs2[0] = 0;
                    _room[roomNum].Levels[0] = 0;
                    _room[roomNum].Levels2[0] = 0;
                    _room[roomNum].SuperIDs[0] = 0;
                    _room[roomNum].SuperIDs2[0] = 0;
                    _room[roomNum].GameMode = 0;
                    _room[roomNum].SkinArray[0] = -1;
                    _room[roomNum].SkinArray[1] = -1;
                }

                _room[roomNum].Usernames.Clear();
                _room[roomNum].ShapeChosenIDs.Clear();
                _room[roomNum].ShapeChosenIDs2.Clear();
            }

            if (ServerTCP.TempPlayers.ContainsKey(connectionID))
            {
                ServerTCP.TempPlayers[connectionID].Room = 0;
                ServerTCP.TempPlayers[connectionID].Castbar = 0;
                ServerTCP.TempPlayers[connectionID].inMatch = false;
            }

            _room[roomNum].ConnectionIDTemp = -1;
            _room[roomNum].DontJoin = false;
            _room[roomNum].Friendly = false;
            _room[roomNum].OnlineCheck[0] = _room[roomNum].OnlineCheck[1] = false;
            _room[roomNum].Bot = false;
            _room[roomNum].DiscRecTimes = new DateTime[2];

            unusedRooms.Enqueue(roomNum);
            _room[roomNum].SpectatorIDs.Clear();
        }

        public int FriendlyBattle(int connectionID, string username, int ShapeChosenID, int ShapeChosenID2, string AbilitiesID, string AbilitiesID2, int SuperID, int SuperID2, int TempConnID,int SkinID, int SkinID2)
        {
            string curPPstr = Database.GetPP(username);
            string[] curPParray = curPPstr.Split(',');
            int curPP, curPP2 = 0;
            Int32.TryParse(curPParray[ShapeChosenID], out curPP);
            if (ShapeChosenID2 >= 0 && ShapeChosenID2 < 4) Int32.TryParse(curPParray[ShapeChosenID2], out curPP2);
            unusedRooms.TryDequeue(out int roomNum);
            _room[roomNum].DontJoin = true;
            _room[roomNum].Friendly = true;
            _room[roomNum].ConnectionID[0] = connectionID;
            _room[roomNum].Usernames.Add(connectionID, username);
            _room[roomNum].ShapeChosenIDs.Add(connectionID, ShapeChosenID);
            _room[roomNum].ShapeChosenIDs2.Add(connectionID, ShapeChosenID2);
            _room[roomNum].AbIDs[0] = AbilitiesID;
            _room[roomNum].AbIDs2[0] = AbilitiesID2;
            _room[roomNum].PPs[0] = curPP;
            _room[roomNum].PPs2[0] = curPP2;
            _room[roomNum].SuperIDs[0] = SuperID;
            _room[roomNum].SuperIDs2[0] = SuperID2;
            _room[roomNum].ConnectionIDTemp = TempConnID;
            _room[roomNum].SkinArray[0] = SkinID;
            _room[roomNum].SkinArray[1] = SkinID2;

            string Level1 = Database.GetLevel(username);
            int LevelShapeID1, LevelShapeID2 = 0;
            string[] Lvl1 = Level1.Split(',');
            Int32.TryParse(Lvl1[ShapeChosenID], out LevelShapeID1);
            if (ShapeChosenID2 >= 0 && ShapeChosenID2 < 4) { Int32.TryParse(Lvl1[ShapeChosenID2], out LevelShapeID2); }
            _room[roomNum].Levels[0] = LevelShapeID1;
            _room[roomNum].Levels2[0] = LevelShapeID2;
            ServerTCP.TempPlayers[connectionID].Room = roomNum;
            Console.WriteLine("Room created: " + roomNum + " | Player with " + curPP + " PP added: " + " connectionID: " + connectionID);
            return roomNum;
        }
        public void JoinFriendlyBattle(int connectionID, string username, int ShapeChosenID, int ShapeChosenID2, string AbilitiesID, string AbilitiesID2, int roomNum,int SuperID, int SuperID2, int SkinID, int SkinID2)
        {
            string curPPstr = Database.GetPP(username);
            string[] curPParray = curPPstr.Split(',');
            int curPP, curPP2 = 0;
            Int32.TryParse(curPParray[ShapeChosenID], out curPP);
            if (ShapeChosenID2 >= 0 && ShapeChosenID2 < 4) Int32.TryParse(curPParray[ShapeChosenID2], out curPP2);
            _room[roomNum].ConnectionID[1] = connectionID;
            _room[roomNum].Usernames.Add(connectionID, username);
            _room[roomNum].ShapeChosenIDs.Add(connectionID, ShapeChosenID);
            _room[roomNum].ShapeChosenIDs2.Add(connectionID, ShapeChosenID2);
            _room[roomNum].AbIDs[1] = AbilitiesID;
            _room[roomNum].AbIDs2[1] = AbilitiesID2;
            _room[roomNum].PPs[1] = curPP;
            _room[roomNum].PPs2[1] = curPP2;
            _room[roomNum].SuperIDs[1] = SuperID;
            _room[roomNum].SuperIDs2[1] = SuperID2;
            _room[roomNum].SkinArray[2] = SkinID;
            _room[roomNum].SkinArray[3] = SkinID2;
            string level1 = Database.GetLevel(username);
            int levelShapeID1, levelShapeID2 = 0;
            string[] lvl1 = level1.Split(',');
            Int32.TryParse(lvl1[ShapeChosenID], out levelShapeID1);
            if (ShapeChosenID2 >= 0 && ShapeChosenID2 < 4) { Int32.TryParse(lvl1[ShapeChosenID2], out levelShapeID2); }
            _room[roomNum].Levels[1] = levelShapeID1;
            _room[roomNum].Levels2[1] = levelShapeID2;

            ServerTCP.TempPlayers[connectionID].Room = roomNum;

            int otherConnectionID = _room[roomNum].ConnectionID[0];

            _room[roomNum].Usernames.TryGetValue(otherConnectionID, out string OtherPlayerUsername);
            _room[roomNum].ShapeChosenIDs.TryGetValue(otherConnectionID, out int OtherPlayerShapeChosenID);
            _room[roomNum].ShapeChosenIDs2.TryGetValue(otherConnectionID, out int OtherPlayerShapeChosenID2);

            Console.WriteLine("Room joined: " + roomNum + " | Player with " + curPP + " PP added: " + " connectionID: " + connectionID);
            ServerTCP.PACKET_SendToMatchmaking(connectionID, OtherPlayerUsername, otherConnectionID, OtherPlayerShapeChosenID, OtherPlayerShapeChosenID2, _room[roomNum].AbIDs[0], _room[roomNum].AbIDs2[0], _room[roomNum].PPs[0], _room[roomNum].PPs2[0], _room[roomNum].Levels[0], _room[roomNum].Levels2[0], _room[roomNum].SuperIDs[0], _room[roomNum].SuperIDs2[0], _room[roomNum].SkinArray[0], _room[roomNum].SkinArray[1], 1);
            ServerTCP.PACKET_SendToMatchmaking(otherConnectionID, username, connectionID, ShapeChosenID, ShapeChosenID2, AbilitiesID, AbilitiesID2, curPP, curPP2, levelShapeID1, levelShapeID2,SuperID, SuperID2, SkinID, SkinID2,1);

            ServerTCP.TempPlayers[connectionID].inMatch = true;
            ServerTCP.TempPlayers[otherConnectionID].inMatch = true;
        }
        public void LeaveFriendly(int ConnectionID,bool Denied = false,bool Disc = false)
        {
            int roomNum = ServerTCP.TempPlayers[ConnectionID].Room;
            if (_room[roomNum].ConnectionID[0] == ConnectionID)
            {
                _room[roomNum].ConnectionID[0] = 0;
                _room[roomNum].PPs[0] = 0;
                _room[roomNum].PPs2[0] = 0;
                _room[roomNum].Levels[0] = 0;
                _room[roomNum].Levels2[0] = 0;
                _room[roomNum].SuperIDs[0] = 0;
                _room[roomNum].SuperIDs2[0] = 0;
                _room[roomNum].SkinArray[0] = -1;
                _room[roomNum].SkinArray[1] = -1;
                if (Denied)
                {
                    ServerTCP.PACKET_SendMessage(ConnectionID, "BRD");  //Battle Request Denied
                }

                int otherConnectionID = _room[roomNum].ConnectionID[1];
                if (otherConnectionID != 0)
                {
                    if (Disc)
                    {
                        ServerTCP.PACKET_SendMessage(otherConnectionID, "Leave");
                    }
                    ServerTCP.TempPlayers[otherConnectionID].Room = 0;
                    ServerTCP.TempPlayers[otherConnectionID].Castbar = 0;
                    ServerTCP.TempPlayers[otherConnectionID].inMatch = false;

                    if (_room[roomNum].Usernames.TryGetValue(otherConnectionID, out string OtherUsername))
                        ClientObject.UsernamesDisc.TryRemove(OtherUsername, out int[] aa);

                    _room[roomNum].ConnectionID[1] = 0;
                    _room[roomNum].PPs[1] = 0;
                    _room[roomNum].PPs2[1] = 0;
                    _room[roomNum].Levels[1] = 0;
                    _room[roomNum].Levels2[1] = 0;
                    _room[roomNum].SuperIDs[1] = 0;
                    _room[roomNum].SuperIDs2[1] = 0;
                    _room[roomNum].SkinArray[2] = -1;
                    _room[roomNum].SkinArray[3] = -1;
                }

                _room[roomNum].Usernames.Clear();
                _room[roomNum].ShapeChosenIDs.Clear();
                _room[roomNum].ShapeChosenIDs2.Clear();
            }
            else
            {
                if (Disc) { ServerTCP.PACKET_SendMessage(_room[roomNum].ConnectionID[0], "Leave"); }

                _room[roomNum].ConnectionID[1] = 0;
                _room[roomNum].PPs[1] = 0;
                _room[roomNum].PPs2[1] = 0;
                _room[roomNum].Levels[1] = 0;
                _room[roomNum].Levels2[1] = 0;
                _room[roomNum].SuperIDs[1] = 0;
                _room[roomNum].SuperIDs2[1] = 0;

                int otherConnectionID = _room[roomNum].ConnectionID[0];
                ServerTCP.TempPlayers[otherConnectionID].Room = 0;
                ServerTCP.TempPlayers[otherConnectionID].Castbar = 0;
                ServerTCP.TempPlayers[otherConnectionID].inMatch = false;

                if (_room[roomNum].Usernames.TryGetValue(otherConnectionID, out string OtherUsername))
                    ClientObject.UsernamesDisc.TryRemove(OtherUsername, out int[] aa);

                _room[roomNum].ConnectionID[0] = 0;
                _room[roomNum].PPs[0] = 0;
                _room[roomNum].PPs2[0] = 0;
                _room[roomNum].Levels[0] = 0;
                _room[roomNum].Levels2[0] = 0;
                _room[roomNum].SuperIDs[0] = 0;
                _room[roomNum].SuperIDs2[0] = 0;
                _room[roomNum].SkinArray[0] = -1;
                _room[roomNum].SkinArray[1] = -1;
                _room[roomNum].Usernames.Clear();
                _room[roomNum].ShapeChosenIDs.Clear();
                _room[roomNum].ShapeChosenIDs2.Clear();
            }

            ServerTCP.TempPlayers[ConnectionID].Room = 0;
            ServerTCP.TempPlayers[ConnectionID].Castbar = 0;
            ServerTCP.TempPlayers[ConnectionID].inMatch = false;
            _room[roomNum].DontJoin = false;
            _room[roomNum].Friendly = false;
            _room[roomNum].SpectatorIDs.Clear();
            _room[roomNum].ConnectionIDTemp = -1;
            _room[roomNum].OnlineCheck[0] = _room[roomNum].OnlineCheck[1] = false;
            _room[roomNum].Bot = false;
            _room[roomNum].DiscRecTimes = new DateTime[2];

            unusedRooms.Enqueue(roomNum);           
        }
        private int[] SArrayToIArray(string[] Ar)
        {
            int[] newAr = new int[Ar.Length];
            for (int i = 0; i < Ar.Length; i++)
                Int32.TryParse(Ar[i], out newAr[i]);
            return newAr;
        }
        // 0 = StuckinPlace, 1 = Hardening, 2 = Freeze, 3 = Snowing, 4 = Burn, 5 = FieryEyes, 6 = HelperSpheres, 7 =Fog
        private List<int>[] AddPassives(List<bool>[] dmgDone, List<int> Probs1, List<int> Probs2, string username,int shapeID1, int shapeID2)
        {
            string[] passives = Database.GetPassives(username).Split(',');
            bool[][] passivecounts = new bool[2][]; passivecounts[0] = new bool[2]; passivecounts[1] = new bool[2];
            passivecounts[0][0] = !passives[shapeID1 * 2].Equals("0");
            passivecounts[0][1] = !passives[shapeID1 * 2 + 1].Equals("0");
            passivecounts[1][0] = !passives[shapeID2 * 2].Equals("0");
            passivecounts[1][1] = !passives[shapeID2 * 2 + 1].Equals("0");
            if(passivecounts[0][0] || passivecounts[0][1] || passivecounts[1][0] || passivecounts[1][1])
            {
                for(int i = 0; i < dmgDone[0].Count ; i++)
                {
                    if ((dmgDone[0][i] && passivecounts[0][1]) || (dmgDone[1][i] && passivecounts[0][0]))
                        Probs1.Add(200);
                    if ((dmgDone[1][i] && passivecounts[1][1]) || (dmgDone[0][i] && passivecounts[1][0]))
                        Probs2.Add(200);
                }
            }
            return new List<int>[] { Probs1, Probs2 };
        }
    }

    class Room
    {
        public int roomIndex;
        public int GameMode;
        public int[] ConnectionID = new int[2];
        public Dictionary<int, string> Usernames = new Dictionary<int, string>();
        public Dictionary<int, int> ShapeChosenIDs = new Dictionary<int, int>();
        public Dictionary<int, int> ShapeChosenIDs2 = new Dictionary<int, int>();
        public string[] AbIDs = new string[2];
        public string[] AbIDs2 = new string[2];
        public int[] PPs = new int[2];
        public int[] PPs2 = new int[2];
        public int[] Levels = new int[2];
        public int[] Levels2 = new int[2];
        public bool DontJoin = false;
        public bool Friendly = false;
        public int[] LifePoints = new int[2];
        public bool ReceivedLP = false;
        public List<int> SpectatorIDs;
        public int[] SuperIDs = new int[2];
        public int[] SuperIDs2 = new int[2];
        public int ConnectionIDTemp = -1;
        public int[] SkinArray = new int[4];
        public bool[] OnlineCheck = new bool[2] { false, false };
        public string ReconnectString;
        public bool Bot = false;
        public DateTime[] DiscRecTimes = new DateTime[2];
        public string BotAbilities;
    }
}