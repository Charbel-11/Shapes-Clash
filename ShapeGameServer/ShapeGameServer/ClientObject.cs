using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ShapeGameServer {
    //Represents each connected client on the server
    class ClientObject {
        public static ConcurrentDictionary<string, int> ConnectionIDs;
        public static ConcurrentDictionary<string, int[]> UsernamesDisc = new ConcurrentDictionary<string, int[]>();
        public static ConcurrentDictionary<string, int[]> UsernamesDiscFriendly = new ConcurrentDictionary<string, int[]>();

        public TcpClient Socket;
        public NetworkStream myStream;  //Needed to send and receive data between this specific client client and the server
        public byte[] ReceiveBuffer;    //Stores all the information we are sending from the client to the server or from the server to the client
        public ByteBuffer buffer;
        public List<byte[]> BufferList = new List<byte[]>();

        public int ConnectionID;
        public string IP;
        public bool authenticated;

        public string Username, OldUsername;
        public bool StopReading = false;

        public ClientObject(TcpClient _Socket, int _ConnectionID) {
            if (_Socket == null) return;

            Socket = _Socket;
            ConnectionID = _ConnectionID;

            Socket.NoDelay = true;              //Pipeline packets without ACKs
            Socket.ReceiveBufferSize = 4096;    //Larger sent/receiveed packets are split
            Socket.SendBufferSize = 4096;       //But smaller packets are sent/received together

            buffer = new ByteBuffer();
            ReceiveBuffer = new byte[4096];
            myStream = Socket.GetStream();

            StopReading = authenticated = false;
            IP = Socket.Client.RemoteEndPoint.ToString();

            myStream.BeginRead(ReceiveBuffer, 0, Socket.ReceiveBufferSize, ReceiveCallback, null);  //Starting to listen to the stream
                                                                                                    //Once we receive data, ReceiveCallback is called

            Console.WriteLine("Connection incoming from {0}", IP);
        }

        //When we receive data, this Async function is called
        public void ReceiveCallback(IAsyncResult result) {
            if (StopReading) { return; }
            try {
                if (Socket == null) { Console.WriteLine("Socket is null"); return; }
                if (myStream == null) { Console.WriteLine("Stream is null"); return; }
                if (result == null) { CloseConnection(); return; }
                int readBytes = myStream.EndRead(result);
                if (readBytes <= 0 && authenticated) //Not getting any data
                {
                    //CloseConnection();
                    OldUsername = Username;
                    ServerTCP.IPDictionary.TryAdd(IP.Split(':')[0], this);
                    ServerTCP.PACKET_SendMessage(ConnectionID, "Connect");
                    Socket = null;
                    authenticated = false;
                    Task.Delay(4000).ContinueWith(t => CheckForConnection());
                    return;
                }

                byte[] newBytes = new byte[readBytes];
                Array.Copy(ReceiveBuffer, newBytes, readBytes);
                ServerHandleData.HandleData(ConnectionID, newBytes);

                if (ReceiveBuffer == null) { Console.WriteLine("Rec Buff is null"); return; }
                else if (Socket == null) { Console.WriteLine("Socket is null"); return; }
                else if (myStream == null) { Console.WriteLine("Stream is null"); return; }
                myStream.BeginRead(ReceiveBuffer, 0, Socket.ReceiveBufferSize, ReceiveCallback, null);  //In case we need to receive more data
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                if (StopReading) { return; }
                CloseConnection();
            }
        }

        public void CloseConnection() {
            authenticated = false;
            Console.WriteLine("Connection from {0} has been terminated", IP);

            if (buffer != null) { buffer.Dispose(); buffer = null; }
            ServerTCP.IPDictionary.TryRemove(IP, out ClientObject zzz);
            if (Username == "N" || Username == null) {
                Socket.Close();
                Socket = null;
                return;
            }

            if (Socket != null) { Socket.Close(); Socket = null; }

            Database.SetOnline(Username, false);
            if (ConnectionIDs != null) { ConnectionIDs.TryRemove(Username, out int vv); }

            TempPlayer tempP = ServerTCP.TempPlayers[ConnectionID];
            if (tempP.Spectating) {
                tempP.Spectating = false;
                RoomInstance._room[tempP.SpecRoom].SpectatorIDs.Remove(ConnectionID);
                tempP.SpecRoom = 0;
            }

            int roomNum = tempP.Room;
            if (roomNum != 0) {
                Room room = RoomInstance._room[roomNum];
                if (room.Bot) {
                    room.DiscRecTimes[0] = DateTime.Now;
                    int[] Ar = new int[2];
                    Ar[0] = roomNum;
                    Ar[1] = -1; //Put -1 instead of the other ConnID so that i know it's a bot.
                    UsernamesDisc.TryAdd(Username, Ar);
                }
                else if (room.ConnectionID[1] != 0 && room.ConnectionID[0] != 0) {
                    int OtherConnID = (ConnectionID == room.ConnectionID[0] ? room.ConnectionID[1] : room.ConnectionID[0]);
                    room.Usernames.TryGetValue(OtherConnID, out string OtherUsername);

                    //If the other player disconnected before, end the game
                    if (UsernamesDisc.TryRemove(OtherUsername, out int[] aa)) {
                        RoomInstance.instance.LeaveRoom(ConnectionID, room.GameMode, roomNum);
                    }
                    else {
                        if (room.GameMode == 2) {
                            ServerTCP.PACKET_SendMessage(OtherConnID, "D2");
                            RoomInstance.instance.LeaveRoom(ConnectionID, room.GameMode, roomNum);
                        }
                        else {
                            int[] Ar = new int[2] { roomNum, OtherConnID };
                            UsernamesDisc.TryAdd(Username, Ar);
                            ServerTCP.PACKET_SendMessage(OtherConnID, "D");
                        }
                    }

                    #region Comment
                    /*if (!RoomInstance._room[ServerTCP.TempPlayers[ConnectionID].Room].Friendly)
                    {
                        //RoomInstance.instance.LeaveRoom(ConnectionID, RoomInstance._room[ServerTCP.TempPlayers[ConnectionID].Room].GameMode, true);
                        if (UsernamesDisc.ContainsKey(OtherUsername))
                        {
                            RoomInstance.instance.LeaveRoom(ConnectionID, RoomInstance._room[ServerTCP.TempPlayers[ConnectionID].Room].GameMode);
                            UsernamesDisc.Remove(OtherUsername);
                        }                       
                        else
                        {
                            int[] Ar = new int[2];
                            Ar[0] = ServerTCP.TempPlayers[ConnectionID].Room;
                            if (RoomInstance._room[ServerTCP.TempPlayers[ConnectionID].Room].ConnectionID[1] == ConnectionID)
                                Ar[1] = RoomInstance._room[ServerTCP.TempPlayers[ConnectionID].Room].ConnectionID[0];
                            else
                                Ar[1] = RoomInstance._room[ServerTCP.TempPlayers[ConnectionID].Room].ConnectionID[1];

                            UsernamesDisc.Add(Username, Ar);
                        }
                    }
                    else
                    {
                        //RoomInstance.instance.LeaveFriendly(ConnectionID, false, true);
                        if (UsernamesDiscFriendly.ContainsKey(OtherUsername))
                        {
                            RoomInstance.instance.LeaveRoom(ConnectionID, RoomInstance._room[ServerTCP.TempPlayers[ConnectionID].Room].GameMode);
                            UsernamesDiscFriendly.Remove(OtherUsername);
                        }
                        else
                        {
                            int[] Ar = new int[2];
                            Ar[0] = ServerTCP.TempPlayers[ConnectionID].Room;
                            if (RoomInstance._room[ServerTCP.TempPlayers[ConnectionID].Room].ConnectionID[1] == ConnectionID)
                                Ar[1] = RoomInstance._room[ServerTCP.TempPlayers[ConnectionID].Room].ConnectionID[0];
                            else
                                Ar[1] = RoomInstance._room[ServerTCP.TempPlayers[ConnectionID].Room].ConnectionID[1];

                            UsernamesDiscFriendly.Add(Username, Ar);
                        }
                    }*/
                    #endregion
                }
                //While searching or if the other player already disconnected
                else { RoomInstance.instance.LeaveRoom(ConnectionID, room.GameMode, roomNum); }
            }
            tempP.Room = 0;

            ServerTCP.TempPlayers.TryRemove(ConnectionID, out TempPlayer ww);
            ServerTCP.ClientObjects.TryRemove(ConnectionID, out ClientObject wtv);
        }

        private void CheckForConnection() {
            if (Socket == null) { CloseConnection(); }
            else {
                foreach (byte[] data in BufferList)
                    ServerTCP.SendDataTo(ConnectionID, data);
                BufferList.Clear();
            }
            ServerTCP.IPDictionary.TryRemove(IP.Split(':')[0], out ClientObject www);
        }
    }
}