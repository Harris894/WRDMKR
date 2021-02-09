using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using static Sockets.Client.Client;

namespace Sockets.Database
{
    public class Database : MonoBehaviour
    {
        private string dbPath;
        public string consUsed = "";
        public bool accepted;
        public int score;
        public int amountWordsFound;
        public string wordsUsed = "";

        #region Singleton
        public static Database Instance;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
        #endregion

        public void Start()
        {
            dbPath = "URI=file:" + Application.persistentDataPath + "/MatchDetailDB.db";
            CreateScheme();
        }



        private void CreateScheme()
        {
            using (var conn = new SqliteConnection(dbPath))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS 'match_details'( " +
                        " 'id' INTEGER UNIQUE," +
                        "'consUsed' TEXT," +
                        "'score' INTEGER," +
                        "'amountWordsFound' INTEGER," +
                        "'accepted' BOOL," +
                        "'wordsUsed' TEXT" +
                        ");";
                    var result = cmd.ExecuteNonQuery();
                    Debug.Log("create scheme: " + result);
                }
            }
        }

        public void PassMatchDetailsToDatabase(int id, string consUsed, int score, int amountWordsFound, bool accepted, string wordsUsed)
        {
            using (var conn = new SqliteConnection(dbPath))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO match_details (id,consUsed,score,amountWordsFound,accepted,wordsUsed)" +
                                      "VALUES (@Id, @ConsUsed, @Score, @AmountWordsFound, @Accepted, @WordsUsed);";

                    cmd.Parameters.Add(new SqliteParameter
                    {
                        ParameterName = "Id",
                        Value = id
                    });

                    cmd.Parameters.Add(new SqliteParameter
                    {
                        ParameterName = "ConsUsed",
                        Value = consUsed
                    });
                    cmd.Parameters.Add(new SqliteParameter
                    {
                        ParameterName = "Score",
                        Value = score
                    });
                    cmd.Parameters.Add(new SqliteParameter
                    {
                        ParameterName = "AmountWordsFound",
                        Value = amountWordsFound
                    });
                    cmd.Parameters.Add(new SqliteParameter
                    {
                        ParameterName = "Accepted",
                        Value = accepted
                    });
                    cmd.Parameters.Add(new SqliteParameter
                    {
                        ParameterName = "WordsUsed",
                        Value = wordsUsed
                    });

                    int result = cmd.ExecuteNonQuery();
                    Debug.Log("insert score: " + result);
                    Debug.Log("Data has been passed into the database");

                }

            }

        }
        

        public MatchDetails GetMatchDetailsFromDatabase(int id)
        {
            MatchDetails matchDetails= new MatchDetails();

            using (var conn = new SqliteConnection(dbPath))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.Add(new SqliteParameter
                    {
                        ParameterName = "idRequest",
                        Value = id
                    });

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM match_details WHERE id = @idRequest;";



                    Debug.Log("score (begin)");

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        consUsed = reader.GetString(1);
                        score = reader.GetInt32(2);
                        amountWordsFound = reader.GetInt32(3);
                        accepted = reader.GetBoolean(4);
                        wordsUsed = reader.GetString(5);

                        //var usedWords = reader.GetString(4);
                        string text = string.Format("{0}: {1}", consUsed, score);
                        Debug.Log("Text: " + text);
                        Debug.Log("WordToUse: " + consUsed);
                        Debug.Log("Score: " + score);
                        Debug.Log("AmountWordsFound: " + amountWordsFound);
                        Debug.Log("Accepted: " + accepted);
                        Debug.Log("Words Used: " + wordsUsed);


                    }
                    Debug.Log("scores (end)");
                }


            }

            matchDetails.ConsUsed = consUsed;
            matchDetails.Score = score;
            matchDetails.AmountWordsFound = amountWordsFound;
            matchDetails.Accepted = accepted;
            matchDetails.usedWords = wordsUsed;

            return matchDetails;
        }

        public int ReceiveLatestID()
        {
            int id = 0;

            using (var conn = new SqliteConnection(dbPath))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT id FROM match_details ORDER BY id DESC LIMIT 0, 1;";
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            id = reader.GetInt32(0);
                        }
                    }catch(System.Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }

            return id;
        }


    }
}
