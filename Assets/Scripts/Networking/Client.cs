using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Sockets.Client
{
    public class Client : MonoBehaviour
    {
        public Thread yThread;

        #region Singleton
        public static Client Instance;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
        #endregion
        // Start is called before the first frame update
        public void InitiateConnection(string consUsed, int score, int amountWordsFound, bool accepted, string usedWords)
        {
            Debug.Log("Initiated connection from Client");
            Thread t_PerthOut = new Thread(() => AsyncSocketClient.StartClient(consUsed, score, amountWordsFound, accepted, usedWords));
            t_PerthOut.Start();
        }


        public class ObjectState
        {
            public const int BufferSize = 256;
            public Socket wsSocket;
            public byte[] buffer = new byte[BufferSize];
            public StringBuilder sb = new StringBuilder();

        }

        [System.Serializable]
        public class MatchDetails
        {
            public string ConsUsed;
            public int Score;
            public int AmountWordsFound;
            public bool Accepted;
            public string usedWords;

            public MatchDetails() { }

            public MatchDetails(string consUsed, int score, int amountWordsFound, bool accepted, string usedWords)
            {
                this.ConsUsed = consUsed;
                this.Score = score;
                this.AmountWordsFound = amountWordsFound;
                this.Accepted = accepted;
                this.usedWords = usedWords;
            }
        }


        public class AsyncSocketClient
        {

            private const int Port = 6321;
            private static ManualResetEvent connectCompleted = new ManualResetEvent(false);
            private static ManualResetEvent sendCompleted = new ManualResetEvent(false);
            private static ManualResetEvent receiveCompleted = new ManualResetEvent(false);
            private static string response = string.Empty;


            public static void StartClient(string consUsed, int score, int amountWordsFound, bool accepted, string usedWords)
            {
                try
                {
                    IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress ip = ipHost.AddressList[0];
                    IPEndPoint remoteEndPoint = new IPEndPoint(ip,Port);

                    Socket client = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    client.BeginConnect(remoteEndPoint, new AsyncCallback(ConnectionCallback), client);

                    MatchDetails detailsToUse = new MatchDetails { ConsUsed = consUsed, Score = score, AmountWordsFound=amountWordsFound, Accepted = accepted, usedWords=usedWords};

                    Send(client, ToBytes(detailsToUse));
                    sendCompleted.WaitOne();

                    Receive(client);
                    receiveCompleted.WaitOne();

                    Debug.Log("Client Side Response received, " + " WordUsed: " + detailsToUse.ConsUsed + 
                        "score: " + detailsToUse.Score +
                        "Amount of words found: "+  detailsToUse.AmountWordsFound + 
                        "Accepted or not: " + detailsToUse.Accepted + 
                        "WordsUsed : " + detailsToUse.usedWords);

                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch (Exception e)
                {

                    Debug.Log(e.Message);
                }
            }

            public static byte[] ToBytes(MatchDetails data)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryWriter bw = new BinaryWriter(ms);
                    string jsonData = JsonUtility.ToJson(data);
                    bw.Write("CREATEMATCH||" + jsonData);
                    Debug.Log(jsonData);

                    return ms.ToArray();
                }
            }

            public static MatchDetails FromBytes(byte[] buffer)
            {
                MatchDetails retVal = new MatchDetails();
                Debug.Log("Received stuff");
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    BinaryReader br = new BinaryReader(ms);
                    string jsonData = br.ReadString();
                    Debug.Log(jsonData);
                }

                return retVal;
            }


            private static void Receive(Socket client)
            {
                try
                {
                    ObjectState state = new ObjectState();
                    state.wsSocket = client;
                    client.BeginReceive(state.buffer, 0, ObjectState.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                catch (Exception e)
                {

                    Debug.Log(e);
                }
            }

            private static void ReceiveCallback(IAsyncResult ar)
            {
                try
                {
                    ObjectState state = (ObjectState)ar.AsyncState;
                    var client = state.wsSocket;
                    int byteRead = client.EndReceive(ar);
                    if (byteRead>0)
                    {
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, byteRead));
                        client.BeginReceive(state.buffer, 0, ObjectState.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                    }
                    else
                    {
                        if (state.sb.Length>1)
                        {
                            response = state.sb.ToString();
                        }
                        receiveCompleted.Set();
                    }
                }
                catch (Exception e)
                {

                    Debug.Log(e);
                }
            }

            private static void Send(Socket client, byte[] data)
            {
                byte[] byteData = data;
                client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
            }

            private static void SendCallback(IAsyncResult ar)
            {
                try
                {
                    Socket client = (Socket)ar.AsyncState;
                    int byteSent = client.EndSend(ar);
                    Debug.Log("Sent: " + byteSent + "bytes to server");
                    sendCompleted.Set();
                }
                catch (Exception e)
                {

                    Debug.Log(e);
                }
            }

            private static void ConnectionCallback(IAsyncResult ar)
            {
                try
                {
                    Socket client = (Socket)ar.AsyncState;
                    client.EndConnect(ar);
                    Debug.Log("Socket connection: " + client.RemoteEndPoint.ToString());
                    connectCompleted.Set();
                }
                catch (Exception e)
                {

                    Debug.Log(e);
                }
            }
        }


    }



}

