using System;
using System.Net.Sockets;
using Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientTCP
{
    public static TcpClient ClientSocket;
    private static NetworkStream myStream;
    private static byte[] receiveBuffer;

    public static string Username;
    public static bool Reconnecting = false;

    public static int LPID;

    public static void InitializeClientSocket(string address, int port)
    {
        ClientSocket = new TcpClient();
        ClientSocket.ReceiveBufferSize = 4096;
        ClientSocket.SendBufferSize = 4096;
        receiveBuffer = new byte[4096 * 2]; //*2 because we are sending and receiving data at the same time
        ClientSocket.BeginConnect(address, port, ClientConnectCallBack, ClientSocket);
    }
    //Is called when we connect with the server
    private static void ClientConnectCallBack(IAsyncResult result)
    {
        ClientSocket.EndConnect(result);
        /*try
        {
            ClientSocket.EndConnect(result);
        }
        catch (Exception)
        {
            //Bad, player prefs already set; do this for each case
                       if (Reconnecting)
                        {
                            ClientSocket.Close();
                            Reconnecting = false;
                            SceneManager.LoadScene("LoginOrRegisterScene");
                            return;
                        }
                        ClientManager.New = 3;
                        return;
                        
        }
        */
        if (ClientSocket.Connected == false)
        {
            if (Reconnecting)
            {
                ClientSocket.Close();
                Reconnecting = false;
                SceneManager.LoadScene("LoginOrRegisterScene");
                return;
            }
            ClientManager.New = 3;
            return;
        }
        else if(ClientManager.New != 3)
        {
            ClientSocket.NoDelay = true;
            myStream = ClientSocket.GetStream();
            myStream.BeginRead(receiveBuffer, 0, 4096 * 2, ReceiveCallBack, null);
            Authenticate();

            if (Reconnecting)
            {
                Reconnecting = false;
                return;
            }
            if (Username == "N")
            {
                ClientManager.New = 1;
            }
            else
            {
                ClientManager.New = 2;
                PACKAGE_Login(Username);
            }
        }
    }
    private static void ReceiveCallBack(IAsyncResult result)
    {
        try
        {
            if (ClientSocket == null || myStream == null) { return; }
            int readBytes = myStream.EndRead(result);
            if (readBytes <= 0) { return; }

            byte[] newBytes = new byte[readBytes];
            Array.Copy(receiveBuffer, newBytes, readBytes);
            UnityThread.executeInFixedUpdate(() => { ClientHandleData.HandleData(newBytes); }); //To run HandleData in the Unity Thread
            if (ClientSocket == null || myStream == null) { return; }
            myStream.BeginRead(receiveBuffer, 0, 4096 * 2, ReceiveCallBack, null);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            if (SceneManager.GetActiveScene().name == "OnlineBattleScene" || SceneManager.GetActiveScene().name == "SpectateScene")
            {
                CloseConnection();
                SceneManager.LoadScene("LoginOrRegisterScene");
            }
            throw;
        }
    }
    //We want it to throw exceptions in case something goes wrong
    public static void SendData(byte[] data)
    {
        if (!ClientSocket.Connected)
        {
            ClientSocket.Close();
            throw new Exception();
        }
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((data.GetUpperBound(0) - data.GetLowerBound(0) + 1));
        buffer.WriteBytes(data);
        byte[] tmp = buffer.ToArray();
        myStream.Write(tmp, 0, tmp.Length);
        buffer.Dispose();
    }

    public static void Authenticate()
    {
        ByteBuffer buf = new ByteBuffer();
        buf.WriteInteger(1000000007); buf.WriteInteger(5793973);
        ClientTCP.SendData(buf.ToArray());
        buf.Dispose();
    }
    public static void PACKAGE_NewAccount(string username)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CNewAccount);
        buffer.WriteString(username);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_Login(string username)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CLogin);
        buffer.WriteString(username);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_BattleOnline(string username, int Choice, int GameMode, string OtherUsername)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CBattleOnline);
        buffer.WriteString(username);
        buffer.WriteInteger(TempOpponent.Opponent.ShapeIDUser);
        buffer.WriteInteger(TempOpponent.Opponent.ShapeID2User);    //-1 if in 1v1 mode
        buffer.WriteInteger(GameMode);
        int[] finalIDs = PlayerPrefsX.GetIntArray("ActiveAbilityID" + PlayerPrefs.GetInt("ShapeSelectedID").ToString());
        string Ab = String.Join(",", finalIDs);
        int[] finalIDs2 = PlayerPrefsX.GetIntArray("2ActiveAbilityID" + PlayerPrefs.GetInt("2ShapeSelectedID").ToString());
        string Ab2 = String.Join(",", finalIDs2);
        if (GameMode != 2) { Ab2 = ""; }
        int SID = PlayerPrefs.GetInt("SpecialAbilityID" + PlayerPrefs.GetInt("ShapeSelectedID").ToString());
        int SID2 = PlayerPrefs.GetInt("2SpecialAbilityID" + PlayerPrefs.GetInt("2ShapeSelectedID").ToString());
        buffer.WriteString(Ab);
        buffer.WriteString(Ab2);
        buffer.WriteInteger(SID);
        buffer.WriteInteger(SID2);
        buffer.WriteInteger(Choice);
        buffer.WriteInteger(PlayerPrefs.GetInt("SkinID"));
        buffer.WriteInteger(PlayerPrefs.GetInt("SkinID2"));
        if (Choice == 1)    //requesting friendly
        {
            buffer.WriteString(OtherUsername);
        }
        if (Choice == 2)    //accepting friendly
        {
            buffer.WriteInteger(TempOpponent.Opponent.RoomNum);
        }
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_GetProbability(int OtherPlayerConnectionID,string Username, bool Disc = false)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CGetProbability);
        buffer.WriteInteger(OtherPlayerConnectionID);
        //buffer.WriteInteger(Range);
        buffer.WriteString(Username);
        buffer.WriteString(TempOpponent.Opponent.Username);
        buffer.WriteBool(Disc);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_GetChoice(int OtherPlayerConnectionID)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CGetOtherPlayerChoice);
        buffer.WriteInteger(OtherPlayerConnectionID);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_GiveChoice()
    {
        ByteBuffer buffer = new ByteBuffer();
        var Player1 = GameObject.Find("Game Manager").GetComponent<GameMaster>().player1;
        var Player2 = GameObject.Find("Game Manager").GetComponent<GameMaster>().player2;
        bool Choicedone = Player1.GetChoiceDone();
        int IDChosen = Player1.GetIdOfAnimUsed();
        buffer.WriteInteger((int)ClientPackages.CSendChoice);
        buffer.WriteBool(Choicedone);
        buffer.WriteInteger(IDChosen);
        buffer.WriteInteger(PlayerPrefs.GetInt("BotOnline") == 1 ? -1 : Player2.GetConnectionID());
        buffer.WriteString(PlayerPrefs.GetString("Username"));
        buffer.WriteString(TempOpponent.Opponent.Username);
        Choicedone = Player2.GetChoiceDone();
        buffer.WriteBool(Choicedone);
        if (Player2.GetChoiceDone())
            buffer.WriteInteger(Player2.GetIdOfAnimUsed());
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_ENDGAME(int GameMode, string username, string OtherPlayerUsername, int OtherplayerconnID, int ShapeID, int OtherplayerShapeID, bool Friendly = false, bool Draw = false, int Playerwon = -1, int botPP = -1)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CEndGame);
        if (Playerwon != -1)    //bot online (1v1 only)
        {
            buffer.WriteInteger(-1);
            buffer.WriteString(username);
            buffer.WriteInteger(ShapeID);
            buffer.WriteInteger(Playerwon);
            buffer.WriteInteger(botPP);
            SendData(buffer.ToArray());
            buffer.Dispose();
            return;
        }
        buffer.WriteInteger(OtherplayerconnID);
        buffer.WriteString(username);
        buffer.WriteString(OtherPlayerUsername);
        buffer.WriteInteger(ShapeID);
        buffer.WriteInteger(OtherplayerShapeID);
        buffer.WriteInteger(GameMode);
        buffer.WriteBool(Friendly);
        buffer.WriteBool(Draw);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_DEBUG(string msg, string Username = "", int Chestnum = -1)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CDebug);
        buffer.WriteString(msg);
        if (Username != "")
        {
            buffer.WriteString(Username);
        }
        if (Chestnum != -1)
        {
            buffer.WriteInteger(Chestnum);
        }
        if (msg == "GiveLP")
            buffer.WriteInteger(LPID);

        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_GetLeaderboard(int Lb)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CLeaderboard);
        buffer.WriteInteger(Lb);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_GetAbilities()
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CGetAbilities);
        buffer.WriteString(PlayerPrefs.GetString("Username"));
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_MainMenu(bool Searching, int GameMode)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CMainMenu);
        buffer.WriteBool(Searching);
        buffer.WriteInteger(GameMode);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_GetStats()
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CGetStats);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_Emotes(int ID, string Username = "")
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CEmotes);
        buffer.WriteInteger(ID);
        buffer.WriteInteger(TempOpponent.Opponent.ConnectionID);
        buffer.WriteString(Username);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_AddFriend(string Code)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CAddFriend);
        buffer.WriteString(Code);
        buffer.WriteString(PlayerPrefs.GetString("Username"));
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_GetFriendsList()
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CFriendsList);
        buffer.WriteString(PlayerPrefs.GetString("Username"));
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_CheckFriend(string Username, string PlayerUsername)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CCheckFriend);
        buffer.WriteString(Username);
        buffer.WriteString(PlayerUsername);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_Spectate(string Username)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CSpectate);
        buffer.WriteString(Username);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_LifePoints(int LP1, int LP2, int ConnID,int LifeID)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CLifePoints);
        buffer.WriteInteger(LP1);
        buffer.WriteInteger(LP2);
        buffer.WriteInteger(ConnID);
        GameMaster GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        buffer.WriteString(GM.ReconnectString());
        buffer.WriteInteger(GM.player1.GetEP()); buffer.WriteInteger(GM.player2.GetEP()); buffer.WriteInteger(GM.round);
        buffer.WriteInteger(LifeID);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_Chest(int Time, int ChestNum)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CChest);
        buffer.WriteInteger(Time);
        buffer.WriteString(PlayerPrefs.GetString("Username"));
        buffer.WriteInteger(ChestNum);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_ChestOpening(bool Spin = false)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CChestOpening);
        buffer.WriteString(PlayerPrefs.GetString("Username"));
        buffer.WriteInteger(PlayerPrefs.GetInt("Gold"));
        buffer.WriteInteger(PlayerPrefs.GetInt("Redbolts"));
        buffer.WriteInteger(PlayerPrefs.GetInt("Diamonds"));
        buffer.WriteString(String.Join(",", PlayerPrefsX.GetStringArray("AbilitiesArray")));
        buffer.WriteString(String.Join(",", PlayerPrefsX.GetStringArray("Super100Array")));
        buffer.WriteString(String.Join(",", PlayerPrefsX.GetStringArray("Super200Array")));
        buffer.WriteString(String.Join(",", PlayerPrefsX.GetIntArray("Level")));
        buffer.WriteString(String.Join(",", PlayerPrefsX.GetIntArray("ChestSlots")));
        buffer.WriteString(String.Join(",", PlayerPrefsX.GetIntArray("XP")));
        buffer.WriteString(String.Join(",", PlayerPrefsX.GetIntArray("Backgrounds")));
        buffer.WriteString(String.Join(",", PlayerPrefsX.GetIntArray("EmotesUnlockedAr")));
        buffer.WriteString(String.Join(",", PlayerPrefsX.GetIntArray("SkinsUnlockedAr")));
        buffer.WriteString(String.Join("-", PlayerPrefsX.GetStringArray("TrophyRoadUnlocked")));
        buffer.WriteBool(Spin);
        buffer.WriteString(String.Join(",",PlayerPrefsX.GetIntArray("AdditionalRewards")));
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_Reconnect(int OtherConnID)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CReconnect);
        if(SceneManager.GetActiveScene().name == "OnlineBattleScene")
        {
            Shape_Player Player1 = GameObject.Find("Player1").GetComponent<Shape_Player>();
            Shape_Player Player2 = GameObject.Find("Player2").GetComponent<Shape_Player>();
            GameMaster GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
            Player2.SetConnectionID(OtherConnID);
            TempOpponent.Opponent.ConnectionID = OtherConnID;
            buffer.WriteInteger(Player1.GetLife());
            buffer.WriteInteger(Player2.GetLife());
            buffer.WriteInteger(Player1.GetEP());
            buffer.WriteInteger(Player2.GetEP());
            buffer.WriteInteger(GM.round);
            buffer.WriteString(string.Join(",", GM.ChoicesP1));
            buffer.WriteString(string.Join(",", GM.ChoicesP2));
            buffer.WriteString(string.Join(",", GM.ProbsP1));
            buffer.WriteString(string.Join(",", GM.ProbsP2));
            buffer.WriteInteger(GameMaster.Mode);
            buffer.WriteInteger(OtherConnID);
            buffer.WriteString(PlayerPrefs.GetString("Username"));
            buffer.WriteString(GM.ReconnectString());
            if (Player1.GetChoiceDone() && !Player2.GetChoiceDone())
                buffer.WriteInteger(Player1.GetIdOfAnimUsed());
            else
                buffer.WriteInteger(-1);
        }
        else
        {
            buffer.WriteInteger(-1);
        }
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    public static void PACKAGE_AdReward(int timeLeftBeforeReset)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)ClientPackages.CAdReward);
        buffer.WriteString(PlayerPrefs.GetString("Username"));
        buffer.WriteInteger(PlayerPrefs.GetInt("Gold"));
        buffer.WriteInteger(PlayerPrefs.GetInt("Redbolts"));
        buffer.WriteInteger(PlayerPrefs.GetInt("Diamonds"));
        buffer.WriteInteger(timeLeftBeforeReset);
        if (timeLeftBeforeReset != -1)
        {
            buffer.WriteString(String.Join(",", PlayerPrefsX.GetIntArray("AdsRem")));
        }
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
   
    public static void CloseConnection()
    {
        if(ClientSocket != null)
            ClientSocket.Close();
    }
}