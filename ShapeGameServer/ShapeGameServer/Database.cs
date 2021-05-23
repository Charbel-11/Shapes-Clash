using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace ShapeGameServer
{
    class Database
    {
        static string[] Alphabet = new string[26] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        public static void NewAccount(string username)
        {
            string Abilities = "1,1,1,1,0,1,0,0,1,0,0,0,0,0,0,1,0,1,0,1,0,1,1,1,1,0,0,1,0,0,0,0,1,0,0,0,1,0,0,0,1,0,1,1,0,1,0,0,0,1,0,1,0,0,0,0,0";
            string Passives = "0,0,0,0,0,0,0,0";
            string Super100 = "0,0,0,0";
            string Super200 = "0,0,0,0";
            string XP = "0,0,0,0";
            string PP = "0,0,0,0";
            string ShapeLvl = "0,0,0,0";
            string Code = "";
            string SecretCode = "/";
            Random rand = new Random();
            for (int i = 0; i < 5; i++)
            {
                int Num = rand.Next(0, 25);
                string Letter = Alphabet[Num];            
                Code = Code + Letter;
            }
            for (int i = 0; i < 5; i++)
            {
                int Num = rand.Next(0, 25);
                string Letter = Alphabet[Num];
                SecretCode = SecretCode + Letter;
            }
            string query = "INSERT INTO account(username,gold,exp,PP,Abilities,Super100,Super200,ShapeLvl,Code,Passives,SecretCode) VALUES(@Username" +
                ",'" + 150 +
                "','" + XP +
                "','" + PP +
                "','" + Abilities +
                "','" + Super100 +
                "','" + Super200 +
                "','" + ShapeLvl +
                "','" + Code +
                "','" + Passives +
                "','" + SecretCode + "')";

            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                }

                try { cmd.ExecuteNonQuery(); }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            UpdateLastLogin(username, true);
            Database.UpdateLastSpin(username, false);
        }
        public static bool AccountExists(string username)
        {
            string query = "SELECT username FROM account WHERE username = @Username";
            using(MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                using (MySqlDataReader Reader = cmd.ExecuteReader())
                {
                    return Reader.HasRows;
                }                
            }            
        }
        public static bool PasswordOk(string username, string password)
        {
            string query = "SELECT password FROM account WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                string tempPass;
                using(MySqlDataReader Reader = cmd.ExecuteReader())
                {
                    tempPass = Reader["password"] + "";
                    return password == tempPass;
                }               
            }
                
        }
        public static int GetGold(string username)
        {
            int Gold = 50;
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                string query = "SELECT gold FROM account WHERE username= @Username";
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }                
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Int32.TryParse((Reader["gold"] + ""), out Gold);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }               
            return Gold;
        }
        public static int GetRedbolts(string username)
        {
            int RB = 0;
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                string query = "SELECT Redbolts FROM account WHERE username= @Username";
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }                
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Int32.TryParse((Reader["Redbolts"] + ""), out RB);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }               
            return RB;
        }
        public static int GetDiamonds(string username)
        {
            int Diamonds = 10;
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                string query = "SELECT Diamonds FROM account WHERE username= @Username";
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Int32.TryParse((Reader["Diamonds"] + ""), out Diamonds);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return Diamonds;
        }
        public static void SetGold(string username, int Gold)
        {
            string query = "UPDATE account SET gold= @Gold WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Gold", Gold);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }                
        }
        public static void SetRedbolts(string username, int Redbolts)
        {
            string query = "UPDATE account SET Redbolts= @Gold WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Gold", Redbolts);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static void SetDiamonds(string username, int Diamonds)
        {
            string query = "UPDATE account SET Diamonds= @Gold WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Gold", Diamonds);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static string GetLevel(string username)
        {
            string query = "SELECT ShapeLvl FROM account WHERE username= @Username";
            string Lvl = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Lvl = Reader.GetString("ShapeLvl");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return Lvl;
        }
        public static string GetXP(string username)
        {
            string query = "SELECT exp FROM account WHERE username= @Username";
            string Lvl = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Lvl = Reader.GetString("exp");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return Lvl;
        }
        public static string GetPP(string username)
        {
            string query = "SELECT PP FROM account WHERE username= @Username";
            string Lvl = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Lvl = Reader.GetString("PP");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return Lvl;
        }
        public static string GetCode(string username)
        {
            string query = "SELECT Code, ID FROM account WHERE username= @Username";
            string Code = "";
            int ID = 0;
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Code = Reader.GetString("Code");
                            ID = Reader.GetInt32("ID");
                        }
                        cmd.Cancel();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }                
            string NewCode = Code + ID;
            return NewCode;
        }
        public static string GetSecretCode(string username)
        {
            string query = "SELECT SecretCode, ID FROM account WHERE username= @Username";
            string Code = "";
            int ID = 0;
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Code = Reader.GetString("SecretCode");
                            ID = Reader.GetInt32("ID");
                        }
                        cmd.Cancel();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            string NewCode = Code + ID;
            return NewCode;
        }
        public static string GetUsername(string Code)
        {
            int Place = 0;
            foreach (char a in Code)
            {
                if (a >= '1' && a <= '9') { break; }
                Place++;
            }
            string NewCode = Code.Substring(0, Place);
            Int32.TryParse(Code.Substring(Place), out int ID);
            string query = "SELECT username FROM account WHERE Code= @Code AND ID= @ID";
            string Username = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Code", NewCode);
                    cmd.Parameters.AddWithValue("@ID", ID);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Username = Reader.GetString("username");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }               
            return Username;
        }
        public static string CheckSecretCode(string Code)
        {
            int Place = 0;
            foreach (char a in Code)
            {
                if (a >= '1' && a <= '9') { break; }
                Place++;
            }
            string NewCode = Code.Substring(0, Place);
            Int32.TryParse(Code.Substring(Place), out int ID);
            string query = "SELECT username FROM account WHERE SecretCode= @Code AND ID= @ID";
            string Username = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Code", NewCode);
                    cmd.Parameters.AddWithValue("@ID", ID);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Username = Reader.GetString("username");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return Username;
        }
        public static string GetFriends(string username, bool me = false)
        {
            string query = "SELECT Friends FROM account WHERE username= @Username";
            string Friends = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Friends = Reader.GetString("Friends");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                if (me) { return Friends; }
                string[] FriendsList = Friends.Split(',');
                string[] Ons = new string[FriendsList.Length];
                for (int i = 0; i < FriendsList.Length; i++)
                {
                    if (FriendsList[i] != "")
                    {
                        bool On = Online(FriendsList[i]);
                        Ons[i] = (On ? "1" : "0");
                    }
                }
                string OnlineStatus = String.Join(",", Ons);
                Friends = Friends + "/" + OnlineStatus;
            }                           
            return Friends;
        }
        public static bool Online(string username)
        {
            string query = "SELECT Online FROM account WHERE username= @Username";
            bool Online = false;

            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                int On = 0;
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            On = Reader.GetInt32("Online");
                        }
                        cmd.Cancel();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Online = On == 1;
            }                
            return Online;
        }
        public static void SetOnline(string username, bool Online)
        {
            int On = 0;
            if (Online) { On = 1; }
            string query = "UPDATE account SET Online= @Online WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Online", On);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }                
        }
        public static void AddFriend(string username, string Friend,bool me = false)
        {
            if (me)
            {
                string query = "UPDATE account SET Friends= @Friends WHERE username= @Username";
                using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                    try
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Friends", Friend);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                return;
            }
            string Friends = GetFriends(username, true);
            string[] List = Friends.Split(',');
            bool IsThere = false;
            foreach (string c in List)
            {
                if (c.Equals(Friend))
                {
                    IsThere = true;
                    break;
                }
            }
            if (!IsThere)
            {
                if (Friends != "") { Friends = Friends + "," + Friend; }
                else { Friends = Friend; }
                string query = "UPDATE account SET Friends= @Friends WHERE username= @Username";
                using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                    try
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Friends", Friends);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }                    
            }
        }
        public static void SetPP(string username, int newPP, int ShapeID)
        {
            string PP = Database.GetPP(username);
            Int32.TryParse(Database.GetMaxPP(username).Split(',')[ShapeID], out int MaxPP);
            string[] PP1 = PP.Split(',');
            if (newPP >= 0)
            {
                PP1[ShapeID] = newPP.ToString();
                if (newPP > MaxPP)
                {
                    Database.SetMaxPP(username ,string.Join(",", PP1));
                }
            }
            else
            {
                PP1[ShapeID] = "0";
            }
            string NewPPstr = string.Join(",", PP1);

            string query = "UPDATE account SET PP= @NewPP WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@NewPP", NewPPstr);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }              
        }
        public static void SetLvl(string username, int newLvl, int ShapeID, string ShapeLevel = "")
        {
            string NewLvlstr = "";
            if (ShapeLevel == "")
            {
                string Lvl = Database.GetLevel(username);
                string[] Lvl1 = Lvl.Split(',');
                Lvl1[ShapeID] = newLvl.ToString();
                NewLvlstr = string.Join(",", Lvl1);
            }
            else
            {
                NewLvlstr = ShapeLevel;
            }
            string query = "UPDATE account SET ShapeLvl= @NewLvl WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@NewLvl", NewLvlstr);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }                
        }
        public static void SetXP(string username, int newXP, int ShapeID, string XPStr = "")
        {
            string NewXPstr = "";
            if (XPStr == "")
            {
                string XP = Database.GetXP(username);
                string[] XP1 = XP.Split(',');
                XP1[ShapeID] = newXP.ToString();
                NewXPstr = string.Join(",", XP1);
            }
            else
            {
                NewXPstr = XPStr;
            }

            string query = "UPDATE account SET exp= @NewXP WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@NewXP", NewXPstr);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }                
        }
        public static void SetReplay(string username, string NewReplay)
        {
            string OldReplay = GetReplay(username, true);
            string Replay;
            DateTime CurrentTime = DateTime.Now;
            int[] Time = { CurrentTime.Year, CurrentTime.Month, CurrentTime.Day, CurrentTime.Hour, CurrentTime.Minute, CurrentTime.Second };
            string TheTime = string.Join(",", Time);
            NewReplay = NewReplay + "/" + TheTime;
            if (OldReplay == "N") { Replay = NewReplay; }
            else
            {
                string[] Replays = OldReplay.Split('|');
                if (Replays.Length == 9)
                {
                    Array.Resize(ref Replays, Replays.Length - 1);
                    OldReplay = string.Join("|", Replays);
                }
                Replay = NewReplay + "|" + OldReplay;
            }

            string query = "UPDATE account SET Replay= @Replay WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Replay", Replay);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }                
        }
        public static string GetRanking(int ShapeID)
        {
            bool Firsttime = true;
            int i = 0;
            //Dictionary<int, List<string>> UxPP = new Dictionary<int, List<string>>();
            Dictionary<int, string> UxPP = new Dictionary<int, string>();
            List<int> PPArray = new List<int>();
            string Rankings = "";
            string usernames;

            string query = "SELECT username, PP FROM account";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        usernames = Reader.GetString("username");
                        string PP = Reader.GetString("PP");
                        string[] PPArrStr = PP.Split(',');
                        int PPShapeID;
                        Int32.TryParse(PPArrStr[ShapeID], out PPShapeID);
                        if (!PPArray.Contains(PPShapeID))
                        {
                            PPArray.Add(PPShapeID);
                        }
                        //List<string> Users = new List<string>();
                        string Users = "";
                        if (UxPP.TryGetValue(PPShapeID, out Users))
                        {
                            //Users.Add(usernames);
                            Users = Users + "," + usernames;
                            UxPP[PPShapeID] = Users;
                        }
                        else
                        {
                            //Users.Add(usernames);
                            Users = usernames;
                            UxPP.Add(PPShapeID, Users);
                        }
                        i++;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }              
            PPArray.Sort();
            PPArray.Reverse();
            foreach (int c in PPArray)
            {
                //List<string> Usernames = new List<string>();
                string Usernames;
                UxPP.TryGetValue(c, out Usernames);
                string[] UsersArray = Usernames.Split(',');
                foreach (string d in UsersArray)
                {
                    if (Firsttime)
                    {
                        Rankings = d + "     " + c.ToString();
                        Firsttime = false;
                    }
                    else
                    {
                        Rankings = Rankings + Environment.NewLine + d + "     " + c.ToString();
                    }

                }
            }
            return Rankings;
        }
        public static string GetAbilities(string username)
        {
            string query = "SELECT Abilities FROM account WHERE username= @Username";
            string Abilities = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
            try
            {
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Username", username);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            try
            {
                using (MySqlDataReader Reader = cmd.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        Abilities = Reader.GetString("Abilities");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            }               
            return Abilities;
        }
        public static string GetPassives(string username)
        {
            string query = "SELECT Passives FROM account WHERE username= @Username";
            string Abilities = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Abilities = Reader.GetString("Passives");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return Abilities;
        }
        public static string GetSuper100(string username)
        {
            string query = "SELECT Super100 FROM account WHERE username= @Username";
            string Abilities = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Abilities = Reader.GetString("Super100");
                        }
                        cmd.Cancel();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return Abilities;
        }
        public static string GetSuper200(string username)
        {
            string query = "SELECT Super200 FROM account WHERE username= @Username";
            string Abilities = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Abilities = Reader.GetString("Super200");
                        }
                        cmd.Cancel();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return Abilities;
        }
        public static void SetAbilities(string username, string Abilities)
        {
            string query = "UPDATE account SET Abilities = @Abilities WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Abilities", Abilities);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }               
        }
        public static void SetPassives(string username, string Abilities)
        {
            string query = "UPDATE account SET Passives = @Abilities WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Abilities", Abilities);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }               
        }
        public static void SetSuper100(string username, string Abilities)
        {
            string query = "UPDATE account SET Super100 = @Abilities WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Abilities", Abilities);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static void SetSuper200(string username, string Abilities)
        {
            string query = "UPDATE account SET Super200 = @Abilities WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Abilities", Abilities);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static string GetReplay(string username, bool Me = false)
        {
            string query = "SELECT Replay FROM account WHERE username= @Username";
            string Replay = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Replay = Reader.GetString("Replay");
                        }
                        cmd.Cancel();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }               
            if (Replay != "N" && !Me)
            {
                string[] Replays = Replay.Split('|');
                string[][] Replays1 = new string[Replays.Length][];
                string[] ReplaysNew = new string[Replays.Length];
                int i = 0;
                foreach (string c in Replays)
                {
                    Replays1[i] = c.Split('/');
                    string[] Replays2 = Replays1[i][Replays1[i].Length - 1].Split(',');
                    int[] TimeInt = new int[Replays2.Length];
                    int j = 0;
                    foreach (string d in Replays2)
                    {
                        Int32.TryParse(d, out TimeInt[j]);
                        j++;
                    }
                    DateTime TimeThen = new DateTime(TimeInt[0], TimeInt[1], TimeInt[2], TimeInt[3], TimeInt[4], TimeInt[5]);
                    TimeSpan TimePassed = DateTime.Now - TimeThen;
                    int[] TimePassedArray = { TimePassed.Days, TimePassed.Hours, TimePassed.Minutes, TimePassed.Seconds };
                    Replays1[i][Replays1[i].Length - 1] = string.Join(",", TimePassedArray);
                    ReplaysNew[i] = string.Join("/", Replays1[i]);
                    i++;
                }
                Replay = string.Join("|", ReplaysNew);
            }
            return Replay;
        }
        public static void SetWinstreak(string username, int Winstreak)
        {
            if (Winstreak != 0)
            {
                int WinStreakOld = GetWinstreak(username);
                Winstreak = WinStreakOld + 1;
            }
            if(Winstreak > GetMaxWS(username))
            {
                SetMaxWS(username, Winstreak);
            }
            string query = "UPDATE account SET WinStreak= @Winstreak WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Winstreak", Winstreak);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }               
        }
        public static int GetWinstreak(string username)
        {
            string query = "SELECT WinStreak FROM account WHERE username= @Username";
            int Winstreak = 0;
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Winstreak = Reader.GetInt32("WinStreak");
                        }
                        cmd.Cancel();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }               
            return Winstreak;
        }
        public static void SetRecentIDs(string username, int ID)
        {
            string RecentIDs;
            if (ID == -1)
            {
                RecentIDs = "N";
            }
            else
            {
                RecentIDs = GetRecentIDs(username);
                if (RecentIDs != "N")
                {
                    RecentIDs = RecentIDs + "," + ID.ToString();
                }
                else
                {
                    RecentIDs = ID.ToString();
                }
            }
            string query = "UPDATE account SET RecentIDs= @RecentIDs WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@RecentIDs", RecentIDs);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }               
        }
        public static string GetRecentIDs(string username)
        {
            string query = "SELECT RecentIDs FROM account WHERE username= @Username";
            string IDs = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            IDs = Reader.GetString("RecentIDs");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return IDs;
        }
        public static void SetChest(string username, int Time, int ChestNum)
        {
            string OldChest = GetChestMe(username);
            DateTime Now = DateTime.Now;
            string Chest = Now.Year + "," + Now.Month + "," + Now.Day + "," + Now.Hour + "," + Now.Minute + "," + Now.Second + "," + Time;
            string[] OldChestAr = OldChest.Split('/');
            int i = 0;
            if (OldChestAr.Length != 4)
            {
                string[] OldChestArNew = new string[4];
                foreach (string c in OldChestAr)
                {
                    OldChestArNew[i] = c;
                    i++;
                }
                while (i != 4)
                {
                    OldChestArNew[i] = "";
                    i++;
                }

                OldChestAr = new string[4];
                for(i = 0; i < 4; i++) { OldChestAr[i] = OldChestArNew[i]; }
            }
            OldChestAr[ChestNum] = Chest;
            Chest = string.Join("/", OldChestAr);
            string query = "UPDATE account SET Chest= @Chest WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Chest", Chest);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }                
        }
        public static int GetChest(string username, int ChestNum)
        {
            string query = "SELECT Chest FROM account WHERE username= @Username";
            string TimeString = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            TimeString = Reader.GetString("Chest");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }               
            string[] Array = TimeString.Split('/');
            string[] TimeArrayStr = Array[ChestNum].Split(',');
            int[] TimeArray = new int[TimeArrayStr.Length];
            int i = 0;
            foreach (string c in TimeArrayStr)
            {
                Int32.TryParse(c, out TimeArray[i]);
                i++;
            }
            DateTime TimeThen = new DateTime(TimeArray[0], TimeArray[1], TimeArray[2], TimeArray[3], TimeArray[4], TimeArray[5]);
            int Seconds = TimeArray[6];
            TimeSpan Ts = DateTime.Now - TimeThen;
            Seconds -= Convert.ToInt32(Ts.TotalSeconds);
            if (Seconds < 0) { Seconds = 0; }
            return Seconds;
        }
        public static string GetChestMe(string username)
        {
            string query = "SELECT Chest FROM account WHERE username= @Username";
            string TimeString = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            TimeString = Reader.GetString("Chest");
                        }
                        cmd.Cancel();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return TimeString;
        }
        public static string GetChestSlots(string username)
        {
            string query = "SELECT ChestSlots FROM account WHERE username= @Username";
            string IDs = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            IDs = Reader.GetString("ChestSlots");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return IDs;
        }
        public static void SetChestSlots(string username, string chestSlot)
        {
            string query = "UPDATE account SET ChestSlots= @Slots WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Slots", chestSlot);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }                
        }
        public static string GetBackgrounds(string username)
        {
            string query = "SELECT Backgrounds FROM account WHERE username= @Username";
            string Lvl = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Lvl = Reader.GetString("Backgrounds");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return Lvl;
        }
        public static string GetEmotes(string username)
        {
            string query = "SELECT Emotes FROM account WHERE username= @Username";
            string Lvl = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Lvl = Reader.GetString("Emotes");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return Lvl;
        }
        public static string GetSkins(string username)
        {
            string query = "SELECT Skins FROM account WHERE username= @Username";
            string Lvl = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Lvl = Reader.GetString("Skins");
                        }
                        cmd.Cancel();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return Lvl;
        }
        public static void SetBackgrounds(string username, string Backgrounds)
        {
            string query = "UPDATE account SET Backgrounds= @Backgrounds WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Backgrounds", Backgrounds);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }               
        }
        public static void SetEmotes(string username, string Emotes)
        {
            string query = "UPDATE account SET Emotes= @Backgrounds WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Backgrounds", Emotes);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static void SetSkins(string username, string Skins)
        {
            string query = "UPDATE account SET Skins= @Backgrounds WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Backgrounds", Skins);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static string GetTrophyRoad(string username)
        {
            string query = "SELECT TrophyRoad FROM account WHERE username= @Username";
            string Lvl = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }               
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Lvl = Reader.GetString("TrophyRoad");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }                
            }
            return Lvl;
        }
        public static void SetTrophyRoad(string username, string TrophyRoad)
        {
            string query = "UPDATE account SET TrophyRoad= @TrophyRoad WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@TrophyRoad", TrophyRoad);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }                
        }
        public static string GetLastDeck(string username)
        {
            string query = "SELECT LastDeck FROM account WHERE username= @Username";
            string Lvl = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Lvl = Reader.GetString("LastDeck");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return Lvl;
        }
        public static void SetLastDeck(string username, string LastDeck)
        {
            string query = "UPDATE account SET LastDeck= @LastDeck WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@LastDeck", LastDeck);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static string GetMaxPP(string username)
        {
            string query = "SELECT MaxPP FROM account WHERE username= @Username";
            string Lvl = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Lvl = Reader.GetString("MaxPP");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return Lvl;
        }
        public static void SetMaxPP(string username, string MaxPP)
        {
            string query = "UPDATE account SET MaxPP= @MaxPP WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@MaxPP", MaxPP);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static int GetMaxWS(string username)
        {
            string query = "SELECT MaxWinstreak FROM account WHERE username= @Username";
            int WS = 0;
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            WS = Reader.GetInt32("MaxWinstreak");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return WS;
        }
        public static void SetMaxWS(string username, int MaxWS)
        {
            string query = "UPDATE account SET MaxWinstreak= @MaxWinstreak WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@MaxWinstreak", MaxWS);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static void UpdateProfile(string Username)
        {
            #region Creating the Profile
            string[] OldP = GetProfile(Username).Split('|');
            List<string> buffer = new List<string>();
            if (OldP[0] == "N") { buffer.Add("0"); }
            else
            {
                Int32.TryParse(OldP[0], out int Version);
                buffer.Add(((Version + 1)%1000).ToString());
            }
            buffer.Add(Username);
            buffer.Add(Database.GetPP(Username));
            buffer.Add(Database.GetLastDeck(Username));
            buffer.Add(Database.GetMaxPP(Username));
            buffer.Add(Database.GetAbilities(Username));
            buffer.Add(Database.GetSuper100(Username));
            buffer.Add(Database.GetSuper200(Username));
            buffer.Add(Database.GetPassives(Username));
            buffer.Add(Database.GetLevel(Username));
            buffer.Add(Database.Online(Username)? "1" : "0");
            buffer.Add(Database.GetWinstreak(Username).ToString());
            buffer.Add(Database.GetMaxWS(Username).ToString());
            #endregion
            string query = "UPDATE account SET Profile= @Profile WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", Username);
                    cmd.Parameters.AddWithValue("@Profile", string.Join("|",buffer));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static string GetProfile(string username)
        {
            string query = "SELECT Profile FROM account WHERE username= @Username";
            string Lvl = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Lvl = Reader.GetString("Profile");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return Lvl;
        }
        public static string GetAdsRem(string username)  
        {
            string query = "SELECT AdsRem FROM account WHERE username= @Username";
            string Rem = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Rem = Reader.GetString("AdsRem");
                        }
                        cmd.Cancel();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            string[] Dec = Rem.Split('/');
            DateTime Now = DateTime.Now;
            string prevTime = Dec[0];
            string curTime = Now.Year + "," + Now.Month + "," + Now.Day + "," + Now.Hour + "," + Now.Minute + "," + Now.Second;
            string curAdsRem = "1,10";   //Default
            int timeLeftBeforeReset = 86400; //24h

            if (prevTime != "")
            {
                string[] TimeArrayStr = prevTime.Split(',');
                int[] TimeArray = new int[TimeArrayStr.Length];
                int i = 0;
                foreach (string c in TimeArrayStr)
                {
                    Int32.TryParse(c, out TimeArray[i]);
                    i++;
                }
                DateTime TimeThen = new DateTime(TimeArray[0], TimeArray[1], TimeArray[2], TimeArray[3], TimeArray[4], TimeArray[5]);
                TimeSpan Ts = Now - TimeThen;
                int timeSpentInSec = Convert.ToInt32(Ts.TotalSeconds);
                if (timeSpentInSec < 86400) { timeLeftBeforeReset = 86400 - timeSpentInSec; curAdsRem = Dec[1]; }
            }

            if (timeLeftBeforeReset == 86400) { SetAdsRem(username, timeLeftBeforeReset, curAdsRem); }

            string ans = string.Join("/", new string[] { timeLeftBeforeReset.ToString(), curAdsRem });
            return ans;
        }
        public static void SetAdsRem(string username, int timeLeftBeforeReset, string AdsRem)
        {
            int timeBack = 86400 - timeLeftBeforeReset;
            DateTime Now = DateTime.Now.AddSeconds(-timeBack);
//            Console.WriteLine(timeLeftBeforeReset.ToString() + "  " + timeBack.ToString() + " " + Now.ToString());
            string curTime = Now.Year + "," + Now.Month + "," + Now.Day + "," + Now.Hour + "," + Now.Minute + "," + Now.Second;
            string ans = string.Join("/", new string[] { curTime, AdsRem });

            string query = "UPDATE account SET AdsRem= @Backgrounds WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Backgrounds", ans);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static void SetDailyRewards(string username, int b)
        {
            string query = "UPDATE account SET DailyRewards= @MaxPP WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@MaxPP", b);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static int GetDailyRewards(string username)
        {
            string query = "SELECT DailyRewards FROM account WHERE username= @Username";
            int WS = 0;
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            WS = Reader.GetInt32("DailyRewards");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return WS;
        }
        public static void SetConsecutiveDays(string username, int b)
        {
            string query = "UPDATE account SET ConsecutiveDays = @MaxPP WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@MaxPP", b);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static int GetConsecutiveDays(string username)
        {
            string query = "SELECT ConsecutiveDays FROM account WHERE username= @Username";
            int WS = 0;
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            WS = Reader.GetInt32("ConsecutiveDays");
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return WS;
        }
        public static void UpdateLastLogin(string username,bool first = false)
        {
            DateTime d = DateTime.Now;
            if (!first)
            {
                string[] oldar = GetLastLogin(username).Split('|');
                int[] timear = new int[oldar.Length];
                int i = 0;
                foreach (string s in oldar)
                {
                    Int32.TryParse(s, out timear[i]);
                    i++;
                }
                DateTime old = new DateTime(timear[0], timear[1], timear[2], timear[3], timear[4], timear[5]);
                TimeSpan t = d - old;
                if(t.Days >= 1)
                {
                    int days = GetConsecutiveDays(username) - 5 * t.Days;
                    if (days < 0) { days = 0; }
                    SetConsecutiveDays(username, days);
                }
            }
            string[] ar = new string[] { d.Year.ToString(), d.Month.ToString(), d.Day.ToString(), d.Hour.ToString(), d.Minute.ToString() , d.Second.ToString() };
            string query = "UPDATE account SET LastLogin = @MaxPP WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@MaxPP", String.Join("|",ar));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static string GetLastLogin(string username)
        {
            string query = "SELECT LastLogin FROM account WHERE username= @Username";
            string l = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            l = Reader.GetString("LastLogin");
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return l;
        }
        public static void UpdateLastSpin(string username,bool login)
        {
            DateTime d = DateTime.Now;
            if (login)
            {
                string[] oldar = GetLastSpin(username).Split('|');
                int[] timear = new int[oldar.Length];
                int i = 0;
                foreach (string s in oldar)
                {
                    Int32.TryParse(s, out timear[i]);
                    i++;
                }
                DateTime old = new DateTime(timear[0], timear[1], timear[2], timear[3], timear[4], timear[5]);
                TimeSpan t = d - old;
                if (t.Days >= 1)
                {
                    SetDailyRewards(username,1);
                }
                return;
            }
            string[] ar = new string[] { d.Year.ToString(), d.Month.ToString(), d.Day.ToString(), d.Hour.ToString(), d.Minute.ToString(), d.Second.ToString() };
            string query = "UPDATE account SET LastSpin = @MaxPP WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@MaxPP", String.Join("|", ar));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static string GetLastSpin(string username)
        {
            string query = "SELECT LastSpin FROM account WHERE username= @Username";
            string l = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            l = Reader.GetString("LastSpin");
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return l;
        }
        public static string GetAdditionalRewards(string username)
        {
            string query = "SELECT AdditionalRewards FROM account WHERE username= @Username";
            string Lvl = "";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            Lvl = Reader.GetString("AdditionalRewards");
                        }
                        cmd.Cancel();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return Lvl;
        }
        public static void SetAdditionalRewards(string username, string AdditionalRewards)
        {
            string query = "UPDATE account SET AdditionalRewards = @Backgrounds WHERE username= @Username";
            using (MySqlConnection SQLToUse = ServerTCP.CreateSQLConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, SQLToUse);
                try
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Backgrounds", AdditionalRewards);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}