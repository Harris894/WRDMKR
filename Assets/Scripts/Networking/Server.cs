using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using Sockets.Database;

namespace Sockets.Server
{
    public class Server : MonoBehaviour
    {
        public Thread mThread;
        private void Start()
        {
            Debug.Log("Initiated");
            ThreadStart ts = new ThreadStart(AsyncSocketListener.StartListener);
            mThread = new Thread(ts);
            mThread.Start();
        }

        public class ObjectState
        {
            public Socket wSocket = null;
            public const int bufferSize = 1024;
            public byte[] buffer = new byte[bufferSize];
            public StringBuilder sb = new StringBuilder();
        }


        public class AsyncSocketListener
        {
            [SerializeField]
            public static int port=6321;
            public static ManualResetEvent allCompleted = new ManualResetEvent(false);

            public static void StartListener()
            {
                byte[] bytes = new byte[1024];
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ip = ipHost.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ip, port);
                Socket listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(100);

                    while (true)
                    {
                        allCompleted.Reset();
                        Debug.Log("Waiting for incoming connections...");
                        listener.BeginAccept(new System.AsyncCallback(AcceptCallback), listener);
                        allCompleted.WaitOne();
                    }
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }


            }

            private static void AcceptCallback(IAsyncResult ar)
            {
                allCompleted.Set();
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                ObjectState state = new ObjectState();
                state.wSocket = handler;
                handler.BeginReceive(state.buffer, 0, ObjectState.bufferSize, 0, new AsyncCallback(ReadCallback), state);
            }

            private static void ReadCallback(IAsyncResult ar)
            {
                ObjectState state = (ObjectState)ar.AsyncState;
                Socket handler = state.wSocket;

                int bytesRead = handler.EndReceive(ar);
                if (bytesRead > 0)
                {
                    byte[] byteData = state.buffer;
                    string data = null;
                    using (MemoryStream ms = new MemoryStream(byteData))
                    {
                        BinaryReader br = new BinaryReader(ms);
                        data = br.ReadString();
                    }

                    string[] dataParts = data.Split(new string[] { "||" }, StringSplitOptions.None);
                    string command = dataParts[0];
                    string jsonData = dataParts[1];

                    Debug.Log("Command: " + command + ", Data: " + jsonData);
                    Client.Client.MatchDetails matchDetails = JsonUtility.FromJson<Client.Client.MatchDetails>(jsonData);
                    Debug.Log(matchDetails.usedWords);

                    if (command.IndexOf("CREATEMATCH", StringComparison.Ordinal) > -1)
                    {
                        int id = Database.Database.Instance.ReceiveLatestID() + 1;
                        Debug.Log("ID" + id);
                        if (id == 0)
                        {
                            id = 1;
                        }

                        Database.Database.Instance.PassMatchDetailsToDatabase(id, matchDetails.ConsUsed, matchDetails.Score, matchDetails.AmountWordsFound, matchDetails.Accepted, matchDetails.usedWords);
                        Debug.Log("Read: " + jsonData.Length + "bytes from \n socket Data: " + jsonData);
                        Send(handler, matchDetails);
                    }
                    else
                    {
                        handler.BeginReceive(state.buffer, 0, ObjectState.bufferSize, 0,
                            new AsyncCallback(ReadCallback), state);
                    }

                }
            }

            private static void Send(Socket handler, Client.Client.MatchDetails content)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryWriter bw = new BinaryWriter(ms);
                    string jsonData = JsonUtility.ToJson(content);
                    bw.Write("CREATEMATCH||" + jsonData);
                    Debug.Log(jsonData);

                    byte[] byteData = ms.ToArray();
                    handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
                }

            }

            private static void SendCallback(IAsyncResult ar)
            {
                try
                {
                    Socket handler = (Socket)ar.AsyncState;
                    int byteSent = handler.EndSend(ar);
                    Debug.Log("Sent: " + byteSent + " to client");
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                catch (Exception e)
                {

                    Debug.Log(e);
                }
            }
        }
    }

}



